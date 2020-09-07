// <copyright file="Example2.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;

namespace RogueElements.Examples.Ex2_Rooms
{
    public static class Example2
    {
        public static void Run()
        {
            Console.Clear();
            const string title = "2: A Map Made with Rooms and Halls";

            var layout = new MapGen<MapGenContext>();

            // Initialize a 54x40 floorplan with which to populate with rectangular floor and halls.
            InitFloorPlanStep<MapGenContext> startGen = new InitFloorPlanStep<MapGenContext>(54, 40);
            layout.GenSteps.Add(-2, startGen);

            // Create some room types to place
            var genericRooms = new SpawnList<RoomGen<MapGenContext>>
            {
                { new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)), 10 }, // cross
                { new RoomGenRound<MapGenContext>(new RandRange(5, 9), new RandRange(5, 9)), 10 }, // round
            };

            // Create some hall types to place
            var genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>>
            {
                { new RoomGenAngledHall<MapGenContext>(0, new RandRange(3, 7), new RandRange(3, 7)), 10 },
                { new RoomGenSquare<MapGenContext>(new RandRange(1), new RandRange(1)), 20 },
            };

            // Feed the room and hall types to a path that is composed of a branching tree
            FloorPathBranch<MapGenContext> path = new FloorPathBranch<MapGenContext>(genericRooms, genericHalls)
            {
                HallPercent = 50,
                FillPercent = new RandRange(45),
                BranchRatio = new RandRange(0, 25),
            };

            layout.GenSteps.Add(-1, path);

            // Draw the rooms onto the tiled map, with 1 TILE padded on each side
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
