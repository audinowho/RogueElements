using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements.Examples.Ex6_Items
{
    public static class Example6
    {
        public static void Run()
        {
            string title = "6: A Map with Randomly Placed Items";
            MapGen<MapGenContext> layout = new MapGen<MapGenContext>();

            //Initialize a 54x40 floorplan with which to populate with rectangular floor and halls.
            InitFloorPlanStep<MapGenContext> startGen = new InitFloorPlanStep<MapGenContext>();
            startGen.Width = 54;
            startGen.Height = 40;
            layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(-2, startGen));


            //Create a path that is composed of a branching tree
            FloorPathBranch<MapGenContext> path = new FloorPathBranch<MapGenContext>();
            path.HallPercent = 50;
            path.FillPercent = new RandRange(60);
            path.BranchRatio = new RandRange(0, 25);

            //Give it some room types to place
            SpawnList<RoomGen<MapGenContext>> genericRooms = new SpawnList<RoomGen<MapGenContext>>();
            //square
            genericRooms.Add(new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)));
            //round
            genericRooms.Add(new RoomGenRound<MapGenContext>(new RandRange(5, 9), new RandRange(5, 9)));
            path.GenericRooms = genericRooms;

            //Give it some hall types to place
            SpawnList<PermissiveRoomGen<MapGenContext>> genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>>();
            genericHalls.Add(new RoomGenAngledHall<MapGenContext>(0, new RandRange(3), new RandRange(3)), 10);
            genericHalls.Add(new RoomGenSquare<MapGenContext>(new RandRange(1), new RandRange(1)), 20);
            path.GenericHalls = genericHalls;

            layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(-1, path));

            {
                ConnectBranchStep<MapGenContext> step = new ConnectBranchStep<MapGenContext>(100);
                PresetPicker<PermissiveRoomGen<MapGenContext>> picker = new PresetPicker<PermissiveRoomGen<MapGenContext>>();
                picker.ToSpawn = new RoomGenAngledHall<MapGenContext>(0);
                step.GenericHalls = picker;
                layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(-1, step));
            }
            {
                ConnectRoomStep<MapGenContext> step = new ConnectRoomStep<MapGenContext>(new RandRange(30));
                PresetPicker<PermissiveRoomGen<MapGenContext>> picker = new PresetPicker<PermissiveRoomGen<MapGenContext>>();
                picker.ToSpawn = new RoomGenAngledHall<MapGenContext>(0);
                step.GenericHalls = picker;
                layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(-1, step));
            }

            //Draw the rooms onto the tiled map, with 1 TILE padded on each side
            layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(0, new DrawFloorToTileStep<MapGenContext>(1)));




            //Add the stairs up and down
            layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(2, new FloorStairsStep<MapGenContext, StairsUp, StairsDown>(new StairsUp(), new StairsDown())));


            //Generate water (specified by user as Terrain 2) with a frequency of 30%, using Perlin Noise in an order of 3.
            int terrain = 2;
            BlobWaterStep<MapGenContext> waterPostProc = new BlobWaterStep<MapGenContext>(new RandRange(5), new Tile(terrain), 0, new RandRange(40));
            layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(3, waterPostProc));

            //Remove walls where diagonals of water exist and replace with water
            layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(4, new DropDiagonalBlockStep<MapGenContext>(new Tile(terrain))));
            //Remove water stuck in the walls
            layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(4, new EraseIsolatedStep<MapGenContext>(new Tile(terrain))));




            //Apply Items
            SpawnList<Item> itemSpawns = new SpawnList<Item>();
            itemSpawns.Add(new Item((int)'!'), 10);
            itemSpawns.Add(new Item((int)']'), 10);
            itemSpawns.Add(new Item((int)'='), 10);
            itemSpawns.Add(new Item((int)'?'), 10);
            itemSpawns.Add(new Item((int)'$'), 10);
            itemSpawns.Add(new Item((int)'/'), 10);
            itemSpawns.Add(new Item((int)'*'), 50);
            RandomSpawnStep<MapGenContext, Item> itemPlacement = new RandomSpawnStep<MapGenContext, Item>(new PickerSpawner<MapGenContext, Item>(new LoopedRand<Item>(itemSpawns, new RandRange(10, 19))));
            layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(6, itemPlacement));


            layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(7, new DetectIsolatedStep<MapGenContext, StairsUp>()));


            //ulong seed = 0;
            //try
            //{
            //    for (int ii = 0; ii < 100000; ii++)
            //    {
            //        seed = MathUtils.Rand.NextUInt64();
            //        MapGenContext context = layout.GenMap(seed);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.Write("ERROR: " + seed);
            //}

            //Run the generator and print
            MapGenContext context = layout.GenMap(MathUtils.Rand.NextUInt64());
            Print(context.Map, title);
        }


        public static void Print(Map map, string title)
        {
            int oldLeft = Console.CursorLeft;
            int oldTop = Console.CursorTop;
            Console.SetCursorPosition(0, 0);
            StringBuilder topString = new StringBuilder("");
            string turnString = title;
            topString.Append(String.Format("{0,-82}", turnString));
            topString.Append('\n');
            for (int i = 0; i < map.Width + 1; i++)
                topString.Append("=");
            topString.Append('\n');

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    Loc loc = new Loc(x, y);
                    char tileChar = ' ';
                    Tile tile = map.Tiles[x][y];
                    if (tile.ID <= 0)//wall
                        tileChar = '#';
                    else if (tile.ID == 1)//floor
                        tileChar = '.';
                    else if (tile.ID == 2)//water
                        tileChar = '~';
                    else
                        tileChar = '?';


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
