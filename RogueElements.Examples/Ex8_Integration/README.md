# Example 8: Integration

Integrate RogueElements with external libraries using the MapCreationStrategy pattern.

## What You'll Learn

- How to integrate RogueElements with RogueSharp
- Creating a custom `IMapCreationStrategy`
- Adapting the tile system for external libraries
- Building a complete, reusable generation pipeline

## Prerequisites

- All previous examples
- Basic understanding of RogueSharp (optional)

## Concepts

### Integration Strategy

RogueElements is designed to be **library-agnostic**. This example shows integration with **RogueSharp**, but the same pattern works for any game framework:

```
RogueElements (procedural generation)
    --> Adapter Layer
    --> External Library (RogueSharp, MonoGame, Unity, etc.)
```

### IMapCreationStrategy

RogueSharp uses the Strategy pattern for map generation. By implementing `IMapCreationStrategy<T>`, we can plug RogueElements into RogueSharp's ecosystem.

### CellTile Adapter

RogueSharp uses `Cell` objects instead of our `Tile` class. `CellTile` bridges the gap by implementing both `Cell` and `ITile`.

## Code Walkthrough

### Step 1: Create the MapCreationStrategy

```csharp
public class ExampleCreationStrategy<T> : IMapCreationStrategy<T>
    where T : Map, new()
{
    public ExampleCreationStrategy()
    {
        this.Layout = new MapGen<MapGenContext>();
    }

    public ulong Seed { get; set; }
    public MapGen<MapGenContext> Layout { get; set; }

    public T CreateMap()
    {
        MapGenContext context = this.Layout.GenMap(this.Seed);
        return (T)context.Map;
    }
}
```

Key points:
- Holds the `MapGen` pipeline as a property
- `Seed` allows deterministic generation
- `CreateMap()` executes the pipeline and returns the RogueSharp Map

### Step 2: Create the CellTile Adapter

```csharp
public class CellTile : Cell, ITile
{
    public CellTile(int x, int y, bool isTransparent, bool isWalkable, bool isInFov)
        : base(x, y, isTransparent, isWalkable, isInFov)
    {
    }

    public static CellTile FromCell(ICell other) => new CellTile(other);

    public bool TileEquivalent(ITile other)
        => (other is ICell cell) && cell?.IsWalkable == this.IsWalkable;

    public ITile Copy() => new CellTile(this);
}
```

This adapter:
- Extends RogueSharp's `Cell`
- Implements RogueElements' `ITile`
- Enables both libraries to work with the same object

### Step 3: Create a RogueSharp-Compatible Context

```csharp
public class MapGenContext : ITiledGenContext, IRoomGridGenContext
{
    public MapGenContext()
    {
        this.Map = new Map();  // RogueSharp.Map
    }

    public Map Map { get; set; }  // RogueSharp.Map (not BaseMap!)

    // ITile implementations using CellTile
    public ITile RoomTerrain => new CellTile(0, 0, true, true, false);
    public ITile WallTerrain => new CellTile(0, 0, false, false, false);

    public ITile GetTile(Loc loc)
        => CellTile.FromCell(this.Map.GetCell(loc.X, loc.Y));

    public bool TrySetTile(Loc loc, ITile tile)
    {
        Cell cell = (Cell)tile;
        this.Map.SetCellProperties(loc.X, loc.Y,
            cell.IsTransparent, cell.IsWalkable, cell.IsExplored);
        return true;
    }

    public void CreateNew(int width, int height, bool wrap = false)
    {
        this.Map.Initialize(width, height);
    }

    // ... other interface implementations
}
```

Key differences from previous examples:
- Uses `RogueSharp.Map` instead of custom `BaseMap`
- `RoomTerrain`/`WallTerrain` are `CellTile` objects
- Tile operations translate to RogueSharp's cell system

### Step 4: Configure the Pipeline

```csharp
public static void Run()
{
    ExampleCreationStrategy<Map> exampleCreation = new ExampleCreationStrategy<Map>();

    // Standard grid setup
    var startGen = new InitGridPlanStep<MapGenContext>(1)
    {
        CellX = 6, CellY = 4,
        CellWidth = 9, CellHeight = 9,
    };
    exampleCreation.Layout.GenSteps.Add(-4, startGen);

    // Branching path
    var path = new GridPathBranch<MapGenContext>
    {
        RoomRatio = new RandRange(70),
        BranchRatio = new RandRange(0, 50),
    };
    // ... room and hall setup ...
    exampleCreation.Layout.GenSteps.Add(-4, path);

    // Grid -> FloorPlan -> Tiles
    exampleCreation.Layout.GenSteps.Add(-2, new DrawGridToFloorStep<MapGenContext>());
    exampleCreation.Layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));

    // Generate using RogueSharp's Map.Create pattern
    exampleCreation.Seed = MathUtils.Rand.NextUInt64();
    Map map = Map.Create(exampleCreation);
}
```

