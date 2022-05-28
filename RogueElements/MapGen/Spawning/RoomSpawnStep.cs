// <copyright file="RoomSpawnStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public abstract class RoomSpawnStep<TGenContext, TSpawnable> : BaseSpawnStep<TGenContext, TSpawnable>
        where TGenContext : class, IFloorPlanGenContext, IPlaceableGenContext<TSpawnable>
        where TSpawnable : ISpawnable
    {
        protected RoomSpawnStep()
            : base()
        {
            this.Filters = new List<BaseRoomFilter>();
        }

        protected RoomSpawnStep(IStepSpawner<TGenContext, TSpawnable> spawn)
            : base(spawn)
        {
            this.Filters = new List<BaseRoomFilter>();
        }

        /// <summary>
        /// Determines the rooms eligible to spawn the objects in.
        /// </summary>
        public List<BaseRoomFilter> Filters { get; set; }

        public virtual void SpawnRandInCandRooms(TGenContext map, SpawnList<RoomHallIndex> spawningRooms, List<TSpawnable> spawns, int successPercent)
        {
            while (spawningRooms.Count > 0 && spawns.Count > 0)
            {
                int randIndex = spawningRooms.PickIndex(map.Rand);
                RoomHallIndex roomIndex = spawningRooms.GetSpawn(randIndex);

                // try to spawn the item
                if (this.SpawnInRoom(map, roomIndex, spawns[spawns.Count - 1]))
                {
                    GenContextDebug.DebugProgress("Placed Object");

                    // remove the item spawn
                    spawns.RemoveAt(spawns.Count - 1);

                    if (successPercent <= 0)
                    {
                        spawningRooms.RemoveAt(randIndex);
                    }
                    else
                    {
                        int newRate = Math.Max(1, spawningRooms.GetSpawnRate(randIndex) * successPercent / 100);
                        spawningRooms.SetSpawnRate(randIndex, newRate);
                    }
                }
                else
                {
                    spawningRooms.RemoveAt(randIndex);
                }
            }
        }

        public virtual bool SpawnInRoom(TGenContext map, RoomHallIndex roomIndex, TSpawnable spawn)
        {
            IRoomGen room = map.RoomPlan.GetRoomHall(roomIndex).RoomGen;
            List<Loc> freeTiles = map.GetFreeTiles(room.Draw);

            if (freeTiles.Count > 0)
            {
                int randIndex = map.Rand.Next(freeTiles.Count);
                map.PlaceItem(freeTiles[randIndex], spawn);
                return true;
            }

            return false;
        }
    }
}
