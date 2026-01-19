// <copyright file="TerrainHallBrush.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A rectangular brush for painting hallways with a custom terrain type.
    /// Paints a rectangular area of tiles using a specified terrain instead of the default room terrain.
    /// </summary>
    [Serializable]
    public class TerrainHallBrush : BaseHallBrush
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TerrainHallBrush"/> class.
        /// </summary>
        public TerrainHallBrush()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerrainHallBrush"/> class with specified dimensions and terrain.
        /// </summary>
        /// <param name="size">The dimensions of the brush in tiles.</param>
        /// <param name="terrain">The terrain type to paint.</param>
        public TerrainHallBrush(Loc size, ITile terrain)
        {
            this.Dims = size;
            this.Terrain = terrain;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerrainHallBrush"/> class as a copy of another.
        /// </summary>
        /// <param name="other">The instance to copy from.</param>
        public TerrainHallBrush(TerrainHallBrush other)
        {
            this.Dims = other.Dims;
            this.Terrain = other.Terrain;
        }

        /// <summary>
        /// Gets or sets the terrain type to paint with this brush.
        /// </summary>
        public ITile Terrain { get; set; }

        /// <summary>
        /// Gets or sets the dimensions of the brush in tiles.
        /// </summary>
        public Loc Dims { get; set; }

        /// <inheritdoc/>
        public override Loc Size { get => this.Dims; }

        /// <inheritdoc/>
        public override Loc Center { get => Loc.Zero; }

        /// <inheritdoc/>
        public override BaseHallBrush Clone()
        {
            return new TerrainHallBrush(this);
        }

        /// <inheritdoc/>
        public override void DrawHallBrush(ITiledGenContext map, Rect bounds, LocRay4 ray, int length)
        {
            for (int ii = 0; ii < length; ii++)
            {
                Loc point = ray.Traverse(ii);
                Rect brushRect = new Rect(point, this.Dims);
                for (int xx = brushRect.X; xx < brushRect.Right; xx++)
                {
                    for (int yy = brushRect.Y; yy < brushRect.Bottom; yy++)
                    {
                        Loc dest = new Loc(xx, yy);
                        if (map.CanSetTile(dest, this.Terrain))
                            map.SetTile(dest, this.Terrain.Copy());
                    }
                }
            }
        }
    }
}
