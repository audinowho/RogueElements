// <copyright file="SetGridSpecialRoomStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Takes an existing grid plan and changes one of the rooms into the specified room type.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    [Serializable]
    public class SetGridSpecialRoomStep<TGenContext, TTile> : GridPlanStep<TGenContext, TTile>
        where TGenContext : class, IRoomGridGenContext<TTile>
        where TTile : ITile<TTile>
    {
        public SetGridSpecialRoomStep()
            : base()
        {
            this.Filters = new List<BaseRoomFilter<TTile>>();
            this.RoomComponents = new ComponentCollection();
        }

        /// <summary>
        /// The type of room to place.  It can be chosen out of several possibilities, but only one room will be placed.
        /// </summary>
        public IRandPicker<RoomGen<TGenContext, TTile>> Rooms { get; set; }

        /// <summary>
        /// Determines which rooms are eligible to be turned into the new room type.
        /// </summary>
        public List<BaseRoomFilter<TTile>> Filters { get; set; }

        /// <summary>
        /// Components that the newly added room will be labeled with.
        /// </summary>
        public ComponentCollection RoomComponents { get; set; }

        public override void ApplyToPath(IRandom rand, GridPlan<TTile> floorPlan)
        {
            IRoomGen<TTile> newGen = this.Rooms.Pick(rand).Copy();
            Loc size = newGen.ProposeSize(rand);

            // choose certain rooms in the list to be special rooms
            // special rooms are required; so make sure they don't overlap
            List<int> room_indices = new List<int>();
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                GridRoomPlan<TTile> plan = floorPlan.GetRoomPlan(ii);
                if (!BaseRoomFilter<TTile>.PassesAllFilters(plan, this.Filters))
                    continue;
                if (plan.PreferHall)
                    continue;
                Loc boundsSize = GetBoundsSize(floorPlan, plan);
                if (boundsSize.X >= size.X && boundsSize.Y >= size.Y)
                    room_indices.Add(ii);
            }

            if (room_indices.Count > 0)
            {
                int ind = rand.Next(room_indices.Count);
                GridRoomPlan<TTile> plan = floorPlan.GetRoomPlan(room_indices[ind]);
                plan.RoomGen = newGen;
                foreach (RoomComponent component in this.RoomComponents)
                    plan.Components.Set(component.Clone());
                room_indices.RemoveAt(ind);
                GenContextDebug.DebugProgress("Set Special Room");
            }
        }

        private static Loc GetBoundsSize(GridPlan<TTile> floorPlan, GridRoomPlan<TTile> plan)
        {
            Loc cellSize = new Loc(plan.Bounds.Width * floorPlan.WidthPerCell, plan.Bounds.Height * floorPlan.HeightPerCell);
            Loc cellWallSize = new Loc((plan.Bounds.Width - 1) * floorPlan.CellWall, (plan.Bounds.Height - 1) * floorPlan.CellWall);
            return cellSize + cellWallSize;
        }
    }
}
