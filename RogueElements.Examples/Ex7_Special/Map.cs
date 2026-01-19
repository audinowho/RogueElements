// <copyright file="Map.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueElements;

namespace RogueElements.Examples.Ex7_Special
{
    /// <summary>
    /// Map class that stores tiles, stairs, and items for the special room example.
    /// Extends BaseMap with entity storage for demonstrating filtered spawning.
    /// </summary>
    /// <remarks>
    /// This map combines features from Ex4_Stairs (stairs) and Ex6_Items (items).
    /// See Ex7_Special.Example7 for how RoomComponents enable filtered spawning.
    /// </remarks>
    public class Map : BaseMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Map"/> class.
        /// </summary>
        public Map()
        {
            this.GenEntrances = new List<StairsUp>();
            this.GenExits = new List<StairsDown>();
            this.Items = new List<Item>();
        }

        /// <summary>
        /// Gets or sets the list of upward stairs (level entrances).
        /// </summary>
        public List<StairsUp> GenEntrances { get; set; }

        /// <summary>
        /// Gets or sets the list of downward stairs (level exits).
        /// </summary>
        public List<StairsDown> GenExits { get; set; }

        /// <summary>
        /// Gets or sets the list of items placed on the map.
        /// Items in treasure rooms are filtered separately from general items.
        /// </summary>
        public List<Item> Items { get; set; }
    }
}
