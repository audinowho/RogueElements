# Example 6: Item Spawning

Place items and monsters randomly across the map.

## What You'll Learn

- How to define spawnable item and mob types
- Using `RandomSpawnStep` for entity placement
- Configuring spawn lists with weighted probabilities
- Using `LoopedRand` for quantity control

## Prerequisites

- [Example 5: Terrain Features](../Ex5_Terrain/README.md)
- Understanding of the ISpawnable pattern

## Concepts

### Random Spawning

`RandomSpawnStep` places entities at random valid locations across the entire map. It uses:
- A **spawner** to generate entities
- The context's placement methods to find valid tiles

### Spawn Lists

`SpawnList<T>` provides weighted random selection. Higher weights = more likely to spawn.

### LoopedRand

`LoopedRand<T>` repeatedly picks from a spawn list to generate multiple items from a single step.

## Code Walkthrough

### Step 1: Standard Setup

```csharp
// Grid + stairs + water (same as Example 5)
// ... setup code ...
layout.GenSteps.Add(4, new DropDiagonalBlockStep<MapGenContext>(new Tile(terrain)));
layout.GenSteps.Add(4, new EraseIsolatedStep<MapGenContext>(new Tile(terrain)));
```

### Step 2: Define Item Types

```csharp
var itemSpawns = new SpawnList<Item>
{
    { new Item((int)'!'), 10 },  // Potion
    { new Item((int)']'), 10 },  // Armor
    { new Item((int)'='), 10 },  // Ring
    { new Item((int)'?'), 10 },  // Scroll
    { new Item((int)'$'), 10 },  // Gold
    { new Item((int)'/'), 10 },  // Wand
    { new Item((int)'*'), 50 },  // Gem (5x more common)
};
```

Each item uses its ASCII character as an ID for easy display. Weights determine spawn probability:
- Gems (`*`) have weight 50 = 5x more likely than others
- All others have weight 10 = equal probability among themselves

### Step 3: Create Item Spawner

```csharp
RandomSpawnStep<MapGenContext, Item> itemPlacement = new RandomSpawnStep<MapGenContext, Item>(
    new PickerSpawner<MapGenContext, Item>(
        new LoopedRand<Item>(itemSpawns, new RandRange(10, 19))
    )
);
layout.GenSteps.Add(6, itemPlacement);
```

Breaking this down:
- `LoopedRand<Item>(itemSpawns, RandRange(10, 19))`: Pick 10-18 items from the spawn list
- `PickerSpawner`: Wraps the random picker into a spawner
- `RandomSpawnStep`: Places each spawned item at a random valid location

### Step 4: Define Mob Types

```csharp
var mobSpawns = new SpawnList<Mob>
{
    { new Mob((int)'r'), 20 },  // Rat (common)
    { new Mob((int)'T'), 10 },  // Troll (medium)
    { new Mob((int)'D'), 5 },   // Dragon (rare)
};
```

Weighted so rats are 4x more common than dragons.

### Step 5: Create Mob Spawner

```csharp
RandomSpawnStep<MapGenContext, Mob> mobPlacement = new RandomSpawnStep<MapGenContext, Mob>(
    new PickerSpawner<MapGenContext, Mob>(
        new LoopedRand<Mob>(mobSpawns, new RandRange(10, 19))
    )
);
layout.GenSteps.Add(6, mobPlacement);
```

Same pattern as items, spawning 10-18 mobs.

## Entity Classes

### Item

```csharp
public class Item : ISpawnable
{
    public Item(int id) { this.ID = id; }
    public Item(int id, Loc loc) { this.ID = id; this.Loc = loc; }

    public int ID { get; set; }
    public Loc Loc { get; set; }
    public ISpawnable Copy() => new Item(this);
}
```

### Mob

```csharp
public class Mob : ISpawnable
{
    public Mob(int id) { this.ID = id; }
    public Mob(int id, Loc loc) { this.ID = id; this.Loc = loc; }

    public int ID { get; set; }
    public Loc Loc { get; set; }
    public ISpawnable Copy() => new Mob(this);
}
```

## Map Class Changes

```csharp
public class Map : BaseMap
{
    public Map()
    {
        this.GenEntrances = new List<StairsUp>();
        this.GenExits = new List<StairsDown>();
        this.Items = new List<Item>();   // NEW
        this.Mobs = new List<Mob>();     // NEW
    }

    public List<Item> Items { get; set; }
    public List<Mob> Mobs { get; set; }
}
```

