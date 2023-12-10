// <copyright file="EraseIsolatedStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Erases blobs of terrain that do not touch walkable ground.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    /// <typeparam name="TEntrance"></typeparam>
    [Serializable]
    public class EraseIsolatedFromSpawnStep<TGenContext, TEntrance> : GenStep<TGenContext>
        where TGenContext : class, ITiledGenContext, IViewPlaceableGenContext<TEntrance>
        where TEntrance : IEntrance
    {
        public EraseIsolatedFromSpawnStep()
        {
        }

        public EraseIsolatedFromSpawnStep(ITile terrain)
        {
            this.Terrain = terrain;
        }

        public ITile Terrain { get; set; }

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
