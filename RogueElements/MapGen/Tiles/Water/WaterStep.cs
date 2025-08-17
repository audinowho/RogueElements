// <copyright file="WaterStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public abstract class WaterStep<T> : GenStep<T>, IWaterStep
        where T : class, ITiledGenContext
    {
        protected WaterStep()
        {
            this.TerrainStencil = new DefaultTerrainStencil<T>();
        }

        protected WaterStep(ITile terrain, ITerrainStencil<T> check)
        {
            this.Terrain = terrain;
            this.TerrainStencil = check;
        }

        /// <summary>
        /// Tile representing the water terrain to paint with.
        /// </summary>
        public ITile Terrain { get; set; }

        /// <summary>
        /// Determines which tiles are eligible to be painted on.
        /// </summary>
        public ITerrainStencil<T> TerrainStencil { get; set; }

        /// <summary>
        /// Draws a blob with the specified bounds and test method
        /// </summary>
        /// <param name="map"></param>
        /// <param name="rect"></param>
        /// <param name="blobTest">The method to test for terrain presence.  Passes in global location on the map.</param>
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
