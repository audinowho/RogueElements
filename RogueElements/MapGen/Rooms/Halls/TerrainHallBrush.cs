// <copyright file="TerrainHallBrush.cs" company="Audino">
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
    public class TerrainHallBrush : BaseHallBrush
    {
        public TerrainHallBrush()
        {
        }

        public TerrainHallBrush(Loc size, ITile terrain)
        {
            this.Dims = size;
            this.Terrain = terrain;
        }

        public TerrainHallBrush(TerrainHallBrush other)
        {
            this.Dims = other.Dims;
            this.Terrain = other.Terrain;
        }

        public ITile Terrain { get; set; }

        /// <summary>
        /// Dimensions of the brush, in Tiles
        /// </summary>
        public Loc Dims { get; set; }

        public override Loc Size { get => this.Dims; }

        public override Loc Center { get => Loc.Zero; }

        public override BaseHallBrush Clone()
        {
            return new TerrainHallBrush(this);
        }

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
                        if (Collision.InBounds(map.Width, map.Height, dest))
                            map.SetTile(dest, this.Terrain.Copy());
                    }
                }
            }
        }
    }
}
