#!/usr/bin/env node
/**
 * MCP Server for RogueElements
 *
 * Provides documentation resources, class browsing, search, and code generation tools
 * for working with the RogueElements procedural map generation library.
 *
 * Uses tree-sitter for proper AST-based C# parsing to extract XML documentation.
 */

import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import { z } from "zod";
import * as fs from "fs";
import * as path from "path";
import { fileURLToPath } from "url";
import Parser from "web-tree-sitter";
type Language = Parser.Language;
type SyntaxNode = Parser.SyntaxNode;

// Get __dirname equivalent for ES modules
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// =============================================================================
// DIRECTORY DISCOVERY
// =============================================================================

function findDocsDir(): string {
  const candidates = [
    path.resolve(__dirname, "../../docs/claude"),
    path.resolve(__dirname, "../docs/claude"),
    path.resolve(process.cwd(), "docs/claude"),
  ];

  for (const dir of candidates) {
    if (fs.existsSync(dir)) return dir;
  }

  return path.resolve(process.cwd(), "docs/claude");
}

function findRogueElementsDir(): string {
  const candidates = [
    path.resolve(__dirname, "../../RogueElements"),
    path.resolve(__dirname, "../RogueElements"),
    path.resolve(process.cwd(), "RogueElements"),
  ];

  for (const dir of candidates) {
    if (fs.existsSync(dir)) return dir;
  }

  return path.resolve(process.cwd(), "RogueElements");
}

const DOCS_DIR = findDocsDir();
const ROGUE_DIR = findRogueElementsDir();

// =============================================================================
// TREE-SITTER INITIALIZATION
// =============================================================================

let csharpParser: Parser | null = null;
let csharpLanguage: Language | null = null;

async function initializeParser(): Promise<void> {
  if (csharpParser) return;

  await Parser.init();
  csharpParser = new Parser();

  // Load C# grammar - try multiple locations
  const wasmCandidates = [
    path.resolve(__dirname, "../node_modules/tree-sitter-c-sharp/tree-sitter-c_sharp.wasm"),
    path.resolve(__dirname, "../../node_modules/tree-sitter-c-sharp/tree-sitter-c_sharp.wasm"),
    path.resolve(process.cwd(), "mcp-server/node_modules/tree-sitter-c-sharp/tree-sitter-c_sharp.wasm"),
  ];

  let wasmPath: string | null = null;
  for (const candidate of wasmCandidates) {
    if (fs.existsSync(candidate)) {
      wasmPath = candidate;
      break;
    }
  }

  if (!wasmPath) {
    throw new Error("Could not find tree-sitter-c_sharp.wasm");
  }

  csharpLanguage = await Parser.Language.load(wasmPath);
  csharpParser.setLanguage(csharpLanguage);
}

// =============================================================================
// TREE-SITTER HELPERS
// =============================================================================

function findNodesByType(node: SyntaxNode, type: string, results: SyntaxNode[] = []): SyntaxNode[] {
  if (node.type === type) results.push(node);
  for (let i = 0; i < node.childCount; i++) {
    const child = node.child(i);
    if (child) findNodesByType(child, type, results);
  }
  return results;
}

function getDocComments(node: SyntaxNode): string {
  const comments: string[] = [];
  let prev = node.previousSibling;

  while (prev && prev.type === "comment") {
    comments.unshift(prev.text);
    prev = prev.previousSibling;
  }

  return comments.join("\n");
}

function parseDocComment(docText: string): { summary: string; remarks: string; inheritdoc: boolean } {
  const summaryMatch = docText.match(/<summary>\s*([\s\S]*?)\s*<\/summary>/);
  const summary = summaryMatch
    ? summaryMatch[1].replace(/^\s*\/\/\/\s*/gm, "").trim()
    : "";

  const remarksMatch = docText.match(/<remarks>\s*([\s\S]*?)\s*<\/remarks>/);
  const remarks = remarksMatch
    ? remarksMatch[1].replace(/^\s*\/\/\/\s*/gm, "").trim()
    : "";

  const inheritdoc = docText.includes("<inheritdoc");

  return { summary, remarks, inheritdoc };
}

function getFieldText(node: SyntaxNode, fieldName: string): string {
  const field = node.childForFieldName(fieldName);
  return field ? field.text : "";
}

// =============================================================================
// CLASS CATEGORIES
// =============================================================================

const CLASS_CATEGORIES = {
  genstep: {
    dirs: ["MapGen"],
    baseClasses: ["GenStep", "IGenStep"],
    description: "Generation pipeline steps - the building blocks of map generation",
    recursive: true
  },
  roomgen: {
    dirs: ["MapGen/Rooms"],
    baseClasses: ["RoomGen", "PermissiveRoomGen", "IRoomGen"],
    description: "Room shape generators - define how individual rooms are shaped",
    recursive: true
  },
  gridpath: {
    dirs: ["MapGen/Grid/Paths"],
    baseClasses: [], // Include all - GridPathStartStep, IGridPathBranch, etc.
    description: "Grid traversal algorithms - create room layouts on a grid",
    recursive: true
  },
  floorpath: {
    dirs: ["MapGen/FloorPlan/Paths"],
    baseClasses: [], // Include all - FloorPathStartStep, IFloorPathBranch, etc.
    description: "Floor plan path generators - create connected room layouts",
    recursive: true
  },
  spawning: {
    dirs: ["MapGen/Spawning"],
    baseClasses: [], // Include all spawning classes
    description: "Entity spawning - place items, enemies, stairs on maps",
    recursive: true
  },
  tiles: {
    dirs: ["MapGen/Tiles"],
    baseClasses: [], // Include all tile-related classes
    description: "Tile manipulation steps - water, terrain, cleanup operations",
    recursive: true
  },
  rand: {
    dirs: ["Rand"],
    baseClasses: [], // Include all public classes
    description: "RNG utilities - SpawnList, RandRange, noise generators",
    recursive: true
  },
  context: {
    dirs: ["MapGen"],
    baseClasses: ["IGenContext", "ITiledGenContext", "IFloorPlanGenContext", "IRoomGridGenContext", "IPlaceableGenContext", "IViewPlaceableGenContext", "IReplaceableGenContext", "ISpawningGenContext"],
    description: "Context interfaces - define map generation capabilities",
    recursive: true, // Changed: need to find interfaces in subdirs
    interfacesOnly: true
  },
  priority: {
    dirs: ["Priority"],
    baseClasses: [],
    description: "Priority system - ordering mechanism for generation steps",
    recursive: true
  },
  floorplan: {
    dirs: ["MapGen/FloorPlan"],
    baseClasses: [], // Include all - FloorPlan has no base class
    description: "Floor plan data structures - room and hallway management",
    recursive: false
  },
  gridplan: {
    dirs: ["MapGen/Grid"],
    baseClasses: [], // Include all - GridPlan has no base class
    description: "Grid plan data structures - grid-based room layout",
    recursive: false
  },
  core: {
    dirs: ["", "MapGen"],
    baseClasses: [], // Include all root-level utility classes
    description: "Core utilities - Loc, Rect, Grid, BlobMap, MapGen orchestrator",
    recursive: false
  }
} as const;

type ClassCategory = keyof typeof CLASS_CATEGORIES;

// =============================================================================
// CLASS DOCUMENTATION PARSING
// =============================================================================

interface ConstructorDoc {
  signature: string;
  parameters: Array<{ name: string; type: string; summary: string }>;
  summary: string;
}

interface ClassDoc {
  name: string;
  namespace: string;
  baseClass: string;
  interfaces: string[];
  summary: string;
  remarks: string;
  properties: Array<{ name: string; type: string; summary: string }>;
  methods: Array<{ name: string; signature: string; summary: string; returnType: string }>;
  constructors: ConstructorDoc[];
  filePath: string;
  isInterface: boolean;
  isStruct: boolean;
  isAbstract: boolean;
  isGeneric: boolean;
  genericParams: string;
}

