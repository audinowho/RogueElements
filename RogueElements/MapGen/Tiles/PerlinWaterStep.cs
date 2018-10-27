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

        public PerlinWaterStep() { }

        public PerlinWaterStep(int waterFrequency, int order, ITile terrain)
        {
            WaterFrequency = waterFrequency;
            Order = order;
            Terrain = terrain;
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
                        if (map.GetTile(new Loc(xx,yy)).TileEquivalent(map.WallTerrain))
                            map.SetTile(new Loc(xx,yy), Terrain.Copy());
                    }
                }
                return;
            }


            int degree = Order;
            int[][] noise = NoiseGen.PerlinNoise(map.Rand, map.Width, map.Height, degree);

            for (int xx = 0; xx < map.Width; xx++)
            {
                for (int yy = 0; yy < map.Height; yy++)
                {
                    int heightPercent = Math.Min(100, Math.Min(Math.Min(xx * 100 / BUFFER_SIZE, yy * 100 / BUFFER_SIZE), Math.Min((map.Width - 1 - xx) * 100 / BUFFER_SIZE, (map.Height - 1 - yy) * 100 / BUFFER_SIZE)));
                    heightPercent = heightPercent * WaterFrequency / 100;

                    if (!map.GetTile(new Loc(xx, yy)).TileEquivalent(map.WallTerrain))
                    {

                    }
                    else if (noise[xx][yy] * 50 < (heightPercent - 50) * (int)Math.Pow(2, degree + 1))
                        map.SetTile(new Loc(xx, yy), Terrain.Copy());

                }
            }
        }

    }
}
