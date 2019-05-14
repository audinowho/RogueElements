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
        protected WaterStep(ITile terrain)
        {
            this.Terrain = terrain;
        }

        /// <summary>
        /// Tile representing the water terrain to paint with.
        /// </summary>
        protected ITile Terrain { get; }

        protected void DrawBlob(T map, BlobMap blobMap, int index, Loc offset, bool encroach)
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
                        // can place anything if encroaching
                        // otherwise, can place anything except roomterrain
                        if (encroach || !map.GetTile(destLoc).TileEquivalent(map.RoomTerrain))
                            map.TrySetTile(new Loc(xx, yy), this.Terrain.Copy());
                    }
                }
            }

            GenContextDebug.DebugProgress("Draw Blob");
        }
    }
}
