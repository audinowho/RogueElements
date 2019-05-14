// <copyright file="SpecificTilesStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class SpecificTilesStep<T> : GenStep<T>
        where T : class, ITiledGenContext
    {
        private readonly ITile[][] tiles;
        private Loc offset;

        public SpecificTilesStep(ITile[][] tiles)
        {
            this.tiles = tiles;
            this.offset = Loc.Zero;
        }

        public SpecificTilesStep(ITile[][] tiles, Loc offset)
        {
            this.tiles = tiles;
            this.offset = offset;
        }

        public override void Apply(T map)
        {
            // initialize map array to empty
            // set default map values
            for (int xx = 0; xx < this.tiles.Length; xx++)
            {
                for (int yy = 0; yy < this.tiles[0].Length; yy++)
                    map.SetTile(new Loc(this.offset.X + xx, this.offset.Y + yy), this.tiles[xx][yy].Copy());
            }
        }
    }
}
