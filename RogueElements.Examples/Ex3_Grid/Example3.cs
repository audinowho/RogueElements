// <copyright file="Example3.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;

namespace RogueElements.Examples.Ex3_Grid
{
    public static class Example3
    {
        public static void Run()
        {
            Console.Clear();
            const string title = "3: A Map made with Rooms and Halls arranged in a grid.";

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

            // Create a path that is composed of branches in grid lock
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

            var genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>> { { new RoomGenAngledHall<MapGenContext>(50), 10 } };
            path.GenericHalls = genericHalls;

            layout.GenSteps.Add(-4, path);

            // Output the rooms into a FloorPlan
            layout.GenSteps.Add(-2, new DrawGridToFloorStep<MapGenContext>());

            // Draw the rooms of the FloorPlan onto the tiled map, with 1 TILE padded on each side
            layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));

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
                        default:
                            tileChar = '?';
                            break;
                    }

                    topString.Append(tileChar);
                }

                topString.Append('\n');
            }

            Console.Write(topString.ToString());
        }
    }
}