## MapGenContext Changes

Implement `IPlaceableGenContext<T>` for both Item and Mob:

```csharp
public class MapGenContext : BaseMapGenContext<Map>, IRoomGridGenContext,
    IViewPlaceableGenContext<StairsUp>, IViewPlaceableGenContext<StairsDown>,
    IPlaceableGenContext<Item>, IPlaceableGenContext<Mob>  // NEW
{
    void IPlaceableGenContext<Item>.PlaceItem(Loc loc, Item item)
    {
        Item newItem = new Item(item.ID, loc);
        this.Map.Items.Add(newItem);
    }

    void IPlaceableGenContext<Mob>.PlaceItem(Loc loc, Mob item)
    {
        Mob newItem = new Mob(item.ID, loc);
        this.Map.Mobs.Add(newItem);
    }

    // Check if tile is occupied by existing items/mobs
    private bool IsTileOccupied(Loc loc)
    {
        if (this.Map.Tiles[loc.X][loc.Y].ID != Map.ROOM_TERRAIN_ID)
            return true;

        foreach (Item item in this.Map.Items)
            if (item.Loc == loc) return true;

        foreach (Mob mob in this.Map.Mobs)
            if (mob.Loc == loc) return true;

        return false;
    }
}
```

## Try It

```bash
dotnet run --project RogueElements.Examples/RogueElements.Examples.csproj
```

Press `6` to run Example 6.

**What to observe:**
- Items displayed as `!`, `]`, `=`, `?`, `$`, `/`, `*`
- Mobs displayed as `r`, `T`, `D`
- Gems (`*`) appear more frequently than other items
- Rats (`r`) appear more frequently than dragons (`D`)
- Nothing spawns on water, walls, or stairs

**Example output:**
```
6: A Map with Randomly Placed Items/Mobs
=======================================================
######################################################
#######..r......######################################
#######...*~....####*#################################
#######..<~~~...######~~~....#####r###################
#######..T~~~...#####~~~~....###########*#############
#######~.....r..#####.~~~....#########################
#######~~~~~....#####........##############!..........
########~~~~~~~~~~~~~~~......#########.........r*D....
#################~~~~~~~~~~~~~~~~~~~~.......].........
#################..~~~~~~......#####.........?........
#################........>.....#####....~~....*.......
######################################################
```

## Key Classes Used

| Class | Purpose |
|-------|---------|
| `RandomSpawnStep<TContext, TSpawn>` | Places entities at random valid locations |
| `PickerSpawner<TContext, TSpawn>` | Generates entities from a random picker |
| `LoopedRand<T>` | Picks multiple items from a spawn list |
| `SpawnList<T>` | Weighted random selection |
| `Item`, `Mob` | Custom spawnable entity classes |
| `IPlaceableGenContext<T>` | Interface for entity placement |

## Spawn Weight Math

With these weights:

```csharp
{ new Mob((int)'r'), 20 },  // Rat
{ new Mob((int)'T'), 10 },  // Troll
{ new Mob((int)'D'), 5 },   // Dragon
```

Total weight = 20 + 10 + 5 = 35

Probabilities:
- Rat: 20/35 = 57%
- Troll: 10/35 = 29%
- Dragon: 5/35 = 14%

## Collision Handling

The `IsTileOccupied` check ensures:
1. Only floor tiles receive spawns
2. Existing items/mobs block new spawns
3. Stairs are protected
4. Water tiles are skipped

## Key Takeaways

1. **Weighted Spawning**: Control rarity with spawn list weights
2. **Quantity Control**: `LoopedRand` sets min/max spawn counts
3. **Collision Prevention**: Check occupied tiles before placing
4. **Extensible Pattern**: Same pattern works for any spawnable type

## Advanced Usage

Different spawn strategies:

```csharp
// Random placement (this example)
new RandomSpawnStep<T, Item>(spawner)

// Room-based placement (Example 7)
new RandomRoomSpawnStep<T, Item>(spawner)

// Specific room placement (with filters)
var step = new RandomRoomSpawnStep<T, Item>(spawner);
step.Filters.Add(new RoomFilterComponent(false, new TreasureRoomComponent()));
```

## Next Steps

[Example 7: Special Rooms](../Ex7_Special/README.md) adds special hand-crafted rooms with targeted item spawning.
