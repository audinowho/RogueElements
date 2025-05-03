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
    public abstract class FloorPathStartStep<TGenContext, TTile> : FloorPlanStep<TGenContext, TTile>
        where TGenContext : class, IFloorPlanGenContext<TTile>
        where TTile : ITile<TTile>
    {
        public void CreateErrorPath(IRandom rand, FloorPlan<TTile> floorPlan)
        {
            floorPlan.Clear();
            RoomGen<TGenContext, TTile> room = this.GetDefaultGen();
            room.PrepareSize(rand, Loc.One);
            room.SetLoc(Loc.Zero);
            floorPlan.AddRoom(room, new ComponentCollection());
        }

        public virtual RoomGen<TGenContext, TTile> GetDefaultGen()
        {
            return new RoomGenDefault<TGenContext, TTile>();
        }
    }
}
