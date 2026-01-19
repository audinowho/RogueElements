# FloorPlan Paths

[![RogueElements](https://img.shields.io/nuget/v/RogueElements?label=RogueElements)](https://www.nuget.org/packages/RogueElements)

Path generation algorithms for freeform floor plan layouts. This module provides steps that populate `FloorPlan` objects with rooms and halls in various configurations.

## Purpose

FloorPlan Paths generate the room/hall connectivity graph for freeform (non-grid) map layouts. Unlike grid paths which place rooms in fixed cells, floor plan paths allow rooms to be positioned freely, creating more organic layouts.

## Core Classes

### FloorPathStartStep

Abstract base class for all floor plan path generators:

```csharp
public abstract class FloorPathStartStep<T> : FloorPlanStep<T>
    where T : class, IFloorPlanGenContext
{
    public void CreateErrorPath(IRandom rand, FloorPlan floorPlan);
    public virtual RoomGen<T> GetDefaultGen();
}
```

### FloorPathStartStepGeneric

Extends `FloorPathStartStep` with room/hall spawning support:

```csharp
public abstract class FloorPathStartStepGeneric<T> : FloorPathStartStep<T>
{
    // The room types that can be used for rooms
    public IRandPicker<RoomGen<T>> GenericRooms { get; set; }

    // Components to label rooms with
    public ComponentCollection RoomComponents { get; set; }

    // The room types that can be used for halls
    public IRandPicker<PermissiveRoomGen<T>> GenericHalls { get; set; }

    // Components to label halls with
    public ComponentCollection HallComponents { get; set; }
}
```

## Path Algorithm Classes

### FloorPathBranch

The primary floor plan path generator. Creates a minimum spanning tree of connected rooms and halls using a branching algorithm.

```csharp
var genericRooms = new SpawnList<RoomGen<MapGenContext>>
{
    { new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)), 10 },
    { new RoomGenRound<MapGenContext>(new RandRange(5, 9), new RandRange(5, 9)), 10 },
};

var genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>>
{
    { new RoomGenAngledHall<MapGenContext>(0, new RandRange(3, 7), new RandRange(3, 7)), 10 },
    { new RoomGenSquare<MapGenContext>(new RandRange(1), new RandRange(1)), 20 },
};

var path = new FloorPathBranch<MapGenContext>(genericRooms, genericHalls)
{
    FillPercent = new RandRange(45),   // Target 45% floor coverage
    HallPercent = 50,                   // 50% chance of hall between rooms
    BranchRatio = new RandRange(0, 25), // Branching rate
};

layout.GenSteps.Add(-1, path);
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `FillPercent` | `RandRange` | Percentage of floor plan area to fill with rooms |
| `HallPercent` | `int` | Chance (0-100) that rooms connect via an intermediate hall |
| `BranchRatio` | `RandRange` | Branching rate (0 = worm, 50 = tree, 100+ = fuzzy worm) |
| `NoForcedBranches` | `bool` | Prevent forced branches even if quota not met |

#### BranchRatio Guide

- **0**: Linear layout (worm shape)
- **50**: Moderate branching (tree shape)
- **100**: Branch on every extension
- **200**: Multiple branches per extension (fuzzy worm)

### IFloorPathBranch

Interface for branching path algorithms:

```csharp
public interface IFloorPathBranch
{
    RandRange FillPercent { get; set; }
    int HallPercent { get; set; }
    RandRange BranchRatio { get; set; }
}
```

## Usage Example

From `Ex2_Rooms`:

```csharp
var layout = new MapGen<MapGenContext>();

// Initialize a 54x40 floorplan
InitFloorPlanStep<MapGenContext> startGen = new InitFloorPlanStep<MapGenContext>(54, 40);
layout.GenSteps.Add(-2, startGen);

// Create room types to place
var genericRooms = new SpawnList<RoomGen<MapGenContext>>
{
    { new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)), 10 },
    { new RoomGenRound<MapGenContext>(new RandRange(5, 9), new RandRange(5, 9)), 10 },
};

// Create hall types to place
var genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>>
{
    { new RoomGenAngledHall<MapGenContext>(0, new RandRange(3, 7), new RandRange(3, 7)), 10 },
    { new RoomGenSquare<MapGenContext>(new RandRange(1), new RandRange(1)), 20 },
};

