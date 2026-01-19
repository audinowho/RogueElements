# Halls

[![RogueElements](https://img.shields.io/nuget/v/RogueElements?label=RogueElements)](https://www.nuget.org/packages/RogueElements)

Hall connector generators for procedural roguelike map generation. This module provides classes for connecting rooms with hallways of various styles and widths.

## Purpose

The Halls module generates hallways that connect rooms. It handles complex scenarios including:
- Straight hallways between aligned rooms
- Angled/bent hallways between misaligned rooms
- Multi-way intersections (3-way, 4-way)
- Right-angle connections
- Variable hallway widths via brushes

## Hall Generator

### RoomGenAngledHall

The primary hall generator that connects room exits with narrow hallways. It handles all combinations of exits from all directions.

```csharp
// Basic 1-tile wide hall
var hall = new RoomGenAngledHall<MapGenContext>(turnBias: 0);

// Hall with 50% chance of making turns
var angledHall = new RoomGenAngledHall<MapGenContext>(turnBias: 50);

// Hall with custom dimensions
var wideHall = new RoomGenAngledHall<MapGenContext>(
    turnBias: 0,
    width: new RandRange(3, 7),
    height: new RandRange(3, 7)
);

// Hall with custom brush
var customHall = new RoomGenAngledHall<MapGenContext>(
    turnBias: 50,
    brush: new SquareHallBrush(new Loc(2, 2))
);
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `HallTurnBias` | `int` | Percentage chance (0-100) for the hall to make a turn |
| `Brush` | `BaseHallBrush` | The brush used to draw the hall tiles |
| `Width` | `RandRange` | Preferred width of the hall area |
| `Height` | `RandRange` | Preferred height of the hall area |

#### How It Works

1. **Side Requirements**: Analyzes which directions need connections based on adjacent rooms
2. **Path Selection**: Chooses between straight paths, angled paths, or multi-way intersections
3. **Brush Drawing**: Uses the configured brush to paint tiles along the chosen path

## Hall Brushes

Hall brushes define how hallway tiles are painted. They control the width, shape, and terrain of hallways.

### BaseHallBrush (Abstract)

```csharp
public abstract class BaseHallBrush
{
    public abstract Loc Size { get; }    // Brush dimensions
    public abstract Loc Center { get; }  // Center point for alignment

    public abstract BaseHallBrush Clone();
    public abstract void DrawHallBrush(ITiledGenContext map, Rect bounds, LocRay4 ray, int length);
}
```

### DefaultHallBrush

A simple 1x1 tile brush. The most common choice for standard hallways.

```csharp
var brush = new DefaultHallBrush();
// Size: 1x1
// Draws single-tile wide corridors
```

### SquareHallBrush

A rectangular brush for wider hallways.

```csharp
// 2x2 tile brush for wider halls
var brush = new SquareHallBrush(new Loc(2, 2));

// 3x1 horizontal corridor
var horizontalBrush = new SquareHallBrush(new Loc(3, 1));
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Dims` | `Loc` | Dimensions of the brush in tiles |

### TerrainHallBrush

A rectangular brush that paints a specific terrain type instead of the default room terrain.

```csharp
// Create a brush that paints water tiles
var waterBrush = new TerrainHallBrush(new Loc(2, 2), waterTile);
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Dims` | `Loc` | Dimensions of the brush in tiles |
| `Terrain` | `ITile` | The terrain type to paint |

## Usage Example

From `Ex2_Rooms`:

```csharp
// Create hall types with spawn weights
var genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>>
{
    // Angled halls that may turn, with variable dimensions
    { new RoomGenAngledHall<MapGenContext>(0, new RandRange(3, 7), new RandRange(3, 7)), 10 },

    // Simple 1x1 connector rooms
    { new RoomGenSquare<MapGenContext>(new RandRange(1), new RandRange(1)), 20 },
};

// Use in a floor path
var path = new FloorPathBranch<MapGenContext>(genericRooms, genericHalls)
{
    HallPercent = 50,  // 50% chance to use halls between rooms
    FillPercent = new RandRange(45),
    BranchRatio = new RandRange(0, 25),
};
```

Grid-based example from `Ex3_Grid`:

```csharp
var genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>>
{
    // 50% turn bias for more interesting layouts
    { new RoomGenAngledHall<MapGenContext>(50), 10 }
};
path.GenericHalls = genericHalls;
```

## Creating Custom Hall Brushes

1. Inherit from `BaseHallBrush`
2. Implement `Size` and `Center` properties
3. Implement `DrawHallBrush()` to paint tiles along the ray

```csharp
[Serializable]
public class DiagonalHallBrush : BaseHallBrush
{
    public override Loc Size => new Loc(2, 2);
    public override Loc Center => Loc.Zero;

    public override BaseHallBrush Clone() => new DiagonalHallBrush();

    public override void DrawHallBrush(ITiledGenContext map, Rect bounds, LocRay4 ray, int length)
    {
        for (int ii = 0; ii < length; ii++)
        {
            Loc point = ray.Traverse(ii);

            // Draw main tile
            map.SetTile(point, map.RoomTerrain.Copy());

            // Add diagonal accent based on direction
            Loc offset = ray.Dir.ToAxis() == Axis4.Horiz
                ? new Loc(0, ii % 2 == 0 ? 1 : -1)
                : new Loc(ii % 2 == 0 ? 1 : -1, 0);

            Loc accentLoc = point + offset;
            if (Collision.InBounds(bounds, accentLoc))
                map.SetTile(accentLoc, map.RoomTerrain.Copy());
        }
    }
}
```

## Hall Connection Logic

The `RoomGenAngledHall` handles several connection scenarios:

### Straight Connections
When rooms align on one axis, a direct straight hall is drawn.

### Right-Angle Connections
When only two non-opposite directions have connections, an L-shaped path is created.

### Multi-Way Intersections
For 3-way or 4-way connections, the hall draws:
1. Primary hall (first pair of opposite sides)
2. Secondary hall (remaining sides, connecting to primary)

### Turn Bias
The `HallTurnBias` property controls whether aligned rooms get straight or bent connections:
- `0`: Prefer straight halls when possible
- `50`: 50/50 chance of turn vs straight
- `100`: Always turn when possible

## Related Modules

- **[../](../)** - Parent Rooms module (RoomGen base classes)
- **[FloorPlan/](../../FloorPlan/)** - Freeform room placement using halls
- **[Grid/](../../Grid/)** - Grid-based room layouts using halls

## See Also

- `Ex2_Rooms` - Freeform hall generation example
- `Ex3_Grid` - Grid-based hall generation example
