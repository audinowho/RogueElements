// <copyright file="StairsUp.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using RogueElements;

namespace RogueElements.Examples
{
    /// <summary>
    /// Represents upward stairs serving as the entrance point for a dungeon floor.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class implements both <see cref="ISpawnable"/> (inherited from <see cref="Stairs"/>)
    /// and <see cref="IEntrance"/>, making it suitable for use with entrance placement steps.
    /// The <see cref="IEntrance"/> marker interface tells RogueElements that this entity
    /// represents where the player enters the floor.
    /// </para>
    /// <para>
    /// In typical roguelike conventions:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Upstairs lead back to the previous (shallower) floor</description></item>
    /// <item><description>The player starts on upstairs when entering a new floor from above</description></item>
    /// <item><description>Often displayed as '&lt;' in ASCII roguelikes</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Register upstairs in a spawn list for entrance placement
    /// var entranceList = new SpawnList&lt;IEntrance&gt;();
    /// entranceList.Add(new StairsUp(), 10);
    ///
    /// // Add to generation pipeline
    /// layout.GenSteps.Add(new FloorStairsStep&lt;MyContext, IEntrance, IExit&gt;(entranceList, exitList));
    /// </code>
    /// </example>
    /// <seealso cref="StairsDown"/>
    /// <seealso cref="Stairs"/>
    /// <seealso cref="IEntrance"/>
    public class StairsUp : Stairs, IEntrance
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StairsUp"/> class.
        /// </summary>
        public StairsUp()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StairsUp"/> class by copying another.
        /// </summary>
        /// <param name="other">The stairs instance to copy.</param>
        protected StairsUp(StairsUp other)
            : base(other)
        {
        }

        /// <summary>
        /// Creates a deep copy of this stair entity.
        /// </summary>
        /// <returns>A new <see cref="StairsUp"/> instance with the same location.</returns>
        public override ISpawnable Copy() => new StairsUp(this);
    }
}