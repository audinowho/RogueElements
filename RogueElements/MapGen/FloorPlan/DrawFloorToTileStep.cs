// <copyright file="DrawFloorToTileStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class DrawFloorToTileStep<T> : GenStep<T>
        where T : class, IFloorPlanGenContext
    {
        public DrawFloorToTileStep(int padding = 0)
        {
            this.Padding = padding;
        }

        public int Padding { get; set; }

        public override void Apply(T map)
        {
            // draw on map
            map.CreateNew(
                map.RoomPlan.DrawRect.Width + (2 * this.Padding),
                map.RoomPlan.DrawRect.Height + (2 * this.Padding));
            for (int ii = 0; ii < map.Width; ii++)
            {
                for (int jj = 0; jj < map.Height; jj++)
                    map.SetTile(new Loc(ii, jj), map.WallTerrain.Copy());
            }

            map.RoomPlan.MoveStart(new Loc(this.Padding));
            GenContextDebug.DebugProgress("Moved Floor");
            map.RoomPlan.DrawOnMap(map);
        }
    }
}
