using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements.Examples.Ex4_Stairs
{
    public static class Example4
    {
        public static void Run()
        {
            string title = "4: A Map with Stairs Up and Down";
            MapGen<MapGenContext> layout = new MapGen<MapGenContext>();

            //Initialize a 3x2 grid of 10x10 cells.
            InitGridPlanStep<MapGenContext> startGen = new InitGridPlanStep<MapGenContext>(1);
            startGen.CellX = 3;
            startGen.CellY = 2;

            startGen.CellWidth = 9;
            startGen.CellHeight = 9;
            layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(-4, startGen));



            //Create a path that is composed of a ring around the edge
            GridPathBranch<MapGenContext> path = new GridPathBranch<MapGenContext>();
            path.RoomRatio = new RandRange(70);
            path.BranchRatio = new RandRange(0, 50);

            SpawnList<RoomGen<MapGenContext>> genericRooms = new SpawnList<RoomGen<MapGenContext>>();
            //cross
            genericRooms.Add(new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)));
            //round
            genericRooms.Add(new RoomGenRound<MapGenContext>(new RandRange(5, 9), new RandRange(5, 9)));
            path.GenericRooms = genericRooms;

            SpawnList<PermissiveRoomGen<MapGenContext>> genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>>();
            genericHalls.Add(new RoomGenAngledHall<MapGenContext>(50));
            path.GenericHalls = genericHalls;

            layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(-4, path));



            //Output the rooms into a FloorPlan
            layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(-2, new DrawGridToFloorStep<MapGenContext>()));




            //Draw the rooms of the FloorPlan onto the tiled map, with 1 TILE padded on each side
            layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(0, new DrawFloorToTileStep<MapGenContext>(1)));



            //Add the stairs up and down
            layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(2, new FloorStairsStep<MapGenContext, StairsUp, StairsDown>(new StairsUp(), new StairsDown())));



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

                    topString.Append(tileChar);
                }
                topString.Append('\n');
            }
            Console.Write(topString.ToString());
        }
    }
}
