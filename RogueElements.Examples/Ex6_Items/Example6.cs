// <copyright file="Example6.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;

namespace RogueElements.Examples.Ex6_Items
{
    /// <summary>
    /// Example 6: Demonstrates entity spawning with weighted random selection.
    /// This example shows how to place items and mobs using SpawnList for
    /// weighted randomization and RandomSpawnStep for automatic placement.
    /// </summary>
    /// <remarks>
    /// Key concepts introduced:
    /// - ISpawnable: Interface for entities that can be spawned (requires Copy() method)
    /// - SpawnList&lt;T&gt;: Collection with weighted random selection
    /// - RandomSpawnStep: Places spawnable entities at valid map locations
    /// - PickerSpawner: Determines WHAT to spawn using a SpawnList
    /// - LoopedRand: Determines HOW MANY times to spawn from the list
    /// - Multiple IPlaceableGenContext&lt;T&gt;: Same context can spawn different entity types
    /// </remarks>
    public static class Example6
    {
        /// <summary>
        /// Runs the item/mob spawning example, creating a map populated with entities.
        /// </summary>
        public static void Run()
        {
            Console.Clear();
            const string title = "6: A Map with Randomly Placed Items/Mobs";
            var layout = new MapGen<MapGenContext>();

            // Initialize a 6x4 grid of 10x10 cells.
            var startGen = new InitGridPlanStep<MapGenContext>(1)
            {
                CellX = 6,
                CellY = 4,
                CellWidth = 9,
                CellHeight = 9,
            };
            layout.GenSteps.Add(-4, startGen);

            // Create a path that is composed of a ring around the edge
            var path = new GridPathBranch<MapGenContext>
            {
                RoomRatio = new RandRange(70),
                BranchRatio = new RandRange(0, 50),
            };

            var genericRooms = new SpawnList<RoomGen<MapGenContext>>
            {
                { new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)), 10 }, // cross
                { new RoomGenRound<MapGenContext>(new RandRange(5, 9), new RandRange(5, 9)), 10 }, // round
            };
            path.GenericRooms = genericRooms;

            var genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>>
            {
                { new RoomGenAngledHall<MapGenContext>(50), 10 },
            };
            path.GenericHalls = genericHalls;

            layout.GenSteps.Add(-4, path);

            // Output the rooms into a FloorPlan
            layout.GenSteps.Add(-2, new DrawGridToFloorStep<MapGenContext>());

