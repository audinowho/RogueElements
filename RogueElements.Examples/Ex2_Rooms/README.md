# Example 2: Freeform Rooms

Generate maps with randomly placed rooms connected by hallways using `FloorPlan`.

## What You'll Learn

- How to use `FloorPlan` for freeform room placement
- Creating room and hall generators with `SpawnList`
- Using `FloorPathBranch` to create branching dungeon layouts
- Converting a `FloorPlan` to actual tiles

## Prerequisites

- [Example 1: Static Tiles](../Ex1_Tiles/README.md)
- Understanding of the pipeline architecture

## Concepts

### FloorPlan Architecture

`FloorPlan` is an intermediate representation that defines rooms and hallways **before** they become tiles:

```
FloorPlan (abstract room positions)
    --> DrawFloorToTileStep
    --> Tile Grid (concrete tiles)
```

This separation allows the algorithm to focus on room connectivity without worrying about tile details.

### Freeform vs Grid-Based

This example uses **freeform** placement where rooms can be positioned anywhere. This contrasts with [Example 3](../Ex3_Grid/README.md) which constrains rooms to a grid.

## Code Walkthrough

### Step 1: Initialize the FloorPlan

```csharp
InitFloorPlanStep<MapGenContext> startGen = new InitFloorPlanStep<MapGenContext>(54, 40);
layout.GenSteps.Add(-2, startGen);
```

Creates a 54x40 `FloorPlan`. Note the negative priority (`-2`) - this ensures it runs before the path generation step.

### Step 2: Define Room Types

```csharp
var genericRooms = new SpawnList<RoomGen<MapGenContext>>
{
    { new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)), 10 },
    { new RoomGenRound<MapGenContext>(new RandRange(5, 9), new RandRange(5, 9)), 10 },
};
```

`SpawnList` is a weighted random picker:
- **RoomGenSquare**: Creates rectangular rooms (4-8 tiles wide/tall)
- **RoomGenRound**: Creates rounded/elliptical rooms (5-9 tiles wide/tall)
- Weight `10` gives each equal probability

### Step 3: Define Hall Types

```csharp
var genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>>
{
    { new RoomGenAngledHall<MapGenContext>(0, new RandRange(3, 7), new RandRange(3, 7)), 10 },
    { new RoomGenSquare<MapGenContext>(new RandRange(1), new RandRange(1)), 20 },
};
```

Hallways connect rooms:
- **RoomGenAngledHall**: L-shaped or straight hallways (3-7 tiles)
- **RoomGenSquare(1,1)**: Single-tile connection points (weight 20 = more common)

### Step 4: Create the Path Generator

```csharp
FloorPathBranch<MapGenContext> path = new FloorPathBranch<MapGenContext>(genericRooms, genericHalls)
{
    HallPercent = 50,
    FillPercent = new RandRange(45),
    BranchRatio = new RandRange(0, 25),
};

layout.GenSteps.Add(-1, path);
```

`FloorPathBranch` creates a tree-like dungeon structure:

| Property | Purpose |
|----------|---------|
| `HallPercent` | Chance (%) that connections include visible halls (vs direct room adjacency) |
| `FillPercent` | Target percentage of the map to fill with rooms |
| `BranchRatio` | Chance of creating branches vs extending the main path |

### Step 5: Convert to Tiles

```csharp
layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));
```

`DrawFloorToTileStep` converts the abstract `FloorPlan` to actual tiles:
- Parameter `1` = padding (1 tile of wall around each room)
- This ensures rooms don't touch each other directly

### Step 6: Generate

```csharp
MapGenContext context = layout.GenMap(MathUtils.Rand.NextUInt64());
```

The seed ensures reproducibility - same seed = same map.

## MapGenContext Changes

The context must implement `IFloorPlanGenContext`:

```csharp
public class MapGenContext : BaseMapGenContext<Map>, IFloorPlanGenContext
{
    public FloorPlan RoomPlan { get; private set; }

    public void InitPlan(FloorPlan plan)
    {
        this.RoomPlan = plan;
    }
}
```

This allows `FloorPlan`-based steps to work with the context.

## Try It

```bash
dotnet run --project RogueElements.Examples/RogueElements.Examples.csproj
```

Press `2` to run Example 2.

**What to observe:**
- Different layout each run (procedural generation!)
- Mix of square and round rooms
- Hallways connecting rooms in a tree structure
- Some rooms at dead ends, others along the main path

**Example output:**
```
2: A Map Made with Rooms and Halls
=======================================================
######################################################
###########.........##################################
###########.........#####################............#
###########.........#####################............#
###########............................##............#
###########.........###################..............#
###########.........########...........##############
###........#........########...........##############
... (procedurally generated layout)
```

## Key Classes Used

| Class | Purpose |
|-------|---------|
| `InitFloorPlanStep<T>` | Creates the FloorPlan structure |
| `FloorPathBranch<T>` | Generates branching room layouts |
| `RoomGenSquare<T>` | Creates rectangular rooms |
| `RoomGenRound<T>` | Creates elliptical rooms |
| `RoomGenAngledHall<T>` | Creates L-shaped or straight hallways |
| `SpawnList<T>` | Weighted random selection |
| `DrawFloorToTileStep<T>` | Converts FloorPlan to tiles |
| `RandRange` | Random range (min, max) |

## Key Takeaways

1. **FloorPlan Abstraction**: Design rooms abstractly, then render to tiles
2. **Weighted Spawning**: `SpawnList` enables controlled randomness
3. **Path Algorithms**: `FloorPathBranch` creates natural dungeon layouts
4. **Padding**: The `1` in `DrawFloorToTileStep(1)` prevents rooms from merging

## Comparison: Freeform vs Grid

| Aspect | FloorPlan (Freeform) | GridPlan (Grid-Based) |
|--------|---------------------|----------------------|
| Room placement | Anywhere | Locked to grid cells |
| Hall length | Variable | Determined by cell spacing |
| Layout feel | Organic, sprawling | Structured, orderly |
| Use case | Natural caves, forests | Traditional dungeons |

## Next Steps

[Example 3: Grid-Based Layouts](../Ex3_Grid/README.md) introduces `GridPlan` for more structured dungeon generation.
