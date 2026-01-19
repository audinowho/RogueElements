# Example 7: Special Rooms

Add hand-crafted special rooms with filtered item spawning.

## What You'll Learn

- Creating custom room layouts with `RoomGenSpecific`
- Adding special rooms using `SetSpecialRoomStep`
- Tagging rooms with components for identification
- Filtering spawns to target specific room types

## Prerequisites

- [Example 6: Item Spawning](../Ex6_Items/README.md)
- Understanding of room components

## Concepts

### Special Rooms

A **special room** is a hand-designed room added to the procedural layout. It could be:
- A treasure vault
- A boss arena
- A shrine
- A trap room

### Room Components

**Components** are tags attached to rooms during generation. They enable:
- Room identification (is this a treasure room?)
- Filtered spawning (spawn items only in treasure rooms)
- Conditional logic (connect boss room to main path)

### RoomGenSpecific

`RoomGenSpecific` creates a room from a predefined tile pattern, allowing complete control over room shape and terrain.

## Code Walkthrough

### Step 1: Freeform Room Setup

```csharp
InitFloorPlanStep<MapGenContext> startGen = new InitFloorPlanStep<MapGenContext>(54, 40);
layout.GenSteps.Add(-2, startGen);

// Main path with room components
FloorPathBranch<MapGenContext> path = new FloorPathBranch<MapGenContext>(genericRooms, genericHalls)
{
    HallPercent = 50,
    FillPercent = new RandRange(40),
    BranchRatio = new RandRange(0, 25),
};

// Tag rooms and halls on the main path
path.RoomComponents.Set(new MainRoomComponent());
path.HallComponents.Set(new MainHallComponent());

layout.GenSteps.Add(-1, path);
```

Main path rooms get `MainRoomComponent`, halls get `MainHallComponent`.

### Step 2: Define the Special Room Pattern

```csharp
string[] custom = new string[]
{
    "~~~..~~~",
    "~~~..~~~",
    "~~#..#~~",
    "........",
    "........",
    "~~#..#~~",
    "~~~..~~~",
    "~~~..~~~",
};
```

A treasure room with:
- Water (`~`) around the edges
- Small pillars (`#`) in the corners
- Open floor (`.`) for loot placement

### Step 3: Create RoomGenSpecific

```csharp
public static RoomGenSpecific<T> CreateRoomGenSpecific<T>(string[] level)
    where T : class, ITiledGenContext
{
    RoomGenSpecific<T> roomGen = new RoomGenSpecific<T>(
        level[0].Length,                    // width
        level.Length,                       // height
        new Tile(BaseMap.ROOM_TERRAIN_ID)   // default tile
    );

    roomGen.Tiles = new Tile[level[0].Length][];
    for (int xx = 0; xx < level[0].Length; xx++)
    {
        roomGen.Tiles[xx] = new Tile[level.Length];
        for (int yy = 0; yy < level.Length; yy++)
        {
            if (level[yy][xx] == '#')
                roomGen.Tiles[xx][yy] = new Tile(BaseMap.WALL_TERRAIN_ID);
            else if (level[yy][xx] == '~')
                roomGen.Tiles[xx][yy] = new Tile(BaseMap.WATER_TERRAIN_ID);
            else
                roomGen.Tiles[xx][yy] = new Tile(BaseMap.ROOM_TERRAIN_ID);
        }
    }
    return roomGen;
}
```

### Step 4: Add the Special Room

```csharp
SetSpecialRoomStep<MapGenContext> listSpecialStep = new SetSpecialRoomStep<MapGenContext>
{
    Rooms = new PresetPicker<RoomGen<MapGenContext>>(
        CreateRoomGenSpecific<MapGenContext>(custom)
    ),
};

// Tag with TreasureRoomComponent
listSpecialStep.RoomComponents.Set(new TreasureRoomComponent());

// Configure hall connection
PresetPicker<PermissiveRoomGen<MapGenContext>> picker = new PresetPicker<PermissiveRoomGen<MapGenContext>>
{
    ToSpawn = new RoomGenAngledHall<MapGenContext>(0),
};
listSpecialStep.Halls = picker;

layout.GenSteps.Add(-1, listSpecialStep);
```

The special room gets tagged with `TreasureRoomComponent` for later filtering.

### Step 5: Standard Item Spawning

```csharp
// Regular items throughout the map
var itemSpawns = new SpawnList<Item>
{
    { new Item((int)'!'), 10 },
    { new Item((int)']'), 10 },
    // ... other items ...
    { new Item((int)'*'), 50 },
};

RandomRoomSpawnStep<MapGenContext, Item> itemPlacement = new RandomRoomSpawnStep<MapGenContext, Item>(
    new PickerSpawner<MapGenContext, Item>(
        new LoopedRand<Item>(itemSpawns, new RandRange(10, 19))
    )
);
layout.GenSteps.Add(6, itemPlacement);
```

