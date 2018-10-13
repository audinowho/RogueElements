using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class AddGridDefaultsStep<T> : GridPlanStep<T> where T : class, IRoomGridGenContext
    {

        public RandRange DefaultRatio;

        public AddGridDefaultsStep()
        { }

        public AddGridDefaultsStep(RandRange defaultRatio)
        {
            DefaultRatio = defaultRatio;
        }

        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            List<int> candidates = new List<int>();
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                if (!floorPlan.GetRoomPlan(ii).Immutable)
                {
                    List<int> adjacents = floorPlan.GetAdjacentRooms(ii);
                    if (adjacents.Count > 1)
                        candidates.Add(ii);
                }
            }
            //our candidates are all rooms except immutables and terminals
            int amountToDefault = DefaultRatio.Pick(rand) * candidates.Count / 100;
            for (int ii = 0; ii < amountToDefault; ii++)
            {
                int randIndex = rand.Next(candidates.Count);
                floorPlan.SetRoomGen(candidates[randIndex], new RoomGenDefault<T>());
                candidates.RemoveAt(randIndex);
            }
        }

        
    }
}
