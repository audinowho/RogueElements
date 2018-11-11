using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class BlobWaterStep<T, E, F> : GenStep<T> where T : class, ITiledGenContext, IViewPlaceableGenContext<E>, IViewPlaceableGenContext<F>
    {

        const int AUTOMATA_ROUNDS = 5;

        public int WaterFrequency;
        public ITile Terrain;
        //Decides if the water can paint over floor tiles if the blob itself does not break connectivity
        public bool RespectFloor;
        //Decides if the water can be painted on non-floor tiles if the blob itself DOES break connectivity
        public bool KeepDisconnects;

        public BlobWaterStep() { }

        public BlobWaterStep(int waterFrequency, ITile terrain) : this()
        {
            WaterFrequency = waterFrequency;
            Terrain = terrain;
        }

        public BlobWaterStep(int waterFrequency, ITile terrain, bool respectFloor, bool keepDisconnects) : this(waterFrequency, terrain)
        {
            RespectFloor = respectFloor;
            KeepDisconnects = keepDisconnects;
        }

        public override void Apply(T map)
        {
            if (WaterFrequency == 0)
                return;

            if (WaterFrequency == 100)
            {
                for (int xx = 0; xx < map.Width; xx++)
                {
                    for (int yy = 0; yy < map.Height; yy++)
                    {
                        if (map.GetTile(new Loc(xx, yy)).TileEquivalent(map.WallTerrain))
                            map.SetTile(new Loc(xx, yy), Terrain.Copy());
                    }
                }
                return;
            }

            bool[][] noise = new bool[map.Width][];
            for (int xx = 0; xx < map.Width; xx++)
            {
                noise[xx] = new bool[map.Height];
                for (int yy = 0; yy < map.Height; yy++)
                {
                    //create a buffer equal to automata rounds to prevent terrain from smooshing up on the wall
                    if (xx >= AUTOMATA_ROUNDS && xx < map.Width - AUTOMATA_ROUNDS && yy >= AUTOMATA_ROUNDS && yy < map.Height - AUTOMATA_ROUNDS)
                        noise[xx][yy] = (map.Rand.Next(100) < WaterFrequency);
                }
            }

            noise = NoiseGen.IterateAutomata(noise, CellRule.Gte5, CellRule.Gte4, AUTOMATA_ROUNDS);

            Grid.LocTest isWaterValid = (Loc loc) => { return noise[loc.X][loc.Y]; };

            BlobMap blobMap = Detection.DetectBlobs(new Rect(0,0,map.Width, map.Height), isWaterValid);


            //alternative approach:
            //run automata with a CONSTANT water ratio
            //iterate the blobs in the automata
            //check to see if this blob tries to convert start or end
            //attempt to place it in random locations
            //then do the same for all the other blobs
            //until we get a value over the WaterFrequency

            bool[][] stairsGrid = new bool[map.Width][];
            for (int xx = 0; xx < map.Width; xx++)
                stairsGrid[xx] = new bool[map.Height];
            //check against stairs
            for (int jj = 0; jj < ((IViewPlaceableGenContext<E>)map).Count; jj++)
            {
                Loc stairs = ((IViewPlaceableGenContext<E>)map).GetLoc(jj);
                if (blobMap.Map[stairs.X][stairs.Y] > -1)
                    stairsGrid[stairs.X][stairs.Y] = true;
            }
            for (int jj = 0; jj < ((IViewPlaceableGenContext<F>)map).Count; jj++)
            {
                Loc stairs = ((IViewPlaceableGenContext<F>)map).GetLoc(jj);
                if (blobMap.Map[stairs.X][stairs.Y] > -1)
                    stairsGrid[stairs.X][stairs.Y] = true;
            }


            Grid.LocTest isMapValid = (Loc loc) => { return map.GetTile(loc).TileEquivalent(map.RoomTerrain); };

            int blobIdx = 0;
            Loc blobStart = new Loc();
            Grid.LocTest isBlobValid = (Loc loc) =>
            {
                Loc srcLoc = blobStart + loc;
                return blobMap.Map[srcLoc.X][srcLoc.Y] == blobIdx;
            };

            for (; blobIdx < blobMap.Blobs.Count; blobIdx++)
            {
                bool disconnects = false;
                Rect blobRect = blobMap.Blobs[blobIdx].Bounds;
                blobStart = blobRect.Start;

                for (int xx = 0; xx < blobRect.Width; xx++)
                {
                    for (int yy = 0; yy < blobRect.Height; yy++)
                    {
                        if (blobMap.Map[xx + blobRect.X][yy + blobRect.Y] == blobIdx)
                        {
                            if (stairsGrid[xx][yy])
                                disconnects = true;
                        }
                    }
                }
                //pass this into the walkable detection function
                disconnects |= Detection.DetectDisconnect(new Rect(0, 0, map.Width, map.Height), isMapValid, blobRect.Start, blobRect.Size, isBlobValid, true);

                //if it's a pass, draw on tile if it's a wall terrain or a room terrain
                if (!disconnects)
                    drawBlob(map, blobMap, blobIdx, blobRect.Start, !RespectFloor);
                else
                {
                    //if it's a fail, draw on the tile only if wall terrain (or not at all?)
                    if (KeepDisconnects)
                        drawBlob(map, blobMap, blobIdx, blobRect.Start, false);
                }
            }



        }

        private void drawBlob(T map, BlobMap blobMap, int index, Loc offset, bool encroach)
        {
            MapBlob mapBlob = blobMap.Blobs[index];
            for (int xx = Math.Max(0, offset.X); xx < Math.Min(map.Width, offset.X + mapBlob.Bounds.Width); xx++)
            {
                for (int yy = Math.Max(0, offset.Y); yy < Math.Min(map.Height, offset.Y + mapBlob.Bounds.Width); yy++)
                {
                    Loc destLoc = new Loc(xx, yy);
                    Loc srcLoc = destLoc - offset;
                    if (blobMap.Map[srcLoc.X][srcLoc.Y] == index)
                    {
                        if (map.GetTile(destLoc).TileEquivalent(map.WallTerrain) || !map.TileBlocked(destLoc) && encroach)
                            map.SetTile(new Loc(xx, yy), Terrain.Copy());
                    }
                }
            }
        }

    }
}
