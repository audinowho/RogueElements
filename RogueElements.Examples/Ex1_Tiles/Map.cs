// <copyright file="Map.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueElements;

namespace RogueElements.Examples.Ex1_Tiles
{
    /// <summary>
    /// The map data structure for Example 1 (Tiles).
    ///
    /// This class represents the final output of map generation - the actual game map
    /// that will be used at runtime. It holds the tile data and any other game-specific
    /// information your map needs.
    ///
    /// Key Concepts:
    /// - Map is separate from MapGenContext (context is for generation, map is the result)
    /// - The context holds a Map instance and populates it during generation
    /// - After generation, you extract the Map from the context for use in your game
    ///
    /// Separation of Concerns:
    /// - MapGenContext: Implements RogueElements interfaces, manages generation state
    /// - Map: Your game's map class, holds runtime data (tiles, entities, etc.)
    ///
    /// This separation allows RogueElements to work with any game's map format.
    /// </summary>
    /// <remarks>
    /// BaseMap provides the common functionality:
    /// - Tiles[x][y]: 2D array of Tile objects (column-major order)
    /// - Width/Height: Dimensions derived from Tiles array
    /// - Rand: Random number generator (ReRandom) for reproducible generation
    /// - InitializeTiles(): Creates and initializes the tile array
    /// - Terrain ID constants: WALL_TERRAIN_ID (0), ROOM_TERRAIN_ID (1), WATER_TERRAIN_ID (2)
    ///
    /// In a real game, you would add:
    /// - Entity lists (monsters, items, NPCs)
    /// - Spawn points
    /// - Lighting/visibility data
    /// - Any other map metadata
    /// </remarks>
    public class Map : BaseMap
    {
        // This example uses BaseMap as-is with no additions.
        // BaseMap provides everything needed for basic tile-based maps:
        //
        // From BaseMap:
        // - public Tile[][] Tiles { get; set; }    // The tile data
        // - public int Width => Tiles.Length       // Map width
        // - public int Height => Tiles[0].Length   // Map height
        // - public ReRandom Rand { get; set; }     // RNG for generation
        // - public void InitializeTiles(w, h)      // Create tile array
        //
        // Constants for tile IDs:
        // - WALL_TERRAIN_ID = 0   // Impassable wall
        // - ROOM_TERRAIN_ID = 1   // Passable floor
        // - WATER_TERRAIN_ID = 2  // Water terrain (see Ex5)
        //
        // Later examples will add more data to their Map classes,
        // such as item spawn locations (Ex6) and special rooms (Ex7).
    }
}
