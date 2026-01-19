// <copyright file="DefaultBlobStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Provides a blob stencil that allows all blob placements to pass.
    /// </summary>
    /// <typeparam name="T">The type of map context that implements <see cref="ITiledGenContext"/>.</typeparam>
    [Serializable]
    public class DefaultBlobStencil<T> : IBlobStencil<T>
        where T : class, ITiledGenContext
    {
        /// <inheritdoc/>
        public bool Test(T map, Rect rect, Grid.LocTest blobTest)
        {
            return true;
        }

        public override string ToString()
        {
            return string.Format("Any Tiles for Blob");
        }
    }
}
