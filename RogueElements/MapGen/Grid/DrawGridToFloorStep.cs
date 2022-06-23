// <copyright file="DrawGridToFloorStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Takes the grid plan of the map and draws all cells and halls into rooms of a floor plan.
    /// This is typically done once per floor generation.  It must only be done after the grid plan itself is complete.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
            floorPlan.InitSize(map.GridPlan.Size, map.GridPlan.Wrap);
            map.InitPlan(floorPlan);

            map.GridPlan.PlaceRoomsOnFloor(map);
        }

        public override string ToString()
        {
            return string.Format("{0}", this.GetType().Name);
        }
    }
}
