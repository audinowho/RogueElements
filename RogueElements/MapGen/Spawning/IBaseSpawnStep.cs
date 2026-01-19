// <copyright file="IBaseSpawnStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Provides a non-generic interface for spawn steps, enabling type-agnostic access to spawn configuration.
    /// </summary>
    /// <seealso cref="BaseSpawnStep{TGenContext, TSpawnable}"/>
    public interface IBaseSpawnStep
    {
        /// <summary>
        /// Gets the spawner that generates the list of items to place.
        /// </summary>
        IStepSpawner Spawn { get; }

        /// <summary>
        /// Gets the type of spawnable entity this step handles.
        /// </summary>
        Type SpawnType { get; }
    }

    /// <summary>
    /// Spawns objects of type E to IPlaceableGenContext T.
    /// Child classes offer a different way to place the list of spawns provided by Spawn.
    /// </summary>
    /// <typeparam name="TGenContext">The generation context type, which must support placing spawnable entities.</typeparam>
    /// <typeparam name="TSpawnable">The type of spawnable entity to place on the map.</typeparam>
    [Serializable]
    public abstract class BaseSpawnStep<TGenContext, TSpawnable> : GenStep<TGenContext>, IBaseSpawnStep
        where TGenContext : class, IPlaceableGenContext<TSpawnable>
        where TSpawnable : ISpawnable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSpawnStep{TGenContext, TSpawnable}"/> class.
        /// </summary>
        protected BaseSpawnStep()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSpawnStep{TGenContext, TSpawnable}"/> class with the specified spawner.
        /// </summary>
        /// <param name="spawn">The spawner that generates the list of items to place.</param>
        protected BaseSpawnStep(IStepSpawner<TGenContext, TSpawnable> spawn)
        {
            this.Spawn = spawn;
        }

        /// <summary>
        /// The generator that creates a list of items for the step to spawn.
        /// </summary>
        public IStepSpawner<TGenContext, TSpawnable> Spawn { get; set; }

        /// <inheritdoc/>
        IStepSpawner IBaseSpawnStep.Spawn => this.Spawn;

        /// <inheritdoc/>
        public Type SpawnType => typeof(TSpawnable);

        /// <summary>
        /// Distributes the given spawns across the map using a placement strategy defined by the subclass.
        /// </summary>
        /// <param name="map">The generation context to place spawns in.</param>
        /// <param name="spawns">The list of spawnable entities to distribute.</param>
        public abstract void DistributeSpawns(TGenContext map, List<TSpawnable> spawns);

        /// <summary>
        /// Applies the spawn step to the map by generating spawns and distributing them.
        /// </summary>
        /// <param name="map">The generation context to apply spawns to.</param>
        public override void Apply(TGenContext map)
        {
            if (this.Spawn is null)
                return;

            List<TSpawnable> spawns = this.Spawn.GetSpawns(map);

            if (spawns.Count > 0)
                this.DistributeSpawns(map, spawns);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.Spawn == null)
                return string.Format("{0}<{1}>: [EMPTY]", this.GetType().GetFormattedTypeName(), typeof(TSpawnable).Name);
            return string.Format("{0}<{1}>: {2}", this.GetType().GetFormattedTypeName(), typeof(TSpawnable).Name, this.Spawn.ToString());
        }
    }
}
