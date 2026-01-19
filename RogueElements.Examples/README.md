# RogueElements.Examples

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET Core](https://img.shields.io/badge/.NET%20Core-2.1-blue.svg)](https://dotnet.microsoft.com/)

Interactive examples demonstrating RogueElements library features, from basic tiles to complete dungeon generation. Each example builds on previous concepts, providing a structured learning path.

## Overview

This project contains 8 progressive examples that teach RogueElements concepts incrementally. Run the examples interactively and watch maps generate in your terminal.

## Running the Examples

```bash
# From repository root
dotnet run --project RogueElements.Examples/RogueElements.Examples.csproj

# Or from this directory
cd RogueElements.Examples
dotnet run
```

### Interactive Controls

```
1-8     Run example 1-8
F4      Enable debug mode (step-by-step generation)
F5      Step into (while debugging)
F6      Step out (while debugging)
ESC     Exit
```

## Learning Progression

Follow this roadmap to learn RogueElements from fundamentals to advanced usage:

```
Ex1_Tiles ──> Ex2_Rooms ──> Ex3_Grid ──> Ex4_Stairs
    │             │             │             │
    v             v             v             v
 Basics      FloorPlan      GridPlan      Spawning
                                              │
                                              v
Ex5_Terrain <── Ex6_Items <── Ex7_Special <── Ex8_Integration
    │               │             │               │
    v               v             v               v
  Water         Items/Mobs    Components     RogueSharp
```

## Examples Reference

| Example | Title | Key Concepts | GenSteps Used |
|---------|-------|--------------|---------------|
| **Ex1** | Static Tiles | Tile grids, `MapGen<T>`, basic pipeline | `InitTilesStep`, `SpecificTilesStep` |
| **Ex2** | Rooms & Halls | Freeform room placement, `FloorPlan` | `InitFloorPlanStep`, `FloorPathBranch`, `DrawFloorToTileStep` |
| **Ex3** | Grid Layout | Grid-based generation, `GridPlan` | `InitGridPlanStep`, `GridPathBranch`, `DrawGridToFloorStep` |
| **Ex4** | Stairs | Entity spawning, entrance/exit placement | `FloorStairsStep` |
| **Ex5** | Terrain | Perlin noise, water generation, cleanup | `PerlinWaterStep`, `DropDiagonalBlockStep`, `EraseIsolatedStep` |
| **Ex6** | Items & Mobs | `SpawnList`, random placement, weighted distribution | `RandomSpawnStep`, `PickerSpawner`, `LoopedRand` |
| **Ex7** | Special Rooms | Room components, filters, custom room shapes | `SetSpecialRoomStep`, `RoomFilterComponent`, `RoomGenSpecific` |
| **Ex8** | Integration | RogueSharp interop, external library usage | `IMapCreationStrategy` pattern |

## Example Details

### Ex1: Static Tiles

**Concepts**: Basic pipeline setup, tile initialization, direct tile manipulation

```csharp
var layout = new MapGen<MapGenContext>();

// Initialize 30x25 grid of walls
layout.GenSteps.Add(0, new InitTilesStep<MapGenContext>(30, 25));

// Draw specific tiles at offset (2, 3)
var drawStep = new SpecificTilesStep<MapGenContext>(tiles, new Loc(2, 3));
layout.GenSteps.Add(0, drawStep);

MapGenContext context = layout.GenMap(seed);
```

### Ex2: Rooms & Halls (FloorPlan)

**Concepts**: Freeform room placement, room/hall types, branching paths

```csharp
// Initialize FloorPlan
layout.GenSteps.Add(-2, new InitFloorPlanStep<MapGenContext>(54, 40));

// Define room types with weights
var rooms = new SpawnList<RoomGen<MapGenContext>>
{
    { new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)), 10 },
    { new RoomGenRound<MapGenContext>(new RandRange(5, 9), new RandRange(5, 9)), 10 }
};

// Create branching path
var path = new FloorPathBranch<MapGenContext>(rooms, halls)
{
    HallPercent = 50,
    FillPercent = new RandRange(45),
    BranchRatio = new RandRange(0, 25)
};
layout.GenSteps.Add(-1, path);

// Render to tiles
layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));
```

### Ex3: Grid Layout (GridPlan)

**Concepts**: Cell-based layout, grid constraints, two-phase rendering

```csharp
// Initialize 6x4 grid of 9x9 cells
var startGen = new InitGridPlanStep<MapGenContext>(1)
{
    CellX = 6, CellY = 4,
    CellWidth = 9, CellHeight = 9
};
layout.GenSteps.Add(-4, startGen);

// Grid-constrained path
var path = new GridPathBranch<MapGenContext>
{
    RoomRatio = new RandRange(70),
    BranchRatio = new RandRange(0, 50)
};
layout.GenSteps.Add(-4, path);

// Grid -> FloorPlan -> Tiles (two-phase)
layout.GenSteps.Add(-2, new DrawGridToFloorStep<MapGenContext>());
layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));
```

### Ex4: Stairs

**Concepts**: Entity spawning, `IPlaceableGenContext`, entrance/exit placement

```csharp
// Place stairs after tiles are drawn
layout.GenSteps.Add(2, new FloorStairsStep<MapGenContext, StairsUp, StairsDown>(
    0,              // Goal amount (0 = random valid position)
    new StairsUp(),
    new StairsDown()
));
```

### Ex5: Terrain

**Concepts**: Perlin noise, terrain stencils, post-processing cleanup

```csharp
// Generate water with 35% coverage, order 3 noise, softness 1
const int waterTerrain = 2;
var waterStep = new PerlinWaterStep<MapGenContext>(
    new RandRange(35),
    3,  // noise order
    new Tile(waterTerrain),
    new MapTerrainStencil<MapGenContext>(false, true, false, false),
    1   // softness
);
layout.GenSteps.Add(3, waterStep);

// Cleanup steps
layout.GenSteps.Add(4, new DropDiagonalBlockStep<MapGenContext>(new Tile(waterTerrain)));
layout.GenSteps.Add(4, new EraseIsolatedStep<MapGenContext>(new Tile(waterTerrain)));
```

### Ex6: Items & Mobs

**Concepts**: `SpawnList` weights, `LoopedRand`, picker spawners

```csharp
// Weighted item spawns
var itemSpawns = new SpawnList<Item>
{
    { new Item('!'), 10 },  // Potion
    { new Item('*'), 50 }   // Gold (5x more common)
};

// Spawn 10-18 items
var itemStep = new RandomSpawnStep<MapGenContext, Item>(
    new PickerSpawner<MapGenContext, Item>(
        new LoopedRand<Item>(itemSpawns, new RandRange(10, 19))
    )
);
layout.GenSteps.Add(6, itemStep);
```

### Ex7: Special Rooms

**Concepts**: Room components, room filters, custom room definitions

```csharp
// Define custom room shape
string[] treasureRoom = {
    "~~~..~~~",
    "~~#..#~~",
    "........",
    "~~#..#~~",
    "~~~..~~~"
};

// Create special room step with component
var specialStep = new SetSpecialRoomStep<MapGenContext>
{
    Rooms = new PresetPicker<RoomGen<MapGenContext>>(
        CreateRoomGenSpecific<MapGenContext>(treasureRoom)
    )
};
specialStep.RoomComponents.Set(new TreasureRoomComponent());
layout.GenSteps.Add(-1, specialStep);

// Filter spawns to treasure rooms only
var treasureSpawns = new RandomRoomSpawnStep<MapGenContext, Item>(...);
treasureSpawns.Filters.Add(new RoomFilterComponent(false, new TreasureRoomComponent()));
```

### Ex8: RogueSharp Integration

**Concepts**: External library integration, `IMapCreationStrategy` pattern

```csharp
// Implement RogueSharp's creation strategy
public class ExampleCreationStrategy<T> : IMapCreationStrategy<T>
    where T : IMap, new()
{
    public MapGen<MapGenContext> Layout { get; } = new MapGen<MapGenContext>();
    public ulong Seed { get; set; }

    public T CreateMap()
    {
        MapGenContext context = Layout.GenMap(Seed);
        // Convert to RogueSharp map...
        return map;
    }
}

// Use with RogueSharp
Map map = Map.Create(new ExampleCreationStrategy<Map>());
```

## Project Structure

```
RogueElements.Examples/
├── Program.cs              # Interactive example runner
├── ExampleDebug.cs         # Debug visualization system
├── DebugState.cs           # Debug state tracking
├── Common/                 # Shared code for examples
│   ├── BaseMap.cs          # Base map implementation
│   ├── BaseMapGenContext.cs # Base context with ITiledGenContext
│   ├── Tile.cs             # Simple tile implementation
│   ├── Stairs.cs           # Stairs base class
│   ├── StairsUp.cs         # Upward stairs
│   ├── StairsDown.cs       # Downward stairs
│   ├── MainRoomComponent.cs    # Main path room marker
│   ├── MainHallComponent.cs    # Main path hall marker
│   └── TreasureRoomComponent.cs # Treasure room marker
├── Ex1_Tiles/              # Example 1: Static tiles
│   └── Example1.cs
├── Ex2_Rooms/              # Example 2: FloorPlan rooms
│   └── Example2.cs
├── Ex3_Grid/               # Example 3: GridPlan layout
│   └── Example3.cs
├── Ex4_Stairs/             # Example 4: Stair placement
│   └── Example4.cs
├── Ex5_Terrain/            # Example 5: Water/terrain
│   └── Example5.cs
├── Ex6_Items/              # Example 6: Items and mobs
│   ├── Example6.cs
│   ├── Item.cs             # Item spawnable
│   └── Mob.cs              # Mob spawnable
├── Ex7_Special/            # Example 7: Special rooms
│   └── Example7.cs
└── Ex8_Integration/        # Example 8: RogueSharp integration
    ├── Example8.cs
    └── ExampleCreationStrategy.cs
```

## Debug Mode

Press **F4** before running an example to enable step-by-step debugging:

```csharp
// Debug hooks are attached in Program.cs
GenContextDebug.OnInit += ExampleDebug.Init;
GenContextDebug.OnStep += ExampleDebug.OnStep;
GenContextDebug.OnStepIn += ExampleDebug.StepIn;
GenContextDebug.OnStepOut += ExampleDebug.StepOut;
```

This allows you to see the map state after each GenStep executes.

## Creating Your Own Context

Use `BaseMapGenContext` as a starting point:

```csharp
public class MapGenContext : BaseMapGenContext<Map>,
    IFloorPlanGenContext,
    IRoomGridGenContext,
    IPlaceableGenContext<Item>
{
    public FloorPlan FloorPlan { get; set; }
    public GridPlan GridPlan { get; set; }

    // Implement IPlaceableGenContext<Item>
    public List<Item> Items { get; } = new List<Item>();
    // ...
}
```

## See Also

- **[RogueElements/](../RogueElements/)** - Core library documentation
- **[RogueElements.Tests/](../RogueElements.Tests/)** - Unit tests for reference
- **[CLAUDE.md](../CLAUDE.md)** - Full architecture documentation

---

![Repobeats analytics](https://repobeats.axiom.co/api/embed/placeholder.svg "Repobeats analytics image")
