using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class BlobWaterStep<T> : GenStep<T> where T : class, ITiledGenContext
    {
        const int BUFFER_SIZE = 5;
        const int AUTOMATA_ROUNDS = 5;

        public int WaterFrequency;
        //public int Blobs;
        public int Terrain;

        public BlobWaterStep() { }

        public BlobWaterStep(int waterFrequency, int terrain) : this()
        {
            WaterFrequency = waterFrequency;
            //Blobs = blobs;
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

            bool[][] noise = new bool[map.Width][];
            for (int xx = 0; xx < map.Width; xx++)
            {
                noise[xx] = new bool[map.Height];
                for (int yy = 0; yy < map.Height; yy++)
                {
                    if (xx >= BUFFER_SIZE && xx < map.Width - BUFFER_SIZE && yy >= BUFFER_SIZE && yy < map.Height - BUFFER_SIZE)
                        noise[xx][yy] = (map.Rand.Next(100) < WaterFrequency);
                }
            }

            noise = NoiseGen.IterateAutomata(noise, 240, 248, AUTOMATA_ROUNDS);

            //BlobMap blobMap = MapTexturer.DetectBlobs(noise);

            for (int x = 1; x < map.Width - 1; x++)
            {
                for (int y = 1; y < map.Height - 1; y++)
                {
                    if (map.Tiles[x][y].ID != map.WallTerrain)
                    {

                    }
                    else if (noise[x][y])
                        map.Tiles[x][y].ID = Terrain;
                }
            }
        }

    }
}
