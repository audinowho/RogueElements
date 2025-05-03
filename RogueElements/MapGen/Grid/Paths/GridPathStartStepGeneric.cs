// <copyright file="GridPathStartStepGeneric.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public abstract class GridPathStartStepGeneric<TGenContext, TTile> : GridPathStartStep<TGenContext, TTile>
        where TGenContext : class, IRoomGridGenContext<TTile>
        where TTile : ITile<TTile>
    {
        protected GridPathStartStepGeneric()
        {
            this.RoomComponents = new ComponentCollection();
            this.HallComponents = new ComponentCollection();
        }

        /// <summary>
        /// The room types that can be used for the rooms of the layout.
        /// </summary>
        public IRandPicker<RoomGen<TGenContext, TTile>> GenericRooms { get; set; }

        /// <summary>
        /// Components that the newly added rooms will be labeled with.
        /// </summary>
        public ComponentCollection RoomComponents { get; set; }

        /// <summary>
        /// The room types that can be used for the halls of the layout.
        /// </summary>
        public IRandPicker<PermissiveRoomGen<TGenContext, TTile>> GenericHalls { get; set; }

        /// <summary>
        /// Components that the newly added halls will be labeled with.
        /// </summary>
        public ComponentCollection HallComponents { get; set; }

        public override void Apply(TGenContext map)
        {
            if (!this.GenericRooms.CanPick || !this.GenericHalls.CanPick)
                throw new InvalidOperationException("Can't create a path without rooms or halls.");

            base.Apply(map);
        }
    }
}
