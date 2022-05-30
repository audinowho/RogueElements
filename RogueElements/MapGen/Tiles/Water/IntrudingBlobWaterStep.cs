﻿// <copyright file="IntrudingBlobWaterStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Creates blobs of water using cellular automata, and places them around the map.
    /// It will allow itself to be placed in locations that overlap walkable area, but never in a way that disconnects it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class IntrudingBlobWaterStep<T> : BlobWaterStep<T>
        where T : class, ITiledGenContext
    {
        public IntrudingBlobWaterStep()
            : base()
        {
        }

        public IntrudingBlobWaterStep(RandRange blobs, ITile terrain, ITerrainStencil<T> stencil, int minScale, RandRange startScale)
            : base(blobs, terrain, stencil, minScale, startScale)
        {
        }

        protected override bool AttemptBlob(T map, BlobMap blobMap, int blobIdx)
        {
            bool IsMapValid(Loc loc) => map.GetTile(loc).TileEquivalent(map.RoomTerrain);

            // the XY to add to translate from point on the map to point on the blob map
            Loc offset = Loc.Zero;
            bool IsBlobValid(Loc loc)
            {
                Loc srcLoc = loc + blobMap.Blobs[blobIdx].Bounds.Start;
                if (!Collision.InBounds(blobMap.Blobs[blobIdx].Bounds, srcLoc))
                    return false;
                Loc destLoc = loc + offset;
                if (!map.CanSetTile(destLoc, this.Terrain))
                    return false;
                return blobMap.Map[srcLoc.X][srcLoc.Y] == blobIdx;
            }

            // attempt to place in 20 locations
            for (int jj = 0; jj < 20; jj++)
            {
                Rect blobRect = blobMap.Blobs[blobIdx].Bounds;
                offset = new Loc(map.Rand.Next(0, map.Width - blobRect.Width), map.Rand.Next(0, map.Height - blobRect.Height));
                Loc blobMod = blobMap.Blobs[blobIdx].Bounds.Start - offset;

                // pass this into the walkable detection function
                bool disconnects = Detection.DetectDisconnect(new Rect(0, 0, map.Width, map.Height), IsMapValid, offset, blobRect.Size, IsBlobValid, true);

                // if it's a pass, draw on tile if it's a wall terrain or a room terrain
                if (disconnects)
                    continue;

                this.DrawBlob(map, blobMap, blobIdx, offset);
                return true;
            }

            return false;
        }
    }
}
