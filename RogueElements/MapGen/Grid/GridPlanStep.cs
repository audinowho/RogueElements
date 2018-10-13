using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements
{

    [Serializable]
    public abstract class GridPlanStep<T> : GenStep<T> where T : class, IRoomGridGenContext
    {
        public GridPlanStep() { }

        public abstract void ApplyToPath(IRandom rand, GridPlan floorPlan);

        public override void Apply(T map)
        {
            //actual map creation step
            ApplyToPath(map.Rand, map.GridPlan);
        }

    }
}
