# Water / Terrain Generation

[![RogueElements](https://img.shields.io/nuget/v/RogueElements?label=RogueElements)](https://www.nuget.org/packages/RogueElements)

Water and terrain pattern generation for procedural roguelike maps. This module provides classes for generating natural-looking terrain features using Perlin noise and cellular automata.

## Purpose

The Water module generates terrain patterns (water, lava, chasms, etc.) on existing tile maps. It supports:
- Perlin noise-based continuous terrain generation
- Cellular automata blob generation
- Stencil-based placement rules (which tiles can be painted)
- Chokepoint-aware placement (prevents breaking map connectivity)

## Water Generation Steps

### PerlinWaterStep

Generates random terrain spread using Perlin noise, creating natural-looking continuous patterns.

```csharp
const int waterTerrain = 2;

// Generate water covering approximately 35% of eligible tiles
var waterStep = new PerlinWaterStep<MapGenContext>(
    waterPercent: new RandRange(35),
    complexity: 3,                      // Higher = more variation
    terrain: new Tile(waterTerrain),
    stencil: new MapTerrainStencil<MapGenContext>(false, true, false, false),  // Only on floor tiles
    softness: 1                         // Minimum water tile size (2^softness)
);
waterStep.Bowl = true;  // Prevents edge cutoffs

layout.GenSteps.Add(3, waterStep);
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `WaterPercent` | `RandRange` | Percentage of map to cover with terrain |
| `OrderComplexity` | `int` | Perlin noise iterations (higher = more varied) |
| `OrderSoftness` | `int` | Minimum tile group size (0 = 1x1, 1 = 2x2, etc.) |
| `Bowl` | `bool` | Distort edges to prevent awkward boundary cutoffs |

### BlobWaterStep

Creates distinct blobs of terrain using cellular automata, then places them randomly around the map.

```csharp
var blobStep = new BlobWaterStep<MapGenContext>(
    blobs: new RandRange(3, 6),           // Number of blobs to place
    terrain: new Tile(waterTerrain),
    stencil: new MapTerrainStencil<MapGenContext>(false, true, false, false),
    blobStencil: new DefaultBlobStencil<MapGenContext>(),
    areaScale: new IntRange(16, 25),      // Final blob size range (NxN tiles)
    generateScale: new IntRange(20, 30)   // Generation size range (larger than final)
);

layout.GenSteps.Add(3, blobStep);
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Blobs` | `RandRange` | Number of blobs to place |
| `GenerateScale` | `IntRange` | Size of area to generate blob in |
| `AreaScale` | `IntRange` | Acceptable final blob size |
| `BlobStencil` | `IBlobStencil<T>` | Blob-level placement validation |

## Terrain Stencils

Stencils determine which tiles are eligible for terrain placement.

### ITerrainStencil&lt;T&gt;

```csharp
public interface ITerrainStencil<T>
    where T : class, ITiledGenContext
{
    bool Test(T map, Loc loc);
}
```

### MapTerrainStencil

Filters by tile type (room, wall, blocked):

```csharp
// Allow painting on floor tiles only
var floorOnly = new MapTerrainStencil<MapGenContext>(
    room: false,    // Don't allow room terrain
    wall: true,     // Allow where floor currently is
    blocked: false, // Don't allow blocked tiles
    not: false      // Don't invert
);

// Allow painting anywhere EXCEPT walls
var notWalls = new MapTerrainStencil<MapGenContext>(
    room: false, wall: true, blocked: false, not: true
);
```

### DefaultTerrainStencil

Allows all tiles (no filtering):

```csharp
var allowAll = new DefaultTerrainStencil<MapGenContext>();
```

### MultiTerrainStencil

Combines multiple stencils with AND/OR logic:

```csharp
var combined = new MultiTerrainStencil<MapGenContext>(
    stencil1,
    stencil2
);
```

### MatchTerrainStencil

Matches specific terrain types:

```csharp
var matchWater = new MatchTerrainStencil<MapGenContext>(waterTile);
```

### BorderTerrainStencil

Only allows placement at map borders:

```csharp
var borderOnly = new BorderTerrainStencil<MapGenContext>(borderWidth: 2);
```

### NoChokepointTerrainStencil

Prevents painting tiles that would break map connectivity:

```csharp
// Prevent water from blocking paths
var noChokepoint = new NoChokepointTerrainStencil<MapGenContext>(
    new MapTerrainStencil<MapGenContext>(false, true, false, false)
);
```

## Blob Stencils

Blob stencils validate entire blob placements (all-or-nothing).

### IBlobStencil&lt;T&gt;

```csharp
public interface IBlobStencil<T>
    where T : class, ITiledGenContext
{
    bool Test(T map, Rect rect, Grid.LocTest blobTest);
}
```

### DefaultBlobStencil

Allows all blob placements:

```csharp
var allowAll = new DefaultBlobStencil<MapGenContext>();
```

### NoChokepointStencil

Prevents blobs that would disconnect the map:

```csharp
// Only place blobs that don't block paths
var safeBlobs = new NoChokepointStencil<MapGenContext>(
    new MapTerrainStencil<MapGenContext>(false, true, false, false)
);
safeBlobs.Global = true;  // Check entire map connectivity
```

### MultiBlobStencil

Combines multiple blob stencils:

```csharp
var combined = new MultiBlobStencil<MapGenContext>(stencil1, stencil2);
```

### BlobTileStencil

Validates blobs based on tile-level stencil for all tiles in blob:

```csharp
var tileCheck = new BlobTileStencil<MapGenContext>(
    new MapTerrainStencil<MapGenContext>(false, true, false, false)
);
```

### BlobTilePercentStencil

Requires a percentage of blob tiles to pass stencil:

```csharp
var percentCheck = new BlobTilePercentStencil<MapGenContext>(
    new MapTerrainStencil<MapGenContext>(false, true, false, false),
    minPercent: 80  // At least 80% must be valid
);
```

## Usage Example

From `Ex5_Terrain`:

```csharp
var layout = new MapGen<MapGenContext>();

// ... grid and path setup ...

// Add stairs
layout.GenSteps.Add(2, new FloorStairsStep<MapGenContext, StairsUp, StairsDown>(
    0, new StairsUp(), new StairsDown()
));

// Generate water (terrain ID 2) with 35% coverage
const int terrain = 2;
var waterPostProc = new PerlinWaterStep<MapGenContext>(
    new RandRange(35),
    3,                    // Complexity
    new Tile(terrain),
    new MapTerrainStencil<MapGenContext>(false, true, false, false),  // Floor tiles only
    1                     // Softness
);
layout.GenSteps.Add(3, waterPostProc);

// Fix diagonal water connections
layout.GenSteps.Add(4, new DropDiagonalBlockStep<MapGenContext>(new Tile(terrain)));

// Remove isolated water in walls
layout.GenSteps.Add(4, new EraseIsolatedStep<MapGenContext>(new Tile(terrain)));

MapGenContext context = layout.GenMap(MathUtils.Rand.NextUInt64());
```

## Creating Custom Terrain Steps

1. Inherit from `WaterStep<T>`
2. Override `Apply()` and use `DrawBlob()` or `DrawLocs()` helpers

```csharp
[Serializable]
public class RiverStep<T> : WaterStep<T>
    where T : class, ITiledGenContext
{
    public int RiverWidth { get; set; } = 2;

    public override void Apply(T map)
    {
        // Generate river path from top to bottom
        var riverLocs = new List<Loc>();
        int x = map.Rand.Next(RiverWidth, map.Width - RiverWidth);

        for (int y = 0; y < map.Height; y++)
        {
            // Meander left or right
            x += map.Rand.Next(-1, 2);
            x = Math.Clamp(x, RiverWidth, map.Width - RiverWidth - 1);

            // Add river width
            for (int w = -RiverWidth / 2; w <= RiverWidth / 2; w++)
                riverLocs.Add(new Loc(x + w, y));
        }

        DrawLocs(map, riverLocs.ToArray());
    }
}
```

## Creating Custom Stencils

### Terrain Stencil

```csharp
[Serializable]
public class DistanceFromEdgeStencil<T> : ITerrainStencil<T>
    where T : class, ITiledGenContext
{
    public int MinDistance { get; set; }

    public bool Test(T map, Loc loc)
    {
        int distX = Math.Min(loc.X, map.Width - 1 - loc.X);
        int distY = Math.Min(loc.Y, map.Height - 1 - loc.Y);
        return Math.Min(distX, distY) >= MinDistance;
    }
}
```

### Blob Stencil

```csharp
[Serializable]
public class MaxAreaBlobStencil<T> : IBlobStencil<T>
    where T : class, ITiledGenContext
{
    public int MaxArea { get; set; }

    public bool Test(T map, Rect rect, Grid.LocTest blobTest)
    {
        int count = 0;
        for (int x = rect.X; x < rect.End.X; x++)
        {
            for (int y = rect.Y; y < rect.End.Y; y++)
            {
                if (blobTest(new Loc(x, y)))
                    count++;
            }
        }
        return count <= MaxArea;
    }
}
```

## Related Modules

- **[../](../)** - Parent Tiles module (tile initialization, cleanup)
- **[Rooms/](../../Rooms/)** - Room generators
- **[Rand/](../../../Rand/)** - Noise generation utilities

## See Also

- `Ex5_Terrain` - Water generation with Perlin noise example
- `NoiseGen` - Perlin noise and cellular automata algorithms
