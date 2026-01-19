# Example 3: Grid-Based Layouts

Generate dungeons using a grid structure where rooms occupy fixed cells.

## What You'll Learn

- How to use `GridPlan` for structured room placement
- Configuring grid cells and dimensions
- Using `GridPathBranch` for grid-constrained paths
- The two-step conversion: GridPlan --> FloorPlan --> Tiles

## Prerequisites

- [Example 2: Freeform Rooms](../Ex2_Rooms/README.md)
- Understanding of FloorPlan concepts

## Concepts

### Grid-Based Generation

`GridPlan` divides the map into a grid of cells. Each cell can contain:
- A room
- Part of a hallway
- Empty space (wall)

This creates more structured, traditional dungeon layouts compared to freeform placement.

### The Two-Step Conversion

```
GridPlan (cells with room assignments)
    --> DrawGridToFloorStep
    --> FloorPlan (concrete room positions)
    --> DrawFloorToTileStep
    --> Tile Grid
```

Grid-based generation adds an extra abstraction layer for structured control.

## Code Walkthrough

### Step 1: Initialize the Grid

```csharp
var startGen = new InitGridPlanStep<MapGenContext>(1)
{
    CellX = 6,
    CellY = 4,
    CellWidth = 9,
    CellHeight = 9,
};
layout.GenSteps.Add(-4, startGen);
```

Creates a 6x4 grid of cells, each 9x9 tiles:

| Property | Value | Purpose |
|----------|-------|---------|
| `CellX` | 6 | Number of cells horizontally |
| `CellY` | 4 | Number of cells vertically |
| `CellWidth` | 9 | Tile width of each cell |
| `CellHeight` | 9 | Tile height of each cell |
| Constructor arg `1` | 1 | Default hall width |

Total map size = (6 * 9) x (4 * 9) = 54 x 36 tiles.

### Step 2: Create the Grid Path

```csharp
var path = new GridPathBranch<MapGenContext>
{
    RoomRatio = new RandRange(70),
    BranchRatio = new RandRange(0, 50),
};
```

`GridPathBranch` creates paths within the grid:

| Property | Purpose |
|----------|---------|
| `RoomRatio` | Percentage of cells to fill with rooms (70%) |
| `BranchRatio` | Chance of branching vs extending (0-50%) |

### Step 3: Define Rooms and Halls

```csharp
var genericRooms = new SpawnList<RoomGen<MapGenContext>>
{
    { new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)), 10 },
    { new RoomGenRound<MapGenContext>(new RandRange(5, 9), new RandRange(5, 9)), 10 },
};
path.GenericRooms = genericRooms;

var genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>>
{
    { new RoomGenAngledHall<MapGenContext>(50), 10 },
};
path.GenericHalls = genericHalls;
```

Same room types as Example 2, but constrained to fit within grid cells.

Note: `RoomGenAngledHall(50)` means 50% chance of an angled (L-shaped) hall vs straight.

### Step 4: Convert Grid to FloorPlan

```csharp
layout.GenSteps.Add(-2, new DrawGridToFloorStep<MapGenContext>());
```

`DrawGridToFloorStep` converts the abstract grid into a concrete `FloorPlan` with positioned rooms.

### Step 5: Convert FloorPlan to Tiles

```csharp
layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));
```

Same as Example 2 - converts the FloorPlan to actual tile data.

## MapGenContext Changes

The context must implement `IRoomGridGenContext` (which extends `IFloorPlanGenContext`):

```csharp
public class MapGenContext : BaseMapGenContext<Map>, IRoomGridGenContext
{
    public FloorPlan RoomPlan { get; private set; }
    public GridPlan GridPlan { get; private set; }

    public void InitPlan(FloorPlan plan) { this.RoomPlan = plan; }
    public void InitGrid(GridPlan plan) { this.GridPlan = plan; }
}
```

## Try It

```bash
dotnet run --project RogueElements.Examples/RogueElements.Examples.csproj
```

Press `3` to run Example 3.

**What to observe:**
- Rooms are aligned to a grid pattern
- Hallways run between adjacent cells
- More structured layout than Example 2
- Regular spacing between rooms

**Example output:**
```
3: A Map made with Rooms and Halls arranged in a grid.
=======================================================
######################################################
########.......#######################################
########.......####.......############################
########.......####.......############################
########.......####.......#####........###############
########..............#########........###############
########.......####...#########........###############
########.......####...#########........###############
... (grid-aligned layout)
```

## Key Classes Used

| Class | Purpose |
|-------|---------|
| `InitGridPlanStep<T>` | Creates the grid structure |
| `GridPathBranch<T>` | Generates branching paths on the grid |
| `DrawGridToFloorStep<T>` | Converts GridPlan to FloorPlan |
| `DrawFloorToTileStep<T>` | Converts FloorPlan to tiles |
| `IRoomGridGenContext` | Context interface for grid-based generation |

## Grid vs FloorPlan: When to Use Which

| Use Case | Approach |
|----------|----------|
| Traditional dungeon crawler | GridPlan |
| Roguelike with regular room spacing | GridPlan |
| Cave systems, organic layouts | FloorPlan |
| Need precise room placement control | GridPlan |
| Want rooms to vary in position freely | FloorPlan |

## Hybrid Approaches

You can combine both:
1. Start with `GridPlan` for the main structure
2. Add `FloorPlan` modifications for special areas
3. Both convert to the same tile format

## Key Takeaways

1. **Grid Abstraction**: Cells provide structure before room generation
2. **Two-Step Conversion**: Grid --> FloorPlan --> Tiles
3. **Cell Sizing**: Room sizes are constrained by cell dimensions
4. **Structured Layouts**: Grid-based maps feel more "designed"

## Next Steps

[Example 4: Stair Placement](../Ex4_Stairs/README.md) adds entrance and exit stairs to the map.
