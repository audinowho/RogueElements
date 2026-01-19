// <copyright file="InitTilesStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Initializes a map of Width x Height tiles filled with wall terrain.
    /// </summary>
    /// <typeparam name="T">The type of map context that implements <see cref="ITiledGenContext"/>.</typeparam>
    [Serializable]
    public class InitTilesStep<T> : GenStep<T>
        where T : class, ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitTilesStep{T}"/> class.
        /// </summary>
        public InitTilesStep()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InitTilesStep{T}"/> class with the specified dimensions.
        /// </summary>
        /// <param name="width">The width of the map in tiles.</param>
        /// <param name="height">The height of the map in tiles.</param>
        public InitTilesStep(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Gets or sets the width of the map in tiles.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the map in tiles.
        /// </summary>
        public int Height { get; set; }

        /// <inheritdoc/>
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
            return string.Format("{0}: Size:{1}x{2}", this.GetType().GetFormattedTypeName(), this.Width, this.Height);
        }
    }
}
