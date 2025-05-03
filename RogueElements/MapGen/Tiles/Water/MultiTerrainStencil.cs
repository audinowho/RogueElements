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
    /// <typeparam name="TGenContext"></typeparam>
    [Serializable]
    public class MultiTerrainStencil<TGenContext, TTile> : ITerrainStencil<TGenContext, TTile>
        where TGenContext : class, ITiledGenContext<TTile>
        where TTile : ITile<TTile>
    {
        public MultiTerrainStencil()
        {
            this.List = new List<ITerrainStencil<TGenContext, TTile>>();
        }

        public MultiTerrainStencil(bool requireAny, params ITerrainStencil<TGenContext, TTile>[] stencils)
        {
            this.RequireAny = requireAny;
            this.List = new List<ITerrainStencil<TGenContext, TTile>>();
            this.List.AddRange(stencils);
        }

        /// <summary>
        /// Determines if the entire map should be checked for connectivity, or just the immediate surrounding tiles.
        /// </summary>
        public List<ITerrainStencil<TGenContext, TTile>> List { get; set; }

        public bool RequireAny { get; set; }

        public bool Test(TGenContext map, Loc loc)
        {
            foreach (ITerrainStencil<TGenContext, TTile> subReq in this.List)
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

        public override string ToString()
        {
            if (this.RequireAny)
                return string.Format("Any of {0} Reqs", this.List.Count);
            return string.Format("All of {0} Reqs", this.List.Count);
        }
    }
}
