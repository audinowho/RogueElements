// <copyright file="AddDisconnectedRoomsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements
{
    [Serializable]
    public class AddDisconnectedRoomsStep<T> : FloorPlanStep<T>
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

        public IRandPicker<RoomGen<T>> GenericRooms { get; set; }

        public ComponentCollection Components { get; set; }

        public RandRange Amount { get; set; }

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

                for (int jj = 0; jj < 30; jj++)
                {
                    // place in a random location
                    Loc testStart = new Loc(
                       rand.Next(floorPlan.DrawRect.Left, floorPlan.DrawRect.Right - room.Draw.Width + 1),
                       rand.Next(floorPlan.DrawRect.Top, floorPlan.DrawRect.Bottom - room.Draw.Height + 1));

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
    }
}
