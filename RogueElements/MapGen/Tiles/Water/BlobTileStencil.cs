// <copyright file="BlobTileStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Provides a blob stencil that applies a terrain stencil to each individual tile in the blob.
    /// </summary>
    /// <typeparam name="T">The type of map context that implements <see cref="ITiledGenContext"/>.</typeparam>
    [Serializable]
    public class BlobTileStencil<T> : IBlobStencil<T>
        where T : class, ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlobTileStencil{T}"/> class.
        /// </summary>
        public BlobTileStencil()
        {
            this.TileStencil = new DefaultTerrainStencil<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobTileStencil{T}"/> class with the specified terrain stencil.
        /// </summary>
        /// <param name="tileStencil">The terrain stencil to apply to each tile.</param>
        public BlobTileStencil(ITerrainStencil<T> tileStencil)
        {
            this.TileStencil = tileStencil;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobTileStencil{T}"/> class with the specified terrain stencil and logic mode.
        /// </summary>
        /// <param name="tileStencil">The terrain stencil to apply to each tile.</param>
        /// <param name="requireAny">Whether any single tile passing is sufficient (OR logic), or all must pass (AND logic).</param>
        public BlobTileStencil(ITerrainStencil<T> tileStencil, bool requireAny)
        {
            this.TileStencil = tileStencil;
            this.RequireAny = requireAny;
        }

        /// <summary>
        /// Gets or sets the terrain stencil to apply to each tile in the blob.
        /// </summary>
        public ITerrainStencil<T> TileStencil { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether any single tile passing is sufficient.
        /// When <c>true</c>, uses OR logic; when <c>false</c>, uses AND logic requiring all tiles to pass.
        /// </summary>
        public bool RequireAny { get; set; }

        /// <inheritdoc/>
        public bool Test(T map, Rect rect, Grid.LocTest blobTest)
        {
            for (int xx = rect.X; xx < rect.End.X; xx++)
            {
                for (int yy = rect.Y; yy < rect.End.Y; yy++)
                {
                    Loc testLoc = new Loc(xx, yy);
                    if (blobTest(testLoc))
                    {
                        if (this.RequireAny)
                        {
                            if (this.TileStencil.Test(map, testLoc))
                                return true;
                        }
                        else
                        {
                            if (!this.TileStencil.Test(map, testLoc))
                                return false;
                        }
                    }
                }
            }

            return !this.RequireAny;
        }

        public override string ToString()
        {
            if (this.TileStencil == null)
                return string.Format("Blob Tiles: [EMPTY]");
            return string.Format("Blob Tiles{0}: {1}", this.RequireAny ? " (Any)" : string.Empty, this.TileStencil.ToString());
        }
    }
}
