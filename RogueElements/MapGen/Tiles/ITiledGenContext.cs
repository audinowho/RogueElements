// <copyright file="ITiledGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface ITiledGenContext<TTile> : IGenContext
        where TTile : ITile<TTile>
    {
        TTile RoomTerrain { get; }

        TTile WallTerrain { get; }

        int Width { get; }

        int Height { get; }

        bool Wrap { get; }

        bool TilesInitialized { get; }

        bool TileBlocked(Loc loc);

        bool TileBlocked(Loc loc, bool diagonal);

        TTile GetTile(Loc loc);

        bool CanSetTile(Loc loc, TTile tile);

        bool TrySetTile(Loc loc, TTile tile);

        void SetTile(Loc loc, TTile tile);

        void CreateNew(int tileWidth, int tileHeight, bool wrap = false);
    }
}
