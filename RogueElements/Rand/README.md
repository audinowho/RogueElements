# Rand

[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg)](../../../LICENSE)

Random number generation and weighted selection utilities for procedural roguelike map generation.

## Overview

The `Rand` folder provides all randomization infrastructure used throughout RogueElements. This includes:

- **Seedable RNG** - Deterministic random number generation for reproducible maps
- **Weighted Selection** - Pick items from lists with different spawn rates
- **Noise Generation** - Perlin noise for natural terrain features
- **Range Generators** - Generate random integers within specified bounds

## Core Concepts

### IRandPicker Interface Hierarchy

All random pickers implement `IRandPicker<T>`, enabling polymorphic random selection:

```
IRandPicker<T>
├── RandRange          - Pick integer from [min, max)
├── RandBinomial       - Pick from binomial distribution
├── RandBag<T>         - Pick from unweighted list
├── SpawnList<T>       - Pick from weighted list
└── PresetPicker<T>    - Always return same value

IMultiRandPicker<T>
├── LoopedRand<T>      - Repeat picks N times
└── PresetMultiRand<T> - Return preset list
```

Key interface members:

```csharp
public interface IRandPicker<T>
{
    T Pick(IRandom rand);           // Get random item
    bool CanPick { get; }           // Has items to pick?
    bool ChangesState { get; }      // Modifies internal state on pick?
    IRandPicker<T> CopyState();     // Clone for stateful pickers
}
```

### Why This Matters for Roguelikes

1. **Reproducibility** - Same seed produces identical maps, essential for sharing seeds and debugging
2. **Weighted Spawns** - Control rarity of items, enemies, room types naturally
3. **Floor Variation** - `SpawnRangeList` allows different spawn tables per dungeon floor
4. **Extensibility** - Implement `IRandPicker<T>` to create custom distributions

## Key Classes

### RandRange

Generate random integers within a range (exclusive max):

```csharp
// Single value (always returns 5)
var exact = new RandRange(5);

// Range: returns 3, 4, 5, 6, or 7
var range = new RandRange(3, 8);

// Use in generation
int roomWidth = new RandRange(4, 10).Pick(map.Rand);
```

### RandBinomial

Generate values from a binomial distribution - useful for "attempt N times with X% chance":

```csharp
// Roll 5 dice, each with 50% success, add 2 as baseline
var loot = new RandBinomial(trials: 5, percent: 50, offset: 2);
int goldPiles = loot.Pick(rand);  // Returns 2-7
```

### SpawnList

Weighted random selection - the workhorse for roguelike item/enemy spawning:

```csharp
// Create spawn table for room types
var rooms = new SpawnList<RoomGen<MapGenContext>>();
rooms.Add(new RoomGenSquare<MapGenContext>(), 10);   // Common
rooms.Add(new RoomGenRound<MapGenContext>(), 10);    // Common
rooms.Add(new RoomGenCave<MapGenContext>(), 3);      // Rare

// Pick randomly (weighted)
RoomGen<MapGenContext> room = rooms.Pick(map.Rand);
```

With removal (bag without replacement):

```csharp
var uniqueItems = new SpawnList<Item>(remove: true);
uniqueItems.Add(new Sword(), 10);
uniqueItems.Add(new Shield(), 5);

// First pick might return Sword
// Sword is removed, second pick can only return Shield
```

### SpawnRangeList

Weighted selection that varies by dungeon floor - items appear only on certain floors:

```csharp
var enemies = new SpawnRangeList<Enemy>();

// Slimes appear floors 1-5 with rate 20
enemies.Add(new Slime(), new IntRange(1, 6), 20);

// Dragons appear floors 8-10 with rate 5
enemies.Add(new Dragon(), new IntRange(8, 11), 5);

// Get spawn table for floor 3 (only contains Slime)
SpawnList<Enemy> floor3 = enemies.GetSpawnList(3);

// Pick from floor 9 (contains both, weighted)
Enemy enemy = enemies.Pick(rand, level: 9);
```

### LoopedRand

Combine a spawner with an amount picker to generate multiple items:

```csharp
// Spawn 10-18 items from weighted list
var itemSpawns = new SpawnList<Item>();
itemSpawns.Add(new Potion(), 50);
itemSpawns.Add(new Scroll(), 30);
itemSpawns.Add(new Weapon(), 10);

var spawner = new LoopedRand<Item>(
    itemSpawns,                    // What to spawn
    new RandRange(10, 19)          // How many (10-18)
);

List<Item> items = spawner.Roll(rand);
```

## Integration with MapGen Pipeline

These utilities are used throughout the generation pipeline:

```csharp
// Room type selection uses SpawnList
var path = new GridPathBranch<MapGenContext>();
path.GenericRooms = new SpawnList<RoomGen<MapGenContext>>
{
    { new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)), 10 },
    { new RoomGenRound<MapGenContext>(new RandRange(5, 9), new RandRange(5, 9)), 10 },
};

// Item spawning uses LoopedRand with SpawnList
var itemPlacement = new RandomSpawnStep<MapGenContext, Item>(
    new PickerSpawner<MapGenContext, Item>(
        new LoopedRand<Item>(itemSpawns, new RandRange(10, 19))
    )
);

// Water generation uses RandRange for percentage
var water = new PerlinWaterStep<MapGenContext>(
    new RandRange(35),  // 35% water coverage
    3,                  // Complexity
    new Tile(terrain),
    stencil
);
```

## Subfolders

- **[Noise/](Noise/)** - Perlin noise generation for natural terrain
- **[RNG/](RNG/)** - Core seedable random number generator implementation

## See Also

- [Priority/](../Priority/) - Step ordering system that also uses these random utilities
- [MapGen/Spawning/](../MapGen/Spawning/) - How spawn lists integrate with placement
- [Examples/Ex6_Items](../../RogueElements.Examples/Ex6_Items/) - Item spawning example

---

![Repobeats analytics](https://repobeats.axiom.co/api/embed/3c5a3b7f5e0c1d8a9b7c5e3a1f9d8b7c6e5a4d3c2b1a0.svg "Repobeats analytics image")
