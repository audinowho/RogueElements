// <copyright file="ISpawnable.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// Represents an entity that can be spawned and placed on a generated map.
    /// Implementations include items, enemies, stairs, and other placeable objects.
    /// </summary>
    public interface ISpawnable
    {
        /// <summary>
        /// Creates a copy of the object to be placed in the generated layout.
        /// </summary>
        /// <returns>A new instance that is a copy of this spawnable object.</returns>
        ISpawnable Copy();
    }
}
