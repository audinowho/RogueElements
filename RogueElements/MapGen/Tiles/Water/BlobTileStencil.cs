// <copyright file="BlobTileStencil.cs" company="Audino">
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
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    [Serializable]
    public class BlobTileStencil<TGenContext, TTile> : IBlobStencil<TGenContext, TTile>
        where TGenContext : class, ITiledGenContext<TTile>
        where TTile : ITile<TTile>
    {
        public BlobTileStencil()
        {
            this.TileStencil = new DefaultTerrainStencil<TGenContext, TTile>();
        }

        public BlobTileStencil(ITerrainStencil<TGenContext, TTile> tileStencil)
        {
            this.TileStencil = tileStencil;
        }

        public BlobTileStencil(ITerrainStencil<TGenContext, TTile> tileStencil, bool requireAny)
        {
            this.TileStencil = tileStencil;
            this.RequireAny = requireAny;
        }

        public ITerrainStencil<TGenContext, TTile> TileStencil { get; set; }

        public bool RequireAny { get; set; }

        public bool Test(TGenContext map, Rect rect, Grid.LocTest blobTest)
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
