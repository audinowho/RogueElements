// <copyright file="DefaultHallBrush.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A single-tile brush for painting hallways.
    /// Paints one tile at a time using the map's room terrain.
    /// </summary>
    [Serializable]
    public class DefaultHallBrush : BaseHallBrush
    {
        /// <inheritdoc/>
        public override Loc Size { get => Loc.One; }

        /// <inheritdoc/>
        public override Loc Center { get => Loc.Zero; }

        /// <inheritdoc/>
        public override BaseHallBrush Clone()
        {
            return new DefaultHallBrush();
        }

        /// <inheritdoc/>
        public override void DrawHallBrush(ITiledGenContext map, Rect bounds, LocRay4 ray, int length)
        {
            for (int ii = 0; ii < length; ii++)
            {
                Loc point = ray.Traverse(ii);
                map.SetTile(point, map.RoomTerrain.Copy());
            }
        }
    }
}
