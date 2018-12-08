using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements.Examples.Ex2_Rooms
{
    public static class Example2
    {
        public static void Run()
        {
            Console.Clear();
            string title = "2: A Map Made with Rooms and Halls";

            MapGen<MapGenContext> layout = new MapGen<MapGenContext>();

            //Initialize a 54x40 floorplan with which to populate with rectangular floor and halls.
            InitFloorPlanStep<MapGenContext> startGen = new InitFloorPlanStep<MapGenContext>();
            startGen.Width = 54;
            startGen.Height = 40;
            layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(-2, startGen));


            //Create a path that is composed of a branching tree
            FloorPathBranch<MapGenContext> path = new FloorPathBranch<MapGenContext>();
            path.HallPercent = 50;
            path.FillPercent = new RandRange(45);
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
            genericHalls.Add(new RoomGenAngledHall<MapGenContext>(0, new RandRange(3, 7), new RandRange(3, 7)), 10);
            genericHalls.Add(new RoomGenSquare<MapGenContext>(new RandRange(1), new RandRange(1)), 20);
            path.GenericHalls = genericHalls;

            layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(-1, path));


            //Draw the rooms onto the tiled map, with 1 TILE padded on each side
            layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(0, new DrawFloorToTileStep<MapGenContext>(1)));




            //Run the generator and print
            MapGenContext context = layout.GenMap(MathUtils.Rand.NextUInt64());
            Print(context.Map, title);
        }

        public static void Print(Map map, string title)
        {
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
                    topString.Append(tileChar);
                }
                topString.Append('\n');
            }
            Console.Write(topString.ToString());
        }
    }
}
