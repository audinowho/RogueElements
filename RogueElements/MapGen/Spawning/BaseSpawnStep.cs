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
        private readonly IStepSpawner<TGenContext, TSpawnable> spawn;

        protected BaseSpawnStep(IStepSpawner<TGenContext, TSpawnable> spawn)
        {
            this.spawn = spawn;
        }

        public abstract void DistributeSpawns(TGenContext map, List<TSpawnable> spawns);

        public override void Apply(TGenContext map)
        {
            List<TSpawnable> spawns = this.spawn.GetSpawns(map);

            if (spawns.Count > 0)
                this.DistributeSpawns(map, spawns);
        }
    }
}