async function parseClassFile(filePath: string): Promise<ClassDoc[]> {
  try {
    await initializeParser();
    if (!csharpParser) return [];

    let content = fs.readFileSync(filePath, "utf-8");

    // Strip UTF-8 BOM if present
    if (content.charCodeAt(0) === 0xFEFF) {
      content = content.slice(1);
    }

    const tree = csharpParser.parse(content);
    if (!tree) return [];

    const results: ClassDoc[] = [];

    // Extract namespace
    const namespaceDecls = findNodesByType(tree.rootNode, "namespace_declaration");
    let namespace = "";
    if (namespaceDecls.length > 0) {
      namespace = getFieldText(namespaceDecls[0], "name");
    }
    // Also check for file-scoped namespace
    const fileScopedNs = findNodesByType(tree.rootNode, "file_scoped_namespace_declaration");
    if (fileScopedNs.length > 0) {
      namespace = getFieldText(fileScopedNs[0], "name");
    }

    // Find all class, struct, and interface declarations
    const classDecls = findNodesByType(tree.rootNode, "class_declaration");
    const structDecls = findNodesByType(tree.rootNode, "struct_declaration");
    const interfaceDecls = findNodesByType(tree.rootNode, "interface_declaration");

    const allDecls = [
      ...classDecls.map(n => ({ node: n, isInterface: false, isStruct: false })),
      ...structDecls.map(n => ({ node: n, isInterface: false, isStruct: true })),
      ...interfaceDecls.map(n => ({ node: n, isInterface: true, isStruct: false }))
    ];

    for (const { node: declNode, isInterface, isStruct } of allDecls) {
      const className = getFieldText(declNode, "name");
      if (!className) continue;

      // Check for generic type parameters
      const typeParams = declNode.childForFieldName("type_parameters");
      const genericParams = typeParams ? typeParams.text : "";
      const isGeneric = !!typeParams;

      // Check modifiers for abstract/public
      let isAbstract = false;
      let isPublic = false;
      for (let i = 0; i < declNode.childCount; i++) {
        const child = declNode.child(i);
        if (child?.type === "modifier") {
          if (child.text === "abstract") isAbstract = true;
          if (child.text === "public") isPublic = true;
        }
      }

      // Skip non-public classes
      if (!isPublic) continue;

      // Get base class and interfaces from base_list
      const baseLists = findNodesByType(declNode, "base_list");
      let baseClass = "";
      const interfaces: string[] = [];

      if (baseLists.length > 0) {
        const baseList = baseLists[0];
        let isFirst = true;
        for (let i = 0; i < baseList.childCount; i++) {
          const child = baseList.child(i);
          if (child && child.type !== ":" && child.type !== ",") {
            // Use full type name including generic parameters
            const typeName = child.text.trim();
            if (isFirst && !isInterface) {
              // First item could be base class or interface
              // Check if it's an interface (starts with I followed by uppercase)
              const baseTypeName = typeName.replace(/<.*>/, ""); // Strip generics for interface check
              if (baseTypeName.startsWith("I") && baseTypeName.length > 1 && baseTypeName[1] === baseTypeName[1].toUpperCase()) {
                interfaces.push(typeName);
              } else {
                baseClass = typeName;
              }
              isFirst = false;
            } else {
              interfaces.push(typeName);
            }
          }
        }
      }

      // Get doc comments
      const classDoc = getDocComments(declNode);
      const { summary, remarks } = parseDocComment(classDoc);

      // Extract properties and fields
      const properties: ClassDoc["properties"] = [];

      // Fields
      const fieldDecls = findNodesByType(declNode, "field_declaration");
      for (const field of fieldDecls) {
        // Check if public
        let fieldPublic = false;
        for (let i = 0; i < field.childCount; i++) {
          const child = field.child(i);
          if (child?.type === "modifier" && child.text === "public") {
            fieldPublic = true;
            break;
          }
        }
        if (!fieldPublic) continue;

        const fieldDoc = getDocComments(field);
        const { summary: fieldSummary } = parseDocComment(fieldDoc);
        const fieldType = getFieldText(field, "type") || "unknown";

        const variableDeclarators = findNodesByType(field, "variable_declarator");
        for (const declarator of variableDeclarators) {
          const varName = getFieldText(declarator, "name") || declarator.text.split("=")[0].trim();
          properties.push({ name: varName, type: fieldType, summary: fieldSummary || "" });
        }
      }

      // Properties
      const propDecls = findNodesByType(declNode, "property_declaration");
      for (const prop of propDecls) {
        // Interface members are implicitly public - skip modifier check for interfaces
        let propPublic = isInterface;
        if (!propPublic) {
          for (let i = 0; i < prop.childCount; i++) {
            const child = prop.child(i);
            if (child?.type === "modifier" && child.text === "public") {
              propPublic = true;
              break;
            }
          }
        }
        if (!propPublic) continue;

        const propDoc = getDocComments(prop);
        const { summary: propSummary } = parseDocComment(propDoc);
        const propType = getFieldText(prop, "type") || "unknown";
        const propName = getFieldText(prop, "name");

        if (propName) {
          properties.push({ name: propName, type: propType, summary: propSummary || "" });
        }
      }

      // Extract constructors
      const constructors: ConstructorDoc[] = [];
      const constructorDecls = findNodesByType(declNode, "constructor_declaration");

      for (const ctor of constructorDecls) {
        // Check if public
        let ctorPublic = false;
        for (let i = 0; i < ctor.childCount; i++) {
          const child = ctor.child(i);
          if (child?.type === "modifier" && child.text === "public") {
            ctorPublic = true;
            break;
          }
        }
        if (!ctorPublic) continue;

        const ctorDoc = getDocComments(ctor);
        const { summary: ctorSummary } = parseDocComment(ctorDoc);

        // Extract parameter documentation from <param> tags
        const paramDocs: Record<string, string> = {};
        const paramMatches = ctorDoc.matchAll(/<param\s+name="([^"]+)">\s*([\s\S]*?)\s*<\/param>/g);
        for (const match of paramMatches) {
          paramDocs[match[1]] = match[2].replace(/^\s*\/\/\/\s*/gm, "").trim();
        }

        const paramsNode = ctor.childForFieldName("parameters");
        const params = paramsNode ? paramsNode.text : "()";

        // Parse individual parameters
        const parameters: Array<{ name: string; type: string; summary: string }> = [];
        if (paramsNode) {
          const paramNodes = findNodesByType(paramsNode, "parameter");
          for (const param of paramNodes) {
            const paramName = getFieldText(param, "name");
            const paramType = getFieldText(param, "type") || "unknown";
            parameters.push({
              name: paramName,
              type: paramType,
              summary: paramDocs[paramName] || ""
            });
          }
        }

        constructors.push({
          signature: `${className}${params}`,
          parameters,
          summary: ctorSummary || ""
        });
      }

      // Extract methods
      const methods: ClassDoc["methods"] = [];
      const methodDecls = findNodesByType(declNode, "method_declaration");

      for (const method of methodDecls) {
        // Interface members are implicitly public - skip modifier check for interfaces
        let methodPublic = isInterface;
        if (!methodPublic) {
          for (let i = 0; i < method.childCount; i++) {
            const child = method.child(i);
            if (child?.type === "modifier" && child.text === "public") {
              methodPublic = true;
              break;
            }
          }
        }
        if (!methodPublic) continue;

        const methodDoc = getDocComments(method);
        const { summary: methodSummary, inheritdoc } = parseDocComment(methodDoc);

        const methodName = getFieldText(method, "name");

        // Get return type - try multiple field names used by tree-sitter C#
        let returnType = getFieldText(method, "returns") ||
                         getFieldText(method, "return_type") ||
                         getFieldText(method, "type");

        // If still not found, look for the first type child before the method name
        if (!returnType) {
          for (let i = 0; i < method.childCount; i++) {
            const child = method.child(i);
            if (child?.type === "predefined_type" ||
                child?.type === "identifier" ||
                child?.type === "generic_name" ||
                child?.type === "qualified_name" ||
                child?.type === "nullable_type" ||
                child?.type === "array_type" ||
                child?.type === "void_keyword") {
              // Check if this is before the name
              const nameNode = method.childForFieldName("name");
              if (nameNode && child.endIndex < nameNode.startIndex) {
                returnType = child.text;
                break;
              }
            }
          }
        }

        if (!returnType) returnType = "void";

        const paramsNode = method.childForFieldName("parameters");
        const params = paramsNode ? paramsNode.text : "()";

        const signature = `${returnType} ${methodName}${params}`;

        methods.push({
          name: methodName,
          signature,
          summary: inheritdoc ? "(inherited)" : (methodSummary || ""),
          returnType
        });
      }

      results.push({
        name: className,
        namespace,
        baseClass,
        interfaces,
        summary,
        remarks,
        properties,
        methods,
        constructors,
        filePath,
        isInterface,
        isStruct,
        isAbstract,
        isGeneric,
        genericParams
      });
    }

    return results;
  } catch (err) {
    console.error(`Error parsing ${filePath}:`, err);
    return [];
  }
}

function matchesBaseClass(classDoc: ClassDoc, targetBaseClasses: string[]): boolean {
  if (targetBaseClasses.length === 0) return true; // Match all if no filter

  const { baseClass, interfaces, name } = classDoc;

  for (const target of targetBaseClasses) {
    // Direct base class match
    if (baseClass === target) return true;
    if (baseClass.replace(/<[^>]+>/, "") === target) return true;

    // Interface match
    for (const iface of interfaces) {
      if (iface === target) return true;
      if (iface.replace(/<[^>]+>/, "") === target) return true;
    }

    // Name contains target (for inheritance chains)
    if (baseClass.includes(target)) return true;

    // Self-match for interfaces
    if (classDoc.isInterface && name === target) return true;
  }

  return false;
}

