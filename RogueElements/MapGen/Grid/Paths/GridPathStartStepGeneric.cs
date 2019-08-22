// <copyright file="GridPathStartStepGeneric.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    [Serializable]
    public abstract class GridPathStartStepGeneric<T> : GridPathStartStep<T>
        where T : class, IRoomGridGenContext
    {
        // generic rooms that can be placed in any frequency, anywhere as the ultimate fallback
        // each path layout might have other room lists, but they will always have a generic room list?
        public IRandPicker<RoomGen<T>> GenericRooms { get; set; }

        public IRandPicker<PermissiveRoomGen<T>> GenericHalls { get; set; }

        public override void Apply(T map)
        {
            if (!this.GenericRooms.CanPick || !this.GenericHalls.CanPick)
                throw new InvalidOperationException("Can't create a path without rooms or halls.");

            base.Apply(map);
        }
    }
}
