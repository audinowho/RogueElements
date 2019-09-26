// <copyright file="DrawGridToFloorStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class DrawGridToFloorStep<T> : GenStep<T>
        where T : class, IRoomGridGenContext
    {
        public DrawGridToFloorStep()
        {
        }

        public override void Apply(T map)
        {
            var floorPlan = new FloorPlan();
            floorPlan.InitSize(map.GridPlan.Size);
            map.InitPlan(floorPlan);

            map.GridPlan.PlaceRoomsOnFloor(map);
        }
    }
}
