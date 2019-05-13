﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements.Examples.Ex5_Terrain
{
    public static class Example5
    {
        public static void Run()
        {
            Console.Clear();
            string title = "5: A Map with Terrain Features";
            var layout = new MapGen<MapGenContext>();

            //Initialize a 6x4 grid of 10x10 cells.
            var startGen = new InitGridPlanStep<MapGenContext>(1)
            {
                CellX = 6,
                CellY = 4,
                CellWidth = 9,
                CellHeight = 9
            };
            layout.GenSteps.Add(-4, startGen);



            //Create a path that is composed of a ring around the edge
            var path = new GridPathBranch<MapGenContext>
            {
                RoomRatio = new RandRange(70),
                BranchRatio = new RandRange(0, 50)
            };

            var genericRooms = new SpawnList<RoomGen<MapGenContext>>
            {
                //cross
                new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)),
                //round
                new RoomGenRound<MapGenContext>(new RandRange(5, 9), new RandRange(5, 9))
            };
            path.GenericRooms = genericRooms;

            var genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>> { new RoomGenAngledHall<MapGenContext>(50) };
            path.GenericHalls = genericHalls;

            layout.GenSteps.Add(-4, path);



            //Output the rooms into a FloorPlan
            layout.GenSteps.Add(-2, new DrawGridToFloorStep<MapGenContext>());




            //Draw the rooms of the FloorPlan onto the tiled map, with 1 TILE padded on each side
            layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));



            //Add the stairs up and down
            layout.GenSteps.Add(2, new FloorStairsStep<MapGenContext, StairsUp, StairsDown>(new StairsUp(), new StairsDown()));


            //Generate water (specified by user as Terrain 2) with a frequency of 35%, using Perlin Noise in an order of 3, softness 1.
            int terrain = 2;
            PerlinWaterStep<MapGenContext> waterPostProc = new PerlinWaterStep<MapGenContext>(new RandRange(35), 3, 1, new Tile(terrain), false);
            layout.GenSteps.Add(3, waterPostProc);

            //Remove walls where diagonals of water exist and replace with water
            layout.GenSteps.Add(4, new DropDiagonalBlockStep<MapGenContext>(new Tile(terrain)));
            //Remove water stuck in the walls
            layout.GenSteps.Add(4, new EraseIsolatedStep<MapGenContext>(new Tile(terrain)));
            

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
                    char tileChar;
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

                    topString.Append(tileChar);
                }
                topString.Append('\n');
            }
            Console.Write(topString.ToString());
        }
    }
}
