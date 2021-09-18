// <copyright file="BaseHallBrush.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public abstract class BaseHallBrush
    {
        /// <summary>
        /// Communicates to the Room/HallGen the size of the brush, for alignment purposes.
        /// </summary>
        public abstract Loc Size { get; }

        /// <summary>
        /// Communicates to the Hoom/HallGen the center location of the brush, for alignment purposes.
        /// </summary>
        public abstract Loc Center { get; }

        public abstract BaseHallBrush Clone();

        public abstract void DrawHallBrush(ITiledGenContext map, Rect bounds, Loc point, bool vertical);
    }
}
