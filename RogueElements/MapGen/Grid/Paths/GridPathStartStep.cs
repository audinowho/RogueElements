// <copyright file="GridPathStartStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public abstract class GridPathStartStep<T> : GridPlanStep<T>
        where T : class, IRoomGridGenContext
    {
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

        public virtual void CreateErrorPath(IRandom rand, GridPlan floorPlan)
        {
            floorPlan.Clear();
            floorPlan.AddRoom(new Loc(0, 0), this.GetDefaultGen(), new ComponentCollection());
        }

        public virtual RoomGen<T> GetDefaultGen()
        {
            return new RoomGenDefault<T>();
        }
    }
}