async function findClassesInCategory(category: ClassCategory): Promise<ClassDoc[]> {
  const categoryInfo = CLASS_CATEGORIES[category];
  const classes: ClassDoc[] = [];
  const filesToParse: string[] = [];

  function collectFiles(dir: string, recursive: boolean) {
    if (!fs.existsSync(dir)) return;

    const entries = fs.readdirSync(dir, { withFileTypes: true });

    for (const entry of entries) {
      const fullPath = path.join(dir, entry.name);

      if (entry.isDirectory() && recursive && !entry.name.startsWith(".") && entry.name !== "obj" && entry.name !== "bin") {
        collectFiles(fullPath, recursive);
      } else if (entry.isFile() && entry.name.endsWith(".cs")) {
        filesToParse.push(fullPath);
      }
    }
  }

  for (const dir of categoryInfo.dirs) {
    const fullDir = path.join(ROGUE_DIR, dir);
    collectFiles(fullDir, categoryInfo.recursive);
  }

  for (const filePath of filesToParse) {
    const fileDocs = await parseClassFile(filePath);
    for (const classDoc of fileDocs) {
      // Filter by interfaces only if specified
      if ((categoryInfo as any).interfacesOnly && !classDoc.isInterface) continue;

      // Filter by base class
      if (matchesBaseClass(classDoc, [...categoryInfo.baseClasses])) {
        classes.push(classDoc);
      }
    }
  }

  return classes;
}

async function findClassByName(className: string): Promise<ClassDoc | null> {
  for (const category of Object.keys(CLASS_CATEGORIES) as ClassCategory[]) {
    const classes = await findClassesInCategory(category);
    const found = classes.find(c => c.name.toLowerCase() === className.toLowerCase());
    if (found) return found;
  }
  return null;
}

function levenshteinDistance(a: string, b: string): number {
  const matrix: number[][] = [];

  for (let i = 0; i <= b.length; i++) matrix[i] = [i];
  for (let j = 0; j <= a.length; j++) matrix[0][j] = j;

  for (let i = 1; i <= b.length; i++) {
    for (let j = 1; j <= a.length; j++) {
      if (b.charAt(i - 1) === a.charAt(j - 1)) {
        matrix[i][j] = matrix[i - 1][j - 1];
      } else {
        matrix[i][j] = Math.min(
          matrix[i - 1][j - 1] + 1,
          matrix[i][j - 1] + 1,
          matrix[i - 1][j] + 1
        );
      }
    }
  }

  return matrix[b.length][a.length];
}

// =============================================================================
// ENHANCED SEARCH SCORING
// =============================================================================

function calculateRelevanceScore(cls: ClassDoc, queryLower: string): number {
  const nameLower = cls.name.toLowerCase();
  const summaryLower = (cls.summary || "").toLowerCase();
  const queryWords = queryLower.split(/\s+/).filter(w => w.length >= 2);

  // Exact name match - best
  if (nameLower === queryLower) return 0;

  // Name starts with query
  if (nameLower.startsWith(queryLower)) return 10;

  // Name contains query
  if (nameLower.includes(queryLower)) return 20;

  // All query words in name (multi-word search)
  if (queryWords.length > 1 && queryWords.every(w => nameLower.includes(w))) {
    return 25;
  }

  // Summary contains query
  if (summaryLower.includes(queryLower)) return 50;

  // All query words in summary
  if (queryWords.length > 1 && queryWords.every(w => summaryLower.includes(w))) {
    return 55;
  }

  // Property or method name matches
  const propNames = cls.properties.map(p => p.name.toLowerCase()).join(" ");
  const methodNames = cls.methods.map(m => m.name.toLowerCase()).join(" ");
  if (propNames.includes(queryLower) || methodNames.includes(queryLower)) {
    return 70;
  }

  // Any query word appears anywhere
  const allText = `${nameLower} ${summaryLower} ${propNames} ${methodNames}`;
  if (queryWords.some(w => allText.includes(w))) return 80;

  // Fuzzy match (typo tolerance)
  const distance = levenshteinDistance(queryLower, nameLower);
  if (distance <= 3) return 100 + distance;

  return 1000; // No match
}

// =============================================================================
// EXAMPLES DISCOVERY
// =============================================================================

interface ExampleInfo {
  name: string;
  file: string;
  description: string;
  concepts: string[];
}

function findExamplesDir(): string {
  const candidates = [
    path.resolve(__dirname, "../../RogueElements.Examples"),
    path.resolve(__dirname, "../RogueElements.Examples"),
    path.resolve(process.cwd(), "RogueElements.Examples"),
  ];

  for (const dir of candidates) {
    if (fs.existsSync(dir)) return dir;
  }

  return path.resolve(process.cwd(), "RogueElements.Examples");
}

const EXAMPLES_DIR = findExamplesDir();

const EXAMPLES: ExampleInfo[] = [
  { name: "Ex1_Tiles", file: "Ex1_Tiles/Example1.cs", description: "Static tiles and InitTilesStep basics", concepts: ["InitTilesStep", "ITiledGenContext", "Priority"] },
  { name: "Ex2_Rooms", file: "Ex2_Rooms/Example2.cs", description: "Freeform rooms via FloorPlan", concepts: ["FloorPlan", "RoomGen", "AddConnectedRoomsStep"] },
  { name: "Ex3_Grid", file: "Ex3_Grid/Example3.cs", description: "Grid-based layouts via GridPlan", concepts: ["GridPlan", "GridPathBranch", "SetGridDefaultsStep"] },
  { name: "Ex4_Stairs", file: "Ex4_Stairs/Example4.cs", description: "Stair placement and spawning", concepts: ["StairsStep", "IPlaceableGenContext", "FloorStairsStep"] },
  { name: "Ex5_Terrain", file: "Ex5_Terrain/Example5.cs", description: "Water and terrain via Perlin noise", concepts: ["PerlinWaterStep", "BlobWaterStep", "ITile"] },
  { name: "Ex6_Items", file: "Ex6_Items/Example6.cs", description: "Item spawning and spawn lists", concepts: ["RandomSpawnStep", "SpawnList", "PickerSpawner"] },
  { name: "Ex7_Special", file: "Ex7_Special/Example7.cs", description: "Special room placement", concepts: ["SetGridSpecialRoomStep", "SetSpecialRoomStep", "ImmutableRoom"] },
  { name: "Ex8_Integration", file: "Ex8_Integration/Example8.cs", description: "Full pipeline combining all concepts", concepts: ["MapGen", "GenStep", "Full pipeline"] },
];

async function findSimilarClasses(searchName: string, limit: number = 5): Promise<Array<{ name: string; category: string; score: number }>> {
  const searchLower = searchName.toLowerCase();
  const allMatches: Array<{ name: string; category: string; score: number }> = [];

  for (const category of Object.keys(CLASS_CATEGORIES) as ClassCategory[]) {
    const classes = await findClassesInCategory(category);

    for (const cls of classes) {
      const nameLower = cls.name.toLowerCase();
      let score = levenshteinDistance(searchLower, nameLower);

      if (nameLower.includes(searchLower) || searchLower.includes(nameLower)) {
        score = Math.max(0, score - 5);
      }

      const searchParts = searchLower.replace(/step|gen|room|path|plan/gi, "").trim();
      if (searchParts && nameLower.includes(searchParts)) {
        score = Math.max(0, score - 3);
      }

      allMatches.push({ name: cls.name, category, score });
    }
  }

  return allMatches.sort((a, b) => a.score - b.score).slice(0, limit);
}

interface RelatedClass {
  name: string;
  category: string;
  relation: "same-base" | "same-interface" | "sibling" | "similar-name";
  summary: string;
}

