// <copyright file="GenStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Base class for all map generation steps that modify a specific type of map context.
    /// </summary>
    /// <typeparam name="T">
    /// The type of map context this step operates on. Must implement <see cref="IGenContext"/>.
    /// Constraining to more specific interfaces (like <see cref="ITiledGenContext"/> or
    /// <see cref="IFloorPlanGenContext"/>) enables type-safe access to specialized map features.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="GenStep{T}"/> is the foundation of the RogueElements pipeline architecture.
    /// Each step represents a discrete transformation applied to the map during generation,
    /// such as placing rooms, adding terrain features, or spawning entities.
    /// </para>
    /// <para>
    /// To create a custom generation step:
    /// <list type="number">
    /// <item><description>Inherit from <see cref="GenStep{T}"/> with appropriate type constraints</description></item>
    /// <item><description>Override <see cref="Apply(T)"/> to implement the generation logic</description></item>
    /// <item><description>Add the step to a <see cref="MapGen{T}"/> via its <see cref="MapGen{T}.GenSteps"/> collection</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Steps should use <see cref="IGenContext.Rand"/> for all random decisions to ensure
    /// reproducible map generation when using the same seed.
    /// </para>
    /// <para>
    /// All <see cref="GenStep{T}"/> subclasses should be marked with <see cref="SerializableAttribute"/>
    /// to support save/load functionality and map layout serialization.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // A simple step that fills the map with floor tiles
    /// [Serializable]
    /// public class FillFloorStep : GenStep&lt;ITiledGenContext&gt;
    /// {
    ///     public override void Apply(ITiledGenContext map)
    ///     {
    ///         for (int x = 0; x &lt; map.Width; x++)
    ///         {
    ///             for (int y = 0; y &lt; map.Height; y++)
    ///             {
    ///                 map.SetTile(new Loc(x, y), map.RoomTerrain);
    ///             }
    ///         }
    ///     }
    /// }
    ///
    /// // Add to a layout
    /// layout.GenSteps.Add(new Priority(1), new FillFloorStep());
    /// </code>
    /// </example>
    [Serializable]
    public abstract class GenStep<T> : IGenStep
        where T : class, IGenContext
    {
        /// <summary>
        /// Applies this generation step to the specified map context.
        /// </summary>
        /// <param name="map">
        /// The map context to modify. Provides access to map data, the random number generator,
        /// and any features exposed by the <typeparamref name="T"/> interface.
        /// </param>
        /// <remarks>
        /// <para>
        /// Implementations should use <see cref="IGenContext.Rand"/> for all random decisions
        /// to maintain reproducibility when generating maps with the same seed.
        /// </para>
        /// <para>
        /// This method is called by <see cref="MapGen{T}.GenMap"/> for each step in priority order.
        /// Steps can assume that all lower-priority steps have already been applied.
        /// </para>
        /// </remarks>
        public abstract void Apply(T map);

        /// <summary>
        /// Determines whether this step can be applied to the specified context.
        /// </summary>
        /// <param name="context">The generation context to check for compatibility.</param>
        /// <returns>
        /// <see langword="true"/> if the <paramref name="context"/> is assignable to type <typeparamref name="T"/>;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method enables runtime type checking when working with non-generic step collections,
        /// allowing the generation system to verify compatibility before applying a step.
        /// </remarks>
        public bool CanApply(IGenContext context)
        {
            return context is T;
        }

        /// <summary>
        /// Applies this generation step to the specified context after performing type validation.
        /// </summary>
        /// <param name="context">The generation context to modify.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="context"/> is not assignable to type <typeparamref name="T"/>.
        /// The exception message includes both the actual context type and the expected step type.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method implements <see cref="IGenStep.Apply"/> and provides the non-generic interface
        /// used by <see cref="MapGen{T}"/> during generation. It performs a type cast and delegates
        /// to the strongly-typed <see cref="Apply(T)"/> method.
        /// </para>
        /// <para>
        /// Use <see cref="CanApply"/> to check compatibility before calling this method if you need
        /// to avoid exceptions.
        /// </para>
        /// </remarks>
        public void Apply(IGenContext context)
        {
            if (context is T map)
                this.Apply(map);
            else
                throw new ArgumentException(string.Format("Context was of type '{0}' was passed into '{1}'.", context.GetType().AssemblyQualifiedName, this.GetType().AssemblyQualifiedName));
        }
    }
}
