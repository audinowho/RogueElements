// <copyright file="SpecificTilesStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Creates a map out of specific tiles.
    /// Not very editor-friendly.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    [Serializable]
    public class SpecificTilesStep<TGenContext, TTile> : GenStep<TGenContext>
        where TGenContext : class, ITiledGenContext<TTile>
        where TTile : ITile<TTile>
    {
        public SpecificTilesStep()
        {
            this.Tiles = Array.Empty<TTile[]>();
        }

        public SpecificTilesStep(TTile[][] tiles)
        {
            this.Tiles = tiles;
            this.Offset = Loc.Zero;
        }

        public SpecificTilesStep(TTile[][] tiles, Loc offset)
        {
            this.Tiles = tiles;
            this.Offset = offset;
        }

        public TTile[][] Tiles { get; set; }

        public Loc Offset { get; set; }

        public override void Apply(TGenContext map)
        {
            // initialize map array to empty
            // set default map values
            for (int xx = 0; xx < this.Tiles.Length; xx++)
            {
                for (int yy = 0; yy < this.Tiles[0].Length; yy++)
                    map.SetTile(new Loc(this.Offset.X + xx, this.Offset.Y + yy), this.Tiles[xx][yy].Copy());
            }
        }
    }
}