async function findRelatedClasses(classDoc: ClassDoc, limit: number = 8): Promise<RelatedClass[]> {
  const related: RelatedClass[] = [];
  const seen = new Set<string>();
  seen.add(classDoc.name); // Don't include self

  for (const category of Object.keys(CLASS_CATEGORIES) as ClassCategory[]) {
    const classes = await findClassesInCategory(category);

    for (const cls of classes) {
      if (seen.has(cls.name)) continue;

      // Same base class (e.g., both inherit from GenStep<T>)
      if (classDoc.baseClass && cls.baseClass) {
        const baseA = classDoc.baseClass.replace(/<[^>]+>/, "");
        const baseB = cls.baseClass.replace(/<[^>]+>/, "");
        if (baseA === baseB && baseA !== "object") {
          related.push({
            name: cls.name,
            category,
            relation: "same-base",
            summary: cls.summary || ""
          });
          seen.add(cls.name);
          continue;
        }
      }

      // Shares an interface
      if (classDoc.interfaces.length > 0 && cls.interfaces.length > 0) {
        const docInterfaces = classDoc.interfaces.map(i => i.replace(/<[^>]+>/, ""));
        const clsInterfaces = cls.interfaces.map(i => i.replace(/<[^>]+>/, ""));
        const shared = docInterfaces.filter(i => clsInterfaces.includes(i));
        if (shared.length > 0) {
          related.push({
            name: cls.name,
            category,
            relation: "same-interface",
            summary: cls.summary || ""
          });
          seen.add(cls.name);
          continue;
        }
      }

      // Similar name pattern (e.g., RoomGenSquare and RoomGenRound)
      const docPrefix = classDoc.name.match(/^([A-Z][a-z]+(?:[A-Z][a-z]+)*?)(?=[A-Z][a-z]+$|Step$|Gen$)/)?.[1];
      const clsPrefix = cls.name.match(/^([A-Z][a-z]+(?:[A-Z][a-z]+)*?)(?=[A-Z][a-z]+$|Step$|Gen$)/)?.[1];
      if (docPrefix && clsPrefix && docPrefix === clsPrefix && docPrefix.length > 3) {
        related.push({
          name: cls.name,
          category,
          relation: "sibling",
          summary: cls.summary || ""
        });
        seen.add(cls.name);
        continue;
      }
    }
  }

  // Sort: same-base first, then same-interface, then sibling
  const priority: Record<RelatedClass["relation"], number> = {
    "same-base": 0,
    "same-interface": 1,
    "sibling": 2,
    "similar-name": 3
  };

  return related
    .sort((a, b) => priority[a.relation] - priority[b.relation])
    .slice(0, limit);
}

// =============================================================================
// MCP SERVER SETUP
// =============================================================================

const server = new McpServer({
  name: "rogueelemens-mcp-server",
  version: "2.0.0"
});

// =============================================================================
// RESOURCES - Documentation files
// =============================================================================

const DOC_FILES = ["architecture", "flows", "patterns"] as const;

for (const docName of DOC_FILES) {
  server.resource(
    `rogue-docs-${docName}`,
    `rogue://docs/${docName}`,
    async () => {
      const filePath = path.join(DOCS_DIR, `${docName}.md`);
      try {
        const content = fs.readFileSync(filePath, "utf-8");
        return {
          contents: [{
            uri: `rogue://docs/${docName}`,
            mimeType: "text/markdown",
            text: content
          }]
        };
      } catch {
        return {
          contents: [{
            uri: `rogue://docs/${docName}`,
            mimeType: "text/plain",
            text: `Error: Could not read ${docName}.md`
          }]
        };
      }
    }
  );
}

// =============================================================================
// RESOURCES - Class categories (browsable)
// =============================================================================

for (const [category, info] of Object.entries(CLASS_CATEGORIES)) {
  server.resource(
    `rogue-classes-${category}`,
    `rogue://classes/${category}`,
    async () => {
      const classes = await findClassesInCategory(category as ClassCategory);

      const lines = [
        `# ${category} Classes`,
        "",
        `**Description:** ${info.description}`,
        `**Base Classes:** ${info.baseClasses.length > 0 ? info.baseClasses.map(b => `\`${b}\``).join(", ") : "(all public classes)"}`,
        `**Directories:** ${info.dirs.map(d => `\`RogueElements/${d}\``).join(", ")}`,
        `**Count:** ${classes.length}`,
        "",
        "## Classes",
        ""
      ];

      for (const cls of classes) {
        const typeLabel = cls.isInterface ? "[interface]" : cls.isAbstract ? "[abstract]" : "";
        const genericLabel = cls.isGeneric ? cls.genericParams : "";
        lines.push(`### ${cls.name}${genericLabel} ${typeLabel}`);
        if (cls.summary) lines.push(cls.summary);
        if (cls.baseClass) lines.push(`- **Base:** \`${cls.baseClass}\``);
        if (cls.interfaces.length > 0) {
          lines.push(`- **Implements:** ${cls.interfaces.map(i => `\`${i}\``).join(", ")}`);
        }
        if (cls.properties.length > 0) {
          const propNames = cls.properties.slice(0, 5).map(p => p.name).join(", ");
          const more = cls.properties.length > 5 ? `, ... (+${cls.properties.length - 5} more)` : "";
          lines.push(`- **Properties:** ${propNames}${more}`);
        }
        lines.push("");
      }

      return {
        contents: [{
          uri: `rogue://classes/${category}`,
          mimeType: "text/markdown",
          text: lines.join("\n")
        }]
      };
    }
  );
}

// =============================================================================
// PROMPTS
// =============================================================================

server.prompt(
  "create_roomgen",
  "Guide for creating a custom RoomGen in RogueElements",
  () => ({
    messages: [{
      role: "user",
      content: {
        type: "text",
        text: `Help me create a custom RoomGen for RogueElements.

A RoomGen defines the shape of a room. Here's what I need to implement:

1. Inherit from \`RoomGen<T>\` or \`PermissiveRoomGen<T>\` where T : ITiledGenContext
2. Override \`ProposeSize(IRandom rand)\` to return room dimensions
3. Override \`DrawOnMap(T map)\` to render the room shape to tiles
4. Implement \`Copy()\` for cloning

Use \`rogue_list_classes\` with category "roomgen" to see existing examples.

What shape of room would you like to create?`
      }
    }]
  })
);

server.prompt(
  "create_genstep",
  "Guide for creating a custom GenStep in RogueElements",
  () => ({
    messages: [{
      role: "user",
      content: {
        type: "text",
        text: `Help me create a custom GenStep for RogueElements.

A GenStep is a single step in the map generation pipeline. Here's what I need to implement:

1. Inherit from \`GenStep<T>\` where T constrains to required context interfaces
2. Add \`[Serializable]\` attribute for save/load support
3. Override \`Apply(T map)\` with generation logic
4. Add to pipeline with \`layout.GenSteps.Add(new Priority(N), step)\`

Common context interfaces:
- \`ITiledGenContext\` - Basic tile operations
- \`IFloorPlanGenContext\` - Room-based layouts
- \`IRoomGridGenContext\` - Grid-based layouts
- \`IPlaceableGenContext<T>\` - Entity spawning

Use \`rogue_list_classes\` with category "genstep" to see existing examples.

What kind of generation step would you like to create?`
      }
    }]
  })
);

server.prompt(
  "create_spawnable",
  "Guide for creating a custom spawnable type in RogueElements",
  () => ({
    messages: [{
      role: "user",
      content: {
        type: "text",
        text: `Help me create a custom spawnable type for RogueElements.

A spawnable is any entity that can be placed on the map (items, enemies, traps, etc.).

Steps:
1. Create a class implementing \`ISpawnable\` (just needs \`Copy()\` method)
2. Add \`IPlaceableGenContext<YourType>\` to your map context
3. Use spawn steps like \`RandomSpawnStep\` to place them

Use \`rogue_list_classes\` with category "spawning" to see existing spawning infrastructure.

What kind of entity would you like to spawn?`
      }
    }]
  })
);

server.prompt(
  "create_context",
  "Guide for creating a custom map context in RogueElements",
  () => ({
    messages: [{
      role: "user",
      content: {
        type: "text",
        text: `Help me create a custom map context for RogueElements.

The map context holds all state during generation. Implement interfaces based on what GenSteps you need:

Interface hierarchy:
\`\`\`
IGenContext (base - Rand, InitSeed, FinishGen)
├── ITiledGenContext (tiles - Width, Height, SetTile, GetTile)
│   └── IFloorPlanGenContext (rooms - RoomPlan, InitPlan)
│       └── IRoomGridGenContext (grid - GridPlan, InitGrid)
└── IPlaceableGenContext<T> (spawning - PlaceItem, GetFreeTiles)
\`\`\`

Use \`rogue_list_classes\` with category "context" to see the interface definitions.

What capabilities does your context need?`
      }
    }]
  })
);

// =============================================================================
// TOOLS - Search & Browse
// =============================================================================

