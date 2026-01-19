// <copyright file="MapGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements.Examples.Ex2_Rooms
{
    /// <summary>
    /// The map generation context for Example 2 (Rooms).
    ///
    /// This context adds IFloorPlanGenContext to enable FloorPlan-based room generation.
    /// FloorPlan is the core abstraction for "freeform" room placement, where rooms
    /// can be placed anywhere on the map (as opposed to grid-based placement in Ex3).
    ///
    /// Interface Progression:
    /// - Ex1: ITiledGenContext only (basic tile operations)
    /// - Ex2: ITiledGenContext + IFloorPlanGenContext (room-based generation)
    /// - Ex3: ITiledGenContext + IRoomGridGenContext (grid-based generation)
    ///
    /// IFloorPlanGenContext provides:
    /// - RoomPlan property: Access to the FloorPlan being built
    /// - InitPlan(plan): Initialize the FloorPlan (called by InitFloorPlanStep)
    ///
    /// FloorPlan contains:
    /// - Room list: All rooms with their positions and shapes
    /// - Hall list: Connections between rooms
    /// - Adjacency information: Which rooms connect to which
    /// </summary>
    /// <remarks>
    /// Why separate IFloorPlanGenContext from ITiledGenContext?
    ///
    /// 1. Separation of concerns: Planning (FloorPlan) vs Drawing (Tiles)
    /// 2. Flexibility: Some GenSteps only need FloorPlan, some only need tiles
    /// 3. Composability: You can have multiple planning phases before drawing
    ///
    /// The generation flow is:
    /// 1. InitFloorPlanStep creates empty FloorPlan (needs IFloorPlanGenContext)
    /// 2. Path generators add rooms/halls to FloorPlan (needs IFloorPlanGenContext)
    /// 3. DrawFloorToTileStep converts FloorPlan to tiles (needs both interfaces)
    /// </remarks>
    public class MapGenContext : BaseMapGenContext<Map>, IFloorPlanGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapGenContext"/> class.
        /// </summary>
        public MapGenContext()
            : base()
        {
            // BaseMapGenContext provides ITiledGenContext implementation.
            // This class adds IFloorPlanGenContext on top.
            //
            // Note: RoomPlan starts as null and is initialized by InitFloorPlanStep.
        }

        /// <summary>
        /// Gets the FloorPlan containing the abstract room and hall layout.
        ///
        /// FloorPlan is the central data structure for room-based generation:
        /// - Holds all rooms as RoomGen instances with positions
        /// - Holds all halls connecting rooms
        /// - Tracks adjacency (which rooms connect to which)
        /// - Provides queries like "get all rooms adjacent to room X"
        ///
        /// This is populated by path generators (FloorPathBranch, etc.)
        /// and consumed by DrawFloorToTileStep to create actual tiles.
        /// </summary>
        /// <remarks>
        /// FloorPlan methods you might use in custom GenSteps:
        /// - GetRoom(index): Get a specific room
        /// - GetHall(index): Get a specific hall
        /// - GetAdjacents(index): Get indices of adjacent rooms
        /// - RoomCount: Total number of rooms
        /// - HallCount: Total number of halls
        /// - DrawOnMap(): Render to tiles (used by DrawFloorToTileStep)
        /// </remarks>
        public FloorPlan RoomPlan { get; private set; }

        /// <summary>
        /// Initializes the FloorPlan for this generation context.
        ///
        /// This is called by InitFloorPlanStep to set up the FloorPlan
        /// before room placement begins. The FloorPlan is created with
        /// the map dimensions and stored here for subsequent GenSteps to use.
        /// </summary>
        /// <param name="plan">The FloorPlan instance to use for this generation.</param>
        /// <remarks>
        /// This method is part of the IFloorPlanGenContext interface.
        /// It's called once at the start of generation (by InitFloorPlanStep)
        /// and the plan is then modified by subsequent room placement steps.
        ///
        /// The 'private set' on RoomPlan ensures only InitPlan can set it,
        /// maintaining proper initialization order.
        /// </remarks>
        public void InitPlan(FloorPlan plan)
        {
            // Store the FloorPlan for use by room placement and drawing steps.
            // After this call, GenSteps can access context.RoomPlan to:
            // - Add rooms (path generators)
            // - Add halls (path generators)
            // - Query room positions (spawning steps)
            // - Convert to tiles (DrawFloorToTileStep)
            this.RoomPlan = plan;
        }
    }
}
