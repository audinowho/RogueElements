// <copyright file="MultiTerrainStencil.cs" company="Audino">
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
    public class MultiTerrainStencil<T> : ITerrainStencil<T>
        where T : class, ITiledGenContext
    {
        public MultiTerrainStencil()
        {
            this.List = new List<ITerrainStencil<T>>();
        }

        public MultiTerrainStencil(bool requireAny, params ITerrainStencil<T>[] stencils)
        {
            this.RequireAny = requireAny;
            this.List = new List<ITerrainStencil<T>>();
            this.List.AddRange(stencils);
        }

        /// <summary>
        /// Determines if the entire map should be checked for connectivity, or just the immediate surrounding tiles.
        /// </summary>
        public List<ITerrainStencil<T>> List { get; set; }

        public bool RequireAny { get; set; }

        public bool Test(T map, Loc loc)
        {
            foreach (ITerrainStencil<T> subReq in this.List)
            {
                if (this.RequireAny)
                {
                    if (subReq.Test(map, loc))
                        return true;
                }
                else
                {
                    if (!subReq.Test(map, loc))
                        return false;
                }
            }

            return !this.RequireAny;
        }
    }
}
