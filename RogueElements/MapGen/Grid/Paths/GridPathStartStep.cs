// <copyright file="GridPathStartStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RogueElements
{
    [Serializable]
    public abstract class GridPathStartStep<T> : GridPlanStep<T>
        where T : class, IRoomGridGenContext
    {
        public void CreateErrorPath(IRandom rand, GridPlan floorPlan)
        {
            floorPlan.Clear();
            floorPlan.AddRoom(new Loc(0, 0), GetDefaultGen());
        }
        
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

        public static void SafeAddHall(Loc room1, Loc room2, GridPlan floorPlan, IPermissiveRoomGen hallGen, IRoomGen roomGen, bool preferHall = false)
        {
            floorPlan.SetConnectingHall(room1, room2, hallGen);
            if (floorPlan.GetRoomPlan(room1) == null)
                floorPlan.AddRoom(room1, roomGen, false, preferHall);
            if (floorPlan.GetRoomPlan(room2) == null)
                floorPlan.AddRoom(room2, roomGen, false, preferHall);
        }

        public virtual RoomGen<T> GetDefaultGen()
        {
            return new RoomGenDefault<T>();
        }
    }
    
}
