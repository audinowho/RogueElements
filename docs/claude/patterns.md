# Patterns: Step-by-Step Recipes

Practical recipes for extending RogueElements with custom components.

---

## 1. Add a Custom RoomGen

Room generators define the shape and layout of individual rooms.

### When to Use
- Create custom room shapes (L-shaped, circular, cross-shaped)
- Implement procedural room interiors (pillars, pools, furniture)
- Add rooms with special connection constraints

### Steps

1. **Choose a base class:**
   - `PermissiveRoomGen<T>` - All border tiles accept connections (simple rooms)
   - `RoomGen<T>` - Custom control over which borders accept connections

2. **Create the class with required overrides:**

```csharp
using System;

namespace YourGame
{
    [Serializable]
    public class RoomGenCross<T> : PermissiveRoomGen<T>, ISizedRoomGen
        where T : ITiledGenContext
    {
        public RoomGenCross() { }

        public RoomGenCross(RandRange armWidth, RandRange armLength)
        {
            this.ArmWidth = armWidth;
            this.ArmLength = armLength;
        }

        // Copy constructor for cloning
        protected RoomGenCross(RoomGenCross<T> other)
        {
            this.ArmWidth = other.ArmWidth;
            this.ArmLength = other.ArmLength;
        }

        public RandRange ArmWidth { get; set; }
        public RandRange ArmLength { get; set; }

        // Required: create a copy for placement
        public override RoomGen<T> Copy() => new RoomGenCross<T>(this);

        // Required: propose dimensions based on RNG
        public override Loc ProposeSize(IRandom rand)
        {
            int arm = this.ArmLength.Pick(rand);
            int width = this.ArmWidth.Pick(rand);
            int size = (arm * 2) + width;
            return new Loc(size, size);
        }

        // Required: draw the room onto the map
        public override void DrawOnMap(T map)
        {
            int armWidth = this.ArmWidth.Min;
            int center = this.Draw.Width / 2;
            int halfArm = armWidth / 2;

            // Draw horizontal arm
            for (int x = 0; x < this.Draw.Width; x++)
            {
                for (int y = center - halfArm; y <= center + halfArm; y++)
                {
                    map.SetTile(new Loc(this.Draw.X + x, this.Draw.Y + y),
                                map.RoomTerrain.Copy());
                }
            }

            // Draw vertical arm
            for (int y = 0; y < this.Draw.Height; y++)
            {
                for (int x = center - halfArm; x <= center + halfArm; x++)
                {
                    map.SetTile(new Loc(this.Draw.X + x, this.Draw.Y + y),
                                map.RoomTerrain.Copy());
                }
            }

            // Update borders for hallway connections
            this.SetRoomBorders(map);
        }
    }
}
```

3. **Register in your pipeline:**

```csharp
var rooms = new SpawnList<RoomGen<MapGenContext>>();
rooms.Add(new RoomGenCross<MapGenContext>(new RandRange(3, 5), new RandRange(2, 4)), 10);
rooms.Add(new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)), 20);

layout.GenSteps.Add(new Priority(3), new DrawGridRoomsStep<MapGenContext>(rooms));
```

---

## 2. Add a Custom GenStep

GenSteps are the building blocks of the generation pipeline.

### When to Use
- Add custom terrain features (lava pools, chasms, grass patches)
- Post-process generated maps (smooth walls, add decorations)
- Implement custom spawning logic

### Steps

1. **Identify required interfaces** (constrain `T` appropriately):
   - `ITiledGenContext` - Tile manipulation
   - `IFloorPlanGenContext` - Room-based layouts
   - `IRoomGridGenContext` - Grid-based layouts
   - `IPlaceableGenContext<T>` - Entity spawning

2. **Create the step:**

```csharp
using System;

namespace YourGame
{
    [Serializable]
    public class ScatterPillarsStep<T> : GenStep<T>
        where T : class, ITiledGenContext
    {
        public ScatterPillarsStep() { }

        public ScatterPillarsStep(int count, ITile pillarTile)
        {
            this.Count = count;
            this.PillarTile = pillarTile;
        }

        public int Count { get; set; }
        public ITile PillarTile { get; set; }

        public override void Apply(T map)
        {
            int placed = 0;
            int attempts = this.Count * 10;

            while (placed < this.Count && attempts > 0)
            {
                attempts--;

                // Use map.Rand for reproducibility
                int x = map.Rand.Next(1, map.Width - 1);
                int y = map.Rand.Next(1, map.Height - 1);
                Loc loc = new Loc(x, y);

                // Only place on floor tiles with space around
                if (!map.TileBlocked(loc) && this.HasClearance(map, loc))
                {
                    map.SetTile(loc, this.PillarTile.Copy());
                    placed++;
                    GenContextDebug.DebugProgress("Placed Pillar");
                }
            }
        }

        private bool HasClearance(T map, Loc loc)
        {
            // Check 4 cardinal directions are floor
            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                if (map.TileBlocked(loc + dir.GetLoc()))
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            return $"{this.GetType().GetFormattedTypeName()}: {this.Count} pillars";
        }
    }
}
```

