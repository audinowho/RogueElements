using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class PerlinWaterStep<T> : GenStep<T> where T : class, ITiledGenContext
    {
        const int BUFFER_SIZE = 5;

        public int WaterFrequency;
        public int Order;
        public ITile Terrain;
        //Decides if the water can paint over floor tiles if the blob itself does not break connectivity
        public bool RespectFloor;
        //Decides if the water can be painted on non-floor tiles if the blob itself DOES break connectivity
        public bool KeepDisconnects;

        public PerlinWaterStep() { }

        public PerlinWaterStep(int waterFrequency, int order, ITile terrain)
        {
            WaterFrequency = waterFrequency;
            Order = order;
            Terrain = terrain;
        }

        public PerlinWaterStep(int waterFrequency, int order, ITile terrain, bool respectFloor, bool keepDisconnects) : this(waterFrequency, order, terrain)
        {
            RespectFloor = respectFloor;
            KeepDisconnects = keepDisconnects;
        }

        public override void Apply(T map)
        {
            if (WaterFrequency == 0)
                return;

            int degree = Order;
            int[][] noise = NoiseGen.PerlinNoise(map.Rand, map.Width, map.Height, degree);
            int depthRange = 0x1 << (degree + 1);//aka, 2 ^ degree
            int waterMark = WaterFrequency * depthRange / 100;

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
