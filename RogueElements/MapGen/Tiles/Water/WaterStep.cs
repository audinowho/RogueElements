// <copyright file="WaterStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Provides the base class for water generation steps that place terrain on the map.
    /// </summary>
    /// <typeparam name="T">The type of map context that implements <see cref="ITiledGenContext"/>.</typeparam>
    [Serializable]
    public abstract class WaterStep<T> : GenStep<T>, IWaterStep
        where T : class, ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaterStep{T}"/> class.
        /// </summary>
        protected WaterStep()
        {
            this.TerrainStencil = new DefaultTerrainStencil<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaterStep{T}"/> class with the specified terrain and stencil.
        /// </summary>
        /// <param name="terrain">The water terrain tile to place.</param>
        /// <param name="check">The stencil that determines which tiles are eligible for placement.</param>
        protected WaterStep(ITile terrain, ITerrainStencil<T> check)
        {
            this.Terrain = terrain;
            this.TerrainStencil = check;
        }

        /// <summary>
        /// Gets or sets the tile representing the water terrain to paint.
        /// </summary>
        public ITile Terrain { get; set; }

        /// <summary>
        /// Gets or sets the stencil that determines which tiles are eligible for water placement.
        /// </summary>
        public ITerrainStencil<T> TerrainStencil { get; set; }

        /// <summary>
        /// Draws a blob of water terrain within the specified bounds.
        /// </summary>
        /// <param name="map">The map context to draw on.</param>
        /// <param name="rect">The bounding rectangle for the blob.</param>
        /// <param name="blobTest">The test function that determines which tiles within the bounds belong to the blob.</param>
        protected void DrawBlob(T map, Rect rect, Grid.LocTest blobTest)
        {
            for (int xx = Math.Max(0, rect.X); xx < Math.Min(map.Width, rect.End.X); xx++)
            {
                for (int yy = Math.Max(0, rect.Y); yy < Math.Min(map.Height, rect.End.Y); yy++)
                {
                    Loc destLoc = new Loc(xx, yy);
                    if (blobTest(destLoc))
                    {
                        if (this.TerrainStencil.Test(map, destLoc))
                            map.TrySetTile(destLoc, this.Terrain.Copy());
                    }
                }
            }

            GenContextDebug.DebugProgress("Draw Blob");
        }

        /// <summary>
        /// Draws water terrain at the specified array of locations.
        /// </summary>
        /// <param name="map">The map context to draw on.</param>
        /// <param name="locs">The array of locations to place water terrain.</param>
        protected void DrawLocs(T map, Loc[] locs)
        {
            foreach (Loc loc in locs)
            {
                // check against the stencil
                if (this.TerrainStencil.Test(map, loc))
                    map.TrySetTile(loc, this.Terrain.Copy());
            }

            GenContextDebug.DebugProgress("Draw Locs");
        }
    }
}
