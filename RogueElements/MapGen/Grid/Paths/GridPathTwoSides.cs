// <copyright file="GridPathTwoSides.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Creates a layout with rooms on opposite sides of the grid connected by a corridor.
    /// </summary>
    /// <typeparam name="T">The map context type, which must implement <see cref="IRoomGridGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This path generator places rooms along two opposite edges of the grid (left/right or top/bottom)
    /// and connects them with hallways spanning the gap. Additional halls may connect rooms
    /// on the same side.
    /// </para>
    /// <para>
    /// The layout creates a clear division between two "sides" of the dungeon, useful for
    /// scenarios where players need to cross from one area to another.
    /// </para>
    /// </remarks>
    /// <seealso cref="GridPathStartStepGeneric{T}"/>
    [Serializable]
    public class GridPathTwoSides<T> : GridPathStartStepGeneric<T>
        where T : class, IRoomGridGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridPathTwoSides{T}"/> class.
        /// </summary>
        public GridPathTwoSides()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets the axis along which the gap between sides runs.
        /// </summary>
        /// <remarks>
        /// <see cref="Axis4.Horiz"/> places rooms on left and right with a horizontal gap.
        /// <see cref="Axis4.Vert"/> places rooms on top and bottom with a vertical gap.
        /// </remarks>
        public Axis4 GapAxis { get; set; }

        /// <summary>
        /// Creates the two-sided layout with connecting hallways.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The grid plan to populate.</param>
        /// <exception cref="InvalidOperationException">Thrown when the grid is too small for this layout.</exception>
        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            // open rooms on both sides
            Loc gridSize = new Loc(floorPlan.GridWidth, floorPlan.GridHeight);
            int scalar = gridSize.GetScalar(this.GapAxis);
            int orth = gridSize.GetScalar(this.GapAxis.Orth());

            if (scalar < 2 || orth < 1)
                throw new InvalidOperationException("Not enough room to create path.");

            GenContextDebug.StepIn("Initial Rooms");

            try
            {
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
            }
            catch (Exception ex)
            {
                GenContextDebug.DebugError(ex);
            }

            GenContextDebug.StepOut();

            GenContextDebug.StepIn("Connecting Sides");

            try
            {
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
            }
            catch (Exception ex)
            {
                GenContextDebug.DebugError(ex);
            }

            GenContextDebug.StepOut();
        }

        /// <summary>
        /// Places a hall at the specified position in a direction determined by the axis.
        /// </summary>
        /// <param name="axis">The axis along which to place the hall.</param>
        /// <param name="scalar">The position along the gap axis.</param>
        /// <param name="orth">The position along the orthogonal axis.</param>
        /// <param name="scalarDiff">The direction (+1 or -1) to extend the hall.</param>
        /// <param name="floorPlan">The grid plan to modify.</param>
        /// <param name="hallGen">The hall generator to use.</param>
        public void PlaceOrientedHall(Axis4 axis, int scalar, int orth, int scalarDiff, GridPlan floorPlan, PermissiveRoomGen<T> hallGen)
        {
            Loc loc = this.GapAxis.CreateLoc(scalar, orth);
            floorPlan.SetHall(new LocRay4(loc, axis.GetDir(scalarDiff)), hallGen, this.HallComponents.Clone());
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}: Axis:{1}", this.GetType().GetFormattedTypeName(), this.GapAxis);
        }
    }
}
