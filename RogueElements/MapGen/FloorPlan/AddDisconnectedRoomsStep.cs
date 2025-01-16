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
    /// Takes the current floor plan and adds new rooms that are disconnected from existing rooms.
    /// Sweeps through the entire floor to fit in the new rooms.
    /// Guaranteed to spawn the room, but can cause performance problems for larger floors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class AddDisconnectedRoomsStep<T> : AddDisconnectedRoomsBaseStep<T>
        where T : class, IFloorPlanGenContext
    {
        public AddDisconnectedRoomsStep()
            : base()
        {
        }

        public AddDisconnectedRoomsStep(IRandPicker<RoomGen<T>> genericRooms)
            : base(genericRooms)
        {
        }

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