server.tool(
  "rogue_search",
  `Search for RogueElements classes across all categories by name or summary.

Searches class names and XML documentation summaries. Returns matches ranked by relevance.
Use this when you're not sure which category a class belongs to, or to find classes related to a concept.

Categories searched: ${Object.keys(CLASS_CATEGORIES).join(", ")}`,
  {
    query: z.string()
      .min(2)
      .describe("Search query (e.g., 'water', 'spawn', 'grid', 'room')"),
    limit: z.number()
      .min(1)
      .max(50)
      .default(10)
      .describe("Maximum results to return"),
    response_format: z.enum(["markdown", "json"])
      .default("markdown")
      .describe("Output format: markdown (default) or json")
  },
  async ({ query, limit, response_format }) => {
    const queryLower = query.toLowerCase();
    const results: Array<{
      name: string;
      category: ClassCategory;
      summary: string;
      score: number;
      isInterface: boolean;
      isStruct: boolean;
    }> = [];

    for (const category of Object.keys(CLASS_CATEGORIES) as ClassCategory[]) {
      const classes = await findClassesInCategory(category);

      for (const cls of classes) {
        const score = calculateRelevanceScore(cls, queryLower);
        if (score < 1000) {
          results.push({
            name: cls.name,
            category,
            summary: cls.summary || "(no documentation)",
            score,
            isInterface: cls.isInterface,
            isStruct: cls.isStruct
          });
        }
      }
    }

    const sorted = results.sort((a, b) => a.score - b.score).slice(0, limit);

    if (sorted.length === 0) {
      return {
        content: [{
          type: "text",
          text: `No classes found matching '${query}'. Try a different search term or use rogue_list_classes to browse by category.`
        }]
      };
    }

    if (response_format === "json") {
      return {
        content: [{
          type: "text",
          text: JSON.stringify({
            query,
            count: sorted.length,
            results: sorted.map(r => ({
              name: r.name,
              category: r.category,
              type: r.isInterface ? "interface" : r.isStruct ? "struct" : "class",
              summary: r.summary
            }))
          }, null, 2)
        }]
      };
    }

    const lines = [
      `# Search Results: "${query}"`,
      "",
      `Found ${sorted.length} matching classes:`,
      "",
      "| Class | Category | Type | Summary |",
      "|-------|----------|------|---------|"
    ];

    for (const result of sorted) {
      const summary = result.summary.length > 50
        ? result.summary.substring(0, 47) + "..."
        : result.summary;
      const typeLabel = result.isInterface ? "interface" : result.isStruct ? "struct" : "class";
      lines.push(`| \`${result.name}\` | ${result.category} | ${typeLabel} | ${summary} |`);
    }

    lines.push("");
    lines.push("*Use `rogue_get_class_docs` for full documentation on any class.*");

    return {
      content: [{ type: "text", text: lines.join("\n") }]
    };
  }
);

server.tool(
  "rogue_list_classes",
  `List all RogueElements classes in a specific category.

Categories: ${Object.keys(CLASS_CATEGORIES).join(", ")}

Returns class names with brief summaries from XML documentation.`,
  {
    category: z.enum(Object.keys(CLASS_CATEGORIES) as [ClassCategory, ...ClassCategory[]])
      .describe("Category of classes to list"),
    limit: z.number()
      .min(1)
      .max(100)
      .default(50)
      .describe("Maximum results to return (default 50)"),
    offset: z.number()
      .min(0)
      .default(0)
      .describe("Number of results to skip (for pagination)"),
    response_format: z.enum(["markdown", "json"])
      .default("markdown")
      .describe("Output format: markdown (default) or json")
  },
  async ({ category, limit, offset, response_format }) => {
    const allClasses = await findClassesInCategory(category);
    const categoryInfo = CLASS_CATEGORIES[category];

    if (allClasses.length === 0) {
      return {
        content: [{
          type: "text",
          text: `No classes found in category '${category}' (searched ${categoryInfo.dirs.join(", ")})`
        }]
      };
    }

    const classes = allClasses.slice(offset, offset + limit);
    const hasMore = offset + limit < allClasses.length;

    if (response_format === "json") {
      return {
        content: [{
          type: "text",
          text: JSON.stringify({
            category,
            description: categoryInfo.description,
            total: allClasses.length,
            offset,
            limit,
            hasMore,
            nextOffset: hasMore ? offset + limit : null,
            classes: classes.map(c => ({
              name: c.name + (c.isGeneric ? c.genericParams : ""),
              type: c.isInterface ? "interface" : c.isStruct ? "struct" : c.isAbstract ? "abstract" : "class",
              baseClass: c.baseClass || null,
              summary: c.summary || null
            }))
          }, null, 2)
        }]
      };
    }

    const lines = [
      `# ${category} Classes`,
      "",
      `**Description:** ${categoryInfo.description}`,
      `**Total:** ${allClasses.length}${offset > 0 ? ` (showing ${offset + 1}-${Math.min(offset + limit, allClasses.length)})` : ""}`,
      "",
      "| Class | Type | Summary |",
      "|-------|------|---------|"
    ];

    for (const cls of classes) {
      const summary = cls.summary ? cls.summary.substring(0, 60) : "(no docs)";
      const typeLabel = cls.isInterface ? "interface" : cls.isStruct ? "struct" : cls.isAbstract ? "abstract" : "class";
      const generic = cls.isGeneric ? cls.genericParams : "";
      lines.push(`| \`${cls.name}${generic}\` | ${typeLabel} | ${summary} |`);
    }

    if (hasMore) {
      lines.push("");
      lines.push(`*More results available. Use offset=${offset + limit} for next page.*`);
    }

    return {
      content: [{ type: "text", text: lines.join("\n") }]
    };
  }
);

server.tool(
  "rogue_get_class_docs",
  `Get detailed XML documentation for a specific RogueElements class or interface.

Extracts from C# source files:
- Class summary and remarks
- Namespace and base class
- Implemented interfaces
- Public properties with their types and documentation
- Public methods with their signatures and documentation`,
  {
    class_name: z.string()
      .min(1)
      .describe("Name of the class or interface to get documentation for"),
    response_format: z.enum(["markdown", "json"])
      .default("markdown")
      .describe("Output format: markdown (default) or json")
  },
  async ({ class_name, response_format }) => {
    const classDoc = await findClassByName(class_name);

    if (!classDoc) {
      const suggestions = await findSimilarClasses(class_name, 5);

      let errorMsg = `Class '${class_name}' not found in RogueElements.\n\n`;

      if (suggestions.length > 0) {
        errorMsg += "**Did you mean one of these?**\n\n";
        for (const suggestion of suggestions) {
          errorMsg += `- \`${suggestion.name}\` (${suggestion.category})\n`;
        }
        errorMsg += "\n*Use `rogue_search` for broader search or `rogue_list_classes` to browse by category.*";
      }

      return {
        content: [{ type: "text", text: errorMsg }]
      };
    }

    // Find related classes
    const relatedClasses = await findRelatedClasses(classDoc);

    if (response_format === "json") {
      return {
        content: [{
          type: "text",
          text: JSON.stringify({
            name: classDoc.name,
            genericParams: classDoc.genericParams || null,
            type: classDoc.isInterface ? "interface" : classDoc.isStruct ? "struct" : classDoc.isAbstract ? "abstract" : "class",
            namespace: classDoc.namespace,
            baseClass: classDoc.baseClass || null,
            interfaces: classDoc.interfaces,
            file: classDoc.filePath.replace(ROGUE_DIR, "RogueElements"),
            summary: classDoc.summary || null,
            remarks: classDoc.remarks || null,
            constructors: classDoc.constructors,
            properties: classDoc.properties,
            methods: classDoc.methods,
            relatedClasses: relatedClasses.map(r => ({
              name: r.name,
              category: r.category,
              relation: r.relation
            }))
          }, null, 2)
        }]
      };
    }

    const typeLabel = classDoc.isInterface ? "interface" : classDoc.isStruct ? "struct" : classDoc.isAbstract ? "abstract class" : "class";
    const generic = classDoc.isGeneric ? classDoc.genericParams : "";

    const lines = [
      `# ${classDoc.name}${generic}`,
      "",
      `**Type:** ${typeLabel}`,
      `**Namespace:** \`${classDoc.namespace}\``,
    ];

    if (classDoc.baseClass) {
      lines.push(`**Base Class:** \`${classDoc.baseClass}\``);
    }
    if (classDoc.interfaces.length > 0) {
      lines.push(`**Implements:** ${classDoc.interfaces.map(i => `\`${i}\``).join(", ")}`);
    }
    lines.push(`**File:** \`${classDoc.filePath.replace(ROGUE_DIR, "RogueElements")}\``);
    lines.push("");

    if (classDoc.summary) {
      lines.push("## Summary", "", classDoc.summary, "");
    }

    if (classDoc.remarks) {
      lines.push("## Remarks", "", classDoc.remarks, "");
    }

    if (classDoc.constructors.length > 0) {
      lines.push("## Constructors", "");
      for (const ctor of classDoc.constructors) {
        lines.push(`### \`${ctor.signature}\``);
        if (ctor.summary) lines.push(ctor.summary);
        if (ctor.parameters.length > 0) {
          lines.push("");
          lines.push("**Parameters:**");
          for (const param of ctor.parameters) {
            const paramDoc = param.summary ? ` - ${param.summary}` : "";
            lines.push(`- \`${param.name}\`: \`${param.type}\`${paramDoc}`);
          }
        }
        lines.push("");
      }
    }

    if (classDoc.properties.length > 0) {
      lines.push("## Properties", "");
      for (const prop of classDoc.properties) {
        lines.push(`### \`${prop.name}\` : \`${prop.type}\``);
        if (prop.summary) lines.push(prop.summary);
        lines.push("");
      }
    }

    if (classDoc.methods.length > 0) {
      lines.push("## Methods", "");
      for (const method of classDoc.methods) {
        lines.push(`### \`${method.signature}\``);
        if (method.summary) lines.push(method.summary);
        lines.push("");
      }
    }

    if (relatedClasses.length > 0) {
      lines.push("## Related Classes", "");
      lines.push("| Class | Category | Relation |");
      lines.push("|-------|----------|----------|");
      for (const related of relatedClasses) {
        const relationLabel = related.relation === "same-base" ? "Same base class" :
                              related.relation === "same-interface" ? "Implements same interface" :
                              related.relation === "sibling" ? "Same family" : "Similar name";
        lines.push(`| \`${related.name}\` | ${related.category} | ${relationLabel} |`);
      }
      lines.push("");
      lines.push("*Use `rogue_get_class_docs` to get details on any related class.*");
    }

    return {
      content: [{ type: "text", text: lines.join("\n") }]
    };
  }
);

