// <copyright file="NoChokepointStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A filter for determining the eligible tiles for an operation.
    /// Locations that, if all made unwalkable, do not cause a chokepoint to be removed, are eligible.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    [Serializable]
    public class NoChokepointStencil<TGenContext, TTile> : IBlobStencil<TGenContext, TTile>
        where TGenContext : class, ITiledGenContext<TTile>
        where TTile : ITile<TTile>
    {
        public NoChokepointStencil()
        {
            this.TileStencil = new DefaultTerrainStencil<TGenContext, TTile>();
        }

        public NoChokepointStencil(ITerrainStencil<TGenContext, TTile> tileStencil)
        {
            this.TileStencil = tileStencil;
        }

        /// <summary>
        /// The stencil should return true if the tile is considered walkable. (Tile-being-choked)
        /// </summary>
        public ITerrainStencil<TGenContext, TTile> TileStencil { get; set; }

        /// <summary>
        /// Determines if the entire map should be checked for connectivity, or just the immediate surrounding tiles.
        /// If turned off, the gen will refuse to break ANY chokepoint period.
        /// If turned on, it will break any chokepoint that has another way around somewhere, while still preventing true unsolvability.
        /// </summary>
        public bool Global { get; set; }

        public bool Negate { get; set; }

        public bool Test(TGenContext map, Rect rect, Grid.LocTest blobTest)
        {
            bool IsMapValid(Loc loc) => this.TileStencil.Test(map, loc);
            bool IsBlobValid(Loc loc) => blobTest(loc + rect.Start);

            Rect checkArea;
            if (this.Global)
            {
                checkArea = new Rect(0, 0, map.Width, map.Height);
            }
            else
            {
                checkArea = rect;
                checkArea.Inflate(1, 1);
                if (!map.Wrap)
                    checkArea = Rect.Intersect(checkArea, new Rect(0, 0, map.Width, map.Height));
            }

            return this.Negate == Detection.DetectDisconnect(checkArea, IsMapValid, rect.Start, rect.Size, IsBlobValid, true);
        }

        public override string ToString()
        {
            if (this.TileStencil == null)
                return string.Format("Blob Chokepoint: [EMPTY]");
            return string.Format("{0}{1}Blob Chokepoint of {2}", this.Negate ? "No " : string.Empty, this.Global ? "Global " : string.Empty, this.TileStencil.ToString());
        }
    }
}
