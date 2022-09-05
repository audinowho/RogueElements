// <copyright file="DefaultBlobStencil.cs" company="Audino">
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
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class DefaultBlobStencil<T> : IBlobStencil<T>
        where T : class, ITiledGenContext
    {
        public bool Test(T map, Rect rect)
        {
            return true;
        }
    }
}
