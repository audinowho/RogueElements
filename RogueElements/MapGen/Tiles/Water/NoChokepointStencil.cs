// <copyright file="NoChokepointStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A filter for determining the eligible tiles for an operation.
    /// Locations that, if all made unwalkable, do not cause a chokepoint to be removed, are eligible.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class NoChokepointStencil<T> : ITerrainStencil<T>
        where T : class, ITiledGenContext
    {
        /// <summary>
        /// Determines if the entire map should be checked for connectivity, or just the immediate surrounding tiles.
        /// </summary>
        public bool Global { get; set; }

        public bool Test(T map, Rect rect)
        {
            bool IsMapValid(Loc loc) => map.RoomTerrain.TileEquivalent(map.GetTile(loc));
            bool IsBlobValid(Loc loc) => true;

            Rect checkArea;
            if (this.Global)
            {
                checkArea = new Rect(0, 0, map.Width, map.Height);
            }
            else
            {
                checkArea = rect;
                checkArea.Inflate(1, 1);
                checkArea = Rect.Intersect(checkArea, new Rect(0, 0, map.Width, map.Height));
            }

            return Detection.DetectDisconnect(checkArea, IsMapValid, rect.Start, rect.Size, IsBlobValid, true);
        }
    }
}
