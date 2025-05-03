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
    /// Takes the current floor plan and adds new rooms that are disconnected from existing rooms.
    /// Randomly picks a location to spawn a new room in a finite number of times before giving up.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    [Serializable]
    public class AddDisconnectedRoomsRandStep<TGenContext, TTile> : AddDisconnectedRoomsBaseStep<TGenContext, TTile>
        where TGenContext : class, IFloorPlanGenContext<TTile>
        where TTile : ITile<TTile>
    {
        public AddDisconnectedRoomsRandStep()
            : base()
        {
        }

        public AddDisconnectedRoomsRandStep(IRandPicker<RoomGen<TGenContext, TTile>> genericRooms)
            : base(genericRooms)
        {
        }

        protected override Loc? ChooseViableLoc(IRandom rand, FloorPlan<TTile> floorPlan, Loc roomSize)
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
