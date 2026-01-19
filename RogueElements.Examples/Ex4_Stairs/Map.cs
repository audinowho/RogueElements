// <copyright file="Map.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements.Examples.Ex4_Stairs
{
    /// <summary>
    /// Map class that stores spawned stair entities.
    ///
    /// This map extends BaseMap with collections for entrance and exit stairs.
    /// Spawned entities are stored separately from tiles because:
    /// 1. Entities have additional properties beyond just tile type (e.g., Loc)
    /// 2. Multiple entity types can occupy the same conceptual "layer"
    /// 3. Entity data may need to be serialized/saved differently than tiles
    ///
    /// The Map is the final output of generation. It contains:
    /// - Tiles (inherited from BaseMap)
    /// - GenEntrances (StairsUp entities)
    /// - GenExits (StairsDown entities)
    ///
    /// Note: The naming convention "Gen" prefix indicates these collections
    /// are populated during generation. Your game might have separate
    /// runtime collections for entities that move or change.
    /// </summary>
    public class Map : BaseMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Map"/> class.
        /// Creates empty collections for entrance and exit stairs.
        /// </summary>
        public Map()
        {
            this.GenEntrances = new List<StairsUp>();
            this.GenExits = new List<StairsDown>();
        }

        /// <summary>
        /// Gets or sets the list of entrance stairs (StairsUp) on this map.
        /// These are typically where the player enters the level.
        /// StairsUp implements IEntrance, marking it as an entry point.
        /// </summary>
        /// <remarks>
        /// In a typical roguelike:
        /// - StairsUp leads to the previous floor (going "up" toward the surface)
        /// - Multiple entrances are possible for multi-entrance dungeons
        /// - The first entrance is usually the player's spawn point
        /// </remarks>
        public List<StairsUp> GenEntrances { get; set; }

        /// <summary>
        /// Gets or sets the list of exit stairs (StairsDown) on this map.
        /// These are typically where the player exits to the next level.
        /// StairsDown implements IExit, marking it as an exit point.
        /// </summary>
        /// <remarks>
        /// In a typical roguelike:
        /// - StairsDown leads to the next floor (going "down" deeper into the dungeon)
        /// - Multiple exits allow for branching dungeon paths
        /// - Some games place the exit far from the entrance for exploration
        /// </remarks>
        public List<StairsDown> GenExits { get; set; }
    }
}
