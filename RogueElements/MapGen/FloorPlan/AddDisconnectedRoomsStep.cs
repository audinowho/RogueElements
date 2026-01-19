// <copyright file="AddDisconnectedRoomsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements
{
    /// <summary>
    /// Adds new rooms to the floor plan without connecting them to existing rooms.
    /// </summary>
    /// <typeparam name="T">The generation context type, which must implement <see cref="IFloorPlanGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This step places rooms that are isolated from the existing layout, useful for creating
    /// secret rooms, bonus areas, or areas that will be connected later by other steps.
    /// </para>
    /// <para>
    /// Unlike <see cref="AddDisconnectedRoomsRandStep{T}"/>, this version exhaustively searches
    /// all possible positions, guaranteeing placement if any valid location exists but potentially
    /// causing performance issues on larger floors.
    /// </para>
    /// </remarks>
    /// <seealso cref="AddDisconnectedRoomsRandStep{T}"/>
    [Serializable]
    public class AddDisconnectedRoomsStep<T> : AddDisconnectedRoomsBaseStep<T>
        where T : class, IFloorPlanGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddDisconnectedRoomsStep{T}"/> class.
        /// </summary>
        public AddDisconnectedRoomsStep()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddDisconnectedRoomsStep{T}"/> class with specified room generators.
        /// </summary>
        /// <param name="genericRooms">The picker for room generators.</param>
        public AddDisconnectedRoomsStep(IRandPicker<RoomGen<T>> genericRooms)
            : base(genericRooms)
        {
        }

        /// <summary>
        /// Finds a viable location by exhaustively searching all possible positions.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The floor plan to search within.</param>
        /// <param name="roomSize">The size of the room to place.</param>
        /// <returns>A valid location if found; otherwise, null.</returns>
        protected override Loc? ChooseViableLoc(IRandom rand, FloorPlan floorPlan, Loc roomSize)
        {
            Rect allowedRange = Rect.FromPoints(floorPlan.DrawRect.Start, floorPlan.DrawRect.End - roomSize + new Loc(1));
            if (floorPlan.Wrap)
                allowedRange = Rect.FromPoints(floorPlan.DrawRect.Start, floorPlan.DrawRect.End);

            List<Loc> validStarts = new List<Loc>();

            // try all possibilities
            for (int xx = allowedRange.X; xx < allowedRange.End.X; xx++)
            {
                for (int yy = allowedRange.Y; yy < allowedRange.End.Y; yy++)
                {
                    Loc testStart = new Loc(xx, yy);
                    Rect tryRect = new Rect(testStart, roomSize);
                    tryRect.Inflate(1, 1);

                    List<RoomHallIndex> collisions = floorPlan.CheckCollision(tryRect);
                    if (collisions.Count == 0)
                        validStarts.Add(testStart);
                }
            }

            if (validStarts.Count > 0)
                return validStarts[rand.Next(validStarts.Count)];

            return null;
        }
    }
}
