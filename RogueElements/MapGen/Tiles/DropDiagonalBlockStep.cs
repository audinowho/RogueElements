using System;

namespace RogueElements
{
    [Serializable]
    public class DropDiagonalBlockStep<T> : GenStep<T> where T : class, ITiledGenContext
    {
        public int Terrain;

        public DropDiagonalBlockStep() { }

        public DropDiagonalBlockStep(int terrain)
        {
            Terrain = terrain;
        }

        public override void Apply(T map)
        {
            for (int xx = 0; xx < map.Width - 1; xx++)
            {
                for (int yy = 0; yy < map.Height - 1; yy++)
                {
                    int a1 = map.Tiles[xx][yy].ID;
                    int b1 = map.Tiles[xx + 1][yy].ID;
                    int a2 = map.Tiles[xx][yy + 1].ID;
                    int b2 = map.Tiles[xx + 1][yy + 1].ID;

                    int dropType = map.Rand.Next(3);
                    if (a1 == Terrain && b1 == map.WallTerrain && a2 == map.WallTerrain && b2 == Terrain)
                    {
                        if (dropType % 2 == 0)
                            map.Tiles[xx + 1][yy].ID = Terrain;
                        if (dropType < 2)
                            map.Tiles[xx][yy + 1].ID = Terrain;
                    }
                    else if (a1 == map.WallTerrain && b1 == Terrain && a2 == Terrain && b2 == map.WallTerrain)
                    {
                        if (dropType % 2 == 0)
                            map.Tiles[xx][yy].ID = Terrain;
                        if (dropType < 2)
                            map.Tiles[xx + 1][yy + 1].ID = Terrain;
                    }
                }
            }
        }

    }
}
