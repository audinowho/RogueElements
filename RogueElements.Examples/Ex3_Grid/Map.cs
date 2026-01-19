// <copyright file="Map.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueElements;

namespace RogueElements.Examples.Ex3_Grid
{
    /// <summary>
    /// Map class for grid-based room generation example.
    ///
    /// This map is identical to Ex2's Map because grid-based generation
    /// doesn't require additional map features - it's purely a different
    /// approach to room placement that produces the same tile output.
    ///
    /// The grid structure exists only during generation (in GridPlan/FloorPlan)
    /// and is not stored in the final map. The map only contains the rendered tiles.
    /// </summary>
    public class Map : BaseMap
    {
    }
}
