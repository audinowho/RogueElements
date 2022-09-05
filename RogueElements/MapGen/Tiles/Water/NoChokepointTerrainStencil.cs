// <copyright file="NoChokepointTerrainStencil.cs" company="Audino">
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
    public class NoChokepointTerrainStencil<T> : ITerrainStencil<T>
        where T : class, ITiledGenContext
    {
        public NoChokepointTerrainStencil()
        {
            this.TileStencil = new DefaultTerrainStencil<T>();
        }

        public NoChokepointTerrainStencil(ITerrainStencil<T> tileStencil)
        {
            this.TileStencil = tileStencil;
        }

        public ITerrainStencil<T> TileStencil { get; set; }

        /// <summary>
        /// Determines if the entire map should be checked for connectivity, or just the immediate surrounding tiles.
        /// </summary>
        public bool Global { get; set; }

        public bool Negate { get; set; }

        public bool Test(T map, Loc testLoc)
        {
            bool IsMapValid(Loc loc) => this.TileStencil.Test(map, loc);
            bool IsBlobValid(Loc loc) => true;

            Rect checkArea;
            if (this.Global)
            {
                checkArea = new Rect(0, 0, map.Width, map.Height);
            }
            else
            {
                checkArea = new Rect(testLoc, Loc.One);
                checkArea.Inflate(1, 1);
                checkArea = Rect.Intersect(checkArea, new Rect(0, 0, map.Width, map.Height));
            }

            return this.Negate == Detection.DetectDisconnect(checkArea, IsMapValid, testLoc, Loc.One, IsBlobValid, true);
        }
    }
}
