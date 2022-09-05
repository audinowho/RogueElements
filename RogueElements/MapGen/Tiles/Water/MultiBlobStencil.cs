// <copyright file="MultiBlobStencil.cs" company="Audino">
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
    public class MultiBlobStencil<T> : IBlobStencil<T>
        where T : class, ITiledGenContext
    {
        public MultiBlobStencil()
        {
            this.List = new List<IBlobStencil<T>>();
        }

        public MultiBlobStencil(bool requireAny, params IBlobStencil<T>[] stencils)
        {
            this.RequireAny = requireAny;
            this.List = new List<IBlobStencil<T>>();
            this.List.AddRange(stencils);
        }

        /// <summary>
        /// Determines if the entire map should be checked for connectivity, or just the immediate surrounding tiles.
        /// </summary>
        public List<IBlobStencil<T>> List { get; set; }

        public bool RequireAny { get; set; }

        public bool Test(T map, Rect rect)
        {
            foreach (IBlobStencil<T> subReq in this.List)
            {
                if (this.RequireAny)
                {
                    if (subReq.Test(map, rect))
                        return true;
                }
                else
                {
                    if (!subReq.Test(map, rect))
                        return false;
                }
            }

            return !this.RequireAny;
        }
    }
}
