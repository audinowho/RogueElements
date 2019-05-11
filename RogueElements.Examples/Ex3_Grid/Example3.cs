using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements.Examples.Ex3_Grid
{
    public static class Example3
    {
        public static void Run()
        {
            Console.Clear();
            string title = "3: A Map made with Rooms and Halls arranged in a grid.";

            MapGen<MapGenContext> layout = new MapGen<MapGenContext>();

            //Initialize a 6x4 grid of 10x10 cells.
            InitGridPlanStep<MapGenContext> startGen = new InitGridPlanStep<MapGenContext>(1);
            startGen.CellX = 6;
            startGen.CellY = 4;

            startGen.CellWidth = 9;
            startGen.CellHeight = 9;
            layout.GenSteps.Add(-4, startGen);



            //Create a path that is composed of branches in grid lock
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

            layout.GenSteps.Add(-4, path);



            //Output the rooms into a FloorPlan
            layout.GenSteps.Add(-2, new DrawGridToFloorStep<MapGenContext>());




            //Draw the rooms of the FloorPlan onto the tiled map, with 1 TILE padded on each side
            layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));




            //Run the generator and print
            MapGenContext context = layout.GenMap(MathUtils.Rand.NextUInt64());
            Print(context.Map, title);
        }


        public static void Print(Map map, string title)
        {
            StringBuilder topString = new StringBuilder("");
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
                    char tileChar = ' ';
                    Tile tile = map.Tiles[x][y];
                    if (tile.ID <= 0)//wall
                        tileChar = '#';
                    else if (tile.ID == 1)//floor
                        tileChar = '.';
                    else
                        tileChar = '?';
                    topString.Append(tileChar);
                }
                topString.Append('\n');
            }
            Console.Write(topString.ToString());
        }
    }
}
