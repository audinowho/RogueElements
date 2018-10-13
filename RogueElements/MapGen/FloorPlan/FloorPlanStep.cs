using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements
{
    [Serializable]
    public abstract class FloorPlanStep<T> : GenStep<T> where T : class, IFloorPlanGenContext
    {
        public FloorPlanStep() { }

        public abstract void ApplyToPath(IRandom rand, FloorPlan floorPlan);

        public override void Apply(T map)
        {
            ApplyToPath(map.Rand, map.RoomPlan);
        }

    }
}
