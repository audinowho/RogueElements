// <copyright file="Stairs.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements.Examples
{
    /// <summary>
    /// Abstract base class for stair entities that connect dungeon floors.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Stairs implement <see cref="ISpawnable"/> to participate in RogueElements' spawning system.
    /// This allows them to be placed using spawn steps like <c>FloorStairsStep</c> which
    /// handles stair placement according to floor plan or grid plan room locations.
    /// </para>
    /// <para>
    /// In roguelike games, stairs typically represent:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Entrances from the previous floor (upstairs)</description></item>
    /// <item><description>Exits to the next floor (downstairs)</description></item>
    /// <item><description>Shortcuts, teleporters, or ladders in some games</description></item>
    /// </list>
    /// <para>
    /// See <see cref="StairsUp"/> and <see cref="StairsDown"/> for concrete implementations
    /// that also implement <see cref="IEntrance"/> and <see cref="IExit"/> respectively.
    /// </para>
    /// </remarks>
    /// <seealso cref="StairsUp"/>
    /// <seealso cref="StairsDown"/>
    /// <seealso cref="ISpawnable"/>
    public abstract class Stairs : ISpawnable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Stairs"/> class.
        /// </summary>
        protected Stairs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Stairs"/> class by copying another.
        /// </summary>
        /// <param name="other">The stairs instance to copy.</param>
        /// <remarks>
        /// Used by <see cref="Copy"/> implementations in derived classes to create
        /// independent copies for spawning.
        /// </remarks>
        protected Stairs(Stairs other)
        {
            this.Loc = other.Loc;
        }

        /// <summary>
        /// Gets or sets the map location where this stair is placed.
        /// </summary>
        /// <value>The tile coordinates of the stairs on the map.</value>
        /// <remarks>
        /// Set by spawn steps during map generation. Your game logic uses this
        /// to determine where to place the player when entering/exiting floors.
        /// </remarks>
        public Loc Loc { get; set; }

        /// <summary>
        /// Creates a deep copy of this stair entity.
        /// </summary>
        /// <returns>A new <see cref="ISpawnable"/> instance that is a copy of this stair.</returns>
        /// <remarks>
        /// Required by <see cref="ISpawnable"/> interface. The spawning system uses this
        /// to create instances from templates in spawn lists.
        /// </remarks>
        public abstract ISpawnable Copy();
    }
}
