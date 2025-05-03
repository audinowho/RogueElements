// <copyright file="DefaultTerrainStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A filter for determining the eligible tiles for an operation.
    /// All tiles are eligible.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    [Serializable]
    public class DefaultTerrainStencil<TGenContext, TTile> : ITerrainStencil<TGenContext, TTile>
        where TGenContext : class, ITiledGenContext<TTile>
        where TTile : ITile<TTile>
    {
        public bool Test(TGenContext map, Loc loc)
        {
            return true;
        }

        public override string ToString()
        {
            return string.Format("Any Tile");
        }
    }
}
