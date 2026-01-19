// <copyright file="Example8.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;
using RogueSharp;

namespace RogueElements.Examples.Ex8_Integration
{
    /// <summary>
    /// Reference implementation demonstrating RogueElements integration with RogueSharp.
    /// Shows how to use RogueElements as a MapCreationStrategy for the RogueSharp library.
    /// </summary>
    /// <remarks>
    /// This example combines concepts from earlier examples:
    /// - Grid-based generation (Ex3_Grid): InitGridPlanStep, GridPathBranch
    /// - Room types (Ex2_Rooms): RoomGenSquare, RoomGenRound
    /// - Hall types (Ex2_Rooms): RoomGenAngledHall
    /// - Drawing steps (Ex2_Rooms, Ex3_Grid): DrawGridToFloorStep, DrawFloorToTileStep
    ///
    /// The key integration point is ExampleCreationStrategy, which implements
    /// RogueSharp's IMapCreationStrategy interface using RogueElements' MapGen pipeline.
    /// </remarks>
    public static class Example8
    {
        /// <summary>
        /// Runs the RogueSharp integration example.
        /// </summary>
        public static void Run()
        {
            Console.Clear();
            const string title = "8: Implementation as a MapCreationStrategy in RogueSharp";

            // Create strategy that wraps MapGen for use with RogueSharp's Map.Create()
            ExampleCreationStrategy<Map> exampleCreation = new ExampleCreationStrategy<Map>();

            // Grid initialization (see Ex3_Grid for details)
            // 6x4 grid cells, each 9x9 tiles
            var startGen = new InitGridPlanStep<MapGenContext>(1)
            {
                CellX = 6,
                CellY = 4,
                CellWidth = 9,
                CellHeight = 9,
            };
            exampleCreation.Layout.GenSteps.Add(-4, startGen);

            // Branching path through grid (see Ex3_Grid for GridPath variants)
            var path = new GridPathBranch<MapGenContext>
            {
                RoomRatio = new RandRange(70),
                BranchRatio = new RandRange(0, 50),
            };

            // Room types (see Ex2_Rooms for RoomGen variants)
            var genericRooms = new SpawnList<RoomGen<MapGenContext>>
            {
                { new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)), 10 }, // cross
                { new RoomGenRound<MapGenContext>(new RandRange(5, 9), new RandRange(5, 9)), 10 }, // round
            };
            path.GenericRooms = genericRooms;

            // Hall types (see Ex2_Rooms for hall options)
            var genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>>
            {
                { new RoomGenAngledHall<MapGenContext>(50), 10 },
            };
            path.GenericHalls = genericHalls;

            exampleCreation.Layout.GenSteps.Add(-4, path);

            // Convert GridPlan to FloorPlan (see Ex3_Grid)
            exampleCreation.Layout.GenSteps.Add(-2, new DrawGridToFloorStep<MapGenContext>());

            // Draw FloorPlan to tiles (see Ex2_Rooms)
            exampleCreation.Layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));

            // Generate via RogueSharp's Map.Create() using our strategy
            exampleCreation.Seed = MathUtils.Rand.NextUInt64();
            Map map = Map.Create(exampleCreation);
            Print(map, title);
        }

        /// <summary>
        /// Prints the generated RogueSharp map to the console.
        /// </summary>
        /// <param name="map">The RogueSharp Map to print.</param>
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

            Console.Write(topString.ToString());

            // RogueSharp Map has built-in ToString() for rendering
            Console.Write(map.ToString());
            Console.WriteLine();
        }
    }
}