// =============================================================================
// TOOLS - Documentation Access
// =============================================================================

const DOC_DESCRIPTIONS: Record<string, string> = {
  architecture: "Interface hierarchy, GenStep categories, data flow diagrams, and priority conventions",
  flows: "Traced code paths for key operations like map generation and room placement",
  patterns: "Step-by-step recipes for common modifications and custom implementations"
};

server.tool(
  "rogue_get_docs",
  `Access AI-optimized documentation for RogueElements.

Available documents:
- architecture: ${DOC_DESCRIPTIONS.architecture}
- flows: ${DOC_DESCRIPTIONS.flows}
- patterns: ${DOC_DESCRIPTIONS.patterns}

Omit the name parameter to list all available docs.`,
  {
    name: z.string()
      .optional()
      .describe("Document name (architecture, flows, patterns). Omit to list all.")
  },
  async ({ name }) => {
    if (!name) {
      const lines = [
        "# RogueElements Documentation",
        "",
        "AI-optimized reference documentation for the RogueElements library.",
        "",
        "| Document | Description |",
        "|----------|-------------|"
      ];
      for (const [docName, desc] of Object.entries(DOC_DESCRIPTIONS)) {
        lines.push(`| \`${docName}\` | ${desc} |`);
      }
      lines.push("");
      lines.push("*Use `rogue_get_docs` with a name to read a specific document.*");
      return { content: [{ type: "text", text: lines.join("\n") }] };
    }

    const docPath = path.join(DOCS_DIR, `${name}.md`);
    if (!fs.existsSync(docPath)) {
      return {
        content: [{
          type: "text",
          text: `Document '${name}' not found.\n\nAvailable documents: ${Object.keys(DOC_DESCRIPTIONS).join(", ")}`
        }]
      };
    }

    const content = fs.readFileSync(docPath, "utf-8");
    return { content: [{ type: "text", text: content }] };
  }
);

server.tool(
  "rogue_get_example",
  `Get annotated source code from the RogueElements.Examples project.

Examples demonstrate progressive complexity:
${EXAMPLES.map(e => `- ${e.name}: ${e.description}`).join("\n")}

Use this to see real implementation patterns.`,
  {
    name: z.string()
      .optional()
      .describe("Example name (Ex1_Tiles, Ex2_Rooms, etc.). Omit to list all."),
    concept: z.string()
      .optional()
      .describe("Search for examples using a specific concept (e.g., 'GridPlan', 'spawning')")
  },
  async ({ name, concept }) => {
    // If searching by concept
    if (concept) {
      const conceptLower = concept.toLowerCase();
      const matches = EXAMPLES.filter(e =>
        e.concepts.some(c => c.toLowerCase().includes(conceptLower)) ||
        e.description.toLowerCase().includes(conceptLower) ||
        e.name.toLowerCase().includes(conceptLower)
      );

      if (matches.length === 0) {
        return {
          content: [{
            type: "text",
            text: `No examples found for concept '${concept}'.\n\nAvailable examples: ${EXAMPLES.map(e => e.name).join(", ")}`
          }]
        };
      }

      const lines = [
        `# Examples for "${concept}"`,
        "",
        "| Example | Description | Related Concepts |",
        "|---------|-------------|------------------|"
      ];
      for (const e of matches) {
        lines.push(`| \`${e.name}\` | ${e.description} | ${e.concepts.join(", ")} |`);
      }
      lines.push("");
      lines.push("*Use `rogue_get_example` with a specific name to read the source code.*");
      return { content: [{ type: "text", text: lines.join("\n") }] };
    }

    // List all examples
    if (!name) {
      const lines = [
        "# RogueElements Examples",
        "",
        "Progressive examples from basic to advanced:",
        "",
        "| Example | Description | Key Concepts |",
        "|---------|-------------|--------------|"
      ];
      for (const e of EXAMPLES) {
        lines.push(`| \`${e.name}\` | ${e.description} | ${e.concepts.join(", ")} |`);
      }
      lines.push("");
      lines.push("*Use `rogue_get_example` with a name to read source code, or use `concept` to search.*");
      return { content: [{ type: "text", text: lines.join("\n") }] };
    }

    // Find and return specific example
    const example = EXAMPLES.find(e => e.name.toLowerCase() === name.toLowerCase());
    if (!example) {
      return {
        content: [{
          type: "text",
          text: `Example '${name}' not found.\n\nAvailable examples: ${EXAMPLES.map(e => e.name).join(", ")}`
        }]
      };
    }

    const examplePath = path.join(EXAMPLES_DIR, example.file);
    if (!fs.existsSync(examplePath)) {
      return {
        content: [{
          type: "text",
          text: `Example file '${example.file}' not found at ${examplePath}`
        }]
      };
    }

    const content = fs.readFileSync(examplePath, "utf-8");
    const lines = [
      `# ${example.name}`,
      "",
      `**Description:** ${example.description}`,
      `**Key Concepts:** ${example.concepts.join(", ")}`,
      "",
      "## Source Code",
      "",
      "```csharp",
      content,
      "```"
    ];

    return { content: [{ type: "text", text: lines.join("\n") }] };
  }
);

// =============================================================================
// TOOLS - Code Generation
// =============================================================================

server.tool(
  "rogue_scaffold_roomgen",
  `Generate boilerplate code for a custom RogueElements RoomGen.

Creates a properly structured class with:
- Serializable attribute for save/load
- Configurable properties with proper Clone() support
- ProposeSize() and DrawOnMap() methods
- Example shape logic`,
  {
    name: z.string()
      .min(1)
      .describe("Name for the RoomGen class (e.g., 'Diamond', 'Cross')"),
    shape_description: z.string()
      .describe("Description of the room shape to generate"),
    has_properties: z.boolean()
      .default(true)
      .describe("Include configurable size properties")
  },
  async ({ name, shape_description, has_properties }) => {
    const className = `RoomGen${name}`;
    const code = `using System;
using RogueElements;

namespace YourNamespace
{
    /// <summary>
    /// Generates ${shape_description.toLowerCase()} shaped rooms.
    /// </summary>
    [Serializable]
    public class ${className}<T> : RoomGen<T>
        where T : ITiledGenContext
    {${has_properties ? `
        /// <summary>
        /// Range of possible room widths.
        /// </summary>
        public RandRange Width { get; set; }

        /// <summary>
        /// Range of possible room heights.
        /// </summary>
        public RandRange Height { get; set; }

        public ${className}()
        {
            this.Width = new RandRange(5, 10);
            this.Height = new RandRange(5, 10);
        }

        public ${className}(RandRange width, RandRange height)
        {
            this.Width = width;
            this.Height = height;
        }

        protected ${className}(${className}<T> other)
        {
            this.Width = other.Width;
            this.Height = other.Height;
        }

        public override RoomGen<T> Copy() => new ${className}<T>(this);
` : `
        public override RoomGen<T> Copy() => new ${className}<T>();
`}
        public override Loc ProposeSize(IRandom rand)
        {${has_properties ? `
            return new Loc(this.Width.Pick(rand), this.Height.Pick(rand));` : `
            // TODO: Return appropriate dimensions for ${shape_description}
            return new Loc(rand.Next(5, 10), rand.Next(5, 10));`}
        }

        public override void DrawOnMap(T map)
        {
            // this.Draw contains the room bounds (X, Y, Width, Height, End)
            int centerX = this.Draw.X + this.Draw.Width / 2;
            int centerY = this.Draw.Y + this.Draw.Height / 2;

            for (int x = this.Draw.X; x < this.Draw.End.X; x++)
            {
                for (int y = this.Draw.Y; y < this.Draw.End.Y; y++)
                {
                    Loc loc = new Loc(x, y);

                    // TODO: Implement ${shape_description} shape logic
                    // Example: check distance from center, edges, etc.
                    bool inShape = true; // Replace with your shape condition

                    if (inShape)
                    {
                        map.SetTile(loc, map.RoomTerrain.Copy());
                    }
                }
            }

            // Draw border tiles for fulfillables
            this.SetRoomBorders(map);
        }
    }
}`;

    return {
      content: [{
        type: "text",
        text: `# Generated RoomGen: ${className}

\`\`\`csharp
${code}
\`\`\`

## Next Steps

1. **Implement shape logic** in DrawOnMap():
   - Use \`centerX\`/\`centerY\` for radial shapes
   - Use \`this.Draw\` bounds for edge-based shapes
   - Set \`inShape\` condition for your pattern

2. **Add to room pool**:
   \`\`\`csharp
   var roomGen = new SpawnList<RoomGen<MyContext>>();
   roomGen.Add(new ${className}<MyContext>(new RandRange(5, 8), new RandRange(5, 8)), 10);
   \`\`\`

3. **Use with grid or floor paths**:
   \`\`\`csharp
   layout.GenSteps.Add(new Priority(15), new SetGridDefaultsStep<MyContext>(roomGen));
   \`\`\`

*See \`rogue_get_example\` with name "Ex3_Grid" for complete usage.*`
      }]
    };
  }
);

