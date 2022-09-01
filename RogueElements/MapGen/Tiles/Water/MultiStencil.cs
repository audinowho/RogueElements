// <copyright file="MultiStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A filter for determining the eligible tiles for an operation.
    /// Only considers tiles eligible if they fit any/all conditions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MultiStencil<T> : ITerrainStencil<T>
        where T : class, ITiledGenContext
    {
        public MultiStencil()
        {
            this.List = new List<ITerrainStencil<T>>();
        }

        /// <summary>
        /// Determines if the entire map should be checked for connectivity, or just the immediate surrounding tiles.
        /// </summary>
        public List<ITerrainStencil<T>> List { get; set; }

        public bool RequireAll { get; set; }

        public bool Test(T map, Rect rect)
        {
            foreach (ITerrainStencil<T> subReq in this.List)
            {
                if (this.RequireAll)
                {
                    if (!subReq.Test(map, rect))
                        return false;
                }
                else
                {
                    if (subReq.Test(map, rect))
                        return true;
                }
            }

            return this.RequireAll;
        }
    }
}
