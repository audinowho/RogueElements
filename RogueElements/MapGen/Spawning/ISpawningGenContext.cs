// <copyright file="ISpawningGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Represents a generation context that provides its own spawn tables for generating entities.
    /// Contexts implementing this interface can supply spawnable objects via a random picker.
    /// </summary>
    /// <typeparam name="T">The type of spawnable entity this context can generate.</typeparam>
    /// <seealso cref="IGenContext"/>
    /// <seealso cref="ISpawnable"/>
    /// <seealso cref="ContextSpawner{TGenContext, TSpawnable}"/>
    public interface ISpawningGenContext<T> : IGenContext
        where T : ISpawnable
    {
        /// <summary>
        /// Gets the random picker used to select spawnable entities from this context's spawn tables.
        /// </summary>
        IRandPicker<T> Spawner { get; }
    }
}
