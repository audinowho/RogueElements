// <copyright file="RandomSpawnStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Spawns objects on randomly chosen tiles.
    /// The tile is chosen from the set of tiles where the object is allowed to be placed.
    /// </summary>
    /// <typeparam name="TGenContext">The generation context type, which must support placing spawnable entities.</typeparam>
    /// <typeparam name="TSpawnable">The type of spawnable entity to place on the map.</typeparam>
    [Serializable]
    public class RandomSpawnStep<TGenContext, TSpawnable> : BaseSpawnStep<TGenContext, TSpawnable>
        where TGenContext : class, IPlaceableGenContext<TSpawnable>
        where TSpawnable : ISpawnable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSpawnStep{TGenContext, TSpawnable}"/> class.
        /// </summary>
        public RandomSpawnStep()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSpawnStep{TGenContext, TSpawnable}"/> class with the specified spawner.
        /// </summary>
        /// <param name="spawn">The spawner that generates the list of items to place.</param>
        public RandomSpawnStep(IStepSpawner<TGenContext, TSpawnable> spawn)
            : base(spawn)
        {
        }

        /// <summary>
        /// Distributes spawns by placing each on a randomly selected free tile.
        /// </summary>
        /// <param name="map">The generation context to place spawns in.</param>
        /// <param name="spawns">The list of spawnable entities to distribute.</param>
        public override void DistributeSpawns(TGenContext map, List<TSpawnable> spawns)
        {
            List<Loc> freeTiles = map.GetAllFreeTiles();

            for (int ii = 0; ii < spawns.Count && freeTiles.Count > 0; ii++)
            {
                TSpawnable item = spawns[ii];

                int randIndex = map.Rand.Next(freeTiles.Count);
                map.PlaceItem(freeTiles[randIndex], item);
                freeTiles.RemoveAt(randIndex);
                GenContextDebug.DebugProgress("Placed Object");
            }
        }
    }
}
