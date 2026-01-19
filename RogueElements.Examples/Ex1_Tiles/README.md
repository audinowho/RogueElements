# Example 1: Static Tiles

A minimal example demonstrating direct tile manipulation without procedural generation.

## What You'll Learn

- How to set up a basic `MapGen` pipeline
- Using `InitTilesStep` to create a blank map
- Using `SpecificTilesStep` to draw a predefined pattern
- Understanding the generation context and tile system

## Prerequisites

- Read the [Common](../Common/README.md) folder documentation to understand the base classes

## Concepts

This example creates a **static map** - one that looks the same every time. While not useful for actual roguelike gameplay, it demonstrates the fundamental pipeline architecture before adding randomness.

### The Pipeline Architecture

RogueElements uses a pipeline of `GenStep` objects orchestrated by `MapGen`:

```
MapGen.GenMap(seed)
    --> GenStep 1: InitTilesStep (creates blank tile grid)
    --> GenStep 2: SpecificTilesStep (draws pattern onto grid)
    --> returns MapGenContext
```

## Code Walkthrough

### Step 1: Create the MapGen Instance

```csharp
var layout = new MapGen<MapGenContext>();
```

The `MapGen<T>` is the orchestrator that holds all generation steps and executes them in priority order.

### Step 2: Initialize the Tile Grid

```csharp
InitTilesStep<MapGenContext> startStep = new InitTilesStep<MapGenContext>(30, 25);
layout.GenSteps.Add(0, startStep);
```

`InitTilesStep` creates a 30x25 grid filled with wall tiles (ID 0). The priority `0` determines execution order.

### Step 3: Define the Pattern

```csharp
string[] level =
{
    ".........................",
    ".........................",
    "...........#.............",
    "....###...###...###......",
    // ... more rows ...
};
```

The pattern uses `.` for floor tiles and `#` for walls. This creates a decorative cross/diamond design.

### Step 4: Convert to Tile Array

```csharp
ITile[][] tiles = new ITile[level[0].Length][];
for (int xx = 0; xx < level[0].Length; xx++)
{
    tiles[xx] = new ITile[level.Length];
    for (int yy = 0; yy < level.Length; yy++)
    {
        int id = Map.WALL_TERRAIN_ID;
        if (level[yy][xx] == '.')
            id = Map.ROOM_TERRAIN_ID;
        tiles[xx][yy] = new Tile(id);
    }
}
```

The string array is converted to a 2D tile array, mapping characters to terrain IDs.

### Step 5: Draw the Pattern

```csharp
var drawStep = new SpecificTilesStep<MapGenContext>(tiles, new Loc(2, 3));
layout.GenSteps.Add(0, drawStep);
```

`SpecificTilesStep` places the tile array at offset (2, 3) on the map. Using priority `0` means it runs after (or with) the initialization step.

### Step 6: Generate and Display

```csharp
MapGenContext context = layout.GenMap(MathUtils.Rand.NextUInt64());
Print(context.Map, title);
```

`GenMap()` executes all steps in priority order and returns the completed context.

## Try It

```bash
dotnet run --project RogueElements.Examples/RogueElements.Examples.csproj
```

Press `1` to run Example 1.

**What to observe:**
- The same pattern appears every time (static map)
- The pattern is offset from the top-left corner by (2, 3) tiles
- Wall tiles (`#`) surround the pattern area

**Expected output:**
```
1: A Static Map Example
===============================
##############################
##############################
##############################
##.........................###
##.........................###
##...........#.............###
##....###...###...###......###
... (decorative pattern continues)
```

## Key Classes Used

| Class | Purpose |
|-------|---------|
| `MapGen<T>` | Orchestrates the generation pipeline |
| `InitTilesStep<T>` | Creates a blank tile grid of specified dimensions |
| `SpecificTilesStep<T>` | Draws a predefined tile pattern at an offset |
| `MapGenContext` | Holds map state during generation |
| `Tile` | Simple tile implementation with an ID |

## Key Takeaways

1. **Pipeline Pattern**: Generation is a series of steps, each modifying the context
2. **Priority Ordering**: Steps are executed by priority (lower = earlier)
3. **Context Pattern**: The context holds all state and is passed through steps
4. **Tile IDs**: Terrain types are represented by integer IDs (0=wall, 1=floor)

## Next Steps

[Example 2: Freeform Rooms](../Ex2_Rooms/README.md) introduces procedural room generation using `FloorPlan`.
