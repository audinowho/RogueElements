using System;
using System.Collections.Generic;

namespace RogueElements
{

    public interface ITiledGenContext : IGenContext
    {

        bool TileBlocked(Loc loc);
        bool TileBlocked(Loc loc, bool diagonal);

        int RoomTerrain { get; }
        int WallTerrain { get; }

        ITile[][] Tiles { get; }
        int Width { get; }
        int Height { get; }
        void CreateNew(int tileWidth, int tileHeight);
    }
}
