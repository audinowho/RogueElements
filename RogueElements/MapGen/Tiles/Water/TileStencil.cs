// <copyright file="TileStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A filter for determining the eligible tiles for an operation.
    /// Checking the bounds is checking each individual tile.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class TileStencil<T> : ITerrainStencil<T>
        where T : class, ITiledGenContext
    {
        public bool Test(T map, Rect rect)
        {
            for (int xx = rect.X; xx < rect.End.X; xx++)
            {
                for (int yy = rect.Y; yy < rect.End.Y; yy++)
                {
                    if (!this.TestTile(map, new Loc(xx, yy)))
                        return false;
                }
            }

            return true;
        }

        protected abstract bool TestTile(T map, Loc loc);
    }
}
