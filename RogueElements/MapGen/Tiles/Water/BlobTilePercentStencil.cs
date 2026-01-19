// <copyright file="BlobTilePercentStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Provides a blob stencil that requires a minimum percentage of tiles to pass a terrain stencil.
    /// </summary>
    /// <typeparam name="T">The type of map context that implements <see cref="ITiledGenContext"/>.</typeparam>
    [Serializable]
    public class BlobTilePercentStencil<T> : IBlobStencil<T>
        where T : class, ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlobTilePercentStencil{T}"/> class.
        /// </summary>
        public BlobTilePercentStencil()
        {
            this.TileStencil = new DefaultTerrainStencil<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobTilePercentStencil{T}"/> class with the specified percent and stencil.
        /// </summary>
        /// <param name="percent">The minimum percentage of tiles that must pass the stencil.</param>
        /// <param name="tileStencil">The terrain stencil to apply to each tile.</param>
        public BlobTilePercentStencil(int percent, ITerrainStencil<T> tileStencil)
        {
            this.Percent = percent;
            this.TileStencil = tileStencil;
        }

        /// <summary>
        /// Gets or sets the terrain stencil to apply to each tile in the blob.
        /// </summary>
        public ITerrainStencil<T> TileStencil { get; set; }

        /// <summary>
        /// Gets or sets the minimum percentage of tiles that must pass the stencil for the blob to be eligible.
        /// </summary>
        public int Percent { get; set; }

        /// <inheritdoc/>
        public bool Test(T map, Rect rect, Grid.LocTest blobTest)
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
