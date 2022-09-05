// <copyright file="IBlobStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IBlobStencil<T>
        where T : class, ITiledGenContext
    {
        bool Test(T map, Rect rect);
    }
}
