// <copyright file="Example7.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;

namespace RogueElements.Examples.Ex7_Special
{
    public static class Example7
    {
        public static void Run()
        {
            Console.Clear();
            const string title = "7: A Map with Special Rooms";

            var layout = new MapGen<MapGenContext>();

            // Initialize a 54x40 floorplan with which to populate with rectangular floor and halls.
            InitFloorPlanStep<MapGenContext> startGen = new InitFloorPlanStep<MapGenContext>(54, 40);
            layout.GenSteps.Add(-2, startGen);

            // Create some room types to place
            SpawnList<RoomGen<MapGenContext>> genericRooms = new SpawnList<RoomGen<MapGenContext>>
            {
                { new RoomGenSquare<MapGenContext>(new RandRange(7, 9), new RandRange(7, 9)), 10 }, // square
                { new RoomGenRound<MapGenContext>(new RandRange(6, 10), new RandRange(6, 10)), 10 }, // round
            };

            // Create some hall types to place
            var genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>>
            {
                { new RoomGenAngledHall<MapGenContext>(0, new RandRange(3, 7), new RandRange(3, 7)), 10 },
            };

            // Feed the room and hall types to a path that is composed of a branching tree
            FloorPathBranch<MapGenContext> path = new FloorPathBranch<MapGenContext>(genericRooms, genericHalls)
            {
                HallPercent = 50,
                FillPercent = new RandRange(40),
                BranchRatio = new RandRange(0, 25),
            };

            path.RoomComponents.Set(new MainRoomComponent());
            path.HallComponents.Set(new MainHallComponent());

            layout.GenSteps.Add(-1, path);

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

            SetSpecialRoomStep<MapGenContext> listSpecialStep = new SetSpecialRoomStep<MapGenContext>
            {
                Rooms = new PresetPicker<RoomGen<MapGenContext>>(CreateRoomGenSpecific<MapGenContext>(custom)),
            };
            listSpecialStep.RoomComponents.Set(new TreasureRoomComponent());
            PresetPicker<PermissiveRoomGen<MapGenContext>> picker = new PresetPicker<PermissiveRoomGen<MapGenContext>>
            {
                ToSpawn = new RoomGenAngledHall<MapGenContext>(0),
            };
            listSpecialStep.Halls = picker;
            layout.GenSteps.Add(-1, listSpecialStep);

            // Draw the rooms of the FloorPlan onto the tiled map, with 1 TILE padded on each side
            layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));

            // Add the stairs up and down
            layout.GenSteps.Add(2, new FloorStairsStep<MapGenContext, StairsUp, StairsDown>(new StairsUp(), new StairsDown()));

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
            RandomRoomSpawnStep<MapGenContext, Item> itemPlacement = new RandomRoomSpawnStep<MapGenContext, Item>(new PickerSpawner<MapGenContext, Item>(new LoopedRand<Item>(itemSpawns, new RandRange(10, 19))));
            layout.GenSteps.Add(6, itemPlacement);

            // Apply Treasure Items
            var treasureSpawns = new SpawnList<Item>
            {
                { new Item((int)'!'), 10 },
                { new Item((int)'*'), 50 },
            };
            RandomRoomSpawnStep<MapGenContext, Item> treasurePlacement = new RandomRoomSpawnStep<MapGenContext, Item>(new PickerSpawner<MapGenContext, Item>(new LoopedRand<Item>(treasureSpawns, new RandRange(7, 10))));
            treasurePlacement.Filters.Add(new RoomFilterComponent(false, new TreasureRoomComponent()));
            layout.GenSteps.Add(6, treasurePlacement);

            // Run the generator and print
            MapGenContext context = layout.GenMap(MathUtils.Rand.NextUInt64());
            Print(context.Map, title);
        }

        public static RoomGenSpecific<T> CreateRoomGenSpecific<T>(string[] level)
            where T : class, ITiledGenContext
        {
#pragma warning disable CC0008 // Use object initializer
            RoomGenSpecific<T> roomGen = new RoomGenSpecific<T>(level[0].Length, level.Length, new Tile(BaseMap.ROOM_TERRAIN_ID));
#pragma warning restore CC0008 // Use object initializer
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

                    topString.Append(tileChar);
                }

                topString.Append('\n');
            }

            Console.Write(topString.ToString());
        }
    }
}
