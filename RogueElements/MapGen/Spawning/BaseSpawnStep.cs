// <copyright file="BaseSpawnStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Spawns objects of type E to IPlaceableGenContext T.
    /// Child classes offer a different way to place the list of spawns provided by Spawn.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    /// <typeparam name="TSpawnable"></typeparam>
    [Serializable]
    public abstract class BaseSpawnStep<TGenContext, TSpawnable> : GenStep<TGenContext>
        where TGenContext : class, IPlaceableGenContext<TSpawnable>
        where TSpawnable : ISpawnable
    {
        protected BaseSpawnStep()
        {
        }

        protected BaseSpawnStep(IStepSpawner<TGenContext, TSpawnable> spawn)
        {
            this.Spawn = spawn;
        }

        /// <summary>
        /// The generator that creates a list of items for the step to spawn.
        /// </summary>
        public IStepSpawner<TGenContext, TSpawnable> Spawn { get; set; }

        public abstract void DistributeSpawns(TGenContext map, List<TSpawnable> spawns);

        public override void Apply(TGenContext map)
        {
            if (this.Spawn is null)
                return;

            List<TSpawnable> spawns = this.Spawn.GetSpawns(map);

            if (spawns.Count > 0)
                this.DistributeSpawns(map, spawns);
        }
    }
}
