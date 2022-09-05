// <copyright file="WaterStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public abstract class WaterStep<T> : GenStep<T>
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

        protected void DrawBlob(T map, BlobMap blobMap, int index, Loc offset)
        {
            BlobMap.Blob mapBlob = blobMap.Blobs[index];
            for (int xx = Math.Max(0, offset.X); xx < Math.Min(map.Width, offset.X + mapBlob.Bounds.Width); xx++)
            {
                for (int yy = Math.Max(0, offset.Y); yy < Math.Min(map.Height, offset.Y + mapBlob.Bounds.Height); yy++)
                {
                    Loc destLoc = new Loc(xx, yy);
                    Loc srcLoc = destLoc + mapBlob.Bounds.Start - offset;
                    if (blobMap.Map[srcLoc.X][srcLoc.Y] == index)
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
