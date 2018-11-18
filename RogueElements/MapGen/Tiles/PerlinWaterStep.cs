using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class PerlinWaterStep<T> : WaterStep<T> where T : class, ITiledGenContext
    {
        const int BUFFER_SIZE = 5;

        public RandRange WaterPercent;
        public int OrderComplexity;
        public int OrderSoftness;
        //Decides if the water can paint over floor tiles if the blob itself does not break connectivity
        public bool RespectFloor;

        public PerlinWaterStep() { }

        public PerlinWaterStep(RandRange waterPercent, int complexity, ITile terrain) : base(terrain)
        {
            WaterPercent = waterPercent;
            OrderComplexity = complexity;
        }

        public PerlinWaterStep(RandRange waterPercent, int complexity, int softness, ITile terrain, bool respectFloor) : this(waterPercent, complexity, terrain)
        {
            OrderSoftness = softness;
            RespectFloor = respectFloor;
        }

        public override void Apply(T map)
        {
            int waterPercent = WaterPercent.Pick(map.Rand);
            if (waterPercent == 0)
                return;

            int depthRange = 0x1 << (OrderComplexity + OrderSoftness);//aka, 2 ^ degree
            int minWater = waterPercent * map.Width * map.Height / 100;
            int[][] noise = NoiseGen.PerlinNoise(map.Rand, map.Width, map.Height, OrderComplexity, OrderSoftness);
            int[] depthCount = new int[depthRange];
            for(int xx = 0; xx < map.Width; xx++)
            {
                for (int yy = 0; yy < map.Height; yy++)
                    depthCount[noise[xx][yy]]++;
            }
            int waterMark = 0;
            int totalDepths = 0;
            for(int ii = 0; ii < depthCount.Length; ii++)
            {
                if (totalDepths + depthCount[ii] >= minWater)
                {
                    if (totalDepths + depthCount[ii] - minWater < minWater - totalDepths)
                        waterMark++;
                    break;
                }
                totalDepths += depthCount[ii];
                waterMark++;
            }

            if (RespectFloor)
            {
                drawWhole(map, noise, depthRange, waterMark);
                return;
            }

            while (waterMark > 0)
            {

                Grid.LocTest isWaterValid = (Loc loc) =>
                {
                    int heightPercent = Math.Min(100, Math.Min(Math.Min(loc.X * 100 / BUFFER_SIZE, loc.Y * 100 / BUFFER_SIZE), Math.Min((map.Width - 1 - loc.X) * 100 / BUFFER_SIZE, (map.Height - 1 - loc.Y) * 100 / BUFFER_SIZE)));
                    int noiseVal = noise[loc.X][loc.Y] * heightPercent / 100 + depthRange * (100 - heightPercent) / 100;
                    return noiseVal < waterMark;
                };

                BlobMap blobMap = Detection.DetectBlobs(new Rect(0, 0, map.Width, map.Height), isWaterValid);

                Grid.LocTest isMapValid = (Loc loc) => { return map.GetTile(loc).TileEquivalent(map.RoomTerrain); };

                int blobIdx = 0;
                Grid.LocTest isBlobValid = (Loc loc) =>
                {
                    Loc srcLoc = loc + blobMap.Blobs[blobIdx].Bounds.Start;
                    if (!map.CanSetTile(srcLoc, Terrain))
                        return false;
                    return blobMap.Map[srcLoc.X][srcLoc.Y] == blobIdx;
                };

                for (; blobIdx < blobMap.Blobs.Count; blobIdx++)
                {
                    Rect blobRect = blobMap.Blobs[blobIdx].Bounds;

                    //pass this into the walkable detection function
                    bool disconnects = Detection.DetectDisconnect(new Rect(0, 0, map.Width, map.Height), isMapValid, blobRect.Start, blobRect.Size, isBlobValid, true);

                    //if it's a pass, draw on tile if it's a wall terrain or a room terrain
                    if (!disconnects)
                        drawBlob(map, blobMap, blobIdx, blobRect.Start, true);
                    else
                    {
                        //if it's a fail, draw on the tile only if wall terrain
                        drawBlob(map, blobMap, blobIdx, blobRect.Start, false);
                    }
                }
                waterMark -= Math.Max(1, depthRange / 8);

            }
        }

        private void drawWhole(T map, int[][] noise, int depthRange, int waterMark)
        {
            for (int xx = 0; xx < map.Width; xx++)
            {
                for (int yy = 0; yy < map.Height; yy++)
                {
                    if (!map.GetTile(new Loc(xx, yy)).TileEquivalent(map.WallTerrain))
                        continue;

                    //the last BUFFER_SIZE tiles near the edge gradually multiply the actual noise value by smaller numbers
                    int heightPercent = Math.Min(100, Math.Min(Math.Min(xx * 100 / BUFFER_SIZE, yy * 100 / BUFFER_SIZE), Math.Min((map.Width - 1 - xx) * 100 / BUFFER_SIZE, (map.Height - 1 - yy) * 100 / BUFFER_SIZE)));
                    //interpolate UPWARDS to make it like a bowl
                    int noiseVal = noise[xx][yy] * heightPercent / 100 + depthRange * (100 - heightPercent) / 100;

                    if (noiseVal < waterMark)
                        map.SetTile(new Loc(xx, yy), Terrain.Copy());
                }
            }
        }
        
    }
}
