// <copyright file="MultiBlobStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Provides a blob stencil that combines multiple stencils with AND/OR logic.
    /// </summary>
    /// <typeparam name="T">The type of map context that implements <see cref="ITiledGenContext"/>.</typeparam>
    [Serializable]
    public class MultiBlobStencil<T> : IBlobStencil<T>
        where T : class, ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiBlobStencil{T}"/> class.
        /// </summary>
        public MultiBlobStencil()
        {
            this.List = new List<IBlobStencil<T>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiBlobStencil{T}"/> class with the specified stencils.
        /// </summary>
        /// <param name="requireAny">Whether any single stencil passing is sufficient (OR logic), or all must pass (AND logic).</param>
        /// <param name="stencils">The stencils to combine.</param>
        public MultiBlobStencil(bool requireAny, params IBlobStencil<T>[] stencils)
        {
            this.RequireAny = requireAny;
            this.List = new List<IBlobStencil<T>>();
            this.List.AddRange(stencils);
        }

        /// <summary>
        /// Gets or sets the list of blob stencils to combine.
        /// </summary>
        public List<IBlobStencil<T>> List { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether any single stencil passing is sufficient.
        /// When <c>true</c>, uses OR logic; when <c>false</c>, uses AND logic.
        /// </summary>
        public bool RequireAny { get; set; }

        /// <inheritdoc/>
        public bool Test(T map, Rect rect, Grid.LocTest blobTest)
        {
            foreach (IBlobStencil<T> subReq in this.List)
            {
                if (this.RequireAny)
                {
                    if (subReq.Test(map, rect, blobTest))
                        return true;
                }
                else
                {
                    if (!subReq.Test(map, rect, blobTest))
                        return false;
                }
            }

            return !this.RequireAny;
        }

        public override string ToString()
        {
            if (this.RequireAny)
                return string.Format("Any of {0} Blob Reqs", this.List.Count);
            return string.Format("All of {0} Blob Reqs", this.List.Count);
        }
    }
}
