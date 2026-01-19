// <copyright file="SquareHallBrush.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A rectangular brush for painting hallways.
    /// Paints a rectangular area of tiles using the map's room terrain.
    /// </summary>
    [Serializable]
    public class SquareHallBrush : BaseHallBrush
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SquareHallBrush"/> class.
        /// </summary>
        public SquareHallBrush()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SquareHallBrush"/> class with specified dimensions.
        /// </summary>
        /// <param name="size">The dimensions of the brush in tiles.</param>
        public SquareHallBrush(Loc size)
        {
            this.Dims = size;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SquareHallBrush"/> class as a copy of another.
        /// </summary>
        /// <param name="other">The instance to copy from.</param>
        public SquareHallBrush(SquareHallBrush other)
        {
            this.Dims = other.Dims;
        }

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
            return new SquareHallBrush(this);
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
                        if (map.CanSetTile(dest, map.RoomTerrain))
                            map.SetTile(dest, map.RoomTerrain.Copy());
                    }
                }
            }
        }
    }
}
