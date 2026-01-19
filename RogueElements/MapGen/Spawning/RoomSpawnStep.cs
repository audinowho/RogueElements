// <copyright file="RoomSpawnStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Base class for spawn steps that distribute entities across rooms in a floor plan.
    /// Provides filtering and room-based placement logic used by subclasses.
    /// </summary>
    /// <typeparam name="TGenContext">The type of generation context, which must support floor plans and item placement.</typeparam>
    /// <typeparam name="TSpawnable">The type of spawnable entity to place.</typeparam>
    /// <seealso cref="BaseSpawnStep{TGenContext, TSpawnable}"/>
    /// <seealso cref="RandomRoomSpawnStep{TGenContext, TSpawnable}"/>
    /// <seealso cref="TerminalSpawnStep{TGenContext, TSpawnable}"/>
    [Serializable]
    public abstract class RoomSpawnStep<TGenContext, TSpawnable> : BaseSpawnStep<TGenContext, TSpawnable>
        where TGenContext : class, IFloorPlanGenContext, IPlaceableGenContext<TSpawnable>
        where TSpawnable : ISpawnable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomSpawnStep{TGenContext, TSpawnable}"/> class.
        /// </summary>
        protected RoomSpawnStep()
            : base()
        {
            this.Filters = new List<BaseRoomFilter>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomSpawnStep{TGenContext, TSpawnable}"/> class with the specified spawner.
        /// </summary>
        /// <param name="spawn">The spawner that generates the list of items to place.</param>
        protected RoomSpawnStep(IStepSpawner<TGenContext, TSpawnable> spawn)
            : base(spawn)
        {
            this.Filters = new List<BaseRoomFilter>();
        }

        /// <summary>
        /// Gets or sets the filters that determine which rooms are eligible for spawning.
        /// </summary>
        public List<BaseRoomFilter> Filters { get; set; }

        /// <summary>
        /// Spawns entities into randomly selected rooms from the candidate list.
        /// </summary>
        /// <param name="map">The generation context to place spawns in.</param>
        /// <param name="spawningRooms">The weighted list of candidate rooms.</param>
        /// <param name="spawns">The list of spawnable entities to place.</param>
        /// <param name="decayPercent">The percentage to reduce a room's spawn weight after successful placement. Use 0 to remove the room entirely.</param>
        public virtual void SpawnRandInCandRooms(TGenContext map, SpawnList<RoomHallIndex> spawningRooms, List<TSpawnable> spawns, int decayPercent)
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

                    if (decayPercent <= 0)
                    {
                        spawningRooms.RemoveAt(randIndex);
                    }
                    else
                    {
                        int newRate = Math.Max(1, spawningRooms.GetSpawnRate(randIndex) * decayPercent / 100);
                        spawningRooms.SetSpawnRate(randIndex, newRate);
                    }
                }
                else
                {
                    spawningRooms.RemoveAt(randIndex);
                }
            }
        }

        /// <summary>
        /// Attempts to spawn an entity at a random free tile within the specified room.
        /// </summary>
        /// <param name="map">The generation context to place the spawn in.</param>
        /// <param name="roomIndex">The index of the room to spawn in.</param>
        /// <param name="spawn">The spawnable entity to place.</param>
        /// <returns><c>true</c> if the spawn was successfully placed; otherwise, <c>false</c>.</returns>
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
