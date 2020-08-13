// <copyright file="GridPathTwoSides.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class GridPathTwoSides<T> : GridPathStartStepGeneric<T>
        where T : class, IRoomGridGenContext
    {
        public GridPathTwoSides()
            : base()
        {
        }

        public Axis4 GapAxis { get; set; }

        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            // open rooms on both sides
            Loc gridSize = new Loc(floorPlan.GridWidth, floorPlan.GridHeight);
            int scalar = gridSize.GetScalar(this.GapAxis);
            int orth = gridSize.GetScalar(this.GapAxis.Orth());

            if (scalar < 2 || orth < 1)
                throw new InvalidOperationException("Not enough room to create path.");

            GenContextDebug.StepIn("Initial Rooms");

            for (int ii = 0; ii < orth; ii++)
            {
                // place the rooms at the edge
                floorPlan.AddRoom(this.GapAxis.CreateLoc(0, ii), this.GenericRooms.Pick(rand), this.RoomComponents.Clone());
                GenContextDebug.DebugProgress("Room");
                floorPlan.AddRoom(this.GapAxis.CreateLoc(scalar - 1, ii), this.GenericRooms.Pick(rand), this.RoomComponents.Clone());
                GenContextDebug.DebugProgress("Room");

                if (scalar > 2)
                {
                    // place hall rooms
                    Loc loc = this.GapAxis.CreateLoc(1, ii);
                    Loc size = this.GapAxis.CreateLoc(scalar - 2, 1);
                    floorPlan.AddRoom(new Rect(loc, size), this.GetDefaultGen(), this.HallComponents.Clone(), true);
                    GenContextDebug.DebugProgress("Mid Room");
                }
            }

            GenContextDebug.StepOut();

            GenContextDebug.StepIn("Connecting Sides");

            // halls connecting two tiers of the same side
            bool[][] connections = new bool[orth - 1][];
            for (int ii = 0; ii < orth - 1; ii++)
                connections[ii] = new bool[2];

            // add crosses
            for (int ii = 0; ii < orth - 1; ii++)
            {
                if (rand.Next(2) == 0)
                    connections[ii][0] = true;
                else
                    connections[ii][1] = true;
            }

            // paint hallways
            for (int ii = 0; ii < orth; ii++)
            {
                // place the halls at the sides
                if (ii < orth - 1)
                {
                    if (connections[ii][0])
                    {
                        this.PlaceOrientedHall(this.GapAxis.Orth(), 0, ii, 1, floorPlan, this.GenericHalls.Pick(rand));
                        GenContextDebug.DebugProgress("Side Connection");
                    }

                    if (connections[ii][1])
                    {
                        this.PlaceOrientedHall(this.GapAxis.Orth(), scalar - 1, ii, 1, floorPlan, this.GenericHalls.Pick(rand));
                        GenContextDebug.DebugProgress("Side Connection");
                    }
                }

                // place halls to bridge the gap
                this.PlaceOrientedHall(this.GapAxis, 0, ii, 1, floorPlan, this.GenericHalls.Pick(rand));
                if (scalar > 2)
                    this.PlaceOrientedHall(this.GapAxis, scalar - 1, ii, -1, floorPlan, this.GenericHalls.Pick(rand));
                GenContextDebug.DebugProgress("Bridge");
            }

            GenContextDebug.StepOut();
        }

        public void PlaceOrientedHall(Axis4 axis, int scalar, int orth, int scalarDiff, GridPlan floorPlan, PermissiveRoomGen<T> hallGen)
        {
            Loc loc = this.GapAxis.CreateLoc(scalar, orth);
            floorPlan.SetHall(new LocRay4(loc, axis.GetDir(scalarDiff)), hallGen, this.HallComponents.Clone());
        }
    }
}
