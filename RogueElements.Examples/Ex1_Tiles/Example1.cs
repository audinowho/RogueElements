// <copyright file="Example1.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;

namespace RogueElements.Examples.Ex1_Tiles
{
    /// <summary>
    /// Example 1: Introduction to the RogueElements Map Generation Pipeline.
    ///
    /// This is the simplest possible example demonstrating the core concepts:
    /// - MapGen&lt;T&gt; as the orchestrator that runs the generation pipeline
    /// - GenStep&lt;T&gt; as the base class for all generation passes
    /// - Priority-based ordering of generation steps
    /// - Seed-based reproducibility for deterministic map generation
    ///
    /// This example creates a static, hand-designed map to illustrate the basic
    /// pipeline mechanics before introducing procedural generation in later examples.
    /// </summary>
    public static class Example1
    {
        /// <summary>
        /// Runs Example 1, demonstrating a static tile-based map generation.
        ///
        /// Key RogueElements Concepts Introduced:
        /// 1. MapGen&lt;T&gt; - The orchestrator that holds and executes GenSteps
        /// 2. GenStep&lt;T&gt; - Base class for generation passes (InitTilesStep, SpecificTilesStep)
        /// 3. Priority - Numeric ordering that determines step execution sequence
        /// 4. Seed - A 64-bit value ensuring reproducible generation results
        /// </summary>
        public static void Run()
        {
            Console.Clear();
            const string title = "1: A Static Map Example";

            // ============================================================
            // STEP 1: Create the MapGen orchestrator
            // ============================================================
            // MapGen<T> is the central orchestrator of the generation pipeline.
            // The generic parameter T specifies the context type that GenSteps will operate on.
            // MapGenContext must implement IGenContext (minimum) and typically ITiledGenContext
            // for tile-based operations.
            var layout = new MapGen<MapGenContext>();

            // ============================================================
            // STEP 2: Add an InitTilesStep to create the blank tile array
            // ============================================================
            // InitTilesStep<T> is a built-in GenStep that:
            // - Creates the tile array with specified dimensions (30 wide x 25 tall)
            // - Fills all tiles with the "wall" terrain (ID 0) by default
            //
            // The step is added with Priority 0. Priority determines execution order:
            // - Lower numbers execute first
            // - Steps with the same priority execute in the order they were added
            // - Negative priorities are valid (see Ex2 which uses -2, -1, 0)
            InitTilesStep<MapGenContext> startStep = new InitTilesStep<MapGenContext>(30, 25);
            layout.GenSteps.Add(0, startStep);

            // ============================================================
            // STEP 3: Prepare a hand-designed tile pattern
            // ============================================================
            // This array represents our desired map layout as ASCII art.
            // '.' = floor/room terrain (passable)
            // '#' = wall terrain (impassable)
            //
            // In procedural generation, you would use RoomGen classes instead,
            // but this demonstrates how tiles work at the lowest level.
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

            // Convert the string array to a 2D ITile array.
            // ITile is the interface for tile data; Tile is the concrete implementation.
            // The array is indexed as tiles[x][y] (column-major order).
            ITile[][] tiles = new ITile[level[0].Length][];
            for (int xx = 0; xx < level[0].Length; xx++)
            {
                tiles[xx] = new ITile[level.Length];
                for (int yy = 0; yy < level.Length; yy++)
                {
                    // Map.WALL_TERRAIN_ID (0) = wall/impassable
                    // Map.ROOM_TERRAIN_ID (1) = floor/passable
                    // These constants are defined in BaseMap for consistency across examples.
                    int id = Map.WALL_TERRAIN_ID;
                    if (level[yy][xx] == '.')
                        id = Map.ROOM_TERRAIN_ID;
                    tiles[xx][yy] = new Tile(id);
                }
            }

            // ============================================================
            // STEP 4: Add a SpecificTilesStep to draw the pattern
            // ============================================================
            // SpecificTilesStep<T> is a built-in GenStep that stamps a pre-defined
            // tile array onto the map at a specified offset (Loc is a 2D point).
            //
            // Parameters:
            // - tiles: The 2D array of ITile to stamp
            // - new Loc(2, 3): The X,Y offset where stamping begins (2 tiles from left, 3 from top)
            //
            // This step also has Priority 0, so it executes after InitTilesStep
            // (which was added first at the same priority).
            var drawStep = new SpecificTilesStep<MapGenContext>(tiles, new Loc(2, 3));
            layout.GenSteps.Add(0, drawStep);

            // ============================================================
            // STEP 5: Run the generation pipeline
            // ============================================================
            // GenMap(seed) executes all GenSteps in priority order and returns the context.
            //
            // The seed (a 64-bit unsigned integer) initializes the random number generator.
            // Using the same seed always produces the same map - this is crucial for:
            // - Debugging: reproduce exact scenarios
            // - Multiplayer: all clients generate identical maps
            // - Seeded runs: players can share interesting map seeds
            //
            // MathUtils.Rand.NextUInt64() generates a random seed for variety.
            // For reproducibility, you could use a fixed seed like: layout.GenMap(12345);
            MapGenContext context = layout.GenMap(MathUtils.Rand.NextUInt64());

            // The generated map is accessible through context.Map
            Print(context.Map, title);
        }

        /// <summary>
        /// Prints the generated map to the console using ASCII characters.
        /// </summary>
        /// <param name="map">The generated map containing the tile data.</param>
        /// <param name="title">The title to display above the map.</param>
        /// <remarks>
        /// This is a simple visualization helper. In a real game, you would render
        /// the map using your game engine's graphics system instead.
        ///
        /// Tile ID interpretation:
        /// - ID &lt;= 0: Wall (rendered as '#')
        /// - ID == 1: Floor (rendered as '.')
        /// - Other: Unknown (rendered as '?')
        /// </remarks>
        public static void Print(Map map, string title)
        {
            var topString = new StringBuilder(string.Empty);
            string turnString = title;
            topString.Append($"{turnString,-82}");
            topString.Append('\n');

            // Draw a separator line
            for (int i = 0; i < map.Width + 1; i++)
                topString.Append("=");
            topString.Append('\n');

            // Iterate through all tiles and convert to ASCII characters.
            // Map dimensions come from the Tiles array created by InitTilesStep.
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    char tileChar;
                    Tile tile = map.Tiles[x][y];

                    // Convert tile ID to display character
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
