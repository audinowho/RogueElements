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
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class AddDisconnectedRoomsStep<T> : FloorPlanStep<T>, IAddDisconnectedRoomsStep
        where T : class, IFloorPlanGenContext
    {
        public AddDisconnectedRoomsStep()
            : base()
        {
            this.Components = new ComponentCollection();
        }

        public AddDisconnectedRoomsStep(IRandPicker<RoomGen<T>> genericRooms)
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

                Rect allowedRange = Rect.FromPoints(floorPlan.DrawRect.Start, floorPlan.DrawRect.End - room.Draw.Size + new Loc(1));
                if (floorPlan.Wrap)
                    allowedRange = Rect.FromPoints(floorPlan.DrawRect.Start, floorPlan.DrawRect.End);

                for (int jj = 0; jj < 30; jj++)
                {
                    // place in a random location
                    Loc testStart = new Loc(
                       rand.Next(allowedRange.Start.X, allowedRange.End.X),
                       rand.Next(allowedRange.Start.Y, allowedRange.End.Y));

                    Rect tryRect = new Rect(testStart, room.Draw.Size);

                    tryRect.Inflate(1, 1);

                    List<RoomHallIndex> collisions = floorPlan.CheckCollision(tryRect);
                    if (collisions.Count == 0)
                    {
                        room.SetLoc(testStart);
                        floorPlan.AddRoom(room, this.Components.Clone());
                        GenContextDebug.DebugProgress("Place Disconnected Room");
                        break;
                    }
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: Add:{1}", this.GetType().Name, this.Amount);
        }
    }
}
