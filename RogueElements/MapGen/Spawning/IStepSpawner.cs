// <copyright file="IStepSpawner.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Generates a list of spawnables to be placed in a IGenContext. This class only computes what to spawn, but not where to spawn it.
    /// </summary>
    /// <typeparam name="TGenContext">The IGenContext to place the spawns in.</typeparam>
    /// <typeparam name="TSpawnable">The type of the spawn to place in IGenContext</typeparam>
    public interface IStepSpawner<TGenContext, TSpawnable> : IStepSpawner
        where TGenContext : IGenContext
        where TSpawnable : ISpawnable
    {
        /// <summary>
        /// Generates a list of spawnable entities for placement on the map.
        /// </summary>
        /// <param name="map">The generation context used to inform spawn generation.</param>
        /// <returns>A list of spawnable entities to be placed.</returns>
        List<TSpawnable> GetSpawns(TGenContext map);
    }

    /// <summary>
    /// Provides a non-generic marker interface for step spawners.
    /// </summary>
    /// <seealso cref="IStepSpawner{TGenContext, TSpawnable}"/>
    public interface IStepSpawner
    {
    }
}
