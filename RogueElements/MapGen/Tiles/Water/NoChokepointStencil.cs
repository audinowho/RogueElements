// <copyright file="NoChokepointStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Provides a blob stencil that prevents blob placement from creating chokepoints.
    /// </summary>
    /// <typeparam name="T">The type of map context that implements <see cref="ITiledGenContext"/>.</typeparam>
    /// <remarks>
    /// This stencil tests whether placing the blob would disconnect walkable areas or create
    /// impassable barriers.
    /// </remarks>
    [Serializable]
    public class NoChokepointStencil<T> : IBlobStencil<T>
        where T : class, ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoChokepointStencil{T}"/> class.
        /// </summary>
        public NoChokepointStencil()
        {
            this.TileStencil = new DefaultTerrainStencil<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoChokepointStencil{T}"/> class with the specified terrain stencil.
        /// </summary>
        /// <param name="tileStencil">The terrain stencil that defines walkable tiles.</param>
        public NoChokepointStencil(ITerrainStencil<T> tileStencil)
        {
            this.TileStencil = tileStencil;
        }

        /// <summary>
        /// Gets or sets the terrain stencil that determines which tiles are considered walkable.
        /// </summary>
        public ITerrainStencil<T> TileStencil { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to check the entire map for connectivity.
        /// When <c>false</c>, refuses to break any chokepoint.
        /// When <c>true</c>, allows breaking chokepoints that have alternate paths, preventing only true disconnections.
        /// </summary>
        public bool Global { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to invert the chokepoint detection result.
        /// </summary>
        public bool Negate { get; set; }

        /// <inheritdoc/>
        public bool Test(T map, Rect rect, Grid.LocTest blobTest)
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
