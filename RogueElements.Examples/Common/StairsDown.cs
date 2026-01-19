// <copyright file="StairsDown.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using RogueElements;

namespace RogueElements.Examples
{
    /// <summary>
    /// Represents downward stairs serving as the exit point for a dungeon floor.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class implements both <see cref="ISpawnable"/> (inherited from <see cref="Stairs"/>)
    /// and <see cref="IExit"/>, making it suitable for use with exit placement steps.
    /// The <see cref="IExit"/> marker interface tells RogueElements that this entity
    /// represents where the player leaves the floor to descend deeper.
    /// </para>
    /// <para>
    /// In typical roguelike conventions:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Downstairs lead to the next (deeper) floor</description></item>
    /// <item><description>Reaching downstairs is often the floor's objective</description></item>
    /// <item><description>Often displayed as '&gt;' in ASCII roguelikes</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Register downstairs in a spawn list for exit placement
    /// var exitList = new SpawnList&lt;IExit&gt;();
    /// exitList.Add(new StairsDown(), 10);
    ///
    /// // Add to generation pipeline
    /// layout.GenSteps.Add(new FloorStairsStep&lt;MyContext, IEntrance, IExit&gt;(entranceList, exitList));
    /// </code>
    /// </example>
    /// <seealso cref="StairsUp"/>
    /// <seealso cref="Stairs"/>
    /// <seealso cref="IExit"/>
    public class StairsDown : Stairs, IExit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StairsDown"/> class.
        /// </summary>
        public StairsDown()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StairsDown"/> class by copying another.
        /// </summary>
        /// <param name="other">The stairs instance to copy.</param>
        protected StairsDown(StairsDown other)
            : base(other)
        {
        }

        /// <summary>
        /// Creates a deep copy of this stair entity.
        /// </summary>
        /// <returns>A new <see cref="StairsDown"/> instance with the same location.</returns>
        public override ISpawnable Copy() => new StairsDown(this);
    }
}