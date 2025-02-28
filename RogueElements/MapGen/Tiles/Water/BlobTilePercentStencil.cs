﻿// <copyright file="BlobTilePercentStencil.cs" company="Audino">
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
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class BlobTilePercentStencil<T> : IBlobStencil<T>
        where T : class, ITiledGenContext
    {
        public BlobTilePercentStencil()
        {
            this.TileStencil = new DefaultTerrainStencil<T>();
        }

        public BlobTilePercentStencil(int percent, ITerrainStencil<T> tileStencil)
        {
            this.Percent = percent;
            this.TileStencil = tileStencil;
        }

        public ITerrainStencil<T> TileStencil { get; set; }

        public int Percent { get; set; }

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
