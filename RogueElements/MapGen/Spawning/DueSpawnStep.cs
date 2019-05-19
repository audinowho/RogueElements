using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Places items on the map based on how far they are from the entrance of the map.
    /// Distance is measured in the amount of rooms one must travel to reach the room in question.
    /// </summary>
    /// <typeparam name="T">Type of the MapGenContext.</typeparam>
    /// <typeparam name="E">Type of the item to spawn.</typeparam>
    /// <typeparam name="F">Type of the Map Entrance.</typeparam>
    [Serializable]
    public class DueSpawnStep<T, E, F> : RoomSpawnStep<T, E>
        where T : class, IFloorPlanGenContext, IPlaceableGenContext<E>, IViewPlaceableGenContext<F>
        where E : ISpawnable
        where F : ISpawnable
    {

        /// <summary>
        /// The percentage chance to multiply a room's spawning chance when it successfully spawns an item.
        /// </summary>
        public int SuccessPercent;

        public DueSpawnStep() { }
        public DueSpawnStep(IStepSpawner<T, E> spawn, int successPercent) : base(spawn)
        {
            SuccessPercent = successPercent;
        }

        public override void DistributeSpawns(T map, List<E> spawns)
        {
            //gather up all rooms and put in a spawn list
            //rooms that are farther from the start are more likely to have items

            SpawnList<RoomHallIndex> spawningRooms = new SpawnList<RoomHallIndex>();
            int[] roomWeights = new int[map.RoomPlan.RoomCount];

            //get the start room
            int startRoom = 0;
            for (int ii = 0; ii < map.RoomPlan.RoomCount; ii++)
            {
                IRoomGen room = map.RoomPlan.GetRoom(ii);
                if (Collision.InBounds(room.Draw, map.GetLoc(0)))
                {
                    startRoom = ii;
                    break;
                }
            }

            int maxVal = 1;
            Graph.DistNodeAction nodeAct = (int nodeIndex, int distance) =>
            {
                roomWeights[nodeIndex] = distance + 1;
                maxVal = Math.Max(maxVal, roomWeights[nodeIndex]);
            };
            Graph.GetAdjacents getAdjacents = (int nodeIndex) =>
            {
                return map.RoomPlan.GetAdjacentRooms(nodeIndex);
            };

            Graph.TraverseBreadthFirst(roomWeights.Length, startRoom, nodeAct, getAdjacents);

            int multFactor = Int32.MaxValue / maxVal / roomWeights.Length;
            for (int ii = 0; ii < roomWeights.Length; ii++)
            {
                spawningRooms.Add(new RoomHallIndex(ii, false), roomWeights[ii] * multFactor);
            }

            SpawnRandInCandRooms(map, spawningRooms, spawns, SuccessPercent);
        }
    }
}
