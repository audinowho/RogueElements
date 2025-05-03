// <copyright file="GridPathSpecific.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Populates an empty grid plan of a map by creating a specific path of rooms and hallways.
    /// VERY EDITOR UNFRIENDLY
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    [Serializable]
    public class GridPathSpecific<TGenContext, TTile> : GridPathStartStep<TGenContext, TTile>
        where TGenContext : class, IRoomGridGenContext<TTile>
        where TTile : ITile<TTile>
    {
        public GridPathSpecific()
            : base()
        {
            this.SpecificRooms = new List<SpecificGridRoomPlan<TGenContext, TTile>>();
            this.HallComponents = new ComponentCollection();
        }

        /// <summary>
        /// The rooms to place, and where they go.
        /// </summary>
        public List<SpecificGridRoomPlan<TGenContext, TTile>> SpecificRooms { get; set; }

        /// <summary>
        /// The full array of vertical halls.
        /// </summary>
        public PermissiveRoomGen<TGenContext, TTile>[][] SpecificVHalls { get; set; }

        /// <summary>
        /// The full array of horizontal halls.
        /// </summary>
        public PermissiveRoomGen<TGenContext, TTile>[][] SpecificHHalls { get; set; }

        /// <summary>
        /// Components that the halls will be labeled with.
        /// </summary>
        public ComponentCollection HallComponents { get; set; }

        public static void UnsafeAddHall(LocRay4 locRay, GridPlan<TTile> floorPlan, IPermissiveRoomGen<TTile> hallGen, ComponentCollection components)
        {
            floorPlan.SetHall(locRay, hallGen, components.Clone());
            GenContextDebug.DebugProgress("Hall");
            if (floorPlan.GetRoomPlan(locRay.Loc) == null || floorPlan.GetRoomPlan(locRay.Traverse(1)) == null)
            {
                floorPlan.Clear();
                throw new InvalidOperationException("Can't create a hall without rooms to connect!");
            }
        }

        public override void ApplyToPath(IRandom rand, GridPlan<TTile> floorPlan)
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
