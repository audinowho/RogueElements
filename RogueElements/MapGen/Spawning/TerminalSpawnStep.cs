using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class TerminalSpawnStep<T, E> : RoomSpawnStep<T, E>
        where T : class, IFloorPlanGenContext, IPlaceableGenContext<E>
    {
        public TerminalSpawnStep() { }
        public TerminalSpawnStep(IStepSpawner<T, E> spawn) : base(spawn) { }

        public override void DistributeSpawns(T map, List<E> spawns)
        {
            //random per room, not per-tile

            SpawnList<int> spawningRooms = new SpawnList<int>();
            SpawnList<int> terminalRooms = new SpawnList<int>();

            //TODO: higher likelihoods for terminals at the ends of longer paths
            for (int ii = 0; ii < map.RoomPlan.RoomCount; ii++)
            {
                spawningRooms.Add(ii);
                List<int> adjacent = map.RoomPlan.GetAdjacentRooms(ii);
                if (adjacent.Count == 1)
                    terminalRooms.Add(ii);
            }

            //first attempt to spawn in the terminals; remove from terminal list if successful
            SpawnRandInCandRooms(map, terminalRooms, spawns, 0);
            
            SpawnRandInCandRooms(map, spawningRooms, spawns, 100);
        }
    }
}