            // Draw the rooms of the FloorPlan onto the tiled map, with 1 TILE padded on each side
            layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));

            // Add the stairs up and down
            layout.GenSteps.Add(2, new FloorStairsStep<MapGenContext, StairsUp, StairsDown>(0, new StairsUp(), new StairsDown()));

            // Generate water (covered in Ex5)
            const int terrain = 2;
            var waterPostProc = new PerlinWaterStep<MapGenContext>(new RandRange(35), 3, new Tile(terrain), new MapTerrainStencil<MapGenContext>(false, true, false, false), 1);
            layout.GenSteps.Add(3, waterPostProc);

            // Remove walls where diagonals of water exist and replace with water
            layout.GenSteps.Add(4, new DropDiagonalBlockStep<MapGenContext>(new Tile(terrain)));

            // Remove water stuck in the walls
            layout.GenSteps.Add(4, new EraseIsolatedStep<MapGenContext>(new Tile(terrain)));

            // ===================================================================================
            // ITEM SPAWNING - NEW CONCEPT
            // ===================================================================================

            // SpawnList<T> provides weighted random selection.
            // Each item has a "spawn rate" (weight) that affects its selection probability.
            // Higher weight = more likely to be selected when picking randomly.
            //
            // The weight is relative: if item A has weight 10 and item B has weight 50,
            // B is 5x more likely to be selected than A.
            var itemSpawns = new SpawnList<Item>
            {
                // Common roguelike item symbols with their spawn weights:
                { new Item((int)'!'), 10 },  // Potion - weight 10 (uncommon)
                { new Item((int)']'), 10 },  // Armor - weight 10 (uncommon)
                { new Item((int)'='), 10 },  // Ring - weight 10 (uncommon)
                { new Item((int)'?'), 10 },  // Scroll - weight 10 (uncommon)
                { new Item((int)'$'), 10 },  // Gold - weight 10 (uncommon)
                { new Item((int)'/'), 10 },  // Wand - weight 10 (uncommon)
                { new Item((int)'*'), 50 },  // Rock/gem - weight 50 (5x more common!)
            };

            // RandomSpawnStep places entities at random valid locations on the map.
            // It requires:
            // 1. A "spawner" that decides WHAT to spawn (PickerSpawner)
            // 2. The spawner uses a "picker" that decides HOW MANY (LoopedRand)
            //
            // PickerSpawner: Wraps a randomizer to provide spawnable instances
            // LoopedRand: Picks from itemSpawns a random number of times (10-18 items)
            RandomSpawnStep<MapGenContext, Item> itemPlacement = new RandomSpawnStep<MapGenContext, Item>(new PickerSpawner<MapGenContext, Item>(new LoopedRand<Item>(itemSpawns, new RandRange(10, 19))));
            layout.GenSteps.Add(6, itemPlacement);

            // ===================================================================================
            // MOB SPAWNING - SAME PATTERN, DIFFERENT TYPE
            // ===================================================================================

            // Mobs use the exact same spawning system as items.
            // The context implements IPlaceableGenContext<Mob> to support this.
            var mobSpawns = new SpawnList<Mob>
            {
                // Classic roguelike monster symbols with spawn weights:
                { new Mob((int)'r'), 20 },   // Rat - weight 20 (common)
                { new Mob((int)'T'), 10 },   // Troll - weight 10 (uncommon)
                { new Mob((int)'D'), 5 },    // Dragon - weight 5 (rare, half as likely as Troll)
            };

            // Same pattern: RandomSpawnStep + PickerSpawner + LoopedRand
            // Spawns 10-18 mobs at random valid locations
            RandomSpawnStep<MapGenContext, Mob> mobPlacement = new RandomSpawnStep<MapGenContext, Mob>(new PickerSpawner<MapGenContext, Mob>(new LoopedRand<Mob>(mobSpawns, new RandRange(10, 19))));
            layout.GenSteps.Add(6, mobPlacement);

            // Run the generator and print
            MapGenContext context = layout.GenMap(MathUtils.Rand.NextUInt64());
            Print(context.Map, title);
        }

        /// <summary>
        /// Prints the generated map to the console with items and mobs.
        /// </summary>
        /// <param name="map">The generated map containing tiles, stairs, items, and mobs.</param>
        /// <param name="title">Title to display above the map.</param>
        /// <remarks>
        /// Rendering priority (highest to lowest): Mobs, Items, Stairs, Terrain.
        /// Each entity type can override the display of lower-priority elements.
        /// </remarks>
        public static void Print(Map map, string title)
        {
            var topString = new StringBuilder(string.Empty);
            string turnString = title;
            topString.Append($"{turnString,-82}");
            topString.Append('\n');
            for (int i = 0; i < map.Width + 1; i++)
                topString.Append("=");
            topString.Append('\n');

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    Loc loc = new Loc(x, y);
                    char tileChar;
                    Tile tile = map.Tiles[x][y];
                    switch (tile.ID)
                    {
                        case BaseMap.WALL_TERRAIN_ID:
                            tileChar = '#';
                            break;
                        case BaseMap.ROOM_TERRAIN_ID:
                            tileChar = '.';
                            break;
                        case BaseMap.WATER_TERRAIN_ID:
                            tileChar = '~';
                            break;
                        default:
                            tileChar = '?';
                            break;
                    }

                    foreach (StairsUp entrance in map.GenEntrances)
                    {
                        if (entrance.Loc == loc)
                        {
                            tileChar = '<';
                            break;
                        }
                    }

                    foreach (StairsDown entrance in map.GenExits)
                    {
                        if (entrance.Loc == loc)
                        {
                            tileChar = '>';
                            break;
                        }
                    }

                    // Items render on top of terrain but below mobs
                    foreach (Item item in map.Items)
                    {
                        if (item.Loc == loc)
                        {
                            tileChar = (char)item.ID;
                            break;
                        }
                    }

                    // Mobs render on top of everything
                    foreach (Mob item in map.Mobs)
                    {
                        if (item.Loc == loc)
                        {
                            tileChar = (char)item.ID;
                            break;
                        }
                    }

                    topString.Append(tileChar);
                }

                topString.Append('\n');
            }

            Console.Write(topString.ToString());
        }
    }
}
