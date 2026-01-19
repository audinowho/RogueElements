// <copyright file="IGridPathGrid.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace RogueElements
{
    /// <summary>
    /// Defines the configuration interface for grid path generators.
    /// </summary>
    public interface IGridPathGrid
    {
        /// <summary>
        /// Gets or sets the percentage of perimeter rooms that are full rooms.
        /// </summary>
        int RoomRatio { get; set; }

        /// <summary>
        /// Gets or sets the percentage of additional halls connecting perimeter rooms.
        /// </summary>
        int HallRatio { get; set; }
    }

    /// <summary>
    /// Creates a grid-like layout with an inner corridor network and perimeter rooms.
    /// </summary>
    /// <typeparam name="T">The map context type, which must implement <see cref="IRoomGridGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This path generator fills the interior of the grid with a network of connected
    /// single-tile halls, then adds rooms around the perimeter. Additional halls can
    /// connect adjacent perimeter rooms.
    /// </para>
    /// <para>
    /// The grid must be at least 3x3 to have both an interior network and a perimeter.
    /// </para>
    /// </remarks>
    /// <seealso cref="GridPathStartStepGeneric{T}"/>
    /// <seealso cref="IGridPathGrid"/>
    [Serializable]
    public class GridPathGrid<T> : GridPathStartStepGeneric<T>, IGridPathGrid
        where T : class, IRoomGridGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridPathGrid{T}"/> class.
        /// </summary>
        public GridPathGrid()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets the percentage of perimeter rooms that are full rooms rather than halls.
        /// </summary>
        public int RoomRatio { get; set; }

        /// <summary>
        /// Gets or sets the percentage of additional halls connecting adjacent perimeter rooms.
        /// </summary>
        public int HallRatio { get; set; }

        /// <summary>
        /// Creates the grid layout with inner corridors and perimeter rooms.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The grid plan to populate.</param>
        /// <exception cref="InvalidOperationException">Thrown when the grid is smaller than 3x3.</exception>
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
            return string.Format("{0}: Room:{1}% Hall:{2}%", this.GetType().GetFormattedTypeName(), this.RoomRatio, this.HallRatio);
        }
    }
}
