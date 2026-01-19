# Example 4: Stair Placement

Add entrance and exit stairs to connect dungeon floors.

## What You'll Learn

- How to place spawnable entities on the map
- Using `FloorStairsStep` for stair placement
- Implementing `IPlaceableGenContext<T>` for custom spawnables
- Understanding the `IEntrance` and `IExit` interfaces

## Prerequisites

- [Example 3: Grid-Based Layouts](../Ex3_Grid/README.md)
- Understanding of grid-based generation

## Concepts

### Spawnable Entities

Stairs are **spawnable entities** - objects placed on the map after tile generation. RogueElements uses a generic spawning system:

```
ISpawnable (base interface)
    --> Stairs (abstract, has Loc property)
        --> StairsUp : IEntrance (player spawn point)
        --> StairsDown : IExit (floor exit)
```

### FloorStairsStep

`FloorStairsStep` automatically places exactly one entrance and one exit:
- Entrance is placed first
- Exit is placed at maximum distance from entrance
- Both avoid walls and existing entities

## Code Walkthrough

### Step 1: Standard Grid Setup

```csharp
// Smaller grid for this example: 3x2 cells
var startGen = new InitGridPlanStep<MapGenContext>(1)
{
    CellX = 3,
    CellY = 2,
    CellWidth = 9,
    CellHeight = 9,
};
layout.GenSteps.Add(-4, startGen);

// Standard path generation
var path = new GridPathBranch<MapGenContext> { /* ... */ };
layout.GenSteps.Add(-4, path);

// Grid to FloorPlan to Tiles
layout.GenSteps.Add(-2, new DrawGridToFloorStep<MapGenContext>());
layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));
```

Same setup as Example 3, but with a smaller 3x2 grid.

### Step 2: Add Stair Placement

```csharp
layout.GenSteps.Add(2, new FloorStairsStep<MapGenContext, StairsUp, StairsDown>(
    0,                  // reserved parameter
    new StairsUp(),     // entrance template
    new StairsDown()    // exit template
));
```

`FloorStairsStep<TContext, TEntrance, TExit>` parameters:
- First arg: Reserved (pass 0)
- Second arg: Template for entrance stairs
- Third arg: Template for exit stairs

**Priority 2** ensures this runs after tile drawing (priority 0).

## Map Class Changes

The Map must store the placed stairs:

```csharp
public class Map : BaseMap
{
    public Map()
    {
        this.GenEntrances = new List<StairsUp>();
        this.GenExits = new List<StairsDown>();
    }

    public List<StairsUp> GenEntrances { get; set; }
    public List<StairsDown> GenExits { get; set; }
}
```

## MapGenContext Changes

The context must implement `IViewPlaceableGenContext<T>` for both stair types:

```csharp
public class MapGenContext : BaseMapGenContext<Map>, IRoomGridGenContext,
    IViewPlaceableGenContext<StairsUp>, IViewPlaceableGenContext<StairsDown>
{
    // Protect stairs from being overwritten
    public override bool CanSetTile(Loc loc, ITile tile)
    {
        foreach (var entrance in this.GenEntrances)
            if (entrance.Loc == loc) return false;
        foreach (var exit in this.GenExits)
            if (exit.Loc == loc) return false;
        return true;
    }

    // Placement implementation
    void IPlaceableGenContext<StairsUp>.PlaceItem(Loc loc, StairsUp item)
    {
        var stairs = (StairsUp)item.Copy();
        stairs.Loc = loc;
        this.GenEntrances.Add(stairs);
    }

    // Query methods for finding valid placement locations
    List<Loc> IPlaceableGenContext<StairsUp>.GetAllFreeTiles()
        => this.GetAllFreeTiles(this.GetOpenTiles);

    bool IPlaceableGenContext<StairsUp>.CanPlaceItem(Loc loc)
        => !this.IsTileOccupied(loc);
}
```

Key interface requirements:

| Method | Purpose |
|--------|---------|
| `GetAllFreeTiles()` | Returns all valid placement locations |
| `GetFreeTiles(Rect)` | Returns valid locations within a rectangle |
| `CanPlaceItem(Loc)` | Checks if a specific location is valid |
| `PlaceItem(Loc, T)` | Places the entity at the location |

## Rendering Stairs

The Print method checks for stairs and displays them:

```csharp
foreach (StairsUp entrance in map.GenEntrances)
{
    if (entrance.Loc == loc)
    {
        tileChar = '<';  // Up stairs
        break;
    }
}

foreach (StairsDown exit in map.GenExits)
{
    if (exit.Loc == loc)
    {
        tileChar = '>';  // Down stairs
        break;
    }
}
```

## Try It

```bash
dotnet run --project RogueElements.Examples/RogueElements.Examples.csproj
```

Press `4` to run Example 4.

**What to observe:**
- `<` symbol marks the entrance (stairs up)
- `>` symbol marks the exit (stairs down)
- Stairs are always placed on floor tiles
- Entrance and exit are typically in different rooms

**Example output:**
```
4: A Map with Stairs Up and Down
==============================
##############################
#####.........################
#####.........################
#####.<.......################
#####.........#.......########
#####.........#.......########
##############........########
##############........########
##############........########
##############.>......########
##############........########
##############################
```

## Key Classes Used

| Class | Purpose |
|-------|---------|
| `FloorStairsStep<T, TEntrance, TExit>` | Places entrance and exit stairs |
| `StairsUp` | Entrance stair entity (implements `IEntrance`) |
| `StairsDown` | Exit stair entity (implements `IExit`) |
| `IViewPlaceableGenContext<T>` | Interface for placeable entity contexts |
| `ISpawnable` | Base interface for spawnable entities |

## Placement Algorithm

`FloorStairsStep` uses this algorithm:

1. Find all valid floor tiles
2. Place entrance at a random valid tile
3. Calculate distances from entrance to all other tiles
4. Place exit at the tile with maximum distance
5. This ensures the player must traverse most of the floor

## Creating Custom Spawnables

To create your own spawnable entities:

```csharp
public class MyEntity : ISpawnable
{
    public Loc Loc { get; set; }
    public ISpawnable Copy() => new MyEntity(this);
}
```

Then implement `IPlaceableGenContext<MyEntity>` in your context.

## Key Takeaways

1. **Spawnable Pattern**: Entities are templates that get copied and placed
2. **Interface Segregation**: Different spawn types use different interfaces
3. **Tile Protection**: `CanSetTile` prevents overwriting placed entities
4. **Distance Maximization**: Exit placement maximizes path length

## Next Steps

[Example 5: Terrain Features](../Ex5_Terrain/README.md) adds water and other terrain using Perlin noise.
