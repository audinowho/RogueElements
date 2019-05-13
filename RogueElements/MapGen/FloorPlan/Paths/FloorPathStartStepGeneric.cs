// <copyright file="FloorPathStartStepGeneric.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements
{
    [Serializable]
    public abstract class FloorPathStartStepGeneric<T> : FloorPathStartStep<T>
        where T : class, IFloorPlanGenContext
    {
        //generic rooms that can be placed in any frequency, anywhere as the ultimate fallback
        //each path layout might have other room lists, but they will always have a generic room list?
        public IRandPicker<RoomGen<T>> GenericRooms;

        public IRandPicker<PermissiveRoomGen<T>> GenericHalls;

        public override void Apply(T map)
        {
            if (!GenericRooms.CanPick || !GenericHalls.CanPick)
                throw new InvalidOperationException("Can't create a path without rooms or halls.");

            base.Apply(map);
        }
    }
}
