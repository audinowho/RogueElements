# Tiles

[![RogueElements](https://img.shields.io/nuget/v/RogueElements?label=RogueElements)](https://www.nuget.org/packages/RogueElements)

Tile manipulation operations for procedural roguelike map generation. This module provides classes for initializing, modifying, and cleaning up tile-based maps.

## Purpose

The Tiles module handles direct tile operations including:
- Initializing empty maps with wall tiles
- Drawing specific tile patterns
- Cleaning up terrain anomalies (diagonal blocks, isolated areas)
- Detecting and handling isolated regions

## Core Interface

### ITiledGenContext

The context interface required for tile operations:

```csharp
public interface ITiledGenContext : IGenContext
{
    ITile RoomTerrain { get; }     // Default floor/room tile
    ITile WallTerrain { get; }     // Default wall tile

    int Width { get; }
    int Height { get; }
    bool Wrap { get; }
    bool TilesInitialized { get; }

    bool TileBlocked(Loc loc);
    bool TileBlocked(Loc loc, bool diagonal);
    ITile GetTile(Loc loc);
    bool CanSetTile(Loc loc, ITile tile);
    bool TrySetTile(Loc loc, ITile tile);
    void SetTile(Loc loc, ITile tile);
    void CreateNew(int tileWidth, int tileHeight, bool wrap = false);
}
```

### ITile

Interface for individual tiles:

```csharp
public interface ITile
{
    ITile Copy();
    bool TileEquivalent(ITile other);
}
```

## Tile Steps

### InitTilesStep

Initializes a blank map filled with wall tiles. This is typically the first step in any tile-based generation pipeline.

```csharp
// Initialize a 30x25 map filled with walls
var initStep = new InitTilesStep<MapGenContext>(30, 25);
layout.GenSteps.Add(0, initStep);
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Width` | `int` | Width of the map in tiles |
| `Height` | `int` | Height of the map in tiles |

### SpecificTilesStep

Draws a specific array of tiles onto the map at a given offset. Useful for hand-crafted areas.

```csharp
// Create tile pattern
string[] level = {
    ".........................",
    "...........#.............",
    "....###...###...###......",
    "...#.#.....#.....#.#.....",
};

ITile[][] tiles = new ITile[level[0].Length][];
for (int xx = 0; xx < level[0].Length; xx++)
{
    tiles[xx] = new ITile[level.Length];
    for (int yy = 0; yy < level.Length; yy++)
    {
        int id = level[yy][xx] == '.' ? Map.ROOM_TERRAIN_ID : Map.WALL_TERRAIN_ID;
        tiles[xx][yy] = new Tile(id);
    }
}

// Draw at offset (2, 3)
var drawStep = new SpecificTilesStep<MapGenContext>(tiles, new Loc(2, 3));
layout.GenSteps.Add(0, drawStep);
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Tiles` | `ITile[][]` | 2D array of tiles to draw |
| `Offset` | `Loc` | Position offset for drawing |

### DropDiagonalBlockStep

Merges blobs of terrain that touch only diagonally by filling in one or both of the adjacent wall tiles. Prevents visual artifacts and pathfinding issues.

```csharp
// Fix diagonal water connections
const int terrain = 2;  // Water terrain ID
var dropStep = new DropDiagonalBlockStep<MapGenContext>(new Tile(terrain));
layout.GenSteps.Add(4, dropStep);
```

**Before:**
```
.~#~.
~#.#~
#...#
~#.#~
.~#~.
```

**After (one possible result):**
```
.~~.
~~.~~
~...~
~~.~~
.~~.
```

### EraseIsolatedStep

Erases blobs of a specific terrain that do not touch walkable ground. Removes floating terrain patches trapped in walls.

```csharp
// Remove isolated water patches
var eraseStep = new EraseIsolatedStep<MapGenContext>(new Tile(terrain));
layout.GenSteps.Add(4, eraseStep);
```

**Before:**
```
#####
#~~~#
#####
#...#
#####
```

**After:**
```
#####
#####
#####
#...#
#####
```

### DetectIsolatedStep

Detects isolated walkable areas that cannot be reached from the main floor.

```csharp
var detectStep = new DetectIsolatedStep<MapGenContext>();
layout.GenSteps.Add(5, detectStep);
```

### DetectIsolatedStairsStep

Specifically detects when stairs are placed in isolated areas unreachable from other stairs.

```csharp
var detectStairsStep = new DetectIsolatedStairsStep<MapGenContext>();
layout.GenSteps.Add(5, detectStairsStep);
```

### EraseIsolatedFromSpawnStep

Removes spawn points that are in isolated (unreachable) areas.

```csharp
var eraseSpawnStep = new EraseIsolatedFromSpawnStep<MapGenContext, Item>();
layout.GenSteps.Add(5, eraseSpawnStep);
```

### StairsStep

Places stairs on the map. Works with entrance and exit types.

```csharp
var stairsStep = new StairsStep<MapGenContext, StairsUp, StairsDown>(
    new StairsUp(),
    new StairsDown()
);
layout.GenSteps.Add(2, stairsStep);
```

## Usage Example

From `Ex1_Tiles`:

```csharp
var layout = new MapGen<MapGenContext>();

// Initialize a 30x25 blank map full of Wall tiles
InitTilesStep<MapGenContext> startStep = new InitTilesStep<MapGenContext>(30, 25);
layout.GenSteps.Add(0, startStep);

// Draw a specific array of tiles onto the map at offset X2,Y3
string[] level = {
    ".........................",
    "...........#.............",
    "....###...###...###......",
    "...#.#.....#.....#.#.....",
    "...####...###...####.....",
    // ... more rows ...
};

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

var drawStep = new SpecificTilesStep<MapGenContext>(tiles, new Loc(2, 3));
layout.GenSteps.Add(0, drawStep);

// Run the generator
MapGenContext context = layout.GenMap(MathUtils.Rand.NextUInt64());
```

## Terrain Cleanup Pipeline

A typical terrain cleanup sequence after water generation:

```csharp
// Generate water terrain
const int terrain = 2;
var waterStep = new PerlinWaterStep<MapGenContext>(
    new RandRange(35), 3,
    new Tile(terrain),
    new MapTerrainStencil<MapGenContext>(false, true, false, false),
    1
);
layout.GenSteps.Add(3, waterStep);

// Fix diagonal water touching
layout.GenSteps.Add(4, new DropDiagonalBlockStep<MapGenContext>(new Tile(terrain)));

// Remove isolated water in walls
layout.GenSteps.Add(4, new EraseIsolatedStep<MapGenContext>(new Tile(terrain)));
```

## Creating Custom Tile Steps

1. Inherit from `GenStep<T>` where T implements `ITiledGenContext`
2. Override `Apply()` to modify tiles

```csharp
[Serializable]
public class BorderWallStep<T> : GenStep<T>
    where T : class, ITiledGenContext
{
    public int BorderWidth { get; set; } = 1;

    public override void Apply(T map)
    {
        // Draw wall border around entire map
        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                bool isBorder = x < BorderWidth || x >= map.Width - BorderWidth
                             || y < BorderWidth || y >= map.Height - BorderWidth;
                if (isBorder)
                    map.SetTile(new Loc(x, y), map.WallTerrain.Copy());
            }
        }
    }
}
```

## Related Modules

- **[Water/](./Water/)** - Water and terrain generation (Perlin noise, blob placement)
- **[Rooms/](../Rooms/)** - Room generators that draw tiles
- **[FloorPlan/](../FloorPlan/)** - Floor plan to tile conversion

## See Also

- `Ex1_Tiles` - Static tile map example
- `Ex5_Terrain` - Terrain generation with cleanup steps