server.tool(
  "rogue_scaffold_genstep",
  `Generate boilerplate code for a custom RogueElements GenStep.

Creates a properly structured class with:
- Serializable attribute for save/load
- Configurable properties with proper Clone() support
- Context-specific API comments
- Example iteration patterns`,
  {
    name: z.string()
      .min(1)
      .describe("Name for the GenStep class (e.g., 'AddPillars', 'ScatterItems')"),
    context_type: z.enum(["ITiledGenContext", "IFloorPlanGenContext", "IRoomGridGenContext"])
      .describe("The context interface this step requires"),
    description: z.string()
      .describe("What this generation step does"),
    has_properties: z.boolean()
      .default(true)
      .describe("Include configurable properties")
  },
  async ({ name, context_type, description, has_properties }) => {
    // Don't duplicate "Step" suffix if already present
    const className = name.endsWith("Step") ? name : `${name}Step`;
    const code = `using System;
using RogueElements;

namespace YourNamespace
{
    /// <summary>
    /// ${description}
    /// </summary>
    [Serializable]
    public class ${className}<T> : GenStep<T>
        where T : class, ${context_type}
    {${has_properties ? `
        /// <summary>
        /// Probability of applying the effect (0-100).
        /// </summary>
        public int Chance { get; set; }

        /// <summary>
        /// Terrain to use for the effect.
        /// </summary>
        public ITile Terrain { get; set; }

        public ${className}()
        {
            this.Chance = 50;
        }

        public ${className}(ITile terrain, int chance = 50)
        {
            this.Terrain = terrain;
            this.Chance = chance;
        }
` : ""}
        public override void Apply(T map)
        {
            // Available from ITiledGenContext:
            // - map.Rand: Seeded random number generator
            // - map.Width, map.Height: Map dimensions
            // - map.SetTile(loc, tile): Place a tile
            // - map.GetTile(loc): Get tile at location
            // - map.TileBlocked(loc): Check if wall
            // - map.RoomTerrain, map.WallTerrain: Terrain templates
${context_type === "IFloorPlanGenContext" || context_type === "IRoomGridGenContext" ? `
            // Available from IFloorPlanGenContext:
            // - map.RoomPlan: The floor plan with rooms and halls
            // - map.RoomPlan.RoomCount: Number of rooms
            // - map.RoomPlan.GetRoom(index): Get room by index
            // - map.RoomPlan.GetRoomGen(index): Get the RoomGen that created it
` : ""}${context_type === "IRoomGridGenContext" ? `
            // Available from IRoomGridGenContext:
            // - map.GridPlan: The grid plan
            // - map.GridPlan.GridWidth, GridHeight: Grid dimensions
            // - map.GridPlan.GetRoom(Loc): Get room at grid position
` : ""}
${context_type === "IFloorPlanGenContext" || context_type === "IRoomGridGenContext" ? `
            // Iterate over rooms
            for (int i = 0; i < map.RoomPlan.RoomCount; i++)
            {
                IRoomPlan room = map.RoomPlan.GetRoom(i);
                Rect bounds = room.RoomGen.Draw;

                // Process tiles in this room
                for (int x = bounds.X; x < bounds.End.X; x++)
                {
                    for (int y = bounds.Y; y < bounds.End.Y; y++)
                    {
                        Loc loc = new Loc(x, y);
                        if (!map.TileBlocked(loc, false))
                        {
                            ${has_properties ? `if (map.Rand.Next(100) < this.Chance)
                            {
                                // TODO: Apply effect
                            }` : "// TODO: Apply effect"}
                        }
                    }
                }
            }` : `
            // Iterate over all tiles
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    Loc loc = new Loc(x, y);
                    if (!map.TileBlocked(loc, false))
                    {
                        ${has_properties ? `if (map.Rand.Next(100) < this.Chance)
                        {
                            // TODO: Apply effect
                        }` : "// TODO: Apply effect"}
                    }
                }
            }`}
        }
    }
}`;

    const priorityHint = context_type === "IRoomGridGenContext" ? "10-29" :
                         context_type === "IFloorPlanGenContext" ? "30-59" : "60-89";

    return {
      content: [{
        type: "text",
        text: `# Generated GenStep: ${className}

\`\`\`csharp
${code}
\`\`\`

## Next Steps

1. **Implement your logic** in Apply():
   - Use \`map.Rand\` for randomness
   - Use \`map.SetTile(loc, tile)\` to modify terrain
   - Check \`map.TileBlocked()\` to avoid walls

2. **Add to pipeline** (priority ${priorityHint} for ${context_type}):
   \`\`\`csharp
   layout.GenSteps.Add(new Priority(${priorityHint.split("-")[0]}), new ${className}<MyContext>());
   \`\`\`

3. **Priority guide**:
   - 0-9: Initialization
   - 10-29: Grid operations
   - 30-59: Floor plan operations
   - 60-89: Tile modifications
   - 90-99: Entity spawning

*See \`rogue_get_docs\` with name "architecture" for full priority conventions.*`
      }]
    };
  }
);

server.tool(
  "rogue_list_interfaces",
  "List RogueElements context interfaces with their capabilities",
  {},
  async () => {
    const interfaces = `# RogueElements Context Interfaces

## Interface Hierarchy

\`\`\`
IGenContext (base)
├── ITiledGenContext
│   └── IFloorPlanGenContext
│       └── IRoomGridGenContext
├── IPlaceableGenContext<T>
│   └── IViewPlaceableGenContext<T>
│       └── IReplaceableGenContext<T>
└── ISpawningGenContext<T>
\`\`\`

## Interface Details

### IGenContext
Base interface for all contexts.
- \`IRandom Rand\` - Seeded random number generator
- \`void InitSeed(ulong seed)\` - Initialize with seed
- \`void FinishGen()\` - Called after generation completes

### ITiledGenContext : IGenContext
Tile-based map operations.
- \`int Width, Height\` - Map dimensions
- \`ITile RoomTerrain, WallTerrain\` - Terrain templates
- \`bool TilesInitialized\` - Whether tiles are ready
- \`void SetTile(Loc, ITile)\` - Place a tile
- \`ITile GetTile(Loc)\` - Get tile at location
- \`bool TileBlocked(Loc)\` - Check if blocked
- \`void CreateNew(width, height)\` - Create tile array

### IFloorPlanGenContext : ITiledGenContext
Room-based layouts using FloorPlan.
- \`FloorPlan RoomPlan\` - The floor plan
- \`void InitPlan(FloorPlan)\` - Initialize with plan

### IRoomGridGenContext : IFloorPlanGenContext
Grid-based layouts using GridPlan.
- \`GridPlan GridPlan\` - The grid plan
- \`void InitGrid(GridPlan)\` - Initialize with plan

### IPlaceableGenContext<T> : IGenContext
Entity spawning for type T.
- \`List<Loc> GetAllFreeTiles()\` - Find spawn locations
- \`List<Loc> GetFreeTiles(Rect)\` - Find in area
- \`bool CanPlaceItem(Loc)\` - Check if valid
- \`void PlaceItem(Loc, T)\` - Place entity

### IViewPlaceableGenContext<T> : IPlaceableGenContext<T>
Adds ability to query placed items.
- \`int Count\` - Number of placed items
- \`T GetItem(int index)\` - Get item by index
- \`Loc GetLoc(int index)\` - Get location by index

### IReplaceableGenContext<T> : IViewPlaceableGenContext<T>
Adds ability to remove items.
- \`void RemoveItemAt(int index)\` - Remove item

### ISpawningGenContext<T> : IGenContext
Weighted spawn lists.
- \`SpawnList<T> Spawns\` - Spawn table
`;

    return {
      content: [{ type: "text", text: interfaces }]
    };
  }
);

