// <copyright file="Example2.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;

namespace RogueElements.Examples.Ex2_Rooms
{
    /// <summary>
    /// Example 2: Introduction to FloorPlan-based Room Generation.
    ///
    /// This example introduces the "freeform" approach to procedural room generation
    /// using FloorPlan. Unlike the static tiles in Example 1, rooms are placed
    /// dynamically based on configurable parameters.
    ///
    /// Key Concepts Introduced:
    /// - FloorPlan: An abstract representation of rooms and halls before tile conversion
    /// - RoomGen: Classes that define room shapes (square, round, cave, etc.)
    /// - FloorPathBranch: A path generator that creates branching dungeon layouts
    /// - SpawnList: Weighted random selection for room/hall types
    /// - DrawFloorToTileStep: Converts FloorPlan to actual tiles
    ///
    /// Two-Phase Generation:
    /// 1. PLANNING: Create abstract room/hall layout in FloorPlan (no tiles yet)
    /// 2. DRAWING: Convert FloorPlan to actual tiles
    ///
    /// This separation allows:
    /// - Complex room placement algorithms without worrying about tiles
    /// - Multiple drawing strategies from the same plan
    /// - Validation of room connectivity before committing to tiles
    /// </summary>
    public static class Example2
    {
        /// <summary>
        /// Runs Example 2, demonstrating FloorPlan-based procedural room generation.
        ///
        /// Generation Pipeline:
        /// Priority -2: InitFloorPlanStep - Create empty FloorPlan
        /// Priority -1: FloorPathBranch - Place rooms and halls
        /// Priority  0: DrawFloorToTileStep - Convert to tiles
        ///
        /// The negative priorities ensure planning happens before drawing.
        /// </summary>
        public static void Run()
        {
            Console.Clear();
            const string title = "2: A Map Made with Rooms and Halls";

            // ============================================================
            // STEP 1: Create the MapGen orchestrator
            // ============================================================
            // Same as Example 1, but our context now implements IFloorPlanGenContext
            // in addition to ITiledGenContext.
            var layout = new MapGen<MapGenContext>();

            // ============================================================
            // STEP 2: Initialize the FloorPlan
            // ============================================================
            // InitFloorPlanStep creates an empty FloorPlan with the specified dimensions.
            // The FloorPlan is a planning structure that holds abstract room and hall
            // information - no actual tiles are created yet.
            //
            // Think of FloorPlan as a blueprint: it describes WHERE rooms go and
            // HOW they connect, but doesn't fill in the actual tile data.
            //
            // Priority -2 ensures this runs first (before room placement at -1).
            InitFloorPlanStep<MapGenContext> startGen = new InitFloorPlanStep<MapGenContext>(54, 40);
            layout.GenSteps.Add(-2, startGen);

            // ============================================================
            // STEP 3: Define room types with weighted probabilities
            // ============================================================
            // SpawnList<T> is a weighted random selection container.
            // Each entry has:
            // - The item (a RoomGen that defines a room shape)
            // - A weight (higher = more likely to be chosen)
            //
            // RoomGen<T> is the base class for room shape generators:
            // - RoomGenSquare: Rectangular rooms (most common)
            // - RoomGenRound: Circular/oval rooms
            // - RoomGenCave: Organic cave-like shapes (see Ex3)
            // - RoomGenCross: Cross-shaped rooms
            // - And many more...
            //
            // RandRange specifies a random range: RandRange(4, 8) means 4-7 inclusive.
            var genericRooms = new SpawnList<RoomGen<MapGenContext>>
            {
                // Square rooms: 4-7 tiles wide, 4-7 tiles tall, weight 10
                { new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)), 10 },

                // Round rooms: 5-8 tiles wide, 5-8 tiles tall, weight 10
                // Equal weights mean equal probability of selection.
                { new RoomGenRound<MapGenContext>(new RandRange(5, 9), new RandRange(5, 9)), 10 },
            };

