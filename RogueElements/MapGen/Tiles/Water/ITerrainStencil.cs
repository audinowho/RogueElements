// <copyright file="ITerrainStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface ITerrainStencil<T>
        where T : class, ITiledGenContext
    {
        bool Test(T map, Loc loc);
    }
}
