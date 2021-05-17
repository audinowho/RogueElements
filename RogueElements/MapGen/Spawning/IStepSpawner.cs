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
        List<TSpawnable> GetSpawns(TGenContext map);
    }

    public interface IStepSpawner
    {
    }
}
