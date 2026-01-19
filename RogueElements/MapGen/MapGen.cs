// <copyright file="MapGen.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RogueElements
{
    /// <summary>
    /// Orchestrates procedural map generation by executing a sequence of <see cref="GenStep{T}"/> passes.
    /// </summary>
    /// <typeparam name="T">
    /// The type of map context to generate. Must implement <see cref="IGenContext"/> and have a parameterless constructor.
    /// Common implementations include <see cref="ITiledGenContext"/> for tile-based maps and
    /// <see cref="IFloorPlanGenContext"/> for room-based layouts.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="MapGen{T}"/> is the central orchestrator in the RogueElements pipeline architecture.
    /// It maintains a priority-ordered collection of generation steps and executes them sequentially
    /// to transform an empty context into a fully realized map.
    /// </para>
    /// <para>
    /// The generation pipeline follows this flow:
    /// <list type="number">
    /// <item><description>Create a new instance of <typeparamref name="T"/> via reflection</description></item>
    /// <item><description>Initialize the random seed via <see cref="IGenContext.InitSeed"/></description></item>
    /// <item><description>Execute each <see cref="GenStep{T}"/> in priority order</description></item>
    /// <item><description>Finalize the map via <see cref="IGenContext.FinishGen"/></description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Generation steps are stored in a <see cref="PriorityList{T}"/> allowing fine-grained control
    /// over execution order. Steps with lower priority values execute first. Multiple steps can share
    /// the same priority and will execute in insertion order.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Define a layout with generation steps
    /// var layout = new MapGen&lt;MyMapContext&gt;();
    ///
    /// // Add steps in priority order (lower executes first)
    /// layout.GenSteps.Add(new Priority(1), new InitTilesStep&lt;MyMapContext&gt;(50, 50));
    /// layout.GenSteps.Add(new Priority(2), new DrawFloorPlanStep&lt;MyMapContext&gt;());
    /// layout.GenSteps.Add(new Priority(3), new PlaceStairsStep&lt;MyMapContext&gt;());
    ///
    /// // Generate a map with a specific seed for reproducibility
    /// ulong seed = 12345;
    /// MyMapContext map = layout.GenMap(seed);
    /// </code>
    /// </example>
    [Serializable]
    public class MapGen<T>
        where T : class, IGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapGen{T}"/> class with an empty step collection.
        /// </summary>
        public MapGen()
        {
            this.GenSteps = new PriorityList<GenStep<T>>();
        }

        /// <summary>
        /// Gets the priority-ordered collection of generation steps to execute.
        /// </summary>
        /// <value>
        /// A <see cref="PriorityList{T}"/> containing all <see cref="GenStep{T}"/> instances
        /// that will be executed during map generation. Steps are executed in ascending priority order.
        /// </value>
        /// <remarks>
        /// Add steps using <see cref="PriorityList{T}.Add(Priority, T)"/> with a <see cref="Priority"/>
        /// value to control execution order. Lower priority values execute earlier in the pipeline.
        /// </remarks>
        public PriorityList<GenStep<T>> GenSteps { get; }

        /// <summary>
        /// Generates a complete map by executing all registered generation steps in priority order.
        /// </summary>
        /// <param name="seed">
        /// The random seed used to initialize the map's random number generator.
        /// Using the same seed with the same steps produces identical maps, enabling reproducibility.
        /// </param>
        /// <returns>
        /// A fully generated map context of type <typeparamref name="T"/> after all steps have been applied.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method creates a new instance of <typeparamref name="T"/> using <see cref="Activator.CreateInstance(Type)"/>,
        /// requiring that <typeparamref name="T"/> has a public parameterless constructor.
        /// </para>
        /// <para>
        /// The generation process triggers debug events at key points:
        /// <list type="bullet">
        /// <item><description><see cref="GenContextDebug.OnInit"/> - After map initialization</description></item>
        /// <item><description><see cref="GenContextDebug.OnStepIn"/> - Before each step executes</description></item>
        /// <item><description><see cref="GenContextDebug.OnStepOut"/> - After each step completes</description></item>
        /// <item><description><see cref="GenContextDebug.OnError"/> - If a step throws an exception</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// Exceptions thrown by individual steps are caught and forwarded to <see cref="GenContextDebug.OnError"/>,
        /// allowing generation to continue with subsequent steps. This behavior enables partial map generation
        /// and debugging of problematic steps.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var layout = new MapGen&lt;MyMapContext&gt;();
        /// // ... add generation steps ...
        ///
        /// // Generate with a fixed seed for testing
        /// var testMap = layout.GenMap(42);
        ///
        /// // Generate with a random seed for variety
        /// var randomMap = layout.GenMap((ulong)DateTime.Now.Ticks);
        /// </code>
        /// </example>
        public T GenMap(ulong seed)
        {
            // may not need floor ID
            T map = (T)Activator.CreateInstance(typeof(T));
            map.InitSeed(seed);

            GenContextDebug.DebugInit(map);

            // postprocessing steps:
            StablePriorityQueue<Priority, IGenStep> queue = new StablePriorityQueue<Priority, IGenStep>();
            foreach (Priority priority in this.GenSteps.GetPriorities())
            {
                foreach (IGenStep genStep in this.GenSteps.GetItems(priority))
                    queue.Enqueue(priority, genStep);
            }

            ApplyGenSteps(map, queue);

            map.FinishGen();

            return map;
        }

        /// <summary>
        /// Executes all generation steps from the priority queue on the specified map context.
        /// </summary>
        /// <param name="map">The map context to modify with generation steps.</param>
        /// <param name="queue">
        /// A priority queue containing the generation steps to execute, ordered by <see cref="Priority"/>.
        /// Steps are dequeued and applied sequentially.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method processes each step in priority order, invoking debug events before and after
        /// each step via <see cref="GenContextDebug.StepIn"/> and <see cref="GenContextDebug.StepOut"/>.
        /// </para>
        /// <para>
        /// Exceptions thrown during step execution are caught and reported via <see cref="GenContextDebug.DebugError"/>,
        /// allowing the generation process to continue with remaining steps.
        /// </para>
        /// </remarks>
        protected static void ApplyGenSteps(T map, StablePriorityQueue<Priority, IGenStep> queue)
        {
            while (queue.Count > 0)
            {
                IGenStep postProc = queue.Dequeue();
                GenContextDebug.StepIn(postProc.ToString());

                try
                {
                    postProc.Apply(map);
                }
                catch (Exception ex)
                {
                    GenContextDebug.DebugError(ex);
                }

                GenContextDebug.StepOut();
            }
        }
    }
}
