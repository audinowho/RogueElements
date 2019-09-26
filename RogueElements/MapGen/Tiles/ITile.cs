// <copyright file="ITile.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    public interface ITile
    {
        bool TileEquivalent(ITile other);

        /// <summary>
        /// Creates a copy of the object, to be placed in the generated layout.
        /// </summary>
        /// <returns></returns>
        ITile Copy();
    }
}
