// <copyright file="NoChokepointTerrainStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Provides a terrain stencil that prevents tile placement from creating chokepoints.
    /// </summary>
    /// <typeparam name="T">The type of map context that implements <see cref="ITiledGenContext"/>.</typeparam>
    /// <remarks>
    /// This stencil tests whether placing a tile at a location would disconnect walkable areas
    /// or create impassable barriers.
    /// </remarks>
    [Serializable]
    public class NoChokepointTerrainStencil<T> : ITerrainStencil<T>
        where T : class, ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoChokepointTerrainStencil{T}"/> class.
        /// </summary>
        public NoChokepointTerrainStencil()
        {
            this.TileStencil = new DefaultTerrainStencil<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoChokepointTerrainStencil{T}"/> class with the specified terrain stencil.
        /// </summary>
        /// <param name="tileStencil">The terrain stencil that defines walkable tiles.</param>
        public NoChokepointTerrainStencil(ITerrainStencil<T> tileStencil)
        {
            this.TileStencil = tileStencil;
        }

        /// <summary>
        /// Gets or sets the terrain stencil that determines which tiles are considered valid path tiles.
        /// </summary>
        public ITerrainStencil<T> TileStencil { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to check the entire map for connectivity.
        /// When <c>false</c>, only checks immediate surrounding tiles.
        /// When <c>true</c>, checks the entire map for disconnections.
        /// </summary>
        public bool Global { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to invert the chokepoint detection result.
        /// </summary>
        public bool Negate { get; set; }

        /// <inheritdoc/>
        public bool Test(T map, Loc testLoc)
        {
            bool IsMapValid(Loc loc) => this.TileStencil.Test(map, loc);
            bool IsBlobValid(Loc loc) => true;

            Rect checkArea;
            if (this.Global)
            {
                checkArea = new Rect(0, 0, map.Width, map.Height);
            }
            else
            {
                checkArea = new Rect(testLoc, Loc.One);
                checkArea.Inflate(1, 1);
                if (!map.Wrap)
                    checkArea = Rect.Intersect(checkArea, new Rect(0, 0, map.Width, map.Height));
            }

            return this.Negate == Detection.DetectDisconnect(checkArea, IsMapValid, testLoc, Loc.One, IsBlobValid, true);
        }

        public override string ToString()
        {
            if (this.TileStencil == null)
                return string.Format("Chokepoint: [EMPTY]");
            return string.Format("{0}{1}Chokepoint of {2}", this.Negate ? "No " : string.Empty, this.Global ? "Global " : string.Empty, this.TileStencil.ToString());
        }
    }
}
