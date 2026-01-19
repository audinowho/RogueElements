// <copyright file="DefaultTerrainStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Provides a terrain stencil that allows all tiles to pass.
    /// </summary>
    /// <typeparam name="T">The type of map context that implements <see cref="ITiledGenContext"/>.</typeparam>
    [Serializable]
    public class DefaultTerrainStencil<T> : ITerrainStencil<T>
        where T : class, ITiledGenContext
    {
        /// <inheritdoc/>
        public bool Test(T map, Loc loc)
        {
            return true;
        }

        public override string ToString()
        {
            return string.Format("Any Tile");
        }
    }
}
