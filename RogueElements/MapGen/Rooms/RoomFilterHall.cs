// <copyright file="RoomFilterHall.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements
{
    /// <summary>
    /// Filters for rooms using the hall plan.
    /// Matches rooms that are instances of <see cref="FloorHallPlan"/>.
    /// </summary>
    [Serializable]
    public class RoomFilterHall : BaseRoomFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomFilterHall"/> class.
        /// </summary>
        public RoomFilterHall()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomFilterHall"/> class with negation setting.
        /// </summary>
        /// <param name="negate">If true, the filter passes rooms that are NOT halls.</param>
        public RoomFilterHall(bool negate)
        {
            this.Negate = negate;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to negate the filter result.
        /// When true, the filter passes rooms that are NOT halls.
        /// </summary>
        public bool Negate { get; set; }

        /// <inheritdoc/>
        public override bool PassesFilter(IRoomPlan plan)
        {
            if (plan is FloorHallPlan)
                return !this.Negate;

            return this.Negate;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.Negate)
                return string.Format("{0}^", this.GetType().GetFormattedTypeName());
            else
                return string.Format("{0}", this.GetType().GetFormattedTypeName());
        }
    }
}
