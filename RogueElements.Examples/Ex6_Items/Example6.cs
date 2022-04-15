// <copyright file="Example6.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;

namespace RogueElements.Examples.Ex6_Items
{
    public static class Example6
    {
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
            layout.GenSteps.Add(2, new FloorStairsStep<MapGenContext, StairsUp, StairsDown>(new StairsUp(), new StairsDown()));

            // Generate water (specified by user as Terrain 2) with a frequency of 35%, using Perlin Noise in an order of 3, softness 1.
            const int terrain = 2;
            var waterPostProc = new PerlinWaterStep<MapGenContext>(new RandRange(35), 3, new Tile(terrain), new MapTerrainStencil<MapGenContext>(false, true, false), 1);
            layout.GenSteps.Add(3, waterPostProc);

            // Remove walls where diagonals of water exist and replace with water
            layout.GenSteps.Add(4, new DropDiagonalBlockStep<MapGenContext>(new Tile(terrain)));

            // Remove water stuck in the walls
            layout.GenSteps.Add(4, new EraseIsolatedStep<MapGenContext>(new Tile(terrain)));

            // Apply Items
            var itemSpawns = new SpawnList<Item>
            {
                { new Item((int)'!'), 10 },
                { new Item((int)']'), 10 },
                { new Item((int)'='), 10 },
                { new Item((int)'?'), 10 },
                { new Item((int)'$'), 10 },
                { new Item((int)'/'), 10 },
                { new Item((int)'*'), 50 },
            };
            RandomSpawnStep<MapGenContext, Item> itemPlacement = new RandomSpawnStep<MapGenContext, Item>(new PickerSpawner<MapGenContext, Item>(new LoopedRand<Item>(itemSpawns, new RandRange(10, 19))));
            layout.GenSteps.Add(6, itemPlacement);

            // Apply Mobs
            var mobSpawns = new SpawnList<Mob>
            {
                { new Mob((int)'r'), 20 },
                { new Mob((int)'T'), 10 },
                { new Mob((int)'D'), 5 },
            };
            RandomSpawnStep<MapGenContext, Mob> mobPlacement = new RandomSpawnStep<MapGenContext, Mob>(new PickerSpawner<MapGenContext, Mob>(new LoopedRand<Mob>(mobSpawns, new RandRange(10, 19))));
            layout.GenSteps.Add(6, mobPlacement);

            // Run the generator and print
            MapGenContext context = layout.GenMap(MathUtils.Rand.NextUInt64());
            Print(context.Map, title);
        }

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

                    foreach (Item item in map.Items)
                    {
                        if (item.Loc == loc)
                        {
                            tileChar = (char)item.ID;
                            break;
                        }
                    }

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
