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
    public class TerrainHallBrush<TTile> : BaseHallBrush
        where TTile : ITile<TTile>
    {
        public TerrainHallBrush()
        {
        }

        public TerrainHallBrush(Loc size, TTile terrain)
        {
            this.Dims = size;
            this.Terrain = terrain;
        }

        public TerrainHallBrush(TerrainHallBrush<TTile> other)
        {
            this.Dims = other.Dims;
            this.Terrain = other.Terrain;
        }

        public TTile Terrain { get; set; }

        /// <summary>
        /// Dimensions of the brush, in Tiles
        /// </summary>
        public Loc Dims { get; set; }

        public override Loc Size { get => this.Dims; }

        public override Loc Center { get => Loc.Zero; }

        public override BaseHallBrush Clone()
        {
            return new TerrainHallBrush<TTile>(this);
        }

        // TODO: Figure a way for this to work better
        public override void DrawHallBrush<TTile2>(ITiledGenContext<TTile2> map, Rect bounds, LocRay4 ray, int length)
        {
            if (typeof(TTile) != typeof(TTile2))
                throw new InvalidOperationException("Mismatching tile types!");

            for (int ii = 0; ii < length; ii++)
            {
                Loc point = ray.Traverse(ii);
                Rect brushRect = new Rect(point, this.Dims);
                for (int xx = brushRect.X; xx < brushRect.Right; xx++)
                {
                    for (int yy = brushRect.Y; yy < brushRect.Bottom; yy++)
                    {
                        Loc dest = new Loc(xx, yy);
                        if (map.CanSetTile(dest, (TTile2)(object)this.Terrain))
                            map.SetTile(dest, (TTile2)(object)this.Terrain.Copy());
                    }
                }
            }
        }
    }
}
