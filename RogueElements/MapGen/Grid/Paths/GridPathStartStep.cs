// <copyright file="GridPathStartStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Base class for grid path generators that create initial room and hall layouts.
    /// </summary>
    /// <typeparam name="T">The map context type, which must implement <see cref="IRoomGridGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// Grid path start steps are responsible for populating an empty grid plan with
    /// rooms and connecting halls. They define the overall structure and connectivity
    /// of the dungeon layout.
    /// </para>
    /// <para>
    /// Subclasses implement specific layout patterns such as branching paths, circles,
    /// grids, or crosses. Helper methods are provided for common operations like
    /// adding halls with their connected rooms.
    /// </para>
    /// </remarks>
    /// <seealso cref="GridPlanStep{T}"/>
    /// <seealso cref="GridPathStartStepGeneric{T}"/>
    [Serializable]
    public abstract class GridPathStartStep<T> : GridPlanStep<T>
        where T : class, IRoomGridGenContext
    {
        /// <summary>
        /// Randomly determines whether to perform an action based on a ratio and maximum count.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="ratio">The number of actions remaining to perform. Decremented if successful.</param>
        /// <param name="max">The maximum number of opportunities remaining. Always decremented.</param>
        /// <returns>True if the action should be performed; otherwise, false.</returns>
        /// <remarks>
        /// This method is used for distributing a fixed number of actions across a set of opportunities,
        /// ensuring the target ratio is achieved on average.
        /// </remarks>
        public static bool RollRatio(IRandom rand, ref int ratio, ref int max)
        {
            bool roll = false;
            if (rand.Next() % max < ratio)
            {
                roll = true;
                ratio--;
            }

            max--;
            return roll;
        }

        /// <summary>
        /// Adds a hall and ensures rooms exist on both ends.
        /// </summary>
        /// <param name="locRay">The location and direction of the hall.</param>
        /// <param name="floorPlan">The grid plan to modify.</param>
        /// <param name="hallGen">The hall generator to use.</param>
        /// <param name="roomGen">The room generator to use for missing rooms.</param>
        /// <param name="roomComponents">The components to attach to new rooms.</param>
        /// <param name="hallComponents">The components to attach to the hall.</param>
        /// <param name="preferHall">Whether new rooms should be treated as halls.</param>
        public static void SafeAddHall(LocRay4 locRay, GridPlan floorPlan, IPermissiveRoomGen hallGen, IRoomGen roomGen, ComponentCollection roomComponents, ComponentCollection hallComponents, bool preferHall = false)
        {
            floorPlan.SetHall(locRay, hallGen, hallComponents.Clone());
            ComponentCollection collection = preferHall ? hallComponents : roomComponents;
            if (floorPlan.GetRoomPlan(locRay.Loc) == null)
                floorPlan.AddRoom(locRay.Loc, roomGen, collection.Clone(), preferHall);
            Loc dest = locRay.Traverse(1);
            if (floorPlan.GetRoomPlan(dest) == null)
                floorPlan.AddRoom(dest, roomGen, collection.Clone(), preferHall);
        }

        /// <summary>
        /// Creates a minimal fallback path when the main algorithm fails.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The grid plan to populate.</param>
        public virtual void CreateErrorPath(IRandom rand, GridPlan floorPlan)
        {
            floorPlan.Clear();
            floorPlan.AddRoom(new Loc(0, 0), this.GetDefaultGen(), new ComponentCollection());
        }

        /// <summary>
        /// Gets the default room generator used for placeholder rooms.
        /// </summary>
        /// <returns>A minimal single-tile room generator.</returns>
        public virtual RoomGen<T> GetDefaultGen()
        {
            return new RoomGenDefault<T>();
        }
    }
}
