// <copyright file="GenContextDebug.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RogueElements
{
    /// <summary>
    /// Provides debug events and hooks for monitoring the map generation process.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="GenContextDebug"/> is a static utility class that exposes events fired during
    /// <see cref="MapGen{T}.GenMap"/> execution. These events enable visualization, logging,
    /// debugging, and step-by-step analysis of the generation process.
    /// </para>
    /// <para>
    /// Events are raised at key points in the generation lifecycle:
    /// <list type="bullet">
    /// <item><description><see cref="OnInit"/> - After map context initialization, before any steps run</description></item>
    /// <item><description><see cref="OnStepIn"/> - Before each generation step executes</description></item>
    /// <item><description><see cref="OnStepOut"/> - After each generation step completes</description></item>
    /// <item><description><see cref="OnStep"/> - For progress updates during step execution</description></item>
    /// <item><description><see cref="OnError"/> - When a generation step throws an exception</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// These events are particularly useful for:
    /// <list type="bullet">
    /// <item><description>Building map generation visualizers that show step-by-step progress</description></item>
    /// <item><description>Debugging problematic generation steps</description></item>
    /// <item><description>Logging generation metrics and timing</description></item>
    /// <item><description>Implementing breakpoints or step-through debugging</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Subscribe to generation events for debugging
    /// GenContextDebug.OnInit += (map) =>
    ///     Console.WriteLine($"Generation started with seed: {map.Rand}");
    ///
    /// GenContextDebug.OnStepIn += (stepName) =>
    ///     Console.WriteLine($"Starting step: {stepName}");
    ///
    /// GenContextDebug.OnStepOut += () =>
    ///     Console.WriteLine("Step completed");
    ///
    /// GenContextDebug.OnError += (ex) =>
    ///     Console.WriteLine($"Step failed: {ex.Message}");
    ///
    /// // Now generate a map - events will fire during generation
    /// var map = layout.GenMap(seed);
    /// </code>
    /// </example>
    /// <seealso cref="MapGen{T}"/>
    public static class GenContextDebug
    {
        /// <summary>
        /// Occurs after the map context is initialized but before any generation steps execute.
        /// </summary>
        /// <remarks>
        /// The event handler receives the newly created <see cref="IGenContext"/> instance
        /// after <see cref="IGenContext.InitSeed"/> has been called. This is useful for
        /// capturing the initial map state or setting up per-generation debug state.
        /// </remarks>
        public static event Action<IGenContext> OnInit;

        /// <summary>
        /// Occurs when a generation step reports progress during its execution.
        /// </summary>
        /// <remarks>
        /// This event is triggered by <see cref="DebugProgress"/> and can be called by
        /// <see cref="GenStep{T}"/> implementations to report intermediate progress for
        /// long-running steps. The string parameter contains a progress message.
        /// </remarks>
        public static event Action<string> OnStep;

        /// <summary>
        /// Occurs immediately before a generation step begins execution.
        /// </summary>
        /// <remarks>
        /// The string parameter contains the step's name (from <see cref="object.ToString"/>).
        /// This event pairs with <see cref="OnStepOut"/> to bracket step execution,
        /// enabling timing measurements and hierarchical visualization.
        /// </remarks>
        public static event Action<string> OnStepIn;

        /// <summary>
        /// Occurs immediately after a generation step completes execution.
        /// </summary>
        /// <remarks>
        /// This event fires after each step completes, regardless of whether the step
        /// succeeded or threw an exception. It pairs with <see cref="OnStepIn"/> to
        /// bracket step execution.
        /// </remarks>
        public static event Action OnStepOut;

        /// <summary>
        /// Occurs when a generation step throws an exception during execution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Exceptions are caught by <see cref="MapGen{T}"/> and reported via this event,
        /// allowing generation to continue with subsequent steps. The exception is passed
        /// to handlers for logging or debugging purposes.
        /// </para>
        /// <para>
        /// Note that generation continues after an error, which may result in incomplete
        /// or malformed maps depending on which step failed.
        /// </para>
        /// </remarks>
        public static event Action<Exception> OnError;

        /// <summary>
        /// Raises the <see cref="OnStepIn"/> event to signal that a generation step is starting.
        /// </summary>
        /// <param name="msg">The name or description of the step about to execute.</param>
        /// <remarks>
        /// This method is called internally by <see cref="MapGen{T}"/> before each step executes.
        /// </remarks>
        public static void StepIn(string msg) => OnStepIn?.Invoke(msg);

        /// <summary>
        /// Raises the <see cref="OnStepOut"/> event to signal that a generation step has completed.
        /// </summary>
        /// <remarks>
        /// This method is called internally by <see cref="MapGen{T}"/> after each step completes.
        /// </remarks>
        public static void StepOut() => OnStepOut?.Invoke();

        /// <summary>
        /// Raises the <see cref="OnInit"/> event to signal that generation has been initialized.
        /// </summary>
        /// <param name="map">The newly initialized map context.</param>
        /// <remarks>
        /// This method is called internally by <see cref="MapGen{T}"/> after calling
        /// <see cref="IGenContext.InitSeed"/> but before any generation steps execute.
        /// </remarks>
        public static void DebugInit(IGenContext map) => OnInit?.Invoke(map);

        /// <summary>
        /// Raises the <see cref="OnStep"/> event to report generation progress.
        /// </summary>
        /// <param name="msg">A message describing the current progress.</param>
        /// <remarks>
        /// <see cref="GenStep{T}"/> implementations can call this method to report
        /// intermediate progress for long-running operations.
        /// </remarks>
        public static void DebugProgress(string msg) => OnStep?.Invoke(msg);

        /// <summary>
        /// Raises the <see cref="OnError"/> event to report an exception during generation.
        /// </summary>
        /// <param name="ex">The exception that was thrown.</param>
        /// <remarks>
        /// This method is called internally by <see cref="MapGen{T}"/> when a step throws
        /// an exception. The exception is reported to handlers but generation continues.
        /// </remarks>
        public static void DebugError(Exception ex) => OnError?.Invoke(ex);
    }
}
