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
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class BlobTileStencil<T> : IBlobStencil<T>
        where T : class, ITiledGenContext
    {
        public BlobTileStencil()
        {
            this.TileStencil = new DefaultTerrainStencil<T>();
        }

        public BlobTileStencil(ITerrainStencil<T> tileStencil)
        {
            this.TileStencil = tileStencil;
        }

        public ITerrainStencil<T> TileStencil { get; set; }

        public bool RequireAny { get; set; }

        public bool Test(T map, Rect rect)
        {
            for (int xx = rect.X; xx < rect.End.X; xx++)
            {
                for (int yy = rect.Y; yy < rect.End.Y; yy++)
                {
                    if (this.RequireAny)
                    {
                        if (this.TileStencil.Test(map, new Loc(xx, yy)))
                            return true;
                    }
                    else
                    {
                        if (!this.TileStencil.Test(map, new Loc(xx, yy)))
                            return false;
                    }
                }
            }

            return !this.RequireAny;
        }
    }
}
