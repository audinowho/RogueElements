// <copyright file="DefaultTerrainStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class DefaultTerrainStencil<T> : ITerrainStencil<T>
        where T : class, ITiledGenContext
    {
        public bool Test(T map, Loc loc)
        {
            return true;
        }
    }
}
