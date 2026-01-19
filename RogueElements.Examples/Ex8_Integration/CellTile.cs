// <copyright file="CellTile.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using RogueSharp;

namespace RogueElements
{
    /// <summary>
    /// Adapter that wraps RogueSharp's Cell to implement RogueElements' ITile interface.
    /// Enables RogueElements to work with RogueSharp's map representation.
    /// </summary>
    /// <remarks>
    /// This is the bridge between RogueElements and RogueSharp type systems.
    /// RogueElements uses ITile for tile operations; RogueSharp uses ICell/Cell.
    /// CellTile inherits from Cell (for RogueSharp compatibility) and implements
    /// ITile (for RogueElements compatibility).
    ///
    /// Key mapping:
    /// - ITile.TileEquivalent -> Compares IsWalkable (floor vs wall)
    /// - ITile.Copy -> Creates a new CellTile with same properties
    /// </remarks>
    public class CellTile : Cell, ITile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CellTile"/> class.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="isTransparent">Whether light passes through.</param>
        /// <param name="isWalkable">Whether entities can walk on this tile.</param>
        /// <param name="isInFov">Whether currently in field of view.</param>
        public CellTile(int x, int y, bool isTransparent, bool isWalkable, bool isInFov)
            : base(x, y, isTransparent, isWalkable, isInFov)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CellTile"/> class with exploration state.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="isTransparent">Whether light passes through.</param>
        /// <param name="isWalkable">Whether entities can walk on this tile.</param>
        /// <param name="isInFov">Whether currently in field of view.</param>
        /// <param name="isExplored">Whether the player has seen this tile.</param>
        public CellTile(int x, int y, bool isTransparent, bool isWalkable, bool isInFov, bool isExplored)
            : base(x, y, isTransparent, isWalkable, isInFov, isExplored)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CellTile"/> class by copying another cell.
        /// </summary>
        /// <param name="other">The cell to copy properties from.</param>
        protected CellTile(ICell other)
            : base(other.X, other.Y, other.IsTransparent, other.IsWalkable, other.IsInFov, other.IsExplored)
        {
        }

        /// <summary>
        /// Creates a CellTile from a RogueSharp ICell.
        /// Factory method for wrapping existing cells.
        /// </summary>
        /// <param name="other">The RogueSharp cell to wrap.</param>
        /// <returns>A new CellTile with the same properties.</returns>
        public static CellTile FromCell(ICell other) => new CellTile(other);

        /// <summary>
        /// Checks if this tile is equivalent to another for generation purposes.
        /// Compares walkability - the key distinction between floor and wall.
        /// </summary>
        /// <param name="other">The tile to compare against.</param>
        /// <returns>True if both tiles have the same walkability.</returns>
        public bool TileEquivalent(ITile other) => (other is ICell cell) && cell?.IsWalkable == this.IsWalkable;

        /// <summary>
        /// Creates a copy of this tile.
        /// Required by ITile for tile operations that need independent copies.
        /// </summary>
        /// <returns>A new CellTile with the same properties.</returns>
        public ITile Copy() => new CellTile(this);
    }
}
