using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class DrawGridToFloorStep<T> : GenStep<T> where T : class, IRoomGridGenContext
    {
        public DrawGridToFloorStep() { }

        public override void Apply(T map)
        {
            FloorPlan floorPlan = new FloorPlan();
            floorPlan.InitSize(map.GridPlan.Size);
            map.InitPlan(floorPlan);

            map.GridPlan.PlaceRoomsOnFloor(map);
        }

    }
}
