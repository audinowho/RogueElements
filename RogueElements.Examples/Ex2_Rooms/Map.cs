// <copyright file="Map.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueElements;

namespace RogueElements.Examples.Ex2_Rooms
{
    /// <summary>
    /// The map data structure for Example 2 (Rooms).
    ///
    /// This Map class is identical to Example 1's Map. The procedural room generation
    /// doesn't require any changes to the Map itself - all the new functionality
    /// is in MapGenContext (which implements IFloorPlanGenContext).
    ///
    /// Key Insight: The Map class represents your game's runtime map data.
    /// It doesn't need to know HOW the map was generated (static tiles vs rooms),
    /// only WHAT the final result is (the tile array).
    ///
    /// This separation means:
    /// - Your game code works with Map, not generation internals
    /// - You can swap generation strategies without changing game code
    /// - The Map format is stable even as generation evolves
    /// </summary>
    /// <remarks>
    /// In a real game, your Map class would likely contain additional data:
    ///
    /// Runtime Data:
    /// - List of spawned monsters/NPCs
    /// - List of items on the ground
    /// - Player spawn point
    /// - Exit/stairs locations
    /// - Fog of war / visibility state
    ///
    /// Metadata:
    /// - Dungeon floor number
    /// - Difficulty level
    /// - Theme/tileset to use for rendering
    /// - Music track to play
    ///
    /// The BaseMap class provides the core tile storage that all examples share.
    /// </remarks>
    public class Map : BaseMap
    {
        // Inherits from BaseMap without additions (same as Example 1).
        //
        // The interesting changes in Example 2 are:
        // 1. MapGenContext now implements IFloorPlanGenContext
        // 2. Example2.cs uses FloorPlan-based generation steps
        //
        // The Map output format remains the same - just tiles.
        // This demonstrates how RogueElements separates the generation
        // process from the output format.
        //
        // From BaseMap:
        // - Tile[][] Tiles: The 2D tile array
        // - int Width, Height: Map dimensions
        // - ReRandom Rand: Random number generator
        // - InitializeTiles(w, h): Create tile array
        // - WALL_TERRAIN_ID, ROOM_TERRAIN_ID, WATER_TERRAIN_ID: Tile type constants
    }
}