### Step 5: Use RogueSharp's Native Rendering

```csharp
public static void Print(Map map, string title)
{
    // ... header ...
    Console.Write(map.ToString());  // RogueSharp's built-in rendering!
}
```

RogueSharp's `Map.ToString()` automatically renders the map using its own format.

## Try It

```bash
dotnet run --project RogueElements.Examples/RogueElements.Examples.csproj
```

Press `8` to run Example 8.

**What to observe:**
- Output uses RogueSharp's rendering format
- Same procedural generation, different output library
- Seamless integration between the two libraries

**Example output:**
```
8: Implementation as a MapCreationStrategy in RogueSharp
=======================================================
######################################################
#######.........######################################
#######.........###....###############################
#######.........###....###############################
#######.............#..#####........##################
###############.....####............................##
###############.........#############.................
###############.........#############.................
```

## Key Classes Used

| Class | Purpose |
|-------|---------|
| `ExampleCreationStrategy<T>` | Implements RogueSharp's IMapCreationStrategy |
| `CellTile` | Adapter between Cell and ITile |
| `MapGenContext` | RogueSharp-compatible generation context |
| `RogueSharp.Map` | RogueSharp's native map class |

## Integration Patterns

### For RogueSharp

```csharp
public class MyCreationStrategy : IMapCreationStrategy<Map>
{
    public MapGen<MyContext> Layout { get; set; }

    public Map CreateMap()
    {
        var context = Layout.GenMap(seed);
        return (Map)context.Map;
    }
}

// Usage
Map map = Map.Create(new MyCreationStrategy());
```

### For MonoGame/FNA

```csharp
public class MonoGameMapGenerator
{
    public MapGen<MonoGameContext> Layout { get; set; }

    public Texture2D GenerateMapTexture(GraphicsDevice device, ulong seed)
    {
        var context = Layout.GenMap(seed);
        return RenderToTexture(device, context.Map);
    }
}
```

### For Unity

```csharp
public class UnityMapGenerator : MonoBehaviour
{
    public MapGen<UnityContext> Layout { get; set; }

    public void GenerateMap(ulong seed)
    {
        var context = Layout.GenMap(seed);
        InstantiateTilemap(context.Map);
    }
}
```

## Creating Your Own Integration

1. **Create a Context Class**
   - Implement required interfaces (`ITiledGenContext`, etc.)
   - Use your framework's tile/cell representation

2. **Create a Tile Adapter** (if needed)
   - Implement `ITile`
   - Bridge to your framework's tile system

3. **Create a Generation Entry Point**
   - Hold the `MapGen` pipeline
   - Provide seed/configuration options
   - Return your framework's map type

4. **Configure the Pipeline**
   - Add generation steps as needed
   - Reuse steps across different integrations

## Key Takeaways

1. **Library Agnostic**: RogueElements works with any framework
2. **Adapter Pattern**: Bridge interfaces when needed
3. **Strategy Pattern**: Plug into existing generation frameworks
4. **Reusable Pipelines**: Same steps work across integrations

## Reference Implementation

This example serves as a **reference implementation** for integrating RogueElements. Key patterns:

| Pattern | Implementation |
|---------|---------------|
| Interface adaptation | `CellTile` class |
| Generation entry point | `ExampleCreationStrategy` |
| Context customization | `MapGenContext` with RogueSharp.Map |
| Pipeline reuse | Standard GenSteps work unchanged |

## Complete Pipeline Summary

All 8 examples build upon each other:

| Example | Adds |
|---------|------|
| 1 | Basic pipeline, static tiles |
| 2 | FloorPlan, procedural rooms |
| 3 | GridPlan, structured layouts |
| 4 | Stair spawning |
| 5 | Perlin terrain |
| 6 | Random items/mobs |
| 7 | Special rooms, filtered spawning |
| 8 | External library integration |

## Next Steps

You now have all the tools to:
- Create custom generation pipelines
- Integrate with your game framework
- Build procedural roguelike maps

Explore the main [RogueElements library](../../RogueElements/) for additional steps and features not covered in these examples.
