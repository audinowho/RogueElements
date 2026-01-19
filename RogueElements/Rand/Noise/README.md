# Noise

[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg)](../../../../LICENSE)

Noise generation utilities for natural-looking procedural terrain.

## Overview

The `Noise` folder provides deterministic noise functions used primarily for terrain generation. Unlike sequential RNG, noise functions allow querying random values at any position without affecting other positions - essential for coherent terrain patterns.

## Key Difference: RNG vs Noise

| Aspect | RNG (ReRandom) | Noise (ReNoise) |
|--------|----------------|-----------------|
| **Access** | Sequential | Position-based |
| **Use case** | Picking items, shuffling | Terrain heightmaps |
| **State** | Changes each call | Stateless |
| **Example** | "Give me the next random number" | "What's the value at (x, y)?" |

## Core Classes

### INoise Interface

```csharp
public interface INoise
{
    ulong FirstSeed { get; }

    int GetInt(ulong position);
    int GetInt(ulong position, int maxValue);
    int GetInt(ulong position, int minValue, int maxValue);

    ulong GetUInt64(ulong position);
    ulong Get2DUInt64(ulong x, ulong y);  // 2D noise

    double GetDouble(ulong position);
}
```

### ReNoise

The default noise implementation based on murmur3 hash. Key properties:

- **Deterministic** - Same seed + position always yields same result
- **Uniform distribution** - Values evenly spread across range
- **Fast** - Single hash computation per query
- **2D support** - `Get2DUInt64(x, y)` for spatial coherence

```csharp
// Create noise generator with seed
var noise = new ReNoise(12345);

// Query values at specific positions
int a = noise.GetInt(0);      // Always same for position 0
int b = noise.GetInt(1000);   // Independent of position 0
int c = noise.GetInt(0);      // Same as 'a' - deterministic

// 2D queries for terrain
ulong height = noise.Get2DUInt64(x: 10, y: 20);
```

## Usage in Terrain Generation

The noise utilities power the Perlin noise-based water generation:

```csharp
// From PerlinWaterStep.Apply()
int[][] noise = NoiseGen.PerlinNoise(
    map.Rand,
    map.Width,
    map.Height,
    orderComplexity,  // Number of octaves
    orderSoftness     // Minimum feature size
);

// Convert to water based on threshold
for (int xx = 0; xx < map.Width; xx++)
{
    for (int yy = 0; yy < map.Height; yy++)
    {
        if (noise[xx][yy] < waterMark)
            map.SetTile(new Loc(xx, yy), waterTile);
    }
}
```

### Perlin Noise Parameters

| Parameter | Effect | Typical Value |
|-----------|--------|---------------|
| `OrderComplexity` | Number of noise octaves - higher = more detail | 2-4 |
| `OrderSoftness` | Minimum blob size (2^n tiles) | 0-2 |
| `WaterPercent` | Target coverage percentage | 20-50 |

## Example: Ex5_Terrain

Water generation in action:

```csharp
// Generate water covering ~35% of walkable tiles
const int terrain = 2;  // Water terrain ID
var waterStep = new PerlinWaterStep<MapGenContext>(
    new RandRange(35),   // 35% water coverage
    3,                   // Complexity: 3 octaves
    new Tile(terrain),
    new MapTerrainStencil<MapGenContext>(false, true, false, false),
    1                    // Softness: 2x2 minimum blobs
);
layout.GenSteps.Add(3, waterStep);
```

Result:
```
####################
#........~~........#
#...~~~~~~~~~~~~~~~#
#...~~~~~~~~~~~~~~~#
#....~~~......~~~..#
#......~~~....~~~..#
#..........~~~~....#
####################
```

## Why Noise Matters for Roguelikes

1. **Natural Terrain** - Perlin noise creates organic-looking lakes, forests, elevation
2. **Reproducibility** - Same seed regenerates identical terrain patterns
3. **Position Independence** - Query any tile without computing all previous tiles
4. **Scalability** - Works efficiently at any map size

## Global Access

Noise is accessible via `MathUtils.Noise`:

```csharp
// Global noise instance (seeded automatically)
double value = MathUtils.Noise.GetDouble(position);

// Reseed both RNG and Noise together
MathUtils.ReSeedRand(seed);
```

## See Also

- [RNG/](../RNG/) - Sequential random number generation
- [MapGen/Tiles/Water/](../../MapGen/Tiles/Water/) - Water generation steps using noise
- [Examples/Ex5_Terrain](../../../RogueElements.Examples/Ex5_Terrain/) - Complete terrain example

---

![Repobeats analytics](https://repobeats.axiom.co/api/embed/3c5a3b7f5e0c1d8a9b7c5e3a1f9d8b7c6e5a4d3c2b1a0.svg "Repobeats analytics image")