3. **Add to pipeline with appropriate priority:**

```csharp
// Add after room generation but before spawning
layout.GenSteps.Add(new Priority(5, 1),
    new ScatterPillarsStep<MapGenContext>(10, new Tile(PILLAR_TERRAIN_ID)));
```

---

## 3. Add a Custom Spawnable Type

Spawnables are entities placed on the map (items, enemies, traps, etc.).

### When to Use
- Add new entity categories (traps, NPCs, chests)
- Create entities with custom data (leveled monsters, enchanted items)

### Steps

1. **Implement ISpawnable:**

```csharp
using System;

namespace YourGame
{
    public class Trap : ISpawnable
    {
        public Trap() { }

        public Trap(int trapType, int damage)
        {
            this.TrapType = trapType;
            this.Damage = damage;
        }

        // Copy constructor
        protected Trap(Trap other)
        {
            this.TrapType = other.TrapType;
            this.Damage = other.Damage;
            this.Loc = other.Loc;
        }

        public int TrapType { get; set; }
        public int Damage { get; set; }
        public Loc Loc { get; set; }

        // Required by ISpawnable
        public ISpawnable Copy() => new Trap(this);
    }
}
```

2. **Add IPlaceableGenContext to your map context:**

```csharp
public class MapGenContext : BaseMapGenContext<Map>,
    IRoomGridGenContext,
    IPlaceableGenContext<Trap>  // Add this interface
{
    // Your existing implementation...

    // Implement IPlaceableGenContext<Trap>
    List<Loc> IPlaceableGenContext<Trap>.GetAllFreeTiles()
    {
        return Grid.FindTilesInBox(
            Loc.Zero,
            new Loc(this.Width, this.Height),
            loc => !this.IsTileOccupied(loc));
    }

    List<Loc> IPlaceableGenContext<Trap>.GetFreeTiles(Rect rect)
    {
        return Grid.FindTilesInBox(rect.Start, rect.Size,
            loc => !this.IsTileOccupied(loc));
    }

    bool IPlaceableGenContext<Trap>.CanPlaceItem(Loc loc)
    {
        return !this.IsTileOccupied(loc);
    }

    void IPlaceableGenContext<Trap>.PlaceItem(Loc loc, Trap trap)
    {
        var newTrap = new Trap(trap.TrapType, trap.Damage) { Loc = loc };
        this.Map.Traps.Add(newTrap);
    }
}
```

3. **Use existing spawn steps or create custom:**

```csharp
// Create spawner for traps
var trapSpawner = new PickerSpawner<MapGenContext, Trap>(
    new LoopedRand<Trap>(
        new RandRange(3, 6),
        new SpawnList<Trap>
        {
            { new Trap(SPIKE_TRAP, 5), 10 },
            { new Trap(FIRE_TRAP, 10), 5 },
        }));

layout.GenSteps.Add(new Priority(7),
    new RandomSpawnStep<MapGenContext, Trap>(trapSpawner));
```

---

## 4. Create a Custom Map Context

The map context holds all state during generation and defines available capabilities.

### Interface Hierarchy

```
IGenContext (base - Rand, InitSeed, FinishGen)
    |
    +-- ITiledGenContext (tiles, RoomTerrain, WallTerrain)
    |       |
    |       +-- IFloorPlanGenContext (RoomPlan for freeform rooms)
    |       |
    |       +-- IRoomGridGenContext (GridPlan for grid-based rooms)
    |
    +-- IPlaceableGenContext<T> (spawn entities)
    |
    +-- IViewPlaceableGenContext<T> (read placed entities)
```

### Minimal Context (tiles only)

