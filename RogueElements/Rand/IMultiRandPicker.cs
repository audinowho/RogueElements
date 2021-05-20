// <copyright file="IMultiRandPicker.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A random generator of a list of items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMultiRandPicker<T> : IMultiRandPicker
    {
        /// <summary>
        /// Randomly generates a list of items of type T.
        /// </summary>
        /// <param name="rand"></param>
        /// <returns></returns>
        List<T> Roll(IRandom rand);

        /// <summary>
        /// Returns a IMultiRandPicker of the same state as this instance.
        /// If this instance holds a collection of items, the items themselves are not duplicated.
        /// </summary>
        /// <returns></returns>
        IMultiRandPicker<T> CopyState();
    }

    public interface IMultiRandPicker
    {
        /// <summary>
        /// Determines if this object changes after a call to Roll().
        /// </summary>
        bool ChangesState { get; }

        /// <summary>
        /// Determines if this instance is in a state where Roll() can be called without throwing an exception.
        /// </summary>
        bool CanPick { get; }
    }
}
