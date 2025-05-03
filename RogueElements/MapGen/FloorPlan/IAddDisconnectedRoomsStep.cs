// <copyright file="IAddDisconnectedRoomsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements
{
    public interface IAddDisconnectedRoomsStep
    {
        RandRange Amount { get; set; }
    }

    /// <summary>
    /// Takes the current floor plan and adds new rooms that are disconnected from existing rooms.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    [Serializable]
    public abstract class AddDisconnectedRoomsBaseStep<TGenContext, TTile> : FloorPlanStep<TGenContext, TTile>, IAddDisconnectedRoomsStep
        where TGenContext : class, IFloorPlanGenContext<TTile>
        where TTile : ITile<TTile>
    {
        protected AddDisconnectedRoomsBaseStep()
            : base()
        {
            this.Components = new ComponentCollection();
        }

        protected AddDisconnectedRoomsBaseStep(IRandPicker<RoomGen<TGenContext, TTile>> genericRooms)
            : base()
        {
            this.GenericRooms = genericRooms;
            this.Components = new ComponentCollection();
        }

        /// <summary>
        /// The number of rooms to add.
        /// </summary>
        public RandRange Amount { get; set; }

        /// <summary>
        /// The room types that can be used for the room being added.
        /// </summary>
        public IRandPicker<RoomGen<TGenContext, TTile>> GenericRooms { get; set; }

        /// <summary>
        /// Components that the newly added rooms will be labeled with.
        /// </summary>
        public ComponentCollection Components { get; set; }

        public override void ApplyToPath(IRandom rand, FloorPlan<TTile> floorPlan)
        {
            int amount = this.Amount.Pick(rand);

            for (int ii = 0; ii < amount; ii++)
            {
                // choose a room
                RoomGen<TGenContext, TTile> room = this.GenericRooms.Pick(rand).Copy();

                // decide on acceptable border/size/fulfillables
                Loc size = room.ProposeSize(rand);
                if (size.X > floorPlan.DrawRect.Width)
                    size.X = floorPlan.DrawRect.Width;
                if (size.Y > floorPlan.DrawRect.Height)
                    size.Y = floorPlan.DrawRect.Height;
                room.PrepareSize(rand, size);

                Loc? testStart = this.ChooseViableLoc(rand, floorPlan, room.Draw.Size);

                if (!testStart.HasValue)
                    continue;

                room.SetLoc(testStart.Value);
                floorPlan.AddRoom(room, this.Components.Clone());
                GenContextDebug.DebugProgress("Place Disconnected Room");
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: Add:{1}", this.GetType().GetFormattedTypeName(), this.Amount);
        }

        protected abstract Loc? ChooseViableLoc(IRandom rand, FloorPlan<TTile> floorPlan, Loc roomSize);
    }
}
