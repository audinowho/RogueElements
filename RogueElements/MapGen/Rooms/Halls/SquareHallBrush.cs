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
    /// </summary>
    [Serializable]
    public class SquareHallBrush : BaseHallBrush
    {
        public SquareHallBrush()
        {
        }

        public SquareHallBrush(Loc size)
        {
            this.Dims = size;
        }

        public SquareHallBrush(SquareHallBrush other)
        {
            this.Dims = other.Dims;
        }

        /// <summary>
        /// Dimensions of the brush, in Tiles
        /// </summary>
        public Loc Dims { get; set; }

        public override Loc Size { get => this.Dims; }

        public override Loc Center { get => Loc.Zero; }

        public override BaseHallBrush Clone()
        {
            return new SquareHallBrush(this);
        }

        public override void DrawHallBrush(ITiledGenContext map, Rect bounds, Loc point, bool vertical)
        {
            Rect brushRect = new Rect(point, this.Dims);
            if (bounds.Contains(brushRect))
            {
                for (int xx = brushRect.X; xx < brushRect.Right; xx++)
                {
                    for (int yy = brushRect.Y; yy < brushRect.Bottom; yy++)
                    {
                        map.SetTile(new Loc(xx, yy), map.RoomTerrain.Copy());
                    }
                }
            }
            else
            {
                map.SetTile(point, map.RoomTerrain.Copy());
            }
        }
    }
}