server.tool(
  "rogue_scaffold_spawnable",
  `Generate boilerplate code for a custom spawnable entity and spawn step.

Creates:
- A spawnable entity class implementing ISpawnable
- A spawn step using IPlaceableGenContext
- Integration example with SpawnList`,
  {
    name: z.string()
      .min(1)
      .describe("Name for the spawnable entity (e.g., 'Treasure', 'Enemy', 'Trap')"),
    description: z.string()
      .describe("What this spawnable represents"),
    spawn_type: z.enum(["random", "terminal", "room"])
      .default("random")
      .describe("Spawn placement strategy: random (anywhere), terminal (dead ends), room (per-room)")
  },
  async ({ name, description, spawn_type }) => {
    const entityClass = name;
    const stepClass = `${name}SpawnStep`;

    const code = `using System;
using System.Collections.Generic;
using RogueElements;

namespace YourNamespace
{
    /// <summary>
    /// ${description}
    /// </summary>
    [Serializable]
    public class ${entityClass} : ISpawnable
    {
        /// <summary>
        /// Display name for this entity.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Entity type or category.
        /// </summary>
        public int Type { get; set; }

        public ${entityClass}()
        {
            this.Name = "${name}";
            this.Type = 0;
        }

        public ${entityClass}(string name, int type)
        {
            this.Name = name;
            this.Type = type;
        }

        public ISpawnable Copy()
        {
            return new ${entityClass}
            {
                Name = this.Name,
                Type = this.Type
            };
        }
    }

    /// <summary>
    /// Spawns ${description.toLowerCase()} on the map.
    /// </summary>
    [Serializable]
    public class ${stepClass}<T> : GenStep<T>
        where T : class, ${spawn_type === "room" ? "IFloorPlanGenContext, " : ""}IPlaceableGenContext<${entityClass}>
    {
        /// <summary>
        /// Number of entities to spawn.
        /// </summary>
        public RandRange Amount { get; set; }

        /// <summary>
        /// Weighted spawn table for selecting which ${entityClass} to place.
        /// </summary>
        public SpawnList<${entityClass}> Spawns { get; set; }

        public ${stepClass}()
        {
            this.Amount = new RandRange(3, 8);
            this.Spawns = new SpawnList<${entityClass}>();
        }

        public ${stepClass}(SpawnList<${entityClass}> spawns, RandRange amount)
        {
            this.Spawns = spawns;
            this.Amount = amount;
        }

        public override void Apply(T map)
        {
            int count = this.Amount.Pick(map.Rand);
${spawn_type === "random" ? `
            // Get all available spawn locations
            List<Loc> freeTiles = map.GetAllFreeTiles();

            for (int i = 0; i < count && freeTiles.Count > 0; i++)
            {
                // Pick random location
                int idx = map.Rand.Next(freeTiles.Count);
                Loc loc = freeTiles[idx];
                freeTiles.RemoveAt(idx);

                // Pick random entity from spawn list
                ${entityClass} entity = this.Spawns.Pick(map.Rand).Copy() as ${entityClass};
                map.PlaceItem(loc, entity);
            }` : spawn_type === "terminal" ? `
            // Find terminal (dead-end) rooms
            List<int> terminalRooms = new List<int>();
            for (int i = 0; i < map.RoomPlan.RoomCount; i++)
            {
                IRoomPlan room = map.RoomPlan.GetRoom(i);
                if (room.Adjacents.Count == 1)
                    terminalRooms.Add(i);
            }

            for (int i = 0; i < count && terminalRooms.Count > 0; i++)
            {
                // Pick random terminal room
                int roomIdx = map.Rand.Next(terminalRooms.Count);
                int roomId = terminalRooms[roomIdx];
                terminalRooms.RemoveAt(roomIdx);

                IRoomPlan room = map.RoomPlan.GetRoom(roomId);
                List<Loc> freeTiles = map.GetFreeTiles(room.RoomGen.Draw);

                if (freeTiles.Count > 0)
                {
                    Loc loc = freeTiles[map.Rand.Next(freeTiles.Count)];
                    ${entityClass} entity = this.Spawns.Pick(map.Rand).Copy() as ${entityClass};
                    map.PlaceItem(loc, entity);
                }
            }` : `
            // Spawn in each room
            for (int i = 0; i < map.RoomPlan.RoomCount; i++)
            {
                IRoomPlan room = map.RoomPlan.GetRoom(i);
                List<Loc> freeTiles = map.GetFreeTiles(room.RoomGen.Draw);

                int roomCount = Math.Min(count, freeTiles.Count);
                for (int j = 0; j < roomCount; j++)
                {
                    int idx = map.Rand.Next(freeTiles.Count);
                    Loc loc = freeTiles[idx];
                    freeTiles.RemoveAt(idx);

                    ${entityClass} entity = this.Spawns.Pick(map.Rand).Copy() as ${entityClass};
                    map.PlaceItem(loc, entity);
                }
            }`}
        }
    }
}`;

    const usageCode = `// Create spawn list with weighted entries
var ${name.toLowerCase()}Spawns = new SpawnList<${entityClass}>();
${name.toLowerCase()}Spawns.Add(new ${entityClass}("Common ${name}", 0), 10);  // Weight 10 (common)
${name.toLowerCase()}Spawns.Add(new ${entityClass}("Rare ${name}", 1), 3);    // Weight 3 (rare)
${name.toLowerCase()}Spawns.Add(new ${entityClass}("Epic ${name}", 2), 1);    // Weight 1 (very rare)

// Add to pipeline (priority 90+ for spawning)
var spawnStep = new ${stepClass}<MyContext>(${name.toLowerCase()}Spawns, new RandRange(5, 10));
layout.GenSteps.Add(new Priority(95), spawnStep);`;

    return {
      content: [{
        type: "text",
        text: `# Generated Spawnable: ${entityClass}

\`\`\`csharp
${code}
\`\`\`

## Usage Example

\`\`\`csharp
${usageCode}
\`\`\`

## Context Requirements

Your map context must implement:
\`\`\`csharp
public class MyContext : IGenContext, ${spawn_type !== "random" ? "IFloorPlanGenContext, " : ""}IPlaceableGenContext<${entityClass}>
{
    // IPlaceableGenContext implementation
    public List<Loc> GetAllFreeTiles() { /* ... */ }
    public List<Loc> GetFreeTiles(Rect rect) { /* ... */ }
    public bool CanPlaceItem(Loc loc) { /* ... */ }
    public void PlaceItem(Loc loc, ${entityClass} item) { /* ... */ }
}
\`\`\`

*See \`rogue_get_example\` with name "Ex6_Items" for complete spawning example.*`
      }]
    };
  }
);

// =============================================================================
// SERVER STARTUP
// =============================================================================

async function main() {
  // Initialize parser before connecting
  await initializeParser();

  // Diagnostic info
  console.error("═══════════════════════════════════════════════════════════════");
  console.error("  RogueElements MCP Server v2.2.1");
  console.error("═══════════════════════════════════════════════════════════════");
  console.error(`  Library:    ${fs.existsSync(ROGUE_DIR) ? "✓" : "✗"} ${ROGUE_DIR}`);
  console.error(`  Docs:       ${fs.existsSync(DOCS_DIR) ? "✓" : "✗"} ${DOCS_DIR}`);
  console.error(`  Examples:   ${fs.existsSync(EXAMPLES_DIR) ? "✓" : "✗"} ${EXAMPLES_DIR}`);
  console.error(`  Parser:     ${csharpParser ? "✓ tree-sitter (C#)" : "✗ not initialized"}`);
  console.error("───────────────────────────────────────────────────────────────");
  console.error("  Tools:      rogue_search, rogue_list_classes, rogue_get_class_docs");
  console.error("              rogue_get_docs, rogue_get_example, rogue_list_interfaces");
  console.error("              rogue_scaffold_roomgen, rogue_scaffold_genstep");
  console.error("              rogue_scaffold_spawnable");
  console.error("═══════════════════════════════════════════════════════════════");

  const transport = new StdioServerTransport();
  await server.connect(transport);
  console.error("Server connected via stdio transport");
}

main().catch(error => {
  console.error("Server error:", error);
  process.exit(1);
});
