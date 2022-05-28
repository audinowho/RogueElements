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
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SpecificTilesStep<T> : GenStep<T>
        where T : class, ITiledGenContext
    {
        public SpecificTilesStep()
        {
            this.Tiles = Array.Empty<ITile[]>();
        }

        public SpecificTilesStep(ITile[][] tiles)
        {
            this.Tiles = tiles;
            this.Offset = Loc.Zero;
        }

        public SpecificTilesStep(ITile[][] tiles, Loc offset)
        {
            this.Tiles = tiles;
            this.Offset = offset;
        }

        public ITile[][] Tiles { get; set; }

        public Loc Offset { get; set; }

        public override void Apply(T map)
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
