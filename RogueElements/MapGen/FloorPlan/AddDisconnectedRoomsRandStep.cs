// <copyright file="AddDisconnectedRoomsRandStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements
{
    /// <summary>
    /// Adds new rooms to the floor plan without connecting them, using random sampling with limited retries.
    /// </summary>
    /// <typeparam name="T">The generation context type, which must implement <see cref="IFloorPlanGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This step places rooms that are isolated from the existing layout, useful for creating
    /// secret rooms, bonus areas, or areas that will be connected later by other steps.
    /// </para>
    /// <para>
    /// Unlike <see cref="AddDisconnectedRoomsStep{T}"/>, this version randomly samples positions
    /// with a limited number of retries (30 attempts), providing better performance but potentially
    /// missing valid placements in crowded floor plans.
    /// </para>
    /// </remarks>
    /// <seealso cref="AddDisconnectedRoomsStep{T}"/>
    [Serializable]
    public class AddDisconnectedRoomsRandStep<T> : AddDisconnectedRoomsBaseStep<T>
        where T : class, IFloorPlanGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddDisconnectedRoomsRandStep{T}"/> class.
        /// </summary>
        public AddDisconnectedRoomsRandStep()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddDisconnectedRoomsRandStep{T}"/> class with specified room generators.
        /// </summary>
        /// <param name="genericRooms">The picker for room generators.</param>
        public AddDisconnectedRoomsRandStep(IRandPicker<RoomGen<T>> genericRooms)
            : base(genericRooms)
        {
        }

        /// <summary>
        /// Finds a viable location by randomly sampling positions up to 30 times.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The floor plan to search within.</param>
        /// <param name="roomSize">The size of the room to place.</param>
        /// <returns>A valid location if found within the retry limit; otherwise, null.</returns>
        protected override Loc? ChooseViableLoc(IRandom rand, FloorPlan floorPlan, Loc roomSize)
        {
            Rect allowedRange = Rect.FromPoints(floorPlan.DrawRect.Start, floorPlan.DrawRect.End - roomSize + new Loc(1));
            if (floorPlan.Wrap)
                allowedRange = Rect.FromPoints(floorPlan.DrawRect.Start, floorPlan.DrawRect.End);

            for (int jj = 0; jj < 30; jj++)
            {
                // place in a random location
                Loc testStart = new Loc(
                   rand.Next(allowedRange.Start.X, allowedRange.End.X),
                   rand.Next(allowedRange.Start.Y, allowedRange.End.Y));

                Rect tryRect = new Rect(testStart, roomSize);

                tryRect.Inflate(1, 1);

                List<RoomHallIndex> collisions = floorPlan.CheckCollision(tryRect);
                if (collisions.Count == 0)
                    return testStart;
            }

            return null;
        }
    }
}
