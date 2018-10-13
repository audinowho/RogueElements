using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements
{
    [Serializable]
    public abstract class FloorPathStartStep<T> : FloorPlanStep<T>
        where T : class, IFloorPlanGenContext
    {

        public void CreateErrorPath(IRandom rand, FloorPlan floorPlan)
        {
            floorPlan.Clear();
            RoomGen<T> room = GetDefaultGen();
            room.PrepareSize(rand, new Loc(1));
            room.SetLoc(new Loc());
            floorPlan.AddRoom(room, false);
        }

        public virtual RoomGen<T> GetDefaultGen()
        {
            return new RoomGenDefault<T>();
        }
    }
}
