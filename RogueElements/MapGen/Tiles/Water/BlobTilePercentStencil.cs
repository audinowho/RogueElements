// <copyright file="BlobTilePercentStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A filter for determining the eligible tiles for an operation.
    /// Checking the bounds is checking each individual tile.
    /// The amount of tiles in the blob placement that pass the stencil test must be over the specified percent.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    [Serializable]
    public class BlobTilePercentStencil<TGenContext, TTile> : IBlobStencil<TGenContext, TTile>
        where TGenContext : class, ITiledGenContext<TTile>
        where TTile : ITile<TTile>
    {
        public BlobTilePercentStencil()
        {
            this.TileStencil = new DefaultTerrainStencil<TGenContext, TTile>();
        }

        public BlobTilePercentStencil(int percent, ITerrainStencil<TGenContext, TTile> tileStencil)
        {
            this.Percent = percent;
            this.TileStencil = tileStencil;
        }

        public ITerrainStencil<TGenContext, TTile> TileStencil { get; set; }

        public int Percent { get; set; }

        public bool Test(TGenContext map, Rect rect, Grid.LocTest blobTest)
        {
            int amount = 0;
            for (int xx = rect.X; xx < rect.End.X; xx++)
            {
                for (int yy = rect.Y; yy < rect.End.Y; yy++)
                {
                    Loc testLoc = new Loc(xx, yy);
                    if (blobTest(testLoc) && this.TileStencil.Test(map, testLoc))
                        amount++;
                }
            }

            return amount * 100 > rect.Area * this.Percent;
        }

        public override string ToString()
        {
            if (this.TileStencil == null)
                return string.Format("Blob Tiles Percent: [EMPTY]");
            return string.Format("Blob Tiles {0}%: {1}", this.Percent, this.TileStencil.ToString());
        }
    }
}
