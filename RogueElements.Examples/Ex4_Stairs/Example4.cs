// <copyright file="Example4.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;

namespace RogueElements.Examples.Ex4_Stairs
{
    /// <summary>
    /// Example 4: Entity Spawning with Stairs.
    ///
    /// This example introduces the entity spawning system, using stairs as the example.
    /// Stairs are "spawnable" entities that can be placed on valid floor tiles.
    ///
    /// Key concepts introduced:
    /// - IPlaceableGenContext&lt;T&gt;: Interface for spawning typed entities
    /// - IViewPlaceableGenContext&lt;T&gt;: Extended interface for viewing spawned entities
    /// - GetAllFreeTiles(): Finds valid spawn locations
    /// - PlaceItem(): Adds an entity at a specific location
    /// - FloorStairsStep: Built-in step for placing entrance/exit stairs
    ///
    /// Important: You must implement IPlaceableGenContext separately for EACH
    /// spawnable type. This example implements it for both StairsUp and StairsDown.
    /// </summary>
    public static class Example4
    {
        /// <summary>
        /// Runs the stair spawning example.
        /// Demonstrates how to place typed entities on the generated map.
        /// </summary>
        public static void Run()
        {
            Console.Clear();
            const string title = "4: A Map with Stairs Up and Down";
            var layout = new MapGen<MapGenContext>();

            // ============================================================
            // STEPS 1-4: Grid-based room generation (same as Ex3)
            // ============================================================
            // We use a smaller 3x2 grid for this example to focus on stairs.

            // Initialize a 3x2 grid of 9x9 cells (smaller than Ex3 for clarity)
            var startGen = new InitGridPlanStep<MapGenContext>(1)
            {
                CellX = 3,
                CellY = 2,
                CellWidth = 9,
                CellHeight = 9,
            };
            layout.GenSteps.Add(-4, startGen);

            // Create a branching room layout (see Ex3 for detailed explanation)
            var path = new GridPathBranch<MapGenContext>
            {
                RoomRatio = new RandRange(70),
                BranchRatio = new RandRange(0, 50),
            };

            var genericRooms = new SpawnList<RoomGen<MapGenContext>>
            {
                { new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)), 10 },
                { new RoomGenRound<MapGenContext>(new RandRange(5, 9), new RandRange(5, 9)), 10 },
            };
            path.GenericRooms = genericRooms;

            var genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>>
            {
                { new RoomGenAngledHall<MapGenContext>(50), 10 },
            };
            path.GenericHalls = genericHalls;

            layout.GenSteps.Add(-4, path);

            // Convert GridPlan to FloorPlan, then to tiles
            layout.GenSteps.Add(-2, new DrawGridToFloorStep<MapGenContext>());
            layout.GenSteps.Add(0, new DrawFloorToTileStep<MapGenContext>(1));

            // ============================================================
            // STEP 5: Place Stairs (NEW IN THIS EXAMPLE)
            // ============================================================
            // FloorStairsStep<TContext, TEntrance, TExit> places entrance and exit stairs.
            // It finds valid spawn locations and places the stairs entities.
            //
            // Type parameters:
            // - TContext: The map context (must implement IPlaceableGenContext for both stair types)
            // - TEntrance: The entrance stair type (StairsUp - implements IEntrance)
            // - TExit: The exit stair type (StairsDown - implements IExit)
            //
            // Constructor parameters:
            // - filterIndex: Which spawn filter to use (0 = default)
            // - entrance: Template for entrance stairs (copied when placed)
            // - exit: Template for exit stairs (copied when placed)
            //
            // The step works by:
            // 1. Calling GetAllFreeTiles() on the context to find valid locations
            // 2. Selecting appropriate locations (maximally separated)
            // 3. Calling PlaceItem() to spawn the stairs at those locations
            //
            // This requires MapGenContext to implement:
            // - IPlaceableGenContext<StairsUp>
            // - IPlaceableGenContext<StairsDown>
            layout.GenSteps.Add(2, new FloorStairsStep<MapGenContext, StairsUp, StairsDown>(0, new StairsUp(), new StairsDown()));

            // Run the generator and print
            MapGenContext context = layout.GenMap(MathUtils.Rand.NextUInt64());
            Print(context.Map, title);
        }

        /// <summary>
        /// Prints the generated map to the console, including stair markers.
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
                    Loc loc = new Loc(x, y);
                    Tile tile = map.Tiles[x][y];
                    char tileChar = tile.ID <= BaseMap.WALL_TERRAIN_ID ? '#' : tile.ID == BaseMap.ROOM_TERRAIN_ID ? '.' : '?';

                    // Check if this location has an entrance stair (StairsUp)
                    // Stairs are stored in the Map's entity lists, not in the tile data
                    foreach (StairsUp entrance in map.GenEntrances)
                    {
                        if (entrance.Loc == loc)
                        {
                            tileChar = '<';  // Traditional roguelike symbol for stairs up
                            break;
                        }
                    }

                    // Check if this location has an exit stair (StairsDown)
                    foreach (StairsDown entrance in map.GenExits)
                    {
                        if (entrance.Loc == loc)
                        {
                            tileChar = '>';  // Traditional roguelike symbol for stairs down
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
