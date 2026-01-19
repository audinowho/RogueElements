// <copyright file="IAddConnectedRoomsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements
{
    /// <summary>
    /// Defines the interface for steps that add connected rooms to a floor plan.
    /// </summary>
    public interface IAddConnectedRoomsStep
    {
        /// <summary>
        /// Gets or sets the percentage chance that a hall is added between rooms.
        /// </summary>
        int HallPercent { get; set; }

        /// <summary>
        /// Gets or sets the number of rooms to add.
        /// </summary>
        RandRange Amount { get; set; }
    }

    /// <summary>
    /// Base class for steps that add rooms connected to existing rooms in a floor plan.
    /// </summary>
    /// <typeparam name="T">The generation context type, which must implement <see cref="IFloorPlanGenContext"/>.</typeparam>
    /// <remarks>
    /// This abstract class provides common functionality for adding rooms that connect to existing
    /// rooms, including support for optional hallways and room filtering. Subclasses determine
    /// the specific algorithm for choosing expansion points.
    /// </remarks>
    /// <seealso cref="AddConnectedRoomsStep{T}"/>
    /// <seealso cref="AddConnectedRoomsRandStep{T}"/>
    [Serializable]
    public abstract class AddConnectedRoomsBaseStep<T> : FloorPlanStep<T>, IAddConnectedRoomsStep
        where T : class, IFloorPlanGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddConnectedRoomsBaseStep{T}"/> class.
        /// </summary>
        protected AddConnectedRoomsBaseStep()
            : base()
        {
            this.RoomComponents = new ComponentCollection();
            this.HallComponents = new ComponentCollection();
            this.Filters = new List<BaseRoomFilter>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddConnectedRoomsBaseStep{T}"/> class with specified generators.
        /// </summary>
        /// <param name="genericRooms">The picker for room generators.</param>
        /// <param name="genericHalls">The picker for hall generators.</param>
        protected AddConnectedRoomsBaseStep(IRandPicker<RoomGen<T>> genericRooms, IRandPicker<PermissiveRoomGen<T>> genericHalls)
            : base()
        {
            this.GenericRooms = genericRooms;
            this.GenericHalls = genericHalls;
            this.RoomComponents = new ComponentCollection();
            this.HallComponents = new ComponentCollection();
            this.Filters = new List<BaseRoomFilter>();
        }

        /// <summary>
        /// The number of rooms to add.
        /// </summary>
        public RandRange Amount { get; set; }

        /// <summary>
        /// The chance that an added room is attached using an intermediate hallway.
        /// </summary>
        public int HallPercent { get; set; }

        /// <summary>
        /// Determines which rooms are eligible to have the new rooms added on.
        /// </summary>
        public List<BaseRoomFilter> Filters { get; set; }

        /// <summary>
        /// The room types that can be used for the room being added.
        /// </summary>
        public IRandPicker<RoomGen<T>> GenericRooms { get; set; }

        /// <summary>
        /// Components that the newly added rooms will be labeled with.
        /// </summary>
        public ComponentCollection RoomComponents { get; set; }

        /// <summary>
        /// The room types that can be used as the intermediate hall.
        /// </summary>
        public IRandPicker<PermissiveRoomGen<T>> GenericHalls { get; set; }

        /// <summary>
        /// Components that the newly added halls will be labeled with.
        /// </summary>
        public ComponentCollection HallComponents { get; set; }

        /// <summary>
        /// Applies this step to add connected rooms to the floor plan.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The floor plan to modify.</param>
        public override void ApplyToPath(IRandom rand, FloorPlan floorPlan)
        {
            int amount = this.Amount.Pick(rand);

            for (int kk = 0; kk < amount; kk++)
            {
                FloorPathBranch<T>.ListPathBranchExpansion? expansionResult = this.ChooseRoomExpansion(rand, floorPlan);

                if (!expansionResult.HasValue)
                    continue;

                var expansion = expansionResult.Value;

                RoomHallIndex from = expansion.From;
                if (expansion.Hall != null)
                {
                    floorPlan.AddHall(expansion.Hall, this.HallComponents.Clone(), from);
                    from = new RoomHallIndex(floorPlan.HallCount - 1, true);
                }

                floorPlan.AddRoom(expansion.Room, this.RoomComponents.Clone(), from);

                GenContextDebug.DebugProgress("Extended with Room");
            }
        }

        /// <summary>
        /// Chooses the expansion point and room configuration for adding a new room.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The floor plan to evaluate.</param>
        /// <returns>The expansion details if a valid placement is found; otherwise, null.</returns>
        public abstract FloorPathBranch<T>.ListPathBranchExpansion? ChooseRoomExpansion(IRandom rand, FloorPlan floorPlan);

        /// <summary>
        /// Returns a random generic room or hall that can fit in the specified floor.
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="floorPlan"></param>
        /// <param name="isHall"></param>
        /// <returns></returns>
        public virtual RoomGen<T> PrepareRoom(IRandom rand, FloorPlan floorPlan, bool isHall)
        {
            RoomGen<T> room;
            if (!isHall) // choose a room
                room = this.GenericRooms.Pick(rand).Copy();
            else // chose a hall
                room = this.GenericHalls.Pick(rand).Copy();

            // decide on acceptable border/size/fulfillables
            Loc size = room.ProposeSize(rand);
            if (size.X > floorPlan.DrawRect.Width)
                size.X = floorPlan.DrawRect.Width;
            if (size.Y > floorPlan.DrawRect.Height)
                size.Y = floorPlan.DrawRect.Height;
            room.PrepareSize(rand, size);
            return room;
        }

        /// <summary>
        /// Returns a string representation of this step.
        /// </summary>
        /// <returns>A string describing this step's configuration.</returns>
        public override string ToString()
        {
            return string.Format("{0}: Add:{1} Hall:{2}%", this.GetType().GetFormattedTypeName(), this.Amount, this.HallPercent);
        }
    }
}
