// <copyright file="ExampleCreationStrategy.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using RogueSharp;
using RogueSharp.MapCreation;

namespace RogueElements.Examples.Ex8_Integration
{
    /// <summary>
    /// Adapts RogueElements' MapGen pipeline to RogueSharp's IMapCreationStrategy interface.
    /// Allows using RogueElements generation with RogueSharp's Map.Create() factory.
    /// </summary>
    /// <typeparam name="T">The RogueSharp Map type to create.</typeparam>
    /// <remarks>
    /// RogueSharp uses the Strategy pattern for map creation via IMapCreationStrategy.
    /// This class wraps RogueElements' MapGen to act as a RogueSharp strategy.
    ///
    /// Usage pattern:
    /// 1. Create ExampleCreationStrategy instance
    /// 2. Configure Layout.GenSteps with RogueElements steps
    /// 3. Set Seed for reproducible generation
    /// 4. Call Map.Create(strategy) to generate
    ///
    /// This demonstrates how RogueElements can integrate with other game frameworks
    /// that have their own map representations.
    /// </remarks>
    public class ExampleCreationStrategy<T> : IMapCreationStrategy<T>
        where T : Map, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleCreationStrategy{T}"/> class.
        /// </summary>
        public ExampleCreationStrategy()
        {
            this.Layout = new MapGen<MapGenContext>();
        }

        /// <summary>
        /// Gets or sets the random seed for reproducible generation.
        /// </summary>
        public ulong Seed { get; set; }

        /// <summary>
        /// Gets or sets the MapGen layout containing generation steps.
        /// Configure this with GenSteps before calling CreateMap().
        /// </summary>
        public MapGen<MapGenContext> Layout { get; set; }

        /// <summary>
        /// Creates a new IMap of the specified type using the configured pipeline.
        /// Called by RogueSharp's Map.Create() factory method.
        /// </summary>
        /// <returns>An IMap of the specified type.</returns>
        /// <remarks>
        /// This bridges the two APIs:
        /// - Calls MapGen.GenMap() to run the RogueElements pipeline
        /// - Returns the generated Map from the context
        /// </remarks>
        public T CreateMap()
        {
            // Run the RogueElements pipeline
            MapGenContext context = this.Layout.GenMap(MathUtils.Rand.NextUInt64());

            // Return the RogueSharp Map from the context
            return (T)context.Map;
        }
    }
}
