// <copyright file="ISpawnable.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    public interface ISpawnable
    {
        /// <summary>
        /// Creates a copy of the object, to be placed in the generated layout.
        /// </summary>
        /// <returns></returns>
        ISpawnable Copy();
    }
}
