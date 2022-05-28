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
    /// <typeparam name="TGenContext"></typeparam>
    /// <typeparam name="TSpawnable"></typeparam>
    [Serializable]
    public class SpecificSpawnStep<TGenContext, TSpawnable> : BaseSpawnStep<TGenContext, TSpawnable>
        where TGenContext : class, IPlaceableGenContext<TSpawnable>
        where TSpawnable : ISpawnable
    {
        public SpecificSpawnStep()
            : base()
        {
            this.SpawnLocs = new List<Loc>();
        }

        public SpecificSpawnStep(IStepSpawner<TGenContext, TSpawnable> spawn, List<Loc> spawnLocs)
            : base(spawn)
        {
            this.SpawnLocs = spawnLocs;
        }

        /// <summary>
        /// The locations to spawn the objects.
        /// </summary>
        public List<Loc> SpawnLocs { get; }

        public override void DistributeSpawns(TGenContext map, List<TSpawnable> spawns)
        {
            for (int ii = 0; ii < spawns.Count && ii < this.SpawnLocs.Count; ii++)
            {
                TSpawnable item = spawns[ii];
                map.PlaceItem(this.SpawnLocs[ii], item);
                GenContextDebug.DebugProgress("Placed Object");
            }
        }

        public override string ToString()
        {
            return string.Format("{0}", this.GetType().Name);
        }
    }
}
