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

        ITile[][] Tiles { get; }
        int Width { get; }
        int Height { get; }
        void CreateNew(int tileWidth, int tileHeight);
    }
}
