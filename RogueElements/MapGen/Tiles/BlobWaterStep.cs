using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class BlobWaterStep<T> : WaterStep<T> where T : class, ITiledGenContext
    {

        const int AUTOMATA_CHANCE = 55;
        const int AUTOMATA_ROUNDS = 5;

        public RandRange Blobs;
        public int MinScale;
        public RandRange StartScale;

        public BlobWaterStep() { }
        
        public BlobWaterStep(RandRange blobs, ITile terrain, int minScale, RandRange startScale) : base(terrain)
        {
            Blobs = blobs;
            MinScale = minScale;
            StartScale = startScale;
        }

        public override void Apply(T map)
        {
            int blobs = Blobs.Pick(map.Rand);
            int startScale = Math.Max(MinScale, StartScale.Pick(map.Rand));
            for (int ii = 0; ii < blobs; ii++)
            {                
                Loc size = new Loc(map.Width * startScale / 100, map.Height * startScale / 100);
                int area = size.X * size.Y;
                bool placed = false;
                while (area > 0 && area >= MinScale * map.Width / 100 * MinScale * map.Height / 100)
                {
                    bool[][] noise = new bool[size.X][];
                    for (int xx = 0; xx < size.X; xx++)
                    {
                        noise[xx] = new bool[size.Y];
                        for (int yy = 0; yy < size.Y; yy++)
                            noise[xx][yy] = (map.Rand.Next(100) < AUTOMATA_CHANCE);
                    }

                    noise = NoiseGen.IterateAutomata(noise, CellRule.Gte5, CellRule.Gte4, AUTOMATA_ROUNDS);

                    Grid.LocTest isWaterValid = (Loc loc) => { return noise[loc.X][loc.Y]; };

                    BlobMap blobMap = Detection.DetectBlobs(new Rect(0, 0, noise.Length, noise[0].Length), isWaterValid);

                    if (blobMap.Blobs.Count > 0)
                    {
                        int blobIdx = 0;
                        for (int bb = 1; bb < blobMap.Blobs.Count; bb++)
                        {
                            if (blobMap.Blobs[bb].Area > blobMap.Blobs[blobIdx].Area)
                                blobIdx = bb;
                        }


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
                            placed = true;
                            break;
                        }
                    }
                    if (placed)
                        break;

                    size = size * 7 / 10;
                    area = size.X * size.Y;
                }
            }

        }


    }
}