// Create branching path
FloorPathBranch<MapGenContext> path = new FloorPathBranch<MapGenContext>(genericRooms, genericHalls)
{
    HallPercent = 50,
    FillPercent = new RandRange(45),
    BranchRatio = new RandRange(0, 25),
};

layout.GenSteps.Add(-1, path);

// Draw the floor plan to tiles
layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));

MapGenContext context = layout.GenMap(MathUtils.Rand.NextUInt64());
```

## Algorithm Details

### Room Expansion Process

1. **Start Room**: Place initial room at random valid location
2. **Terminal Extension**: Expand from end nodes (rooms with one connection)
3. **Branching**: Create branches from multi-connected rooms
4. **Collision Check**: Validate new room positions don't overlap
5. **Border Matching**: Ensure rooms can connect at shared borders

### Key Static Methods

```csharp
// Get all possible expansion points
public static List<RoomHallIndex> GetPossibleExpansions(
    FloorPlan floorPlan,
    bool branch  // True for branches, false for extensions
);

// Add valid placement locations for a room
public static void AddLegalPlacements(
    SpawnList<Loc> possiblePlacements,
    FloorPlan floorPlan,
    RoomHallIndex indexFrom,
    IRoomGen roomFrom,
    IRoomGen room,
    Dir4 expandTo
);

// Choose a random room expansion
public static ListPathBranchExpansion? ChooseRandRoomExpansion(
    IRoomGen room,
    IRoomGen hall,
    IRandom rand,
    FloorPlan floorPlan,
    List<RoomHallIndex> availableExpansions
);
```

## Creating Custom Path Algorithms

1. Inherit from `FloorPathStartStepGeneric<T>`
2. Override `ApplyToPath()` to implement your algorithm

```csharp
[Serializable]
public class FloorPathRing<T> : FloorPathStartStepGeneric<T>
    where T : class, IFloorPlanGenContext
{
    public int RoomCount { get; set; } = 8;

    public override void ApplyToPath(IRandom rand, FloorPlan floorPlan)
    {
        // Place rooms in a ring pattern
        int centerX = floorPlan.DrawRect.Width / 2;
        int centerY = floorPlan.DrawRect.Height / 2;
        int radius = Math.Min(centerX, centerY) - 5;

        RoomHallIndex? prevRoom = null;

        for (int i = 0; i < RoomCount; i++)
        {
            double angle = (2 * Math.PI * i) / RoomCount;
            int x = centerX + (int)(radius * Math.Cos(angle));
            int y = centerY + (int)(radius * Math.Sin(angle));

            // Create and position room
            var room = GenericRooms.Pick(rand).Copy();
            var size = room.ProposeSize(rand);
            room.PrepareSize(rand, size);
            room.SetLoc(new Loc(x - size.X / 2, y - size.Y / 2));

            // Add to floor plan
            if (prevRoom.HasValue)
            {
                // Connect with hall
                var hall = GenericHalls.Pick(rand).Copy();
                floorPlan.AddRoom(room, RoomComponents.Clone(), prevRoom.Value);
            }
            else
            {
                floorPlan.AddRoom(room, RoomComponents.Clone());
            }

            prevRoom = new RoomHallIndex(floorPlan.RoomCount - 1, false);
        }

        // Close the ring by connecting last to first
        // ... additional connection logic
    }
}
```

## Expansion Data Structure

```csharp
public struct ListPathBranchExpansion
{
    public RoomHallIndex From;     // Source room/hall
    public IPermissiveRoomGen Hall; // Connecting hall (may be null)
    public IRoomGen Room;          // New room to add
}
```

## Related Modules

- **[../](../)** - Parent FloorPlan module
- **[Grid/Paths/](../../Grid/Paths/)** - Grid-based path algorithms
- **[Rooms/](../../Rooms/)** - Room generators used by paths
- **[Rooms/Halls/](../../Rooms/Halls/)** - Hall generators

## See Also

- `Ex2_Rooms` - Freeform floor plan generation example
- `FloorPlan` - The data structure populated by path steps
- `DrawFloorToTileStep` - Converts floor plan to tiles
