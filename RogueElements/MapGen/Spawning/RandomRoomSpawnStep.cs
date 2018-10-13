using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class RandomRoomSpawnStep<T, E> : RoomSpawnStep<T, E>
        where T : class, IFloorPlanGenContext, IPlaceableGenContext<E>
    {
        public RandomRoomSpawnStep() { }
        public RandomRoomSpawnStep(IStepSpawner<T, E> spawn) : base(spawn) { }

        public override void DistributeSpawns(T map, List<E> spawns)
        {
            //random per room, not per-tile

            SpawnList<int> spawningRooms = new SpawnList<int>();

            for (int ii = 0; ii < map.RoomPlan.RoomCount; ii++)
            {
                spawningRooms.Add(ii);
            }

            SpawnRandInCandRooms(map, spawningRooms, spawns, 100);
        }
    }
}
