// <copyright file="IEntrance.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// Represents an entrance point on the map, such as stairs going up or a dungeon entry.
    /// Players typically start at an entrance when entering a floor.
    /// </summary>
    /// <seealso cref="ISpawnable"/>
    /// <seealso cref="IExit"/>
    public interface IEntrance : ISpawnable
    {
    }
}
