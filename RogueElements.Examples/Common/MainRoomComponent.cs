// <copyright file="MainRoomComponent.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using RogueElements;

namespace RogueElements.Examples
{
    /// <summary>
    /// A room component marker that identifies rooms as "main" rooms in the dungeon layout.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Room components in RogueElements are metadata tags attached to rooms in a <see cref="FloorPlan"/>
    /// or <see cref="GridPlan"/>. They enable generation steps to filter and identify specific
    /// rooms for targeted operations.
    /// </para>
    /// <para>
    /// <b>Common uses for MainRoomComponent:</b>
    /// </para>
    /// <list type="bullet">
    /// <item><description>
    /// Identifying rooms that should receive standard enemy spawns
    /// </description></item>
    /// <item><description>
    /// Distinguishing regular rooms from special rooms (treasure rooms, boss rooms)
    /// </description></item>
    /// <item><description>
    /// Filtering rooms for item placement that should only appear in main areas
    /// </description></item>
    /// </list>
    /// <para>
    /// Apply this component during room generation using steps like <c>SetGridDefaultsStep</c>
    /// or by adding it directly to room generators.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Filter for main rooms when spawning enemies
    /// var spawnStep = new RandomSpawnStep&lt;MyContext, Mob&gt;(mobList)
    /// {
    ///     Filters = new List&lt;BaseRoomFilter&gt;
    ///     {
    ///         new RoomFilterComponent(true, new MainRoomComponent())
    ///     }
    /// };
    /// </code>
    /// </example>
    /// <seealso cref="TreasureRoomComponent"/>
    /// <seealso cref="MainHallComponent"/>
    /// <seealso cref="RoomComponent"/>
    public class MainRoomComponent : RoomComponent
    {
        /// <summary>
        /// Creates a copy of this room component.
        /// </summary>
        /// <returns>A new <see cref="MainRoomComponent"/> instance.</returns>
        /// <remarks>
        /// Required by <see cref="RoomComponent"/> base class. Room components are cloned
        /// when rooms are copied during floor plan manipulation.
        /// </remarks>
        public override RoomComponent Clone()
        {
            return new MainRoomComponent();
        }
    }
}