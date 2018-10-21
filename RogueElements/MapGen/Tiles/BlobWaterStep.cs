using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class BlobWaterStep<T> : GenStep<T> where T : class, ITiledGenContext
    {
        const int AUTOMATA_ROUNDS = 5;

        public int WaterFrequency;
        //public int Blobs;
        public ITile Terrain;

        public BlobWaterStep() { }

        public BlobWaterStep(int waterFrequency, ITile terrain) : this()
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
                for (int xx = 0; xx < map.Width; xx++)
                {
                    for (int yy = 0; yy < map.Height; yy++)
                    {
                        if (map.Tiles[xx][yy].TileEquivalent(map.WallTerrain))
                            map.Tiles[xx][yy] = Terrain.Copy();
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

            noise = NoiseGen.IterateAutomata(noise, 240, 248, AUTOMATA_ROUNDS);

            //BlobMap blobMap = MapTexturer.DetectBlobs(noise);

            for (int xx = 0; xx < map.Width; xx++)
            {
                for (int yy = 0; yy < map.Height; yy++)
                {
                    if (!map.Tiles[xx][yy].TileEquivalent(map.WallTerrain))
                    {

                    }
                    else if (noise[xx][yy])
                        map.Tiles[xx][yy] = Terrain.Copy();
                }
            }
        }

    }
}
