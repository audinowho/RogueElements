// <copyright file="MapGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements.Examples.Ex3_Grid
{
    /// <summary>
    /// Map generation context that supports grid-based room layouts.
    ///
    /// This context implements IRoomGridGenContext, which combines:
    /// - IFloorPlanGenContext: Access to FloorPlan for room placement
    /// - IGridPlanGenContext: Access to GridPlan for grid-based layouts
    ///
    /// The interface hierarchy enables a two-stage generation process:
    /// 1. GridPlan defines the high-level room arrangement in a grid
    /// 2. FloorPlan receives the converted room definitions for rendering
    ///
    /// IRoomGridGenContext is the key interface for grid-based generation.
    /// GenSteps that require grid functionality constrain their type parameter
    /// to this interface (e.g., GridPathBranch&lt;T&gt; where T : IRoomGridGenContext).
    /// </summary>
    public class MapGenContext : BaseMapGenContext<Map>, IRoomGridGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapGenContext"/> class.
        /// </summary>
        public MapGenContext()
            : base()
        {
        }

        /// <summary>
        /// Gets the FloorPlan containing room and hall definitions.
        /// FloorPlan is populated when DrawGridToFloorStep converts the GridPlan.
        /// Required by IFloorPlanGenContext.
        /// </summary>
        public FloorPlan RoomPlan { get; private set; }

        /// <summary>
        /// Gets the GridPlan containing the grid-based room layout.
        /// GridPlan is initialized by InitGridPlanStep and populated by path generators
        /// like GridPathBranch. It represents rooms as cells in a regular grid.
        /// Required by IGridPlanGenContext.
        /// </summary>
        /// <remarks>
        /// GridPlan stores:
        /// - Grid dimensions (cells and cell sizes)
        /// - Which cells contain rooms
        /// - Connections between adjacent cells (halls)
        /// - Room generators assigned to each occupied cell
        ///
        /// This is a higher-level abstraction than FloorPlan. While FloorPlan
        /// stores exact room boundaries, GridPlan stores logical positions
        /// in a grid structure.
        /// </remarks>
        public GridPlan GridPlan { get; private set; }

        /// <summary>
        /// Initializes the FloorPlan for this context.
        /// Called by DrawGridToFloorStep when converting from GridPlan.
        /// Required by IFloorPlanGenContext.
        /// </summary>
        /// <param name="plan">The FloorPlan to use for room placement.</param>
        public void InitPlan(FloorPlan plan)
        {
            this.RoomPlan = plan;
        }

        /// <summary>
        /// Initializes the GridPlan for this context.
        /// Called by InitGridPlanStep at the start of grid-based generation.
        /// Required by IGridPlanGenContext.
        /// </summary>
        /// <param name="plan">The GridPlan defining the grid structure.</param>
        public void InitGrid(GridPlan plan)
        {
            this.GridPlan = plan;
        }
    }
}
