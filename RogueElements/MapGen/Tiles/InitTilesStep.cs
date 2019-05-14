// <copyright file="InitTilesStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class InitTilesStep<T> : GenStep<T>
        where T : class, ITiledGenContext
    {
        private readonly int width;
        private readonly int height;

        public InitTilesStep(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public override void Apply(T map)
        {
            // initialize map array to empty
            // set default map values
            map.CreateNew(this.width, this.height);
            for (int xx = 0; xx < this.width; xx++)
            {
                for (int yy = 0; yy < this.height; yy++)
                    map.SetTile(new Loc(xx, yy), map.WallTerrain.Copy());
            }
        }
    }
}