Note: Using `RandomRoomSpawnStep` instead of `RandomSpawnStep` - this distributes items across rooms.

### Step 6: Filtered Treasure Spawning

```csharp
// Treasure items only in the special room
var treasureSpawns = new SpawnList<Item>
{
    { new Item((int)'!'), 10 },  // Potions
    { new Item((int)'*'), 50 },  // Gems
};

RandomRoomSpawnStep<MapGenContext, Item> treasurePlacement = new RandomRoomSpawnStep<MapGenContext, Item>(
    new PickerSpawner<MapGenContext, Item>(
        new LoopedRand<Item>(treasureSpawns, new RandRange(7, 10))
    )
);

// Filter: only spawn in rooms with TreasureRoomComponent
treasurePlacement.Filters.Add(
    new RoomFilterComponent(false, new TreasureRoomComponent())
);

layout.GenSteps.Add(6, treasurePlacement);
```

The key is the filter:
- `false` = spawn in rooms that DO have the component
- `new TreasureRoomComponent()` = the component to check for

## Room Component Classes

```csharp
public class MainRoomComponent : RoomComponent
{
    public override RoomComponent Clone() => new MainRoomComponent();
}

public class MainHallComponent : RoomComponent
{
    public override RoomComponent Clone() => new MainHallComponent();
}

public class TreasureRoomComponent : RoomComponent
{
    public override RoomComponent Clone() => new TreasureRoomComponent();
}
```

## Try It

```bash
dotnet run --project RogueElements.Examples/RogueElements.Examples.csproj
```

Press `7` to run Example 7.

**What to observe:**
- One room with water edges and pillars (the treasure room)
- Extra items clustered in the treasure room
- Regular items distributed elsewhere
- The special room is connected to the main dungeon

**Example output:**
```
7: A Map with Special Rooms
=======================================================
######################################################
###.......############################################
###.......######~~~..~~~##############################
###.......######~~~..~~~#######......#################
###...........##~~#..#~~#######......#################
###.......#####..***..**#######......#################
####......#####..***..*.##.............................
####......#####~~#..#~~##..............................
###############~~~..~~~#####......#####################
###############~~~..~~~#####......#####################
###############........#####.<....#####################
######################################################
```

## Key Classes Used

| Class | Purpose |
|-------|---------|
| `SetSpecialRoomStep<T>` | Adds a special room to the layout |
| `RoomGenSpecific<T>` | Creates a room from predefined tiles |
| `RoomComponent` | Base class for room tags |
| `TreasureRoomComponent` | Tags treasure rooms |
| `RoomFilterComponent` | Filters spawn steps by room component |
| `RandomRoomSpawnStep<T, TSpawn>` | Spawns items distributed across rooms |
| `PresetPicker<T>` | Always picks the same item |

## Filter Logic

The `RoomFilterComponent` constructor:

```csharp
RoomFilterComponent(bool invert, params RoomComponent[] components)
```

| Invert | Meaning |
|--------|---------|
| `false` | Spawn in rooms WITH the component |
| `true` | Spawn in rooms WITHOUT the component |

Examples:
```csharp
// Only treasure rooms
new RoomFilterComponent(false, new TreasureRoomComponent())

// Everything EXCEPT treasure rooms
new RoomFilterComponent(true, new TreasureRoomComponent())

// Only main path rooms (not side rooms)
new RoomFilterComponent(false, new MainRoomComponent())
```

## Multiple Special Rooms

You can add multiple special room types:

```csharp
// Treasure room
var treasureStep = new SetSpecialRoomStep<T> { /* ... */ };
treasureStep.RoomComponents.Set(new TreasureRoomComponent());

// Boss room
var bossStep = new SetSpecialRoomStep<T> { /* ... */ };
bossStep.RoomComponents.Set(new BossRoomComponent());

// Shrine room
var shrineStep = new SetSpecialRoomStep<T> { /* ... */ };
shrineStep.RoomComponents.Set(new ShrineRoomComponent());
```

## Key Takeaways

1. **Hand-Crafted Rooms**: Use `RoomGenSpecific` for custom designs
2. **Component Tagging**: Label rooms during generation for later reference
3. **Filtered Spawning**: Target specific room types with filters
4. **Separation of Concerns**: Room generation and item placement are decoupled

## Design Patterns

| Pattern | Use Case |
|---------|----------|
| Special treasure room | High-value loot in risky area |
| Boss arena | Large open room for boss fights |
| Puzzle room | Custom layout with traps/switches |
| Shrine room | Safe zone with healing/buffs |

## Next Steps

[Example 8: Integration](../Ex8_Integration/README.md) shows how to integrate RogueElements with the RogueSharp library.
