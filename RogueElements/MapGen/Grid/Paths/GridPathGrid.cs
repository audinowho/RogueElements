// <copyright file="GridPathGrid.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Populates the empty floor plan of a map by creating a path consisting of rooms on the perimeter, with hallways creating a grid inwards.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class GridPathGrid<T> : GridPathStartStepGeneric<T>
        where T : class, IRoomGridGenContext
    {
        public GridPathGrid()
            : base()
        {
        }

        /// <summary>
        /// The percentage of rooms at the perimeter that are NOT default 1-tile halls.
        /// </summary>
        public int RoomRatio { get; set; }

        /// <summary>
        /// The amount of additional halls added to connect adjacent rooms at the perimeter.
        /// </summary>
        public int HallRatio { get; set; }

        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            if (floorPlan.GridWidth < 3 || floorPlan.GridHeight < 3)
                throw new InvalidOperationException("Not enough room to create path.");

            int roomMax = (2 * (floorPlan.GridWidth - 2)) + (2 * (floorPlan.GridHeight - 2));
            int roomOpen = roomMax * this.RoomRatio / 100;
            if (roomOpen < 1)
                roomOpen = 1;

            GenContextDebug.StepIn("Inner Grid");

            try
            {
                // set hallrooms in middle of grid and open hallways between them
                for (int x = 1; x < floorPlan.GridWidth - 1; x++)
                {
                    for (int y = 1; y < floorPlan.GridHeight - 1; y++)
                    {
                        floorPlan.AddRoom(new Loc(x, y), this.GetDefaultGen(), this.HallComponents.Clone(), true);

                        if (x > 1)
                            floorPlan.SetHall(new LocRay4(new Loc(x, y), Dir4.Left), this.GenericHalls.Pick(rand), this.HallComponents.Clone());
                        if (y > 1)
                            floorPlan.SetHall(new LocRay4(new Loc(x, y), Dir4.Up), this.GenericHalls.Pick(rand), this.HallComponents.Clone());

                        GenContextDebug.DebugProgress("Room");
                    }
                }
            }
            catch (Exception ex)
            {
                GenContextDebug.DebugError(ex);
            }

            GenContextDebug.StepOut();

            GenContextDebug.StepIn("Outer Rooms");

            try
            {
                // open random rooms on all sides
                for (int x = 1; x < floorPlan.GridWidth - 1; x++)
                {
                    if (RollRatio(rand, ref roomOpen, ref roomMax))
                    {
                        floorPlan.AddRoom(new Loc(x, 0), this.GenericRooms.Pick(rand), this.RoomComponents.Clone());
                        floorPlan.SetHall(new LocRay4(new Loc(x, 0), Dir4.Down), this.GenericHalls.Pick(rand), this.HallComponents.Clone());
                        GenContextDebug.DebugProgress("Room");
                    }

                    if (RollRatio(rand, ref roomOpen, ref roomMax))
                    {
                        floorPlan.AddRoom(new Loc(x, floorPlan.GridHeight - 1), this.GenericRooms.Pick(rand), this.RoomComponents.Clone());
                        floorPlan.SetHall(new LocRay4(new Loc(x, floorPlan.GridHeight - 1), Dir4.Up), this.GenericHalls.Pick(rand), this.HallComponents.Clone());
                        GenContextDebug.DebugProgress("Room");
                    }
                }

                for (int y = 1; y < floorPlan.GridHeight - 1; y++)
                {
                    if (RollRatio(rand, ref roomOpen, ref roomMax))
                    {
                        floorPlan.AddRoom(new Loc(0, y), this.GenericRooms.Pick(rand), this.RoomComponents.Clone());
                        floorPlan.SetHall(new LocRay4(new Loc(0, y), Dir4.Right), this.GenericHalls.Pick(rand), this.HallComponents.Clone());
                        GenContextDebug.DebugProgress("Room");
                    }

                    if (RollRatio(rand, ref roomOpen, ref roomMax))
                    {
                        floorPlan.AddRoom(new Loc(floorPlan.GridWidth - 1, y), this.GenericRooms.Pick(rand), this.RoomComponents.Clone());
                        floorPlan.SetHall(new LocRay4(new Loc(floorPlan.GridWidth - 1, y), Dir4.Left), this.GenericHalls.Pick(rand), this.HallComponents.Clone());
                        GenContextDebug.DebugProgress("Room");
                    }
                }
            }
            catch (Exception ex)
            {
                GenContextDebug.DebugError(ex);
            }

            GenContextDebug.StepOut();

            GenContextDebug.StepIn("Extra Halls");

            // get all halls eligible to be opened
            List<Loc> hHallSites = new List<Loc>();
            List<Loc> vHallSites = new List<Loc>();
            try
            {
                for (int x = 1; x < floorPlan.GridWidth; x++)
                {
                    if (floorPlan.GetRoomPlan(new Loc(x, 0)) != null || floorPlan.GetRoomPlan(new Loc(x - 1, 0)) != null)
                        hHallSites.Add(new Loc(x, 0));
                    if (floorPlan.GetRoomPlan(new Loc(x, floorPlan.GridHeight - 1)) != null || floorPlan.GetRoomPlan(new Loc(x - 1, floorPlan.GridHeight - 1)) != null)
                        hHallSites.Add(new Loc(x, floorPlan.GridHeight - 1));
                }

                for (int y = 1; y < floorPlan.GridHeight; y++)
                {
                    if (floorPlan.GetRoomPlan(new Loc(0, y)) != null || floorPlan.GetRoomPlan(new Loc(0, y - 1)) != null)
                        vHallSites.Add(new Loc(0, y));
                    if (floorPlan.GetRoomPlan(new Loc(floorPlan.GridWidth - 1, y)) != null || floorPlan.GetRoomPlan(new Loc(floorPlan.GridWidth - 1, y - 1)) != null)
                        vHallSites.Add(new Loc(floorPlan.GridWidth - 1, y));
                }

                int halls = hHallSites.Count + vHallSites.Count;
                int placedHalls = halls * this.HallRatio / 100;

                // place the halls
                for (int ii = 0; ii < hHallSites.Count; ii++)
                {
                    if (rand.Next() % halls < placedHalls)
                    {
                        SafeAddHall(new LocRay4(hHallSites[ii], Dir4.Left), floorPlan, this.GenericHalls.Pick(rand), this.GenericRooms.Pick(rand), this.RoomComponents, this.HallComponents);
                        GenContextDebug.DebugProgress("Hall");
                        placedHalls--;
                    }

                    halls--;
                }

                for (int ii = 0; ii < vHallSites.Count; ii++)
                {
                    if (rand.Next() % halls < placedHalls)
                    {
                        SafeAddHall(new LocRay4(vHallSites[ii], Dir4.Up), floorPlan, this.GenericHalls.Pick(rand), this.GenericRooms.Pick(rand), this.RoomComponents, this.HallComponents);
                        GenContextDebug.DebugProgress("Hall");
                        placedHalls--;
                    }

                    halls--;
                }
            }
            catch (Exception ex)
            {
                GenContextDebug.DebugError(ex);
            }

            GenContextDebug.StepOut();
        }

        public override string ToString()
        {
            return string.Format("{0}: Room:{1}% Hall:{2}%", this.GetType().Name, this.RoomRatio, this.HallRatio);
        }
    }
}
