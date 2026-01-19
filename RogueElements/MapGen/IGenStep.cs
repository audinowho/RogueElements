// <copyright file="IGenStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RogueElements
{
    /// <summary>
    /// Defines the non-generic contract for a generation step in the map generation pipeline.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="IGenStep"/> provides a type-agnostic interface for generation steps, enabling
    /// <see cref="MapGen{T}"/> to work with heterogeneous collections of steps through a common contract.
    /// </para>
    /// <para>
    /// Most implementations should inherit from <see cref="GenStep{T}"/> rather than implementing
    /// this interface directly. <see cref="GenStep{T}"/> provides automatic type checking and
    /// delegation to a strongly-typed <see cref="GenStep{T}.Apply(T)"/> method.
    /// </para>
    /// </remarks>
    /// <seealso cref="GenStep{T}"/>
    /// <seealso cref="MapGen{T}"/>
    public interface IGenStep
    {
        /// <summary>
        /// Determines whether this step can be applied to the specified generation context.
        /// </summary>
        /// <param name="context">The generation context to check for compatibility.</param>
        /// <returns>
        /// <see langword="true"/> if this step is compatible with and can be applied to
        /// the <paramref name="context"/>; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// For <see cref="GenStep{T}"/> implementations, this returns <see langword="true"/>
        /// when the context is assignable to the step's generic type parameter <c>T</c>.
        /// </remarks>
        bool CanApply(IGenContext context);

        /// <summary>
        /// Applies this generation step to the specified context, modifying the map state.
        /// </summary>
        /// <param name="context">The generation context to modify.</param>
        /// <remarks>
        /// <para>
        /// This method is called by <see cref="MapGen{T}.GenMap"/> for each step in priority order.
        /// Implementations should verify context compatibility, either by checking <see cref="CanApply"/>
        /// first or by throwing an appropriate exception for incompatible contexts.
        /// </para>
        /// <para>
        /// Steps should use <see cref="IGenContext.Rand"/> for all random decisions to maintain
        /// reproducibility when generating maps with the same seed.
        /// </para>
        /// </remarks>
        void Apply(IGenContext context);
    }
}
