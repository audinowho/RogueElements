// <copyright file="TestGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RogueElements.Tests
{
    public class TestGenContext : ITiledGenContext
    {
        public IRandom Rand { get; private set; }

        public ITile RoomTerrain => new TestTile(0);

        public ITile WallTerrain => new TestTile(1);

        public bool TilesInitialized => this.Tiles != null;

        public ITile[][] Tiles { get; private set; }

        public int Width => this.Tiles.Length;

        public int Height => this.Tiles[0].Length;

        public bool Wrap => false;

        public static TestGenContext InitGridToContext(string[] inGrid)
        {
            // transposes
            var testContext = new TestGenContext();
            testContext.CreateNew(inGrid[0].Length, inGrid.Length);
            for (int xx = 0; xx < testContext.Width; xx++)
            {
                for (int yy = 0; yy < testContext.Height; yy++)
                {
                    ((TestTile)testContext.Tiles[xx][yy]).ID = (inGrid[yy][xx] == 'X') ? 1 : 0;
                }
            }

            return testContext;
        }

        public void InitSeed(ulong seed)
        {
        }

        public void SetTestRand(IRandom rand) => this.Rand = rand;

        public void FinishGen()
        {
        }

        public bool TileBlocked(Loc loc) => this.TileBlocked(loc, false);

        public bool TileBlocked(Loc loc, bool diagonal) => ((TestTile)this.Tiles[loc.X][loc.Y]).ID != 0;

        public ITile GetTile(Loc loc) => this.Tiles[loc.X][loc.Y];

        public void SetTile(Loc loc, ITile tile) => this.Tiles[loc.X][loc.Y] = tile;

        public bool TrySetTile(Loc loc, ITile tile)
        {
            this.SetTile(loc, tile);
            return true;
        }

        public bool CanSetTile(Loc loc, ITile tile) => true;

        public void CreateNew(int tileWidth, int tileHeight, bool wrap = false)
        {
            this.Tiles = new ITile[tileWidth][];
            for (int ii = 0; ii < tileWidth; ii++)
            {
                this.Tiles[ii] = new ITile[tileHeight];
                for (int jj = 0; jj < tileHeight; jj++)
                    this.Tiles[ii][jj] = new TestTile();
            }
        }
    }
}