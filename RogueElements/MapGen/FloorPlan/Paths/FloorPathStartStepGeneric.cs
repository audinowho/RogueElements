// <copyright file="FloorPathStartStepGeneric.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements
{
    /// <summary>
    /// Base class for floor path steps that use configurable room and hall generators.
    /// </summary>
    /// <typeparam name="T">The generation context type, which must implement <see cref="IFloorPlanGenContext"/>.</typeparam>
    /// <remarks>
    /// This class extends <see cref="FloorPathStartStep{T}"/> with support for generic room and
    /// hall pickers, allowing flexible configuration of what room and hall types are generated.
    /// It also supports component labeling for the created rooms and halls.
    /// </remarks>
    /// <seealso cref="FloorPathBranch{T}"/>
    [Serializable]
    public abstract class FloorPathStartStepGeneric<T> : FloorPathStartStep<T>
        where T : class, IFloorPlanGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FloorPathStartStepGeneric{T}"/> class.
        /// </summary>
        protected FloorPathStartStepGeneric()
        {
            this.RoomComponents = new ComponentCollection();
            this.HallComponents = new ComponentCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FloorPathStartStepGeneric{T}"/> class with specified generators.
        /// </summary>
        /// <param name="genericRooms">The picker for room generators.</param>
        /// <param name="genericHalls">The picker for hall generators.</param>
        protected FloorPathStartStepGeneric(IRandPicker<RoomGen<T>> genericRooms, IRandPicker<PermissiveRoomGen<T>> genericHalls)
        {
            this.GenericRooms = genericRooms;
            this.GenericHalls = genericHalls;
            this.RoomComponents = new ComponentCollection();
            this.HallComponents = new ComponentCollection();
        }

        /// <summary>
        /// Gets or sets the picker for room generators used in the layout.
        /// </summary>
        public IRandPicker<RoomGen<T>> GenericRooms { get; set; }

        /// <summary>
        /// Gets or sets components to add to newly created rooms.
        /// </summary>
        public ComponentCollection RoomComponents { get; set; }

        /// <summary>
        /// Gets or sets the picker for hall generators used in the layout.
        /// </summary>
        public IRandPicker<PermissiveRoomGen<T>> GenericHalls { get; set; }

        /// <summary>
        /// Gets or sets components to add to newly created halls.
        /// </summary>
        public ComponentCollection HallComponents { get; set; }

        /// <summary>
        /// Applies this step to the map, validating that rooms and halls are configured.
        /// </summary>
        /// <param name="map">The generation context.</param>
        /// <exception cref="InvalidOperationException">Thrown when rooms or halls cannot be picked.</exception>
        public override void Apply(T map)
        {
            if (!this.GenericRooms.CanPick || !this.GenericHalls.CanPick)
                throw new InvalidOperationException("Can't create a path without rooms or halls.");

            base.Apply(map);
        }
    }
}
