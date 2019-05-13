// <copyright file="IRandPicker.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections;

namespace RogueElements
{
    /// <summary>
    /// A random generator of a single item.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRandPicker<T> : IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Determines if this object changes after a call to Pick().
        /// </summary>
        bool ChangesState { get; }

        /// <summary>
        /// Determines if this instance is in a state where Pick() can be called without throwing an exception.
        /// </summary>
        bool CanPick { get; }


        /// <summary>
        /// Randomly generates an item of type T.
        /// </summary>
        /// <param name="rand"></param>
        /// <returns></returns>
        T Pick(IRandom rand);


        /// <summary>
        /// Returns a IRandPicker of the same state as this instance.
        /// If this instance holds a collection of items, the items themselves are not duplicated.
        /// </summary>
        /// <returns></returns>
        IRandPicker<T> CopyState();
    }

}
