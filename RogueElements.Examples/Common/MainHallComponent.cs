// <copyright file="MainHallComponent.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using RogueElements;

namespace RogueElements.Examples
{
    /// <summary>
    /// A room component marker that identifies hallways as "main" hallways in the dungeon layout.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Hall components work similarly to room components but are specifically used to tag
    /// hallways (corridors) connecting rooms. In RogueElements, hallways are also represented
    /// as rooms in the <see cref="FloorPlan"/> and can have components attached.
    /// </para>
    /// <para>
    /// <b>Common uses for MainHallComponent:</b>
    /// </para>
    /// <list type="bullet">
    /// <item><description>
    /// Distinguishing primary connection corridors from secret passages
    /// </description></item>
    /// <item><description>
    /// Controlling where hallway-specific spawns can occur (traps, ambushes)
    /// </description></item>
    /// <item><description>
    /// Filtering hallways for terrain modifications (adding puddles, debris)
    /// </description></item>
    /// </list>
    /// <para>
    /// Apply this component during hallway generation using steps like <c>SetGridDefaultsStep</c>
    /// with hall-specific settings.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Set default hall component during grid initialization
    /// var defaultsStep = new SetGridDefaultsStep&lt;MyContext&gt;()
    /// {
    ///     DefaultHallComponent = new MainHallComponent()
    /// };
    /// </code>
    /// </example>
    /// <seealso cref="MainRoomComponent"/>
    /// <seealso cref="RoomComponent"/>
    public class MainHallComponent : RoomComponent
    {
        /// <summary>
        /// Creates a copy of this hall component.
        /// </summary>
        /// <returns>A new <see cref="MainHallComponent"/> instance.</returns>
        /// <remarks>
        /// Required by <see cref="RoomComponent"/> base class. Hall components are cloned
        /// when hallways are copied during floor plan manipulation.
        /// </remarks>
        public override RoomComponent Clone()
        {
            return new MainHallComponent();
        }
    }
}