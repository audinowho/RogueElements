// <copyright file="InitTilesStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

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
            for (int xx = 0; xx < Width; xx++)
            {
                for (int yy = 0; yy < Height; yy++)
                    map.SetTile(new Loc(xx, yy), map.WallTerrain.Copy());
            }
        }

    }
}