            // ============================================================
            // STEP 4: Define hall types with weighted probabilities
            // ============================================================
            // Halls connect rooms together. They use PermissiveRoomGen<T> which
            // is a RoomGen that can be stretched to fit between connection points.
            //
            // Hall types shown:
            // - RoomGenAngledHall: L-shaped or straight halls with variable sizes
            // - RoomGenSquare: Simple 1x1 halls (just a doorway connection)
            var genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>>
            {
                // Angled halls: Can bend around obstacles
                // Parameters: turnBias (0 = no preference), width range, height range
                { new RoomGenAngledHall<MapGenContext>(0, new RandRange(3, 7), new RandRange(3, 7)), 10 },

                // 1x1 "halls" - essentially direct doorway connections between rooms
                // Weight 20 makes these twice as likely as angled halls.
                { new RoomGenSquare<MapGenContext>(new RandRange(1), new RandRange(1)), 20 },
            };

            // ============================================================
            // STEP 5: Create the room placement path
            // ============================================================
            // FloorPathBranch is a path generator that creates a branching tree of rooms.
            // It's one of several path generators available:
            // - FloorPathBranch: Branching tree (most natural for dungeons)
            // - FloorPathStartStepGeneric: Linear path from start to end
            // - FloorPathGridGeneric: Grid-based placement (see Ex3)
            //
            // The path generator determines the overall dungeon structure:
            // - How rooms connect to each other
            // - The branching pattern
            // - Dead ends vs loops
            FloorPathBranch<MapGenContext> path = new FloorPathBranch<MapGenContext>(genericRooms, genericHalls)
            {
                // HallPercent: Chance (0-100) that a hall room is placed between rooms.
                // 50 = 50% chance of halls, creating a mix of direct and hallway connections.
                HallPercent = 50,

                // FillPercent: Target percentage of the map area to fill with rooms.
                // RandRange(45) = exactly 45%. Higher values = more rooms, denser maps.
                FillPercent = new RandRange(45),

                // BranchRatio: Chance (0-100) to branch off the main path.
                // RandRange(0, 25) = 0-24% chance. Higher = more dead ends and side branches.
                BranchRatio = new RandRange(0, 25),
            };

            // Priority -1 ensures room placement happens after FloorPlan initialization (-2)
            // but before tile drawing (0).
            layout.GenSteps.Add(-1, path);

            // ============================================================
            // STEP 6: Convert FloorPlan to tiles
            // ============================================================
            // DrawFloorToTileStep is the bridge between planning and tile data.
            // It iterates through all rooms and halls in the FloorPlan and
            // "draws" them onto the tile grid.
            //
            // Parameter: padding (1 = one tile of wall between rooms and map edge)
            // Padding prevents rooms from touching the map boundary.
            //
            // This step requires both:
            // - IFloorPlanGenContext (to read the FloorPlan)
            // - ITiledGenContext (to write the tiles)
            layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));

            // ============================================================
            // STEP 7: Run the generation pipeline
            // ============================================================
            // Execution order:
            // 1. Priority -2: InitFloorPlanStep creates empty FloorPlan
            // 2. Priority -1: FloorPathBranch places rooms and halls
            // 3. Priority  0: DrawFloorToTileStep converts plan to tiles
            //
            // The seed ensures reproducibility - same seed = same map.
            MapGenContext context = layout.GenMap(MathUtils.Rand.NextUInt64());

            // The context now contains:
            // - context.RoomPlan: The FloorPlan with room/hall layout
            // - context.Map: The tile data ready for gameplay
            Print(context.Map, title);
        }

        /// <summary>
        /// Prints the generated map to the console using ASCII characters.
        /// </summary>
        /// <param name="map">The generated map containing the tile data.</param>
        /// <param name="title">The title to display above the map.</param>
        /// <remarks>
        /// Note how the output looks similar to Example 1, but the layout
        /// was generated procedurally rather than hand-designed.
        ///
        /// The FloorPlan abstraction means we get varied, interesting layouts
        /// every time we run with a different seed, while maintaining
        /// connectivity guarantees (all rooms are reachable).
        /// </remarks>
        public static void Print(Map map, string title)
        {
            var topString = new StringBuilder(string.Empty);
            string turnString = title;
            topString.Append($"{turnString,-82}");
            topString.Append('\n');

            // Draw separator
            for (int i = 0; i < map.Width + 1; i++)
                topString.Append("=");
            topString.Append('\n');

            // Convert tiles to ASCII
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    char tileChar;
                    Tile tile = map.Tiles[x][y];

                    // Use BaseMap constants for consistency across examples
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
