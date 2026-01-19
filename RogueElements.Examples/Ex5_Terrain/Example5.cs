// <copyright file="Example5.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;

namespace RogueElements.Examples.Ex5_Terrain
{
    /// <summary>
    /// Example 5: Demonstrates terrain generation using Perlin noise and post-processing.
    /// This example introduces water terrain that overlays the basic floor tiles,
    /// showing how terrain steps modify existing tile data rather than replacing it.
    /// </summary>
    /// <remarks>
    /// Key concepts introduced:
    /// - PerlinWaterStep: Uses Perlin noise for natural-looking terrain distribution
    /// - MapTerrainStencil: Controls which tiles can be modified by terrain steps
    /// - Post-processing steps: DropDiagonalBlockStep and EraseIsolatedStep clean up artifacts
    /// - Terrain layering: Water terrain (ID=2) coexists with walls (ID=0) and floors (ID=1)
    /// </remarks>
    public static class Example5
    {
        /// <summary>
        /// Runs the terrain generation example, creating a map with Perlin noise-based water.
        /// </summary>
        public static void Run()
        {
            Console.Clear();
            const string title = "5: A Map with Terrain Features";
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
            layout.GenSteps.Add(2, new FloorStairsStep<MapGenContext, StairsUp, StairsDown>(0, new StairsUp(), new StairsDown()));

            // ===================================================================================
            // TERRAIN GENERATION - NEW CONCEPT
            // ===================================================================================
            // Terrain steps add secondary tile types (like water) to an already-generated map.
            // They work by modifying existing tiles, not by creating the initial room structure.

            // Define the terrain ID for water (using a constant makes the code more readable)
            // This matches WATER_TERRAIN_ID in BaseMap (ID=2)
            const int terrain = 2;

            // PerlinWaterStep uses Perlin noise to create natural-looking terrain distribution.
            // Parameters explained:
            // - RandRange(35): Target 35% of eligible tiles to become water
            // - 3: Perlin noise "order" (octaves) - higher = more detail/complexity
            // - new Tile(terrain): The tile type to place (water with ID=2)
            // - MapTerrainStencil: Controls WHERE water can be placed (see below)
            // - 1: "Softness" - smooths transitions at water edges
            //
            // The stencil (false, true, false, false) means:
            // - false: Don't place water on impassable terrain (walls)
            // - true: Allow placing water on floor tiles
            // - false: Don't place water on existing water
            // - false: Don't place water on blocked tiles
            var waterPostProc = new PerlinWaterStep<MapGenContext>(new RandRange(35), 3, new Tile(terrain), new MapTerrainStencil<MapGenContext>(false, true, false, false), 1);
            layout.GenSteps.Add(3, waterPostProc);

            // ===================================================================================
            // POST-PROCESSING CLEANUP STEPS
            // ===================================================================================
            // After placing terrain, some cleanup is often needed to fix visual artifacts.

            // DropDiagonalBlockStep fixes "diagonal wall" issues.
            // When water exists diagonally across a wall corner, it can look unnatural.
            // This step removes such wall tiles and replaces them with water for better flow.
            layout.GenSteps.Add(4, new DropDiagonalBlockStep<MapGenContext>(new Tile(terrain)));

            // EraseIsolatedStep removes terrain that got "stuck" inside walls.
            // Perlin noise doesn't understand room boundaries, so some water tiles
            // may end up completely surrounded by walls. This step erases them.
            layout.GenSteps.Add(4, new EraseIsolatedStep<MapGenContext>(new Tile(terrain)));

            // Run the generator and print
            MapGenContext context = layout.GenMap(MathUtils.Rand.NextUInt64());
            Print(context.Map, title);
        }

        /// <summary>
        /// Prints the generated map to the console with terrain visualization.
        /// </summary>
        /// <param name="map">The generated map containing tiles, stairs, and terrain.</param>
        /// <param name="title">Title to display above the map.</param>
        /// <remarks>
        /// Terrain rendering: Walls='#', Floor='.', Water='~', Stairs='&lt;' and '&gt;'.
        /// </remarks>
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
                    char tileChar;
                    Tile tile = map.Tiles[x][y];

                    // Map tile IDs to display characters
                    // Note the new WATER_TERRAIN_ID case for terrain display
                    switch (tile.ID)
                    {
                        case BaseMap.WALL_TERRAIN_ID:
                            tileChar = '#';
                            break;
                        case BaseMap.ROOM_TERRAIN_ID:
                            tileChar = '.';
                            break;
                        case BaseMap.WATER_TERRAIN_ID:
                            tileChar = '~';
                            break;
                        default:
                            tileChar = '?';
                            break;
                    }

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
