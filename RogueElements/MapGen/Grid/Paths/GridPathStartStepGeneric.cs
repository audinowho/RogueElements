// <copyright file="GridPathStartStepGeneric.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Base class for grid path generators that use configurable room and hall generators.
    /// </summary>
    /// <typeparam name="T">The map context type, which must implement <see cref="IRoomGridGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This class extends <see cref="GridPathStartStep{T}"/> by adding properties for
    /// room and hall generators with their associated components. Most path generation
    /// algorithms inherit from this class rather than directly from GridPathStartStep.
    /// </para>
    /// <para>
    /// Subclasses must ensure that both <see cref="GenericRooms"/> and <see cref="GenericHalls"/>
    /// are configured with at least one option before generation, or an exception will be thrown.
    /// </para>
    /// </remarks>
    /// <seealso cref="GridPathStartStep{T}"/>
    [Serializable]
    public abstract class GridPathStartStepGeneric<T> : GridPathStartStep<T>
        where T : class, IRoomGridGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridPathStartStepGeneric{T}"/> class.
        /// </summary>
        protected GridPathStartStepGeneric()
        {
            this.RoomComponents = new ComponentCollection();
            this.HallComponents = new ComponentCollection();
        }

        /// <summary>
        /// Gets or sets the room generators to use for layout rooms.
        /// </summary>
        public IRandPicker<RoomGen<T>> GenericRooms { get; set; }

        /// <summary>
        /// Gets or sets the components to attach to newly created rooms.
        /// </summary>
        public ComponentCollection RoomComponents { get; set; }

        /// <summary>
        /// Gets or sets the hall generators to use for connecting halls.
        /// </summary>
        public IRandPicker<PermissiveRoomGen<T>> GenericHalls { get; set; }

        /// <summary>
        /// Gets or sets the components to attach to newly created halls.
        /// </summary>
        public ComponentCollection HallComponents { get; set; }

        /// <summary>
        /// Validates that room and hall generators are configured before applying.
        /// </summary>
        /// <param name="map">The map context to modify.</param>
        /// <exception cref="InvalidOperationException">Thrown when no room or hall generators are configured.</exception>
        public override void Apply(T map)
        {
            if (!this.GenericRooms.CanPick || !this.GenericHalls.CanPick)
                throw new InvalidOperationException("Can't create a path without rooms or halls.");

            base.Apply(map);
        }
    }
}
