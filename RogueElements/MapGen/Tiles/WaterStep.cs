using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public abstract class WaterStep<T> : GenStep<T> where T : class, ITiledGenContext
    {
        public ITile Terrain;

        public WaterStep() { }

        public WaterStep(ITile terrain)
        {
            Terrain = terrain;
        }
        

        protected void drawBlob(T map, BlobMap blobMap, int index, Loc offset, bool encroach)
        {
            MapBlob mapBlob = blobMap.Blobs[index];
            for (int xx = Math.Max(0, offset.X); xx < Math.Min(map.Width, offset.X + mapBlob.Bounds.Width); xx++)
            {
                for (int yy = Math.Max(0, offset.Y); yy < Math.Min(map.Height, offset.Y + mapBlob.Bounds.Height); yy++)
                {
                    Loc destLoc = new Loc(xx, yy);
                    Loc srcLoc = destLoc + mapBlob.Bounds.Start - offset;
                    if (blobMap.Map[srcLoc.X][srcLoc.Y] == index)
                    {
                        //can place anything if encroaching
                        //otherwise, can place anything except roomterrain
                        if (encroach || !map.GetTile(destLoc).TileEquivalent(map.RoomTerrain))
                            map.TrySetTile(new Loc(xx, yy), Terrain.Copy());
                    }
                }
            }
        }

    }
}
