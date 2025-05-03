// <copyright file="Tile.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements.Examples
{
    public sealed class Tile : ITile<Tile>
    {
        public Tile()
        {
            this.ID = BaseMap.WALL_TERRAIN_ID;
        }

        public Tile(int id)
        {
            this.ID = id;
        }

        protected Tile(Tile other)
        {
            this.ID = other.ID;
        }

        public int ID { get; set; }

        public Tile Copy() => new Tile(this);

        public bool TileEquivalent(Tile other)
        {
            return this.ID == other.ID;
        }

        ITile ITile.Copy() => this.Copy();

        bool ITile.TileEquivalent(ITile other) => other is Tile t && this.TileEquivalent(t);
    }
}
