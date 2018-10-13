using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IRoomGridGenContext : IFloorPlanGenContext
    {
        void InitGrid(GridPlan plan);
        GridPlan GridPlan { get; }
    }

}
