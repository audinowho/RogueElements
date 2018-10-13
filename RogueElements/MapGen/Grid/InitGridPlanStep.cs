using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class InitGridPlanStep<T> : GenStep<T> where T : class, IRoomGridGenContext
    {
        public int CellWidth;
        public int CellHeight;

        public int CellX;
        public int CellY;

        public InitGridPlanStep() { }

        public override void Apply(T map)
        {
            //initialize grid
            GridPlan floorPlan = new GridPlan();
            floorPlan.InitSize(CellX, CellY, CellWidth, CellHeight);

            map.InitGrid(floorPlan);
            
        }

    }
}
