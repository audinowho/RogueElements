// <copyright file="IAddDisconnectedRoomsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements
{
    /// <summary>
    /// Defines the interface for steps that add disconnected rooms to a floor plan.
    /// </summary>
    public interface IAddDisconnectedRoomsStep
    {
        /// <summary>
        /// Gets or sets the number of rooms to add.
        /// </summary>
        RandRange Amount { get; set; }
    }

    /// <summary>
    /// Base class for steps that add rooms not connected to existing rooms in a floor plan.
    /// </summary>
    /// <typeparam name="T">The generation context type, which must implement <see cref="IFloorPlanGenContext"/>.</typeparam>
    /// <remarks>
    /// This abstract class provides common functionality for adding isolated rooms that do not
    /// connect to any existing rooms. These rooms can later be connected using connect steps.
    /// Subclasses determine the specific algorithm for choosing placement locations.
    /// </remarks>
    /// <seealso cref="AddDisconnectedRoomsStep{T}"/>
    /// <seealso cref="AddDisconnectedRoomsRandStep{T}"/>
    [Serializable]
    public abstract class AddDisconnectedRoomsBaseStep<T> : FloorPlanStep<T>, IAddDisconnectedRoomsStep
        where T : class, IFloorPlanGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddDisconnectedRoomsBaseStep{T}"/> class.
        /// </summary>
        protected AddDisconnectedRoomsBaseStep()
            : base()
        {
            this.Components = new ComponentCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddDisconnectedRoomsBaseStep{T}"/> class with specified room generators.
        /// </summary>
        /// <param name="genericRooms">The picker for room generators.</param>
        protected AddDisconnectedRoomsBaseStep(IRandPicker<RoomGen<T>> genericRooms)
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
        public IRandPicker<RoomGen<T>> GenericRooms { get; set; }

        /// <summary>
        /// Components that the newly added rooms will be labeled with.
        /// </summary>
        public ComponentCollection Components { get; set; }

        /// <summary>
        /// Applies this step to add disconnected rooms to the floor plan.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The floor plan to modify.</param>
        public override void ApplyToPath(IRandom rand, FloorPlan floorPlan)
        {
            int amount = this.Amount.Pick(rand);

            for (int ii = 0; ii < amount; ii++)
            {
                // choose a room
                RoomGen<T> room = this.GenericRooms.Pick(rand).Copy();

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

        /// <summary>
        /// Returns a string representation of this step.
        /// </summary>
        /// <returns>A string describing this step's configuration.</returns>
        public override string ToString()
        {
            return string.Format("{0}: Add:{1}", this.GetType().GetFormattedTypeName(), this.Amount);
        }

        /// <summary>
        /// Finds a viable location for placing a new disconnected room.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The floor plan to search within.</param>
        /// <param name="roomSize">The size of the room to place.</param>
        /// <returns>A valid location if found; otherwise, null.</returns>
        protected abstract Loc? ChooseViableLoc(IRandom rand, FloorPlan floorPlan, Loc roomSize);
    }
}
