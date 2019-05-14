// <copyright file="GridPathSpecific.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class GridPathSpecific<T> : GridPathStartStep<T>
        where T : class, IRoomGridGenContext
    {
        public List<SpecificGridRoomPlan<T>> SpecificRooms;
        public PermissiveRoomGen<T>[][] SpecificVHalls;
        public PermissiveRoomGen<T>[][] SpecificHHalls;

        public GridPathSpecific()
            : base()
        {
            SpecificRooms = new List<SpecificGridRoomPlan<T>>();
        }

        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            if (floorPlan.GridWidth != SpecificVHalls.Length ||
                floorPlan.GridWidth - 1 != SpecificHHalls.Length ||
                floorPlan.GridHeight - 1 != SpecificVHalls[0].Length ||
                floorPlan.GridHeight != SpecificHHalls[0].Length)
                throw new InvalidOperationException("Incorrect hall path sizes.");

            foreach (var chosenRoom in SpecificRooms)
            {
                floorPlan.AddRoom(chosenRoom.Bounds, chosenRoom.RoomGen, chosenRoom.Immutable, chosenRoom.PreferHall);
                GenContextDebug.DebugProgress("Room");
            }

            //place halls
            for (int x = 0; x < SpecificVHalls.Length; x++)
            {
                for (int y = 0; y < SpecificHHalls[0].Length; y++)
                {
                    if (x > 0)
                    {
                        if (SpecificHHalls[x - 1][y] != null)
                            UnsafeAddHall(new LocRay4(new Loc(x, y), Dir4.Left), floorPlan, SpecificHHalls[x - 1][y]);
                    }
                    if (y > 0)
                    {
                        if (SpecificVHalls[x][y - 1] != null)
                            UnsafeAddHall(new LocRay4(new Loc(x, y), Dir4.Up), floorPlan, SpecificVHalls[x][y - 1]);
                    }
                }
            }
        }

        public void UnsafeAddHall(LocRay4 locRay, GridPlan floorPlan, IPermissiveRoomGen hallGen)
        {
            floorPlan.SetHall(locRay, hallGen);
            GenContextDebug.DebugProgress("Hall");
            if (floorPlan.GetRoomPlan(locRay.Loc) == null || floorPlan.GetRoomPlan(locRay.Traverse(1)) == null)
            {
                floorPlan.Clear();
                throw new InvalidOperationException("Can't create a hall without rooms to connect!");
            }
        }
    }

    [Serializable]
    public class SpecificGridRoomPlan<T> where T : ITiledGenContext
    {
        public Rect Bounds;
        public bool Immutable;
        public bool PreferHall;
        public RoomGen<T> RoomGen;

        public SpecificGridRoomPlan(Rect bounds, RoomGen<T> roomGen)
        {
            Bounds = bounds;
            RoomGen = roomGen;
        }

        public bool CountsAsHall()
        {
            if (!PreferHall)
                return false;
            return RoomGen is IPermissiveRoomGen;
        }
    }
}
