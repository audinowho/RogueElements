using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class InitFloorPlanStep<T> : GenStep<T> where T : class, IFloorPlanGenContext
    {
        public int Width;
        public int Height;

        public InitFloorPlanStep() { }

        public override void Apply(T map)
        {
            FloorPlan floorPlan = new FloorPlan();
            floorPlan.InitSize(new Loc(Width, Height));

            map.InitPlan(floorPlan);
        }

    }
}
