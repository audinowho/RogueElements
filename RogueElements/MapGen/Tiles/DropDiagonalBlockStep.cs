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
            for (int x = 1; x < map.Width - 1; x++)
            {
                for (int y = 1; y < map.Height - 1; y++)
                {
                    int a1 = map.Tiles[x][y].ID;
                    int b1 = map.Tiles[x + 1][y].ID;
                    int a2 = map.Tiles[x][y + 1].ID;
                    int b2 = map.Tiles[x + 1][y + 1].ID;

                    int dropType = map.Rand.Next(3);
                    if (a1 == Terrain && b1 == map.WallTerrain && a2 == map.WallTerrain && b2 == Terrain)
                    {
                        if (dropType % 2 == 0)
                            map.Tiles[x + 1][y].ID = Terrain;
                        if (dropType < 2)
                            map.Tiles[x][y + 1].ID = Terrain;
                    }
                    else if (a1 == map.WallTerrain && b1 == Terrain && a2 == Terrain && b2 == map.WallTerrain)
                    {
                        if (dropType % 2 == 0)
                            map.Tiles[x][y].ID = Terrain;
                        if (dropType < 2)
                            map.Tiles[x + 1][y + 1].ID = Terrain;
                    }
                }
            }
        }

    }
}
