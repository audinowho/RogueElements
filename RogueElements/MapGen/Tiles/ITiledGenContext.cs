// <copyright file="ITiledGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

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
        bool CanSetTile(Loc loc, ITile tile);
        bool TrySetTile(Loc loc, ITile tile);
        void SetTile(Loc loc, ITile tile);
        int Width { get; }
        int Height { get; }
        void CreateNew(int tileWidth, int tileHeight);
        bool TilesInitialized { get; }
    }
}
