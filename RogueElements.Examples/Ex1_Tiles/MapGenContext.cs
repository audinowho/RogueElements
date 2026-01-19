// <copyright file="MapGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements.Examples.Ex1_Tiles
{
    /// <summary>
    /// The map generation context for Example 1 (Tiles).
    ///
    /// A "context" in RogueElements is the state object that GenSteps operate on.
    /// It holds all the data being built up during generation (tiles, rooms, spawns, etc.).
    ///
    /// Key Concepts:
    /// - Every MapGen&lt;T&gt; requires a context type T that implements IGenContext
    /// - The context is created fresh for each GenMap() call
    /// - GenSteps read from and write to the context during Apply()
    ///
    /// Interface Hierarchy:
    /// - IGenContext: Minimum interface (Rand, InitSeed, FinishGen)
    /// - ITiledGenContext: Adds tile operations (GetTile, SetTile, CreateNew, etc.)
    /// - IFloorPlanGenContext: Adds FloorPlan for room-based generation (see Ex2)
    /// - IRoomGridGenContext: Adds GridPlan for grid-based layouts (see Ex3)
    ///
    /// This example uses the simplest setup: just ITiledGenContext for basic tile operations.
    /// The heavy lifting is done by BaseMapGenContext&lt;T&gt; in the Common folder.
    /// </summary>
    /// <remarks>
    /// Why create a separate context class for each example?
    /// - Different examples need different interfaces (IFloorPlanGenContext, etc.)
    /// - Keeps each example self-contained and easy to understand
    /// - Allows customization of context behavior per example
    ///
    /// In a real game, you would typically have one context class that implements
    /// all the interfaces your generation pipeline needs.
    /// </remarks>
    public class MapGenContext : BaseMapGenContext<Map>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapGenContext"/> class.
        ///
        /// The base class constructor (BaseMapGenContext) creates a new Map instance.
        /// MapGen.GenMap() will call InitSeed() to set up the random number generator
        /// before any GenSteps are executed.
        /// </summary>
        public MapGenContext()
            : base()
        {
            // The base class handles all initialization:
            // - Creates a new Map instance (this.Map = new TMap())
            // - Provides ITiledGenContext implementation (GetTile, SetTile, etc.)
            // - Provides IGenContext implementation (Rand, InitSeed, FinishGen)
        }

        // Note: This context only implements ITiledGenContext (via BaseMapGenContext).
        // That's sufficient for Example 1's tile-based operations.
        //
        // GenSteps like InitTilesStep<T> and SpecificTilesStep<T> require
        // ITiledGenContext, which provides:
        // - CreateNew(width, height): Initialize the tile array
        // - GetTile(loc): Read a tile at a position
        // - SetTile(loc, tile): Write a tile at a position
        // - TileBlocked(loc): Check if a tile is impassable
        // - RoomTerrain/WallTerrain: Default tile types
    }
}
