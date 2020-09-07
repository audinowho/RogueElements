// <copyright file="Example4.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;

namespace RogueElements.Examples.Ex4_Stairs
{
    public static class Example4
    {
        public static void Run()
        {
            Console.Clear();
            const string title = "4: A Map with Stairs Up and Down";
            var layout = new MapGen<MapGenContext>();

            // Initialize a 3x2 grid of 10x10 cells.
            var startGen = new InitGridPlanStep<MapGenContext>(1)
            {
                CellX = 3,
                CellY = 2,
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
                    Tile tile = map.Tiles[x][y];
                    char tileChar = tile.ID <= BaseMap.WALL_TERRAIN_ID ? '#' : tile.ID == BaseMap.ROOM_TERRAIN_ID ? '.' : '?';

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
