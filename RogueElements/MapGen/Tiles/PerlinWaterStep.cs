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
        public int Terrain;

        public PerlinWaterStep() { }

        public PerlinWaterStep(int waterFrequency, int order, int terrain)
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
                for (int x = 1; x < map.Width - 1; x++)
                {
                    for (int y = 1; y < map.Height - 1; y++)
                        map.Tiles[x][y].ID = Terrain;
                }
                return;
            }


            int degree = Order;
            int[][] noise = NoiseGen.PerlinNoise(map.Rand, map.Width, map.Height, degree);

            for (int x = 1; x < map.Width - 1; x++)
            {
                for (int y = 1; y < map.Height - 1; y++)
                {
                    int heightPercent = Math.Min(100, Math.Min(Math.Min(x * 100 / BUFFER_SIZE, y * 100 / BUFFER_SIZE), Math.Min((map.Width - 1 - x) * 100 / BUFFER_SIZE, (map.Height - 1 - y) * 100 / BUFFER_SIZE)));
                    heightPercent = heightPercent * WaterFrequency / 100;

                    if (map.Tiles[x][y].ID != map.WallTerrain)
                    {

                    }
                    else if (noise[x][y] * 50 < (heightPercent - 50) * (int)Math.Pow(2, degree + 1))
                        map.Tiles[x][y].ID = Terrain;

                }
            }
        }

    }
}
