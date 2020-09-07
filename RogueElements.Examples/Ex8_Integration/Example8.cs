// <copyright file="Example8.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;
using RogueSharp;

namespace RogueElements.Examples.Ex8_Integration
{
    public static class Example8
    {
        public static void Run()
        {
            Console.Clear();
            const string title = "8: Implementation as a MapCreationStrategy in RogueSharp";
            ExampleCreationStrategy<Map> exampleCreation = new ExampleCreationStrategy<Map>();

            // Initialize a 6x4 grid of 10x10 cells.
            var startGen = new InitGridPlanStep<MapGenContext>(1)
            {
                CellX = 6,
                CellY = 4,
                CellWidth = 9,
                CellHeight = 9,
            };
            exampleCreation.Layout.GenSteps.Add(-4, startGen);

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

            exampleCreation.Layout.GenSteps.Add(-4, path);

            // Output the rooms into a FloorPlan
            exampleCreation.Layout.GenSteps.Add(-2, new DrawGridToFloorStep<MapGenContext>());

            // Draw the rooms of the FloorPlan onto the tiled map, with 1 TILE padded on each side
            exampleCreation.Layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));

            // Run the generator and print
            exampleCreation.Seed = MathUtils.Rand.NextUInt64();
            Map map = Map.Create(exampleCreation);
            Print(map, title);
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

            Console.Write(topString.ToString());
            Console.Write(map.ToString());
            Console.WriteLine();
        }
    }
}
