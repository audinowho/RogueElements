// <copyright file="DropDiagonalBlockStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    [Serializable]
    public class DropDiagonalBlockStep<T> : GenStep<T>
        where T : class, ITiledGenContext
    {
        public DropDiagonalBlockStep()
        {
        }

        public DropDiagonalBlockStep(ITile terrain)
        {
            this.Terrain = terrain;
        }

        public ITile Terrain { get; set; }

        public override void Apply(T map)
        {
            for (int xx = 0; xx < map.Width - 1; xx++)
            {
                for (int yy = 0; yy < map.Height - 1; yy++)
                {
                    ITile a1 = map.GetTile(new Loc(xx, yy));
                    ITile b1 = map.GetTile(new Loc(xx + 1, yy));
                    ITile a2 = map.GetTile(new Loc(xx, yy + 1));
                    ITile b2 = map.GetTile(new Loc(xx + 1, yy + 1));

                    int dropType = map.Rand.Next(3);
                    if (a1.TileEquivalent(this.Terrain) && b1.TileEquivalent(map.WallTerrain) && a2.TileEquivalent(map.WallTerrain) && b2.TileEquivalent(this.Terrain))
                    {
                        if (dropType % 2 == 0)
                            map.TrySetTile(new Loc(xx + 1, yy), this.Terrain.Copy());
                        if (dropType < 2)
                            map.TrySetTile(new Loc(xx, yy + 1), this.Terrain.Copy());
                    }
                    else if (a1.TileEquivalent(map.WallTerrain) && b1.TileEquivalent(this.Terrain) && a2.TileEquivalent(this.Terrain) && b2.TileEquivalent(map.WallTerrain))
                    {
                        if (dropType % 2 == 0)
                            map.TrySetTile(new Loc(xx, yy), this.Terrain.Copy());
                        if (dropType < 2)
                            map.TrySetTile(new Loc(xx + 1, yy + 1), this.Terrain.Copy());
                    }
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.GetType().Name, this.Terrain.ToString());
        }
    }
}
