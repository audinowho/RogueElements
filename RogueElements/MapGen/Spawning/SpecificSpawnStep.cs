// <copyright file="SpecificSpawnStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Spawns objects on specific locations.
    /// </summary>
    /// <typeparam name="TGenContext">The generation context type, which must support placing spawnable entities.</typeparam>
    /// <typeparam name="TSpawnable">The type of spawnable entity to place on the map.</typeparam>
    [Serializable]
    public class SpecificSpawnStep<TGenContext, TSpawnable> : BaseSpawnStep<TGenContext, TSpawnable>
        where TGenContext : class, IPlaceableGenContext<TSpawnable>
        where TSpawnable : ISpawnable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificSpawnStep{TGenContext, TSpawnable}"/> class.
        /// </summary>
        public SpecificSpawnStep()
            : base()
        {
            this.SpawnLocs = new List<Loc>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificSpawnStep{TGenContext, TSpawnable}"/> class with the specified spawner and locations.
        /// </summary>
        /// <param name="spawn">The spawner that generates the list of items to place.</param>
        /// <param name="spawnLocs">The specific locations where spawns should be placed.</param>
        public SpecificSpawnStep(IStepSpawner<TGenContext, TSpawnable> spawn, List<Loc> spawnLocs)
            : base(spawn)
        {
            this.SpawnLocs = spawnLocs;
        }

        /// <summary>
        /// Gets the specific locations where objects will be spawned.
        /// </summary>
        public List<Loc> SpawnLocs { get; }

        /// <summary>
        /// Distributes spawns by placing each at the corresponding location in <see cref="SpawnLocs"/>.
        /// </summary>
        /// <param name="map">The generation context to place spawns in.</param>
        /// <param name="spawns">The list of spawnable entities to distribute.</param>
        public override void DistributeSpawns(TGenContext map, List<TSpawnable> spawns)
        {
            for (int ii = 0; ii < spawns.Count && ii < this.SpawnLocs.Count; ii++)
            {
                TSpawnable item = spawns[ii];
                map.PlaceItem(this.SpawnLocs[ii], item);
                GenContextDebug.DebugProgress("Placed Object");
            }
        }
    }
}
