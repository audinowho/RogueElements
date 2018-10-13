using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IFloorPlanGenContext : ITiledGenContext
    {
        void InitPlan(FloorPlan plan);
        FloorPlan RoomPlan { get; }
    }
}
