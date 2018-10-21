using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class InitTilesStep<T> : GenStep<T> where T : class, ITiledGenContext
    {
        public int Width;
        public int Height;
        
        public InitTilesStep() { }

        public override void Apply(T map)
        {
            //initialize map array to empty
            //set default map values
            map.CreateNew(Width, Height);
            for (int ii = 0; ii < Width; ii++)
            {
                for (int jj = 0; jj < Height; jj++)
                    map.Tiles[ii][jj] = map.WallTerrain.Copy();
            }
        }

    }
}
