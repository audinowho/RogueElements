// <copyright file="IGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Defines the core contract for a map generation context that holds state during the generation process.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="IGenContext"/> is the base interface that all map contexts must implement.
    /// It provides the essential infrastructure for procedural generation: a seeded random number
    /// generator and lifecycle hooks for initialization and finalization.
    /// </para>
    /// <para>
    /// Implementations typically extend this interface with additional capabilities:
    /// <list type="bullet">
    /// <item><description><see cref="ITiledGenContext"/> - Tile-based map operations (get/set tiles, wall detection)</description></item>
    /// <item><description><see cref="IFloorPlanGenContext"/> - Freeform room placement via <see cref="FloorPlan"/></description></item>
    /// <item><description><see cref="IRoomGridGenContext"/> - Grid-based room layouts via <see cref="GridPlan"/></description></item>
    /// <item><description><see cref="IPlaceableGenContext{T}"/> - Entity spawning (items, stairs, mobs)</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// The context is created by <see cref="MapGen{T}.GenMap"/> using <see cref="System.Activator.CreateInstance(System.Type)"/>,
    /// so implementations must have a public parameterless constructor.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class MyMapContext : IGenContext, ITiledGenContext
    /// {
    ///     private ReRandom rand;
    ///
    ///     public IRandom Rand => this.rand;
    ///
    ///     public void InitSeed(ulong seed)
    ///     {
    ///         this.rand = new ReRandom(seed);
    ///         // Initialize map data structures
    ///     }
    ///
    ///     public void FinishGen()
    ///     {
    ///         // Final processing, cleanup, or validation
    ///     }
    ///
    ///     // ... additional ITiledGenContext members ...
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="MapGen{T}"/>
    /// <seealso cref="GenStep{T}"/>
    public interface IGenContext
    {
        /// <summary>
        /// Gets the random number generator used for all procedural decisions during map generation.
        /// </summary>
        /// <value>
        /// An <see cref="IRandom"/> instance that provides deterministic random values based on the
        /// seed initialized via <see cref="InitSeed"/>.
        /// </value>
        /// <remarks>
        /// <para>
        /// All <see cref="GenStep{T}"/> implementations should use this property for random decisions
        /// to ensure reproducible map generation. Using the same seed with the same generation steps
        /// will produce identical maps.
        /// </para>
        /// <para>
        /// This property should be initialized in <see cref="InitSeed"/> and remain consistent
        /// throughout the generation process.
        /// </para>
        /// </remarks>
        IRandom Rand { get; }

        /// <summary>
        /// Initializes the generation context with a random seed.
        /// </summary>
        /// <param name="seed">
        /// The seed value used to initialize the random number generator.
        /// The same seed produces the same sequence of random values, enabling reproducible generation.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method is called by <see cref="MapGen{T}.GenMap"/> immediately after creating
        /// the context instance, before any generation steps execute.
        /// </para>
        /// <para>
        /// Implementations should:
        /// <list type="bullet">
        /// <item><description>Initialize the <see cref="Rand"/> property with a seeded RNG</description></item>
        /// <item><description>Allocate initial data structures (tile arrays, room collections, etc.)</description></item>
        /// <item><description>Set default values for generation state</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        void InitSeed(ulong seed);

        /// <summary>
        /// Performs final processing after all generation steps have completed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is called by <see cref="MapGen{T}.GenMap"/> after the last generation step
        /// has been applied. It provides an opportunity for final cleanup, validation, or
        /// post-processing.
        /// </para>
        /// <para>
        /// Common uses include:
        /// <list type="bullet">
        /// <item><description>Converting intermediate data structures to final representations</description></item>
        /// <item><description>Validating map integrity (connectivity, required features, etc.)</description></item>
        /// <item><description>Releasing temporary resources used only during generation</description></item>
        /// <item><description>Computing derived data (visibility grids, pathfinding graphs, etc.)</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        void FinishGen();
    }
}
