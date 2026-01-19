# Grid Paths

[![RogueElements](https://img.shields.io/nuget/v/RogueElements?label=RogueElements)](https://www.nuget.org/packages/RogueElements)

Path generation algorithms for grid-based floor plan layouts. This module provides steps that populate `GridPlan` objects with rooms and halls arranged in regular grid patterns.

## Purpose

Grid Paths generate room/hall connectivity for grid-based map layouts. Unlike freeform floor plans, grid paths place rooms in fixed cells of a grid, creating more structured and predictable dungeon layouts.

## Core Classes

### GridPathStartStep

Abstract base class for all grid path generators:

```csharp
public abstract class GridPathStartStep<T> : GridPlanStep<T>
    where T : class, IRoomGridGenContext
{
    public virtual void CreateErrorPath(IRandom rand, GridPlan floorPlan);
    public virtual RoomGen<T> GetDefaultGen();

    // Utility: Roll probability for ratio-based placement
    public static bool RollRatio(IRandom rand, ref int ratio, ref int max);

    // Utility: Safely add a hall with connected rooms
    public static void SafeAddHall(LocRay4 locRay, GridPlan floorPlan,
        IPermissiveRoomGen hallGen, IRoomGen roomGen,
        ComponentCollection roomComponents, ComponentCollection hallComponents,
        bool preferHall = false);
}
```

### GridPathStartStepGeneric

Extends base class with room/hall spawning support:

```csharp
public abstract class GridPathStartStepGeneric<T> : GridPathStartStep<T>
{
    public IRandPicker<RoomGen<T>> GenericRooms { get; set; }
    public ComponentCollection RoomComponents { get; set; }
    public IRandPicker<PermissiveRoomGen<T>> GenericHalls { get; set; }
    public ComponentCollection HallComponents { get; set; }
}
```

## Path Algorithm Classes

### GridPathBranch

Creates a minimum spanning tree of connected rooms using a branching algorithm. The most versatile grid path generator.

```csharp
var path = new GridPathBranch<MapGenContext>
{
    RoomRatio = new RandRange(70),     // Fill 70% of grid cells
    BranchRatio = new RandRange(0, 50), // Branching rate
};

var genericRooms = new SpawnList<RoomGen<MapGenContext>>
{
    { new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)), 10 },
    { new RoomGenRound<MapGenContext>(new RandRange(5, 9), new RandRange(5, 9)), 10 },
};
path.GenericRooms = genericRooms;

var genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>>
{
    { new RoomGenAngledHall<MapGenContext>(50), 10 }
};
path.GenericHalls = genericHalls;

layout.GenSteps.Add(-4, path);
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `RoomRatio` | `RandRange` | Percentage of grid cells to fill with rooms |
| `BranchRatio` | `RandRange` | Branching rate (0 = worm, 50 = tree, 100+ = fuzzy) |
| `NoForcedBranches` | `bool` | Prevent forced branches if quota not met |

### GridPathCircle

Creates a ring of rooms around the outer edge of the grid, with optional paths going inward.

```csharp
var path = new GridPathCircle<MapGenContext>
{
    CircleRoomRatio = new RandRange(50),  // 50% are real rooms, rest are halls
    Paths = new RandRange(2, 4),          // 2-4 paths to inner area
};
path.GenericRooms = genericRooms;
path.GenericHalls = genericHalls;

layout.GenSteps.Add(-4, path);
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `CircleRoomRatio` | `RandRange` | Percentage of outer rooms that are full rooms (not halls) |
| `Paths` | `RandRange` | Number of paths going to inner area |

### GridPathCross

Creates a cross (plus sign) pattern with a center room and rooms extending in four cardinal directions.

```csharp
var path = new GridPathCross<MapGenContext>();
path.GenericRooms = genericRooms;
path.GenericHalls = genericHalls;

layout.GenSteps.Add(-4, path);
```

Best results with odd-numbered grid dimensions (e.g., 5x5, 7x7).

### GridPathGrid

Creates a grid pattern with rooms on the perimeter and hallways forming an inner grid.

```csharp
var path = new GridPathGrid<MapGenContext>
{
    RoomRatio = 70,   // 70% of perimeter cells get real rooms
    HallRatio = 50,   // 50% of possible extra halls are placed
};
path.GenericRooms = genericRooms;
path.GenericHalls = genericHalls;

layout.GenSteps.Add(-4, path);
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `RoomRatio` | `int` | Percentage of perimeter rooms that are real rooms |
| `HallRatio` | `int` | Percentage of additional halls connecting perimeter rooms |

### GridPathTwoSides

Creates rooms on opposite sides of the grid with hallways bridging the gap.

```csharp
var path = new GridPathTwoSides<MapGenContext>
{
    GapAxis = Axis4.Horiz,  // Rooms on left/right, halls in between
};
path.GenericRooms = genericRooms;
path.GenericHalls = genericHalls;

layout.GenSteps.Add(-4, path);
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `GapAxis` | `Axis4` | Direction of the gap (Horiz = left/right rooms, Vert = top/bottom rooms) |

### GridPathSpecific

Creates an exact layout from pre-specified room and hall positions. Useful for hand-crafted levels.

```csharp
var path = new GridPathSpecific<MapGenContext>();

// Define specific rooms
path.SpecificRooms = new List<SpecificGridRoomPlan<MapGenContext>>
{
    new SpecificGridRoomPlan<MapGenContext>(new Rect(0, 0, 1, 1), roomGen, components),
    new SpecificGridRoomPlan<MapGenContext>(new Rect(1, 0, 1, 1), roomGen, components),
};

// Define halls (2D arrays matching grid dimensions)
path.SpecificVHalls = new PermissiveRoomGen<MapGenContext>[gridWidth][];
path.SpecificHHalls = new PermissiveRoomGen<MapGenContext>[gridWidth - 1][];
// ... populate arrays ...

layout.GenSteps.Add(-4, path);
```

## Usage Example

From `Ex3_Grid`:

```csharp
var layout = new MapGen<MapGenContext>();

// Initialize a 6x4 grid of 10x10 cells
var startGen = new InitGridPlanStep<MapGenContext>(1)
{
    CellX = 6,
    CellY = 4,
    CellWidth = 9,
    CellHeight = 9,
};
layout.GenSteps.Add(-4, startGen);

// Create branching path
var path = new GridPathBranch<MapGenContext>
{
    RoomRatio = new RandRange(70),
    BranchRatio = new RandRange(0, 50),
};

var genericRooms = new SpawnList<RoomGen<MapGenContext>>
{
    { new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)), 10 },
    { new RoomGenRound<MapGenContext>(new RandRange(5, 9), new RandRange(5, 9)), 10 },
};
path.GenericRooms = genericRooms;

var genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>>
{
    { new RoomGenAngledHall<MapGenContext>(50), 10 }
};
path.GenericHalls = genericHalls;

layout.GenSteps.Add(-4, path);

// Convert grid to floor plan
layout.GenSteps.Add(-2, new DrawGridToFloorStep<MapGenContext>());

// Draw floor plan to tiles
layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));

MapGenContext context = layout.GenMap(MathUtils.Rand.NextUInt64());
```

## Algorithm Interfaces

### IGridPathBranch

```csharp
public interface IGridPathBranch
{
    RandRange RoomRatio { get; set; }
    RandRange BranchRatio { get; set; }
}
```

### IGridPathCircle

```csharp
public interface IGridPathCircle
{
    RandRange CircleRoomRatio { get; set; }
    RandRange Paths { get; set; }
}
```

### IGridPathGrid

```csharp
public interface IGridPathGrid
{
    int RoomRatio { get; set; }
    int HallRatio { get; set; }
}
```

## Creating Custom Path Algorithms

1. Inherit from `GridPathStartStepGeneric<T>`
2. Override `ApplyToPath()` to implement your algorithm

```csharp
[Serializable]
public class GridPathSpiral<T> : GridPathStartStepGeneric<T>
    where T : class, IRoomGridGenContext
{
    public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
    {
        floorPlan.Clear();

        // Spiral from outside in
        int left = 0, right = floorPlan.GridWidth - 1;
        int top = 0, bottom = floorPlan.GridHeight - 1;
        Loc? prevLoc = null;

        while (left <= right && top <= bottom)
        {
            // Top edge (left to right)
            for (int x = left; x <= right; x++)
            {
                AddRoomWithHall(rand, floorPlan, new Loc(x, top), prevLoc);
                prevLoc = new Loc(x, top);
            }
            top++;

            // Right edge (top to bottom)
            for (int y = top; y <= bottom; y++)
            {
                AddRoomWithHall(rand, floorPlan, new Loc(right, y), prevLoc);
                prevLoc = new Loc(right, y);
            }
            right--;

            // Bottom edge (right to left)
            if (top <= bottom)
            {
                for (int x = right; x >= left; x--)
                {
                    AddRoomWithHall(rand, floorPlan, new Loc(x, bottom), prevLoc);
                    prevLoc = new Loc(x, bottom);
                }
                bottom--;
            }

            // Left edge (bottom to top)
            if (left <= right)
            {
                for (int y = bottom; y >= top; y--)
                {
                    AddRoomWithHall(rand, floorPlan, new Loc(left, y), prevLoc);
                    prevLoc = new Loc(left, y);
                }
                left++;
            }
        }
    }

    private void AddRoomWithHall(IRandom rand, GridPlan floorPlan, Loc loc, Loc? prevLoc)
    {
        floorPlan.AddRoom(loc, GenericRooms.Pick(rand), RoomComponents.Clone());

        if (prevLoc.HasValue)
        {
            Loc diff = loc - prevLoc.Value;
            Dir4 dir = DirExt.GetDir(diff);
            floorPlan.SetHall(new LocRay4(prevLoc.Value, dir),
                GenericHalls.Pick(rand), HallComponents.Clone());
        }
    }
}
```

## Grid Pipeline

A typical grid-based generation pipeline:

1. `InitGridPlanStep` - Initialize the grid structure
2. Grid Path Step (e.g., `GridPathBranch`) - Populate with rooms/halls
3. `DrawGridToFloorStep` - Convert grid to floor plan
4. `DrawFloorToTileStep` - Render floor plan to tiles
5. Additional steps (stairs, water, spawning, etc.)

## Related Modules

- **[../](../)** - Parent Grid module
- **[FloorPlan/Paths/](../../FloorPlan/Paths/)** - Freeform path algorithms
- **[Rooms/](../../Rooms/)** - Room generators
- **[Rooms/Halls/](../../Rooms/Halls/)** - Hall generators

## See Also

- `Ex3_Grid` - Grid-based generation example
- `GridPlan` - The data structure populated by path steps
- `DrawGridToFloorStep` - Converts grid plan to floor plan
