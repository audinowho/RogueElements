// <copyright file="IExit.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// Represents an exit point on the map, such as stairs going down or a dungeon exit.
    /// Players typically leave a floor by reaching an exit.
    /// </summary>
    /// <seealso cref="ISpawnable"/>
    /// <seealso cref="IEntrance"/>
    public interface IExit : ISpawnable
    {
    }
}
