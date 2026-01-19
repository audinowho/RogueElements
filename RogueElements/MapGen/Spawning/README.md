# Spawning

[![RogueElements](https://img.shields.io/nuget/v/RogueElements?label=RogueElements)](https://www.nuget.org/packages/RogueElements)

Entity placement system for procedural roguelike map generation. This module provides classes for spawning items, stairs, mobs, and other entities onto generated maps.

## Purpose

The Spawning module places entities (items, monsters, stairs, etc.) onto the generated map tiles. It separates the concerns of:
1. **What to spawn** - Determined by spawner classes
2. **Where to spawn** - Determined by spawn step classes

## Core Interfaces

### ISpawnable

The base interface for anything that can be spawned on the map:

```csharp
public interface ISpawnable
{
    ISpawnable Copy();
}
```

### IPlaceableGenContext&lt;T&gt;

The context interface required for entity placement:

```csharp
public interface IPlaceableGenContext<T> : IGenContext
    where T : ISpawnable
{
    List<Loc> GetAllFreeTiles();
    List<Loc> GetFreeTiles(Rect rect);
    bool CanPlaceItem(Loc loc);
    void PlaceItem(Loc loc, T item);
}
```

### IStepSpawner&lt;TGenContext, TSpawnable&gt;

Generates the list of what entities to spawn (but not where):

```csharp
public interface IStepSpawner<TGenContext, TSpawnable>
{
    List<TSpawnable> GetSpawns(TGenContext map);
}
```

## Spawn Steps

Spawn steps are `GenStep` implementations that determine both what and where to spawn.

### RandomSpawnStep

Spawns objects on randomly chosen tiles from the set of valid placement locations.

```csharp
// Create item spawn list
var itemSpawns = new SpawnList<Item>
{
    { new Item((int)'!'), 10 },  // Potion, weight 10
    { new Item((int)'$'), 10 },  // Gold, weight 10
    { new Item((int)'*'), 50 },  // Food, weight 50
};

// Random spawn with 10-18 items
var itemPlacement = new RandomSpawnStep<MapGenContext, Item>(
    new PickerSpawner<MapGenContext, Item>(
        new LoopedRand<Item>(itemSpawns, new RandRange(10, 19))
    )
);

layout.GenSteps.Add(6, itemPlacement);
```

### TerminalSpawnStep

Spawns objects preferentially in terminal (dead-end) rooms. Falls back to normal rooms if all dead-ends are occupied.

```csharp
var terminalSpawn = new TerminalSpawnStep<MapGenContext, Treasure>(spawner)
{
    IncludeHalls = false  // Only consider rooms, not halls
};
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `IncludeHalls` | `bool` | Whether to consider halls as valid spawn locations |

### RoomSpawnStep

Base class for room-aware spawning. Spawns entities within specific rooms based on filters.

```csharp
public abstract class RoomSpawnStep<TGenContext, TSpawnable>
{
    public List<BaseRoomFilter> Filters { get; set; }

    public virtual void SpawnRandInCandRooms(
        TGenContext map,
        SpawnList<RoomHallIndex> spawningRooms,
        List<TSpawnable> spawns,
        int decayPercent  // After spawning, room likelihood is multiplied by this %
    );
}
```

### SpecificSpawnStep

Spawns objects at exact specified locations.

```csharp
var locs = new List<Loc> { new Loc(5, 5), new Loc(10, 10) };
var specificSpawn = new SpecificSpawnStep<MapGenContext, Item>(spawner, locs);
```

### TerrainSpawnStep

Spawns objects on tiles matching specific terrain types.

```csharp
// Spawn items on water tiles
var waterSpawn = new TerrainSpawnStep<MapGenContext, Item>(spawner, waterTerrain);
```

## Spawner Classes

### PickerSpawner

Generates spawns using a randomized picker:

```csharp
var spawner = new PickerSpawner<MapGenContext, Item>(
    new LoopedRand<Item>(itemSpawns, new RandRange(10, 19))
);
```

### ContextSpawner

Generates spawns based on context state:

```csharp
var spawner = new ContextSpawner<MapGenContext, Item>();
```

## Usage Example

From `Ex6_Items`:

```csharp
var layout = new MapGen<MapGenContext>();

// ... room and path setup ...

// Apply Items with weighted spawn list
var itemSpawns = new SpawnList<Item>
{
    { new Item((int)'!'), 10 },  // Potion
    { new Item((int)']'), 10 },  // Armor
    { new Item((int)'='), 10 },  // Ring
    { new Item((int)'?'), 10 },  // Scroll
    { new Item((int)'$'), 10 },  // Gold
    { new Item((int)'/'), 10 },  // Wand
    { new Item((int)'*'), 50 },  // Food (higher weight)
};

// Spawn 10-18 items at random locations
RandomSpawnStep<MapGenContext, Item> itemPlacement =
    new RandomSpawnStep<MapGenContext, Item>(
        new PickerSpawner<MapGenContext, Item>(
            new LoopedRand<Item>(itemSpawns, new RandRange(10, 19))
        )
    );
layout.GenSteps.Add(6, itemPlacement);

// Apply Mobs with weighted spawn list
var mobSpawns = new SpawnList<Mob>
{
    { new Mob((int)'r'), 20 },  // Rat (common)
    { new Mob((int)'T'), 10 },  // Troll
    { new Mob((int)'D'), 5 },   // Dragon (rare)
};

RandomSpawnStep<MapGenContext, Mob> mobPlacement =
    new RandomSpawnStep<MapGenContext, Mob>(
        new PickerSpawner<MapGenContext, Mob>(
            new LoopedRand<Mob>(mobSpawns, new RandRange(10, 19))
        )
    );
layout.GenSteps.Add(6, mobPlacement);
```

## Creating Custom Spawnables

1. Implement `ISpawnable`
2. Add placement logic to your context

```csharp
[Serializable]
public class Trap : ISpawnable
{
    public int TrapType { get; set; }
    public Loc Loc { get; set; }

    public Trap(int trapType)
    {
        TrapType = trapType;
    }

    public ISpawnable Copy()
    {
        return new Trap(TrapType);
    }
}
```

## Creating Custom Spawn Steps

1. Inherit from `BaseSpawnStep<TGenContext, TSpawnable>`
2. Override `DistributeSpawns()` to define placement logic

```csharp
[Serializable]
public class CornerSpawnStep<TGenContext, TSpawnable> : BaseSpawnStep<TGenContext, TSpawnable>
    where TGenContext : class, IPlaceableGenContext<TSpawnable>, ITiledGenContext
    where TSpawnable : ISpawnable
{
    public override void DistributeSpawns(TGenContext map, List<TSpawnable> spawns)
    {
        // Find corner tiles (3 adjacent walls)
        var corners = new List<Loc>();
        for (int x = 1; x < map.Width - 1; x++)
        {
            for (int y = 1; y < map.Height - 1; y++)
            {
                if (!map.TileBlocked(new Loc(x, y)) && IsCorner(map, x, y))
                    corners.Add(new Loc(x, y));
            }
        }

        // Spawn in corners
        for (int i = 0; i < spawns.Count && corners.Count > 0; i++)
        {
            int idx = map.Rand.Next(corners.Count);
            map.PlaceItem(corners[idx], spawns[i]);
            corners.RemoveAt(idx);
        }
    }

    private bool IsCorner(TGenContext map, int x, int y)
    {
        int wallCount = 0;
        foreach (Dir8 dir in DirExt.VALID_DIR8)
        {
            if (map.TileBlocked(new Loc(x, y) + dir.GetLoc()))
                wallCount++;
        }
        return wallCount >= 5;  // At least 5 of 8 neighbors are walls
    }
}
```

## Stair Interfaces

Special interfaces for entrance/exit spawning:

### IEntrance

```csharp
public interface IEntrance : ISpawnable
{
    Loc Loc { get; set; }
}
```

### IExit

```csharp
public interface IExit : ISpawnable
{
    Loc Loc { get; set; }
}
```

## Room Filters

Control which rooms are eligible for spawning:

```csharp
var spawn = new TerminalSpawnStep<MapGenContext, Item>(spawner);

// Only spawn in rooms with specific components
spawn.Filters.Add(new RoomFilterComponent(true, typeof(TreasureRoom)));

// Exclude halls
spawn.Filters.Add(new RoomFilterHall(true));
```

## Related Modules

- **[Rooms/](../Rooms/)** - Room generators that create spawnable areas
- **[FloorPlan/](../FloorPlan/)** - Floor plans that track room locations
- **[Tiles/](../Tiles/)** - Tile operations for terrain-based spawning

## See Also

- `Ex4_Stairs` - Stair placement example
- `Ex6_Items` - Item and mob spawning example
