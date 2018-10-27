using System;
using System.Collections.Generic;

namespace RogueElements
{

    public interface ITiledGenContext : IGenContext
    {
        bool TileBlocked(Loc loc);
        bool TileBlocked(Loc loc, bool diagonal);

        ITile RoomTerrain { get; }
        ITile WallTerrain { get; }

        ITile GetTile(Loc loc);
        void SetTile(Loc loc, ITile tile);
        int Width { get; }
        int Height { get; }
        void CreateNew(int tileWidth, int tileHeight);
        bool TilesInitialized { get; }
    }
}
