# Common

Shared infrastructure classes used across all RogueElements examples.

## Overview

The Common folder provides base classes and reusable components that all examples build upon. This establishes a consistent foundation for map representation, context management, and spawnable entities.

## Classes

### BaseMap

The abstract base class for all map data structures.

```csharp
public abstract class BaseMap
{
    public const int WALL_TERRAIN_ID = 0;
    public const int ROOM_TERRAIN_ID = 1;
    public const int WATER_TERRAIN_ID = 2;

    public ReRandom Rand { get; set; }
    public Tile[][] Tiles { get; set; }
    public int Width => this.Tiles.Length;
    public int Height => this.Tiles[0].Length;

    public void InitializeTiles(int width, int height);
}
```

**Key Points:**
- Defines terrain ID constants used throughout all examples
- Holds the 2D tile array and RNG instance
- Provides `InitializeTiles()` for creating the tile grid

### BaseMapGenContext

The abstract base context that implements `ITiledGenContext` for all examples.

```csharp
public abstract class BaseMapGenContext<TMap> : ITiledGenContext
    where TMap : BaseMap, new()
{
    public TMap Map { get; set; }
    public ITile RoomTerrain => new Tile(BaseMap.ROOM_TERRAIN_ID);
    public ITile WallTerrain => new Tile(BaseMap.WALL_TERRAIN_ID);

    public ITile GetTile(Loc loc);
    public bool TrySetTile(Loc loc, ITile tile);
    public void SetTile(Loc loc, ITile tile);
    public void CreateNew(int width, int height, bool wrap = false);
    public void InitSeed(ulong seed);
}
```

**Key Points:**
- Generic over the Map type, allowing each example to extend BaseMap
- Implements core tile operations required by `ITiledGenContext`
- Initializes the RNG with `ReRandom` for deterministic generation

### Tile

A simple tile implementation of `ITile`.

```csharp
public class Tile : ITile
{
    public int ID { get; set; }
    public ITile Copy();
    public bool TileEquivalent(ITile other);
}
```

### Stairs, StairsUp, StairsDown

Spawnable stair entities for floor transitions.

```csharp
public abstract class Stairs : ISpawnable
{
    public Loc Loc { get; set; }
    public abstract ISpawnable Copy();
}

public class StairsUp : Stairs, IEntrance { }
public class StairsDown : Stairs, IExit { }
```

**Key Points:**
- `StairsUp` implements `IEntrance` - marks the player spawn point
- `StairsDown` implements `IExit` - marks the floor exit
- Used by `FloorStairsStep` for automatic placement

### Room Components

Tags for marking rooms with special purposes:

| Class | Purpose |
|-------|---------|
| `MainRoomComponent` | Marks rooms as part of the main path |
| `MainHallComponent` | Marks hallways as part of the main path |
| `TreasureRoomComponent` | Marks rooms for special treasure spawning |

These components enable filtered spawning - for example, placing special loot only in treasure rooms.

## Architecture Pattern

Each example follows this inheritance pattern:

```
BaseMap (Common)
    |
    +-- Map (Ex1, Ex2, etc.) - adds example-specific data

BaseMapGenContext<TMap> (Common)
    |
    +-- MapGenContext (Ex1, Ex2, etc.) - adds example-specific interfaces
```

This allows examples to progressively add capabilities while reusing core infrastructure.

## Usage

When creating a new example:

1. Create a `Map` class extending `BaseMap`
2. Create a `MapGenContext` class extending `BaseMapGenContext<Map>`
3. Add interfaces to `MapGenContext` as needed (e.g., `IFloorPlanGenContext`, `IPlaceableGenContext<T>`)

## Next Steps

Proceed to [Example 1: Static Tiles](../Ex1_Tiles/README.md) to see these classes in action.
