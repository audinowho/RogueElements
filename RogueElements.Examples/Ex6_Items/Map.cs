// <copyright file="Map.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueElements;

namespace RogueElements.Examples.Ex6_Items
{
    public class Map : BaseMap
    {
        public Map()
        {
            this.GenEntrances = new List<StairsUp>();
            this.GenExits = new List<StairsDown>();
            this.Items = new List<Item>();
            this.Mobs = new List<Mob>();
        }

        public List<StairsUp> GenEntrances { get; set; }

        public List<StairsDown> GenExits { get; set; }

        public List<Item> Items { get; set; }

        public List<Mob> Mobs { get; set; }
    }
}
