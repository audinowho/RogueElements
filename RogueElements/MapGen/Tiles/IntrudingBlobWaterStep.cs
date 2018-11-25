using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class IntrudingBlobWaterStep<T> : BlobWaterStep<T> where T : class, ITiledGenContext
    {

        public IntrudingBlobWaterStep() { }
        
        public IntrudingBlobWaterStep(RandRange blobs, ITile terrain, int minScale, RandRange startScale) : base(blobs, terrain, minScale, startScale)
        {
        }
        
        protected override bool AttemptBlob(T map, BlobMap blobMap, int blobIdx)
        {
            Grid.LocTest isMapValid = (Loc loc) => { return map.GetTile(loc).TileEquivalent(map.RoomTerrain); };

            //the XY to add to translate from point on the map to point on the blob map
            Loc offset = new Loc();
            Grid.LocTest isBlobValid = (Loc loc) =>
            {
                Loc srcLoc = loc + blobMap.Blobs[blobIdx].Bounds.Start;
                if (!Collision.InBounds(blobMap.Blobs[blobIdx].Bounds, srcLoc))
                    return false;
                Loc destLoc = loc + offset;
                if (!map.CanSetTile(destLoc, Terrain))
                    return false;
                return blobMap.Map[srcLoc.X][srcLoc.Y] == blobIdx;
            };

            //attempt to place in 20 locations
            for (int jj = 0; jj < 20; jj++)
            {
                Rect blobRect = blobMap.Blobs[blobIdx].Bounds;
                offset = new Loc(map.Rand.Next(0, map.Width - blobRect.Width), map.Rand.Next(0, map.Height - blobRect.Height));
                Loc blobMod = blobMap.Blobs[blobIdx].Bounds.Start - offset;

                //pass this into the walkable detection function
                bool disconnects = Detection.DetectDisconnect(new Rect(0, 0, map.Width, map.Height), isMapValid, offset, blobRect.Size, isBlobValid, true);

                //if it's a pass, draw on tile if it's a wall terrain or a room terrain
                if (disconnects)
                    continue;

                drawBlob(map, blobMap, blobIdx, offset, true);
                return true;
            }
            return false;
        }
    }
}
