// <copyright file="EraseIsolatedStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Erases terrain blobs that are not connected to any walkable ground by replacing them with wall terrain.
    /// </summary>
    /// <typeparam name="T">The type of map context that implements <see cref="ITiledGenContext"/>.</typeparam>
    [Serializable]
    public class EraseIsolatedStep<T> : GenStep<T>
        where T : class, ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EraseIsolatedStep{T}"/> class.
        /// </summary>
        public EraseIsolatedStep()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EraseIsolatedStep{T}"/> class with the specified terrain.
        /// </summary>
        /// <param name="terrain">The terrain type to check for isolation.</param>
        public EraseIsolatedStep(ITile terrain)
        {
            this.Terrain = terrain;
        }

        /// <summary>
        /// Gets or sets the terrain type to check for isolation and erase if disconnected.
        /// </summary>
        public ITile Terrain { get; set; }

        /// <inheritdoc/>
        public override void Apply(T map)
        {
            bool[][] connectionGrid = new bool[map.Width][];
            for (int xx = 0; xx < map.Width; xx++)
            {
                connectionGrid[xx] = new bool[map.Height];
                for (int yy = 0; yy < map.Height; yy++)
                    connectionGrid[xx][yy] = false;
            }

            for (int xx = 0; xx < map.Width; xx++)
            {
                for (int yy = 0; yy < map.Height; yy++)
                {
                    // upon detecting an unmarked room area, fill with connected marks
                    if (!map.TileBlocked(new Loc(xx, yy)) && !connectionGrid[xx][yy])
                    {
                        Grid.FloodFill(
                            new Rect(0, 0, map.Width, map.Height),
                            (Loc testLoc) =>
                            {
                                bool blocked = map.TileBlocked(testLoc);
                                blocked &= !this.Terrain.TileEquivalent(map.GetTile(testLoc));
                                return connectionGrid[testLoc.X][testLoc.Y] || blocked;
                            },
                            (Loc testLoc) => true,
                            (Loc fillLoc) => connectionGrid[fillLoc.X][fillLoc.Y] = true,
                            new Loc(xx, yy));
                    }
                }
            }

            for (int xx = 0; xx < map.Width; xx++)
            {
                for (int yy = 0; yy < map.Height; yy++)
                {
                    if (this.Terrain.TileEquivalent(map.GetTile(new Loc(xx, yy))) && !connectionGrid[xx][yy])
                        map.SetTile(new Loc(xx, yy), map.WallTerrain.Copy());
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.GetType().GetFormattedTypeName(), this.Terrain.ToString());
        }
    }
}
