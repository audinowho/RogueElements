// <copyright file="DefaultHallBrush.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class DefaultHallBrush : BaseHallBrush
    {
        public override Loc Size { get => Loc.One; }

        public override Loc Center { get => Loc.Zero; }

        public override BaseHallBrush Clone()
        {
            return new DefaultHallBrush();
        }

        public override void DrawHallBrush(ITiledGenContext map, Rect bounds, Loc point, bool vertical)
        {
            map.SetTile(point, map.RoomTerrain.Copy());
        }
    }
}
