// <copyright file="IRandPicker.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A random generator of a single item.
    /// </summary>
    /// <typeparam name="T">The type of item to generate.</typeparam>
    public interface IRandPicker<T> : IRandPicker
    {
        /// <summary>
        /// Randomly generates an item of type T.
        /// </summary>
        /// <param name="rand">The random number generator to use.</param>
        /// <returns>A randomly generated item.</returns>
        T Pick(IRandom rand);

        /// <summary>
        /// Returns a IRandPicker of the same state as this instance.
        /// If this instance holds a collection of items, the items themselves are not duplicated.
        /// </summary>
        /// <returns>A copy of this picker with the same state.</returns>
        IRandPicker<T> CopyState();

        /// <summary>
        /// Enumerates all possible outcomes this picker can produce.
        /// </summary>
        /// <returns>An enumerable of all possible items.</returns>
        IEnumerable<T> EnumerateOutcomes();
    }

    /// <summary>
    /// Non-generic base interface for random item pickers.
    /// </summary>
    public interface IRandPicker
    {
        /// <summary>
        /// Determines if this object changes after a call to Pick().
        /// </summary>
        bool ChangesState { get; }

        /// <summary>
        /// Determines if this instance is in a state where Pick() can be called without throwing an exception.
        /// </summary>
        bool CanPick { get; }
    }
}
