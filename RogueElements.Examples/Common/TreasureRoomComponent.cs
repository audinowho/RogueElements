// <copyright file="TreasureRoomComponent.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using RogueElements;

namespace RogueElements.Examples
{
    /// <summary>
    /// A room component marker that identifies rooms as treasure rooms containing valuable loot.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Treasure rooms are special rooms that typically contain better items, chests, or rewards.
    /// This component allows generation steps to identify these rooms for specialized spawning
    /// behavior.
    /// </para>
    /// <para>
    /// <b>Common uses for TreasureRoomComponent:</b>
    /// </para>
    /// <list type="bullet">
    /// <item><description>
    /// Spawning high-value items or rare equipment only in treasure rooms
    /// </description></item>
    /// <item><description>
    /// Adding treasure chests or containers to marked rooms
    /// </description></item>
    /// <item><description>
    /// Excluding treasure rooms from regular enemy spawns
    /// </description></item>
    /// <item><description>
    /// Applying special visual theming (gold piles, gem decorations)
    /// </description></item>
    /// </list>
    /// <para>
    /// In RogueElements examples, this component is used with <c>SetSpecialRoomStep</c> to
    /// create distinct treasure chambers off the main dungeon path.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create a special treasure room with custom spawning
    /// var treasureRoomStep = new SetSpecialRoomStep&lt;MyContext&gt;()
    /// {
    ///     RoomComponents = new List&lt;RoomComponent&gt; { new TreasureRoomComponent() },
    ///     Room = new RoomGenSquare&lt;MyContext&gt;(new RandRange(6, 8), new RandRange(6, 8))
    /// };
    ///
    /// // Later, spawn items only in treasure rooms
    /// var itemStep = new RandomSpawnStep&lt;MyContext, Item&gt;(treasureItems)
    /// {
    ///     Filters = new List&lt;BaseRoomFilter&gt;
    ///     {
    ///         new RoomFilterComponent(true, new TreasureRoomComponent())
    ///     }
    /// };
    /// </code>
    /// </example>
    /// <seealso cref="MainRoomComponent"/>
    /// <seealso cref="RoomComponent"/>
    public class TreasureRoomComponent : RoomComponent
    {
        /// <summary>
        /// Creates a copy of this room component.
        /// </summary>
        /// <returns>A new <see cref="TreasureRoomComponent"/> instance.</returns>
        /// <remarks>
        /// Required by <see cref="RoomComponent"/> base class. Room components are cloned
        /// when rooms are copied during floor plan manipulation.
        /// </remarks>
        public override RoomComponent Clone()
        {
            return new TreasureRoomComponent();
        }
    }
}