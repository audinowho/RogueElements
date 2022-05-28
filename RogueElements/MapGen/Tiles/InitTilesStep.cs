// <copyright file="InitTilesStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Initializes a map of Width x Height tiles.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class InitTilesStep<T> : GenStep<T>
        where T : class, ITiledGenContext
    {
        public InitTilesStep()
        {
        }

        public InitTilesStep(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public override void Apply(T map)
        {
            // initialize map array to empty
            // set default map values
            map.CreateNew(this.Width, this.Height);
            for (int xx = 0; xx < this.Width; xx++)
            {
                for (int yy = 0; yy < this.Height; yy++)
                    map.SetTile(new Loc(xx, yy), map.WallTerrain.Copy());
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: Size:{1}x{2}", this.GetType().Name, this.Width, this.Height);
        }
    }
}
