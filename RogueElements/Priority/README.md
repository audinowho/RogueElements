# Priority

[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg)](../../../LICENSE)

Priority queue system for ordering GenSteps in the map generation pipeline.

## Overview

The `Priority` folder provides the ordering mechanism that determines when each GenStep executes. This is critical for roguelike generation where steps must occur in a specific order (e.g., create rooms before placing stairs).

## Core Concepts

### Why Priority Ordering?

Map generation requires careful sequencing:

1. Initialize the grid structure
2. Create rooms and hallways
3. Draw rooms to tiles
4. Add terrain features (water, lava)
5. Place stairs
6. Spawn items and enemies

Priority values control this order. Lower priorities execute first.

### The Priority Struct

A hierarchical ordering value supporting dot-notation for fine-grained control:

```csharp
// Simple integer priorities
var init = new Priority(-4);      // Early
var draw = new Priority(0);       // Middle
var spawn = new Priority(6);      // Late

// Hierarchical priorities for sub-ordering
var water = new Priority(3);           // 3
var waterCleanup = new Priority(3, 1); // 3.1 (after water)
var waterErode = new Priority(3, 2);   // 3.2 (after cleanup)

// Extend existing priority
var subStep = new Priority(water, 1);  // Creates 3.1
```

**Comparison:**
```
-4 < 0 < 3 < 3.1 < 3.2 < 6
```

## Key Classes

### Priority

A comparable, multi-level priority value:

```csharp
[Serializable]
public struct Priority : IComparable<Priority>, IEquatable<Priority>
{
    public static Priority Invalid;  // No value
    public static Priority Zero;     // Priority(0)

    public Priority(params int[] vals);
    public Priority(Priority other, params int[] vals);  // Extend existing

    public int Length { get; }       // Number of levels
    public int this[int ii] { get; } // Access level value

    // Full comparison operators: ==, !=, <, >, <=, >=
}
```

### PriorityList<T>

A dictionary-backed collection that maintains items by priority:

```csharp
[Serializable]
public class PriorityList<T> : IPriorityList<T>
{
    public int PriorityCount { get; }  // Number of distinct priorities
    public int Count { get; }          // Total items

    public void Add(Priority priority, T item);
    public void Add(int priority, T item);  // Convenience overload

    public void Insert(Priority priority, int index, T item);
    public void RemoveAt(Priority priority, int index);

    public T Get(Priority priority, int index);
    public void Set(Priority priority, int index, T item);

    public IEnumerable<Priority> GetPriorities();  // Sorted
    public IEnumerable<T> GetItems(Priority priority);
    public IEnumerable<T> EnumerateInOrder();  // All items, sorted by priority
}
```

## Usage in MapGen

`MapGen<T>` uses `PriorityList<GenStep<T>>` to hold generation steps:

```csharp
public class MapGen<T> where T : class, IGenContext
{
    public PriorityList<GenStep<T>> GenSteps { get; }

    public T GenMap(ulong seed)
    {
        // Steps are executed in priority order
        foreach (Priority priority in GenSteps.GetPriorities())
        {
            foreach (IGenStep step in GenSteps.GetItems(priority))
            {
                step.Apply(map);
            }
        }
    }
}
```

## Example: Building a Generation Pipeline

```csharp
var layout = new MapGen<MapGenContext>();

// Priority -4: Initialize grid structure
layout.GenSteps.Add(-4, new InitGridPlanStep<MapGenContext>(1)
{
    CellX = 6, CellY = 4,
    CellWidth = 9, CellHeight = 9
});

// Priority -4: Create room paths (same priority, runs after init)
layout.GenSteps.Add(-4, new GridPathBranch<MapGenContext>
{
    RoomRatio = new RandRange(70),
    BranchRatio = new RandRange(0, 50),
    GenericRooms = roomSpawnList,
    GenericHalls = hallSpawnList
});

// Priority -2: Convert grid to floor plan
layout.GenSteps.Add(-2, new DrawGridToFloorStep<MapGenContext>());

// Priority 0: Draw floor plan to tiles
layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));

// Priority 2: Place stairs
layout.GenSteps.Add(2, new FloorStairsStep<MapGenContext, StairsUp, StairsDown>(
    0, new StairsUp(), new StairsDown()
));

// Priority 3: Generate water terrain
layout.GenSteps.Add(3, new PerlinWaterStep<MapGenContext>(
    new RandRange(35), 3, new Tile(2), stencil, 1
));

// Priority 4: Clean up water artifacts
layout.GenSteps.Add(4, new DropDiagonalBlockStep<MapGenContext>(new Tile(2)));
layout.GenSteps.Add(4, new EraseIsolatedStep<MapGenContext>(new Tile(2)));

// Priority 6: Spawn items and mobs
layout.GenSteps.Add(6, itemPlacementStep);
layout.GenSteps.Add(6, mobPlacementStep);
```

### Execution Order

```
Priority -4: InitGridPlanStep
Priority -4: GridPathBranch
Priority -2: DrawGridToFloorStep
Priority  0: DrawFloorToTileStep
Priority  2: FloorStairsStep
Priority  3: PerlinWaterStep
Priority  4: DropDiagonalBlockStep
Priority  4: EraseIsolatedStep
Priority  6: RandomSpawnStep (items)
Priority  6: RandomSpawnStep (mobs)
```

## Advanced: Hierarchical Priorities

For complex pipelines, use dot-notation to insert steps between existing ones:

```csharp
// Original water step at priority 3
layout.GenSteps.Add(new Priority(3), waterStep);

// Later, need to add a step between water (3) and cleanup (4)
// Use 3.1 instead of renumbering everything
layout.GenSteps.Add(new Priority(3, 1), waterEdgeStep);
```

Execution order: `3 -> 3.1 -> 4`

## Multiple Steps at Same Priority

Items at the same priority execute in insertion order:

```csharp
// Both at priority 6, items runs first (added first)
layout.GenSteps.Add(6, itemPlacement);
layout.GenSteps.Add(6, mobPlacement);
```

## Common Priority Conventions

| Priority | Purpose |
|----------|---------|
| -4 | Grid/structure initialization |
| -2 | Floor plan generation |
| 0 | Tile drawing |
| 2 | Stairs placement |
| 3-4 | Terrain features (water, lava) |
| 6 | Entity spawning (items, mobs) |

## See Also

- [MapGen/MapGen.cs](../MapGen/MapGen.cs) - How priorities drive generation
- [Rand/](../Rand/) - Random utilities used alongside priority ordering
- [Examples/](../../RogueElements.Examples/) - Full pipeline examples

---

![Repobeats analytics](https://repobeats.axiom.co/api/embed/3c5a3b7f5e0c1d8a9b7c5e3a1f9d8b7c6e5a4d3c2b1a0.svg "Repobeats analytics image")
