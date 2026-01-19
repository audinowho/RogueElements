# Rooms

[![RogueElements](https://img.shields.io/nuget/v/RogueElements?label=RogueElements)](https://www.nuget.org/packages/RogueElements)

Room shape generators for procedural roguelike map generation. This module provides the core abstraction for generating room shapes and managing room-to-room connections.

## Purpose

The Rooms module generates individual room shapes that can be placed into floor plans. Each room generator defines its own shape (square, round, cave, etc.) and manages how hallways can connect to it from any of its four cardinal sides.

## Core Interface

### IRoomGen

The fundamental interface that all room generators implement:

```csharp
public interface IRoomGen
{
    Rect Draw { get; }                    // The rectangle the room occupies

    Loc ProposeSize(IRandom rand);        // Returns preferred dimensions
    void PrepareSize(IRandom rand, Loc size);  // Initialize to specified size
    void SetLoc(Loc loc);                 // Position the room
    void DrawOnMap(ITiledGenContext map); // Render tiles to the map

    // Border management for hallway connections
    void AskBorderRange(IntRange range, Dir4 dir);
    void AskBorderFromRoom(Rect sourceDraw, Func<Dir4, int, bool> borderQuery, Dir4 dir);
    bool GetOpenedBorder(Dir4 dir, int index);
    bool GetFulfillableBorder(Dir4 dir, int index);

    IRoomGen Copy();
}
```

## Room Generator Classes

### RoomGenSquare

Generates simple rectangular rooms.

```csharp
// 4-8 tiles wide, 4-8 tiles tall
var room = new RoomGenSquare<MapGenContext>(
    new RandRange(4, 8),  // Width
    new RandRange(4, 8)   // Height
);
```

### RoomGenRound

Generates rounded rooms. Square dimensions produce circles; rectangular dimensions produce capsules.

```csharp
// 5-9 tiles in each dimension
var room = new RoomGenRound<MapGenContext>(
    new RandRange(5, 9),
    new RandRange(5, 9)
);
```

### RoomGenCave

Generates organic cave-like rooms using cellular automata. Falls back to a square if forced to a size it did not propose.

```csharp
var cave = new RoomGenCave<MapGenContext>(
    new RandRange(6, 12),  // Max width
    new RandRange(6, 12)   // Max height
);
```

### RoomGenBump

Generates rectangular rooms with randomly blocked perimeter tiles, creating irregular edges.

```csharp
var bump = new RoomGenBump<MapGenContext>(
    new RandRange(5, 10),  // Width
    new RandRange(5, 10),  // Height
    new RandRange(20, 50)  // Bump percent (chance of perimeter blocks)
);
```

### RoomGenBlocked

Generates rectangular rooms with a rectangular obstacle block inside.

```csharp
var blocked = new RoomGenBlocked<MapGenContext>(
    blockTerrain,          // Tile for the block
    new RandRange(6, 10),  // Room width
    new RandRange(6, 10),  // Room height
    new RandRange(2, 4),   // Block width
    new RandRange(2, 4)    // Block height
);
```

### RoomGenSpecific

Generates rooms with exact tile-by-tile specifications. Useful for hand-crafted special rooms.

```csharp
var specific = new RoomGenSpecific<MapGenContext>(width, height, roomTerrain);
specific.Tiles[x][y] = customTile;
```

## Base Classes

### RoomGen&lt;T&gt;

Abstract base class providing the common logic for all room generators. Key responsibilities:

- **Border Management**: Tracks which border tiles can accept hallway connections
- **Size Preparation**: Validates and applies room dimensions
- **Fulfillment**: Ensures rooms can connect to adjacent rooms/halls

All `RoomGen` implementations must follow these rules:
1. Generate solvable rooms (any entrance can reach any exit)
2. Handle any given size without throwing exceptions
3. Provide at least one opening per cardinal direction if asked

### PermissiveRoomGen&lt;T&gt;

A subclass of `RoomGen` that can accept connections from any border tile. Used for halls and simple rectangular rooms.

```csharp
// PermissiveRoomGen allows halls to connect anywhere on its border
public abstract class PermissiveRoomGen<T> : RoomGen<T>
{
    protected override void PrepareFulfillableBorders(IRandom rand)
    {
        // Mark all border tiles as fulfillable
        foreach (Dir4 dir in DirExt.VALID_DIR4)
            for (int jj = 0; jj < FulfillableBorder[dir].Length; jj++)
                FulfillableBorder[dir][jj] = true;
    }
}
```

## Usage Example

From `Ex2_Rooms`:

```csharp
var layout = new MapGen<MapGenContext>();

// Initialize floor plan
var startGen = new InitFloorPlanStep<MapGenContext>(54, 40);
layout.GenSteps.Add(-2, startGen);

// Create room types with spawn weights
var genericRooms = new SpawnList<RoomGen<MapGenContext>>
{
    { new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)), 10 },
    { new RoomGenRound<MapGenContext>(new RandRange(5, 9), new RandRange(5, 9)), 10 },
};

// Create hall types
var genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>>
{
    { new RoomGenAngledHall<MapGenContext>(0, new RandRange(3, 7), new RandRange(3, 7)), 10 },
    { new RoomGenSquare<MapGenContext>(new RandRange(1), new RandRange(1)), 20 },
};

// Create branching path
var path = new FloorPathBranch<MapGenContext>(genericRooms, genericHalls)
{
    HallPercent = 50,
    FillPercent = new RandRange(45),
    BranchRatio = new RandRange(0, 25),
};
layout.GenSteps.Add(-1, path);

// Draw to tiles
layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));
```

## Creating Custom Room Generators

1. Inherit from `RoomGen<T>` or `PermissiveRoomGen<T>`
2. Implement `ProposeSize()` to return preferred dimensions
3. Implement `DrawOnMap()` to render tiles
4. Override `PrepareFulfillableBorders()` to define valid connection points

```csharp
[Serializable]
public class RoomGenDiamond<T> : RoomGen<T>
    where T : ITiledGenContext
{
    public RandRange Size { get; set; }

    public override Loc ProposeSize(IRandom rand)
    {
        int size = Size.Pick(rand);
        return new Loc(size, size);
    }

    public override void DrawOnMap(T map)
    {
        int center = Draw.Width / 2;
        for (int y = 0; y < Draw.Height; y++)
        {
            int dist = Math.Abs(y - center);
            for (int x = dist; x < Draw.Width - dist; x++)
                map.SetTile(new Loc(Draw.X + x, Draw.Y + y), map.RoomTerrain.Copy());
        }
        SetRoomBorders(map);
    }

    protected override void PrepareFulfillableBorders(IRandom rand)
    {
        // Only allow connections at the diamond's widest points
        int center = Draw.Width / 2;
        FulfillableBorder[Dir4.Up][center] = true;
        FulfillableBorder[Dir4.Down][center] = true;
        FulfillableBorder[Dir4.Left][center] = true;
        FulfillableBorder[Dir4.Right][center] = true;
    }

    public override RoomGen<T> Copy() => new RoomGenDiamond<T>(this);
}
```

## Room Filters

Use `BaseRoomFilter` and its subclasses to control which rooms are eligible for certain operations:

- `RoomFilterComponent` - Filter by room component type
- `RoomFilterDefaultGen` - Filter by room generator type
- `RoomFilterHall` - Filter halls vs rooms

## Related Modules

- **[Halls/](./Halls/)** - Hall connector generators (RoomGenAngledHall, hall brushes)
- **[FloorPlan/](../FloorPlan/)** - Freeform room placement
- **[Grid/](../Grid/)** - Grid-based room layouts

## See Also

- `Ex2_Rooms` - Freeform room generation example
- `Ex3_Grid` - Grid-based room generation example
