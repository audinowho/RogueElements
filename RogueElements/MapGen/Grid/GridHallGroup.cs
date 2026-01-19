// <copyright file="GridHallGroup.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Represents a logical hall connection between two adjacent cells in a <see cref="GridPlan"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A hall group contains one or more <see cref="GridHallPlan"/> segments. In simple cases,
    /// there is only one segment. However, when the rooms on either side of the hall are offset
    /// from each other, the hall may need to be split into multiple segments to properly connect them.
    /// </para>
    /// <para>
    /// The <see cref="MainHall"/> property provides access to the primary hall segment,
    /// which is used for initial hall setup before bounds calculation.
    /// </para>
    /// </remarks>
    /// <seealso cref="GridHallPlan"/>
    /// <seealso cref="GridPlan"/>
    public class GridHallGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridHallGroup"/> class.
        /// </summary>
        public GridHallGroup()
        {
            this.HallParts = new List<GridHallPlan>();
        }

        /// <summary>
        /// Gets the primary hall segment, or null if no hall exists.
        /// </summary>
        public GridHallPlan MainHall => this.HallParts.Count > 0 ? this.HallParts[0] : null;

        /// <summary>
        /// Gets the list of hall segments that make up this logical hall connection.
        /// </summary>
        public List<GridHallPlan> HallParts { get; }

        /// <summary>
        /// Sets or clears the hall for this connection.
        /// </summary>
        /// <param name="plan">The hall plan to set, or null to clear the hall.</param>
        public void SetHall(GridHallPlan plan)
        {
            this.HallParts.Clear();
            if (plan != null)
                this.HallParts.Add(plan);
        }
    }
}
