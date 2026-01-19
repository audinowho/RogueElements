# Example 5: Terrain Features

Add water and other terrain features using Perlin noise.

## What You'll Learn

- How to generate terrain with `PerlinWaterStep`
- Using stencils to control where terrain can appear
- Post-processing steps for terrain cleanup
- Working with multiple terrain types

## Prerequisites

- [Example 4: Stair Placement](../Ex4_Stairs/README.md)
- Understanding of spawnable entities

## Concepts

### Terrain Generation

RogueElements uses **Perlin noise** to create natural-looking terrain distributions. The water appears in organic blobs rather than random scattered tiles.

### Stencils

A **stencil** defines which tiles can be modified by a step. `MapTerrainStencil` filters based on current tile types:

```csharp
new MapTerrainStencil<T>(wall: false, floor: true, water: false, other: false)
```

This means: "only modify floor tiles, leave everything else alone."

### Post-Processing

After placing water, cleanup steps improve the result:
1. `DropDiagonalBlockStep` - Removes wall corners that would cause visual artifacts
2. `EraseIsolatedStep` - Removes water tiles disconnected from larger bodies

## Code Walkthrough

### Step 1: Standard Setup with Stairs

```csharp
// Grid generation (same as Example 4)
var startGen = new InitGridPlanStep<MapGenContext>(1) { /* ... */ };
layout.GenSteps.Add(-4, startGen);
layout.GenSteps.Add(-4, path);
layout.GenSteps.Add(-2, new DrawGridToFloorStep<MapGenContext>());
layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));

// Stairs (same as Example 4)
layout.GenSteps.Add(2, new FloorStairsStep<MapGenContext, StairsUp, StairsDown>(0, new StairsUp(), new StairsDown()));
```

### Step 2: Generate Water with Perlin Noise

```csharp
const int terrain = 2;  // WATER_TERRAIN_ID

var waterPostProc = new PerlinWaterStep<MapGenContext>(
    new RandRange(35),  // 35% coverage
    3,                  // noise order (complexity)
    new Tile(terrain),  // tile to place
    new MapTerrainStencil<MapGenContext>(false, true, false, false),  // only on floor
    1                   // softness
);
layout.GenSteps.Add(3, waterPostProc);
```

`PerlinWaterStep` parameters:

| Parameter | Value | Purpose |
|-----------|-------|---------|
| Coverage | `RandRange(35)` | Target 35% of valid tiles |
| Order | `3` | Noise complexity (higher = more detailed) |
| Terrain | `new Tile(2)` | Water tile to place |
| Stencil | `MapTerrainStencil(F,T,F,F)` | Only replace floor tiles |
| Softness | `1` | Edge smoothness |

### Step 3: Cleanup Diagonal Artifacts

```csharp
layout.GenSteps.Add(4, new DropDiagonalBlockStep<MapGenContext>(new Tile(terrain)));
```

**Problem**: When water touches walls diagonally, it can create visual artifacts:
```
#~
~#
```

**Solution**: `DropDiagonalBlockStep` converts blocking walls to water:
```
~~
~~
```

### Step 4: Remove Isolated Water

```csharp
layout.GenSteps.Add(4, new EraseIsolatedStep<MapGenContext>(new Tile(terrain)));
```

Removes single water tiles that aren't connected to larger bodies. This prevents awkward single-tile puddles.

## Stencil Deep Dive

`MapTerrainStencil` constructor parameters:

```csharp
MapTerrainStencil(bool wall, bool floor, bool water, bool otherTerrain)
```

Common configurations:

| Stencil | Effect |
|---------|--------|
| `(F, T, F, F)` | Only floor tiles (add water) |
| `(T, F, F, F)` | Only wall tiles (carve caves) |
| `(F, T, T, F)` | Floor and water (expand water) |
| `(T, T, F, F)` | Wall and floor (replace anything) |

## Try It

```bash
dotnet run --project RogueElements.Examples/RogueElements.Examples.csproj
```

Press `5` to run Example 5.

**What to observe:**
- `~` symbols represent water
- Water forms organic blobs, not random dots
- Water never appears on walls or stairs
- Different seeds produce different water patterns

**Example output:**
```
5: A Map with Terrain Features
=======================================================
######################################################
#######.........######################################
#######...~.....######################################
#######..<~~~...######~~~....#########################
#######...~~~...#####~~~~....#########################
#######~........#####.~~~....#########################
#######~~~~~....#####........#########################
########~~~~~~~~~~~~~~~......#########................
#################~~~~~~~~~~~~~~~~~~~~.................
#################..~~~~~~......#####..................
#################........>.....#####....~~............
######################################################
```

## Key Classes Used

| Class | Purpose |
|-------|---------|
| `PerlinWaterStep<T>` | Places terrain using Perlin noise |
| `MapTerrainStencil<T>` | Filters which tiles can be modified |
| `DropDiagonalBlockStep<T>` | Fixes diagonal wall/terrain conflicts |
| `EraseIsolatedStep<T>` | Removes disconnected terrain tiles |

## Perlin Noise Parameters

The **order** parameter controls noise detail:

| Order | Effect |
|-------|--------|
| 1 | Large, simple blobs |
| 2 | Medium-sized features |
| 3 | Detailed, organic shapes |
| 4+ | Very detailed, may look noisy |

The **softness** parameter affects edge transitions:

| Softness | Effect |
|----------|--------|
| 0 | Sharp, pixelated edges |
| 1 | Slightly smoothed edges |
| 2+ | Very soft, gradual transitions |

## Tile Protection

Note that stairs are protected from water replacement:

```csharp
public override bool CanSetTile(Loc loc, ITile tile)
{
    // Check if stairs are at this location
    foreach (var entrance in this.GenEntrances)
        if (entrance.Loc == loc) return false;
    // ...
    return true;
}
```

This ensures water generation respects previously placed entities.

## Key Takeaways

1. **Perlin Noise**: Creates natural-looking terrain distributions
2. **Stencil Filtering**: Control which tiles can be modified
3. **Post-Processing**: Cleanup steps improve visual quality
4. **Priority Ordering**: Water runs after stairs to respect placement

## Advanced Usage

You can chain multiple terrain steps:

```csharp
// Add water
layout.GenSteps.Add(3, new PerlinWaterStep<T>(35%, 3, waterTile, floorStencil, 1));

// Add lava (on remaining floor)
layout.GenSteps.Add(3, new PerlinWaterStep<T>(10%, 2, lavaTile, floorStencil, 0));

// Add grass (on remaining floor)
layout.GenSteps.Add(3, new PerlinWaterStep<T>(20%, 4, grassTile, floorStencil, 2));
```

## Next Steps

[Example 6: Item Spawning](../Ex6_Items/README.md) adds randomly placed items and monsters to the map.
