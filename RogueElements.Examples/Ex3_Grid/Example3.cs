// <copyright file="Example3.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;

namespace RogueElements.Examples.Ex3_Grid
{
    /// <summary>
    /// Example 3: Grid-Based Room Generation.
    ///
    /// This example introduces the GridPlan system, which arranges rooms in a regular grid
    /// structure. This is a higher-level abstraction than FloorPlan (Ex2), providing more
    /// control over room distribution and connectivity.
    ///
    /// Key concepts introduced:
    /// - GridPlan: A grid of cells where each cell can contain a room
    /// - IRoomGridGenContext: Interface that provides GridPlan access
    /// - InitGridPlanStep: Creates the initial grid structure
    /// - GridPathBranch: Creates branching room layouts within the grid
    /// - DrawGridToFloorStep: Converts GridPlan to FloorPlan
    ///
    /// The generation pipeline flows: GridPlan -> FloorPlan -> Tiles
    /// This layered approach allows flexible room placement at multiple abstraction levels.
    /// </summary>
    public static class Example3
    {
        /// <summary>
        /// Runs the grid-based room generation example.
        /// Demonstrates how to use GridPlan to create structured room layouts.
        /// </summary>
        public static void Run()
        {
            Console.Clear();
            const string title = "3: A Map made with Rooms and Halls arranged in a grid.";

            var layout = new MapGen<MapGenContext>();

            // ============================================================
            // STEP 1: Initialize the GridPlan
            // ============================================================
            // InitGridPlanStep creates a grid structure that will hold our rooms.
            // Unlike FloorPlan which places rooms at exact pixel coordinates,
            // GridPlan divides the map into a regular grid of cells.
            //
            // GridPlan Properties:
            // - CellX/CellY: Number of grid cells horizontally/vertically (6x4 = 24 potential rooms)
            // - CellWidth/CellHeight: Size of each cell in tiles (9x9 tiles per cell)
            //
            // The constructor parameter (1) is the default wall thickness between cells.
            // Total map size = (CellX * CellWidth) + borders, (CellY * CellHeight) + borders
            var startGen = new InitGridPlanStep<MapGenContext>(1)
            {
                CellX = 6,      // 6 columns of cells
                CellY = 4,      // 4 rows of cells
                CellWidth = 9,  // Each cell is 9 tiles wide
                CellHeight = 9, // Each cell is 9 tiles tall
            };
            layout.GenSteps.Add(-4, startGen);

            // ============================================================
            // STEP 2: Create the Room Layout Path
            // ============================================================
            // GridPathBranch creates a branching tree of rooms within the grid.
            // It starts from a random cell and grows outward, creating branches.
            //
            // GridPathBranch Properties:
            // - RoomRatio: Percentage of grid cells that will contain rooms (70% = ~17 of 24 cells)
            // - BranchRatio: How much the path branches (0-50% chance per expansion to branch)
            //
            // Other path types available:
            // - GridPathTwoSides: Rooms on opposite sides connected
            // - GridPathCircle: Rooms arranged in a ring
            // - GridPathGrid: Full grid connectivity
            var path = new GridPathBranch<MapGenContext>
            {
                RoomRatio = new RandRange(70),      // Fill 70% of cells with rooms
                BranchRatio = new RandRange(0, 50), // Random branching factor 0-50%
            };

            // Define which room generators can be used in grid cells.
            // SpawnList<T> is a weighted random selection list.
            // Each entry has a generator and a weight (10 = equal probability).
            var genericRooms = new SpawnList<RoomGen<MapGenContext>>
            {
                // RoomGenSquare: Rectangular rooms with random dimensions
                // RandRange(4, 8) means width/height between 4-7 tiles
                { new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)), 10 },

                // RoomGenRound: Elliptical/circular rooms
                // RandRange(5, 9) means diameter between 5-8 tiles
                { new RoomGenRound<MapGenContext>(new RandRange(5, 9), new RandRange(5, 9)), 10 },
            };
            path.GenericRooms = genericRooms;

            // Define hall generators for connections between rooms.
            // PermissiveRoomGen is a base class for halls that can connect any two rooms.
            // RoomGenAngledHall creates L-shaped or straight halls.
            // The parameter (50) is the chance of creating an angled (L-shaped) hall vs straight.
            var genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>> { { new RoomGenAngledHall<MapGenContext>(50), 10 } };
            path.GenericHalls = genericHalls;

            // Add the path generator at the same priority as grid initialization.
            // Steps at the same priority run in the order they were added.
            layout.GenSteps.Add(-4, path);

            // ============================================================
            // STEP 3: Convert GridPlan to FloorPlan
            // ============================================================
            // DrawGridToFloorStep transforms the abstract grid layout into
            // concrete room placements in a FloorPlan.
            //
            // This is where grid cells become actual room rectangles with
            // exact positions and dimensions. The FloorPlan then handles
            // the detailed room boundaries and hall connections.
            //
            // Pipeline so far: GridPlan (grid cells) -> FloorPlan (room bounds)
            layout.GenSteps.Add(-2, new DrawGridToFloorStep<MapGenContext>());

            // ============================================================
            // STEP 4: Render FloorPlan to Tiles
            // ============================================================
            // DrawFloorToTileStep converts the FloorPlan's room definitions
            // into actual tile data in the map.
            //
            // The parameter (1) specifies padding - how many tiles of wall
            // to maintain around each room and hall. This creates visual
            // separation between adjacent rooms.
            //
            // Final pipeline: GridPlan -> FloorPlan -> Tiles (the actual map!)
            layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));

            // Run the generator and print
            MapGenContext context = layout.GenMap(MathUtils.Rand.NextUInt64());
            Print(context.Map, title);
        }

        /// <summary>
        /// Prints the generated map to the console.
        /// </summary>
        /// <param name="map">The map to print.</param>
        /// <param name="title">Title to display above the map.</param>
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
