// <copyright file="CellTile.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using RogueSharp;

namespace RogueElements
{
    public class CellTile : Cell, ITile
    {
        public CellTile(int x, int y, bool isTransparent, bool isWalkable, bool isInFov)
            : base(x, y, isTransparent, isWalkable, isInFov)
        {
        }

        public CellTile(int x, int y, bool isTransparent, bool isWalkable, bool isInFov, bool isExplored)
            : base(x, y, isTransparent, isWalkable, isInFov, isExplored)
        {
        }

        protected CellTile(ICell other)
            : base(other.X, other.Y, other.IsTransparent, other.IsWalkable, other.IsInFov, other.IsExplored)
        {
        }

        public static CellTile FromCell(ICell other) => new CellTile(other);

        public bool TileEquivalent(ITile other) => (other is ICell cell) && cell?.IsWalkable == this.IsWalkable;

        public ITile Copy() => new CellTile(this);
    }
}