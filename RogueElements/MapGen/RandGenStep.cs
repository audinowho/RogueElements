// <copyright file="RandGenStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A generation step that randomly selects and executes one or more child steps from a weighted collection.
    /// </summary>
    /// <typeparam name="T">
    /// The type of map context this step operates on. Must implement <see cref="IGenContext"/>.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="RandGenStep{T}"/> enables probabilistic generation by choosing which steps to execute
    /// based on random selection from a weighted pool. This is useful for introducing variety into
    /// map generation, such as randomly selecting room types, enemy spawns, or terrain features.
    /// </para>
    /// <para>
    /// The <see cref="Spawns"/> property accepts any <see cref="IMultiRandPicker{T}"/> implementation,
    /// which determines how many steps are selected and with what probability. Common implementations include:
    /// <list type="bullet">
    /// <item><description><see cref="SpawnList{T}"/> - Weighted random selection of a single item</description></item>
    /// <item><description><see cref="RandBag{T}"/> - Selection from a bag with possible repeats</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// When <see cref="Apply(T)"/> is called, the <see cref="IMultiRandPicker{T}.Roll"/> method determines
    /// which child steps to execute, then each selected step is applied in sequence.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create a step that randomly chooses between different room types
    /// var randStep = new RandGenStep&lt;ITiledGenContext&gt;();
    ///
    /// var spawns = new SpawnList&lt;GenStep&lt;ITiledGenContext&gt;&gt;();
    /// spawns.Add(new RoomGenSquare(), 50);  // 50% weight for square rooms
    /// spawns.Add(new RoomGenRound(), 30);   // 30% weight for round rooms
    /// spawns.Add(new RoomGenCave(), 20);    // 20% weight for cave rooms
    ///
    /// randStep.Spawns = spawns;
    /// layout.GenSteps.Add(new Priority(5), randStep);
    /// </code>
    /// </example>
    /// <seealso cref="GenStep{T}"/>
    /// <seealso cref="IMultiRandPicker{T}"/>
    [Serializable]
    public class RandGenStep<T> : GenStep<T>
        where T : class, IGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandGenStep{T}"/> class with no spawns configured.
        /// </summary>
        /// <remarks>
        /// The <see cref="Spawns"/> property must be set before this step is applied,
        /// otherwise an exception will be thrown during generation.
        /// </remarks>
        public RandGenStep()
        {
        }

        /// <summary>
        /// Gets or sets the random picker that determines which child steps to execute.
        /// </summary>
        /// <value>
        /// An <see cref="IMultiRandPicker{T}"/> that returns a list of <see cref="GenStep{T}"/>
        /// instances to execute when <see cref="IMultiRandPicker{T}.Roll"/> is called.
        /// </value>
        /// <remarks>
        /// <para>
        /// The picker is rolled using <see cref="IGenContext.Rand"/> from the map context,
        /// ensuring reproducible results when using the same generation seed.
        /// </para>
        /// <para>
        /// This property must be set before <see cref="Apply(T)"/> is called.
        /// </para>
        /// </remarks>
        public IMultiRandPicker<GenStep<T>> Spawns { get; set; }

        /// <summary>
        /// Randomly selects and executes child generation steps from the <see cref="Spawns"/> collection.
        /// </summary>
        /// <param name="map">The map context to modify.</param>
        /// <remarks>
        /// This method rolls the <see cref="Spawns"/> picker to determine which steps to execute,
        /// then applies each selected step to the map in sequence. The number of steps executed
        /// depends on the picker implementation.
        /// </remarks>
        public override void Apply(T map)
        {
            List<GenStep<T>> steps = this.Spawns.Roll(map.Rand);
            foreach (GenStep<T> step in steps)
                step.Apply(map);
        }

        /// <summary>
        /// Returns a string representation of this step, including its spawns information.
        /// </summary>
        /// <returns>
        /// A formatted string containing the type name and, if <see cref="Spawns"/> is set,
        /// the spawns description in brackets.
        /// </returns>
        public override string ToString()
        {
            if (this.Spawns == null)
                return string.Format("{0}", this.GetType().GetFormattedTypeName());

            return string.Format("{0}[{1}]", this.GetType().GetFormattedTypeName(), this.Spawns.ToString());
        }
    }
}
