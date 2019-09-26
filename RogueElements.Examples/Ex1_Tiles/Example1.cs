// <copyright file="Example1.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;

namespace RogueElements.Examples.Ex1_Tiles
{
    public static class Example1
    {
        public static void Run()
        {
            Console.Clear();
            const string title = "1: A Static Map Example";
            var layout = new MapGen<MapGenContext>();

            // Initialize a 30x25 blank map full of Wall tiles
            InitTilesStep<MapGenContext> startStep = new InitTilesStep<MapGenContext>(30, 25);
            layout.GenSteps.Add(0, startStep);

            // Draw a specific array of tiles onto the map at offset X2,Y3
            string[] level =
            {
                ".........................",
                ".........................",
                "...........#.............",
                "....###...###...###......",
                "...#.#.....#.....#.#.....",
                "...####...###...####.....",
                "...#.#############.#.....",
                "......##.......##........",
                "......#..#####..#........",
                "......#.#######.#........",
                "...#.##.#######.##.#.....",
                "..#####.###.###.#####....",
                "...#.##.#######.##.#.....",
                "......#.#######.#........",
                "......#..#####..#........",
                "......##.......##........",
                "...#.#############.#.....",
                "...####...###...####.....",
                "...#.#.....#.....#.#.....",
                "....###...###...###......",
                "...........#.............",
            };
            ITile[][] tiles = new ITile[level[0].Length][];
            for (int xx = 0; xx < level[0].Length; xx++)
            {
                tiles[xx] = new ITile[level.Length];
                for (int yy = 0; yy < level.Length; yy++)
                {
                    int id = Map.WALL_TERRAIN_ID;
                    if (level[yy][xx] == '.')
                        id = Map.ROOM_TERRAIN_ID;
                    tiles[xx][yy] = new Tile(id);
                }
            }

            var drawStep = new SpecificTilesStep<MapGenContext>(tiles, new Loc(2, 3));
            layout.GenSteps.Add(0, drawStep);

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
                    if (tile.ID <= 0) // wall
                        tileChar = '#';
                    else if (tile.ID == 1) // floor
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
