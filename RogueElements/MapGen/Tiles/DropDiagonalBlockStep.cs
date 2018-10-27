using System;

namespace RogueElements
{
    [Serializable]
    public class DropDiagonalBlockStep<T> : GenStep<T> where T : class, ITiledGenContext
    {
        public ITile Terrain;

        public DropDiagonalBlockStep() { }

        public DropDiagonalBlockStep(ITile terrain)
        {
            Terrain = terrain;
        }

        public override void Apply(T map)
        {
            for (int xx = 0; xx < map.Width - 1; xx++)
            {
                for (int yy = 0; yy < map.Height - 1; yy++)
                {
                    ITile a1 = map.GetTile(new Loc(xx,yy));
                    ITile b1 = map.GetTile(new Loc(xx+1, yy));
                    ITile a2 = map.GetTile(new Loc(xx, yy+1));
                    ITile b2 = map.GetTile(new Loc(xx+1, yy+1));

                    int dropType = map.Rand.Next(3);
                    if (a1.TileEquivalent(Terrain) && b1.TileEquivalent(map.WallTerrain) && a2.TileEquivalent(map.WallTerrain) && b2.TileEquivalent(Terrain))
                    {
                        if (dropType % 2 == 0)
                            map.SetTile(new Loc(xx + 1, yy), Terrain.Copy());
                        if (dropType < 2)
                            map.SetTile(new Loc(xx, yy + 1), Terrain.Copy());
                    }
                    else if (a1.TileEquivalent(map.WallTerrain) && b1.TileEquivalent(Terrain) && a2.TileEquivalent(Terrain) && b2.TileEquivalent(map.WallTerrain))
                    {
                        if (dropType % 2 == 0)
                            map.SetTile(new Loc(xx, yy), Terrain.Copy());
                        if (dropType < 2)
                            map.SetTile(new Loc(xx + 1, yy + 1), Terrain.Copy());
                    }
                }
            }
        }

    }
}
