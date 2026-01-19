// <copyright file="EraseIsolatedFromSpawnStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Erases terrain blobs that are not reachable from the spawn point by replacing them with wall terrain.
    /// </summary>
    /// <typeparam name="TGenContext">The type of map context that implements tile and placement contexts.</typeparam>
    /// <typeparam name="TEntrance">The type of entrance used to determine the spawn point.</typeparam>
    /// <remarks>
    /// Unlike <see cref="EraseIsolatedStep{T}"/>, this step uses the entrance location as the starting
    /// point for connectivity checks rather than any walkable tile.
    /// </remarks>
    [Serializable]
    public class EraseIsolatedFromSpawnStep<TGenContext, TEntrance> : GenStep<TGenContext>
        where TGenContext : class, ITiledGenContext, IViewPlaceableGenContext<TEntrance>
        where TEntrance : IEntrance
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EraseIsolatedFromSpawnStep{TGenContext, TEntrance}"/> class.
        /// </summary>
        public EraseIsolatedFromSpawnStep()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EraseIsolatedFromSpawnStep{TGenContext, TEntrance}"/> class with the specified terrain.
        /// </summary>
        /// <param name="terrain">The terrain type to check for isolation from spawn.</param>
        public EraseIsolatedFromSpawnStep(ITile terrain)
        {
            this.Terrain = terrain;
        }

        /// <summary>
        /// Gets or sets the terrain type to check for isolation and erase if not reachable from spawn.
        /// </summary>
        public ITile Terrain { get; set; }

        /// <inheritdoc/>
        public override void Apply(TGenContext map)
        {
            bool[][] connectionGrid = new bool[map.Width][];
            for (int xx = 0; xx < map.Width; xx++)
            {
                connectionGrid[xx] = new bool[map.Height];
                for (int yy = 0; yy < map.Height; yy++)
                    connectionGrid[xx][yy] = false;
            }

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
            map.GetLoc(0));

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