```csharp
public class MinimalContext : ITiledGenContext
{
    private int[][] tiles;
    private IRandom rand;

    public int Width => this.tiles.Length;
    public int Height => this.tiles[0].Length;
    public bool Wrap => false;
    public bool TilesInitialized => this.tiles != null;
    public IRandom Rand => this.rand;

    public ITile RoomTerrain => new Tile(0);  // Floor
    public ITile WallTerrain => new Tile(1);  // Wall

    public void InitSeed(ulong seed) => this.rand = new ReRandom(seed);
    public void FinishGen() { }

    public void CreateNew(int width, int height, bool wrap = false)
    {
        this.tiles = new int[width][];
        for (int x = 0; x < width; x++)
            this.tiles[x] = new int[height];
    }

    public ITile GetTile(Loc loc) => new Tile(this.tiles[loc.X][loc.Y]);

    public bool CanSetTile(Loc loc, ITile tile) => true;

    public bool TrySetTile(Loc loc, ITile tile)
    {
        this.tiles[loc.X][loc.Y] = ((Tile)tile).ID;
        return true;
    }

    public void SetTile(Loc loc, ITile tile) => this.TrySetTile(loc, tile);

    public bool TileBlocked(Loc loc) => this.tiles[loc.X][loc.Y] == 1;
    public bool TileBlocked(Loc loc, bool diagonal) => this.TileBlocked(loc);
}
```

### Full Context (grid + spawning)

See `RogueElements.Examples/Ex6_Items/MapGenContext.cs` for a complete example implementing:
- `IRoomGridGenContext` - Grid-based room layouts
- `IPlaceableGenContext<Item>` - Item spawning
- `IPlaceableGenContext<Mob>` - Enemy spawning
- `IViewPlaceableGenContext<StairsUp/Down>` - Stair placement and querying

---

## 5. Integrate with Game Engine

### General Pattern

```csharp
public class MapGenerator
{
    public YourGameMap Generate(ulong seed)
    {
        // 1. Build the layout
        var layout = new MapGen<MapGenContext>();
        this.ConfigureLayout(layout);

        // 2. Generate
        MapGenContext context = layout.GenMap(seed);

        // 3. Copy results to game map
        var gameMap = new YourGameMap(context.Width, context.Height);

        for (int x = 0; x < context.Width; x++)
        {
            for (int y = 0; y < context.Height; y++)
            {
                var tile = context.GetTile(new Loc(x, y));
                gameMap.SetTile(x, y, ConvertTile(tile));
            }
        }

        // Copy entities
        foreach (var item in context.Map.Items)
            gameMap.SpawnItem(item.Loc.X, item.Loc.Y, item.ID);

        return gameMap;
    }
}
```

### Unity

```csharp
public class DungeonGenerator : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public TileBase floorTile;
    public TileBase wallTile;

    public void Generate()
    {
        var layout = BuildLayout();
        var context = layout.GenMap((ulong)Random.Range(0, int.MaxValue));

        for (int x = 0; x < context.Width; x++)
        {
            for (int y = 0; y < context.Height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                var tile = context.GetTile(new Loc(x, y));

                if (IsWall(tile))
                    wallTilemap.SetTile(pos, wallTile);
                else
                    floorTilemap.SetTile(pos, floorTile);
            }
        }
    }
}
```

### MonoGame

```csharp
public class DungeonScreen : GameScreen
{
    private int[,] tileMap;

    public void Generate()
    {
        var layout = BuildLayout();
        var context = layout.GenMap((ulong)new Random().Next());

        this.tileMap = new int[context.Width, context.Height];

        for (int x = 0; x < context.Width; x++)
        {
            for (int y = 0; y < context.Height; y++)
            {
                var tile = (Tile)context.GetTile(new Loc(x, y));
                this.tileMap[x, y] = tile.ID;
            }
        }
    }

    public override void Draw(GameTime gameTime)
    {
        for (int x = 0; x < this.tileMap.GetLength(0); x++)
        {
            for (int y = 0; y < this.tileMap.GetLength(1); y++)
            {
                var sourceRect = GetTileSourceRect(this.tileMap[x, y]);
                spriteBatch.Draw(tileset, new Vector2(x * 16, y * 16), sourceRect, Color.White);
            }
        }
    }
}
```

### Godot (C#)

```csharp
public partial class DungeonGenerator : Node2D
{
    [Export] public TileMap tileMap;

    public void Generate()
    {
        var layout = BuildLayout();
        var context = layout.GenMap((ulong)GD.Randi());

        for (int x = 0; x < context.Width; x++)
        {
            for (int y = 0; y < context.Height; y++)
            {
                var tile = (Tile)context.GetTile(new Loc(x, y));
                int atlasId = tile.ID == 0 ? 0 : 1;  // Floor vs Wall
                tileMap.SetCell(0, new Vector2I(x, y), 0, new Vector2I(atlasId, 0));
            }
        }
    }
}
```
