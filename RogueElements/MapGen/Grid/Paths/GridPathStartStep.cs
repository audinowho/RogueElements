using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements
{
    [Serializable]
    public abstract class GridPathStartStep<T> : GridPlanStep<T>
        where T : class, IRoomGridGenContext
    {

        public void CreateErrorPath(IRandom rand, GridPlan floorPlan)
        {
            floorPlan.Clear();
            floorPlan.AddRoom(0, 0, GetDefaultGen());
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

        public static void SafeAddHall(Loc room1, Loc room2, GridPlan floorPlan, IPermissiveRoomGen hallGen, IRoomGen roomGen)
        {
            floorPlan.SetConnectingHall(room1, room2, hallGen);
            if (floorPlan.GetRoomPlan(room1.X, room1.Y) == null)
                floorPlan.AddRoom(room1.X, room1.Y, roomGen);
            if (floorPlan.GetRoomPlan(room2.X, room2.Y) == null)
                floorPlan.AddRoom(room2.X, room2.Y, roomGen);
        }

        public virtual RoomGen<T> GetDefaultGen()
        {
            return new RoomGenDefault<T>();
        }
    }
    
}
