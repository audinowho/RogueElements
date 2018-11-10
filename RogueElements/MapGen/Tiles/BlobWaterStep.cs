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

            BlobMap blobMap = Detection.DetectBlobs(noise);

            bool[][] mapGrid = new bool[map.Width][];
            for (int xx = 0; xx < map.Width; xx++)
            {
                mapGrid[xx] = new bool[map.Height];
                for (int yy = 0; yy < map.Height; yy++)
                    mapGrid[xx][yy] = map.GetTile(new Loc(xx, yy)).TileEquivalent(map.RoomTerrain);
            }

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


            for (int ii = 0; ii < blobMap.Blobs.Count; ii++)
            {
                bool disconnects = false;
                Rect blobRect = blobMap.Blobs[ii].Bounds;

                //produce a new array for the blob
                bool[][] blobGrid = new bool[blobRect.Width][];
                for(int xx = 0; xx < blobRect.Width; xx++)
                    blobGrid[xx] = new bool[blobRect.Height];

                for(int xx = 0; xx < blobRect.Width; xx++)
                {
                    for (int yy = 0; yy < blobRect.Height; yy++)
                    {
                        if (blobMap.Map[xx + blobRect.X][yy + blobRect.Y] == ii)
                        {
                            blobGrid[xx][yy] = true;
                            if (stairsGrid[xx][yy])
                                disconnects = true;
                        }
                    }
                }
                //pass this into the walkable detection function
                disconnects |= Detection.DetectDisconnect(mapGrid, blobGrid, blobRect.Start, true);

                //if it's a pass, draw on tile if it's a wall terrain or a room terrain
                if (!disconnects)
                    drawBlob(map, mapGrid, blobGrid, blobRect.Start, !RespectFloor);
                else
                {
                    //if it's a fail, draw on the tile only if wall terrain (or not at all?)
                    if (KeepDisconnects)
                        drawBlob(map, mapGrid, blobGrid, blobRect.Start, false);
                }
            }



        }

        private void drawBlob(T map, bool[][] mapGrid, bool[][] blobGrid, Loc offset, bool encroach)
        {
            for (int xx = Math.Max(0, offset.X); xx < Math.Min(map.Width, offset.X + blobGrid.Length); xx++)
            {
                for (int yy = Math.Max(0, offset.Y); yy < Math.Min(map.Height, offset.Y + blobGrid[0].Length); yy++)
                {
                    Loc loc = new Loc(xx, yy);
                    if (blobGrid[xx - offset.X][yy - offset.Y])
                    {
                        if (map.GetTile(loc).TileEquivalent(map.WallTerrain) || !map.TileBlocked(loc) && encroach)
                        {
                            map.SetTile(new Loc(xx, yy), Terrain.Copy());
                            mapGrid[xx][yy] = false;
                        }
                    }
                }
            }
        }

    }
}
