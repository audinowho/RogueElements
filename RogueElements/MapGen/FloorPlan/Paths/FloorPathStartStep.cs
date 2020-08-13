// <copyright file="FloorPathStartStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements
{
    [Serializable]
    public abstract class FloorPathStartStep<T> : FloorPlanStep<T>
        where T : class, IFloorPlanGenContext
    {
        public void CreateErrorPath(IRandom rand, FloorPlan floorPlan)
        {
            floorPlan.Clear();
            RoomGen<T> room = this.GetDefaultGen();
            room.PrepareSize(rand, Loc.One);
            room.SetLoc(Loc.Zero);
            floorPlan.AddRoom(room, new ComponentCollection());
        }

        public virtual RoomGen<T> GetDefaultGen()
        {
            return new RoomGenDefault<T>();
        }
    }
}
