// <copyright file="GridPathSpecific.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Creates a grid layout with explicitly specified room and hall positions.
    /// </summary>
    /// <typeparam name="T">The map context type, which must implement <see cref="IRoomGridGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This path generator creates a layout from explicit room and hall specifications
    /// rather than using procedural algorithms. Every room and hall must be manually defined.
    /// </para>
    /// <para>
    /// <strong>Warning:</strong> This class is difficult to use in visual editors because
    /// the hall arrays must exactly match the grid dimensions. It is primarily useful for
    /// creating specific, handcrafted layouts or for testing purposes.
    /// </para>
    /// </remarks>
    /// <seealso cref="GridPathStartStep{T}"/>
    /// <seealso cref="SpecificGridRoomPlan{T}"/>
    [Serializable]
    public class GridPathSpecific<T> : GridPathStartStep<T>
        where T : class, IRoomGridGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridPathSpecific{T}"/> class.
        /// </summary>
        public GridPathSpecific()
            : base()
        {
            this.SpecificRooms = new List<SpecificGridRoomPlan<T>>();
            this.HallComponents = new ComponentCollection();
        }

        /// <summary>
        /// Gets or sets the list of rooms to place with their specific positions.
        /// </summary>
        public List<SpecificGridRoomPlan<T>> SpecificRooms { get; set; }

        /// <summary>
        /// Gets or sets the 2D array of vertical hall generators.
        /// </summary>
        /// <remarks>
        /// The array dimensions must be [GridWidth][GridHeight-1]. Null entries indicate no hall.
        /// </remarks>
        public PermissiveRoomGen<T>[][] SpecificVHalls { get; set; }

        /// <summary>
        /// Gets or sets the 2D array of horizontal hall generators.
        /// </summary>
        /// <remarks>
        /// The array dimensions must be [GridWidth-1][GridHeight]. Null entries indicate no hall.
        /// </remarks>
        public PermissiveRoomGen<T>[][] SpecificHHalls { get; set; }

        /// <summary>
        /// Gets or sets the components to attach to all halls.
        /// </summary>
        public ComponentCollection HallComponents { get; set; }

        /// <summary>
        /// Adds a hall without checking if connecting rooms exist.
        /// </summary>
        /// <param name="locRay">The location and direction of the hall.</param>
        /// <param name="floorPlan">The grid plan to modify.</param>
        /// <param name="hallGen">The hall generator to use.</param>
        /// <param name="components">The components to attach to the hall.</param>
        /// <exception cref="InvalidOperationException">Thrown when the hall would not connect two rooms.</exception>
        public static void UnsafeAddHall(LocRay4 locRay, GridPlan floorPlan, IPermissiveRoomGen hallGen, ComponentCollection components)
        {
            floorPlan.SetHall(locRay, hallGen, components.Clone());
            GenContextDebug.DebugProgress("Hall");
            if (floorPlan.GetRoomPlan(locRay.Loc) == null || floorPlan.GetRoomPlan(locRay.Traverse(1)) == null)
            {
                floorPlan.Clear();
                throw new InvalidOperationException("Can't create a hall without rooms to connect!");
            }
        }

        /// <summary>
        /// Places the specified rooms and halls into the grid plan.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The grid plan to populate.</param>
        /// <exception cref="InvalidOperationException">Thrown when hall array dimensions do not match the grid size.</exception>
        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            if (floorPlan.GridWidth != this.SpecificVHalls.Length ||
                floorPlan.GridWidth - 1 != this.SpecificHHalls.Length ||
                floorPlan.GridHeight - 1 != this.SpecificVHalls[0].Length ||
                floorPlan.GridHeight != this.SpecificHHalls[0].Length)
                throw new InvalidOperationException("Incorrect hall path sizes.");

            foreach (var chosenRoom in this.SpecificRooms)
            {
                floorPlan.AddRoom(chosenRoom.Bounds, chosenRoom.RoomGen, chosenRoom.Components.Clone(), chosenRoom.PreferHall);
                GenContextDebug.DebugProgress("Room");
            }

            // place halls
            for (int x = 0; x < this.SpecificVHalls.Length; x++)
            {
                for (int y = 0; y < this.SpecificHHalls[0].Length; y++)
                {
                    if (x > 0)
                    {
                        if (this.SpecificHHalls[x - 1][y] != null)
                            UnsafeAddHall(new LocRay4(new Loc(x, y), Dir4.Left), floorPlan, this.SpecificHHalls[x - 1][y], this.HallComponents);
                    }

                    if (y > 0)
                    {
                        if (this.SpecificVHalls[x][y - 1] != null)
                            UnsafeAddHall(new LocRay4(new Loc(x, y), Dir4.Up), floorPlan, this.SpecificVHalls[x][y - 1], this.HallComponents);
                    }
                }
            }
        }
    }
}
