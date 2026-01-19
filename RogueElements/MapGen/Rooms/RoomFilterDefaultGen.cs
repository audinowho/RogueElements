// <copyright file="RoomFilterDefaultGen.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements
{
    /// <summary>
    /// Filters for rooms using the default generator.
    /// Matches rooms whose <see cref="IRoomPlan.RoomGen"/> implements <see cref="IRoomGenDefault"/>.
    /// </summary>
    [Serializable]
    public class RoomFilterDefaultGen : BaseRoomFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomFilterDefaultGen"/> class.
        /// </summary>
        public RoomFilterDefaultGen()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomFilterDefaultGen"/> class with negation setting.
        /// </summary>
        /// <param name="negate">If true, the filter passes rooms that do NOT use the default generator.</param>
        public RoomFilterDefaultGen(bool negate)
        {
            this.Negate = negate;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to negate the filter result.
        /// When true, the filter passes rooms that do NOT use the default generator.
        /// </summary>
        public bool Negate { get; set; }

        /// <inheritdoc/>
        public override bool PassesFilter(IRoomPlan plan)
        {
            if (plan.RoomGen is IRoomGenDefault)
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
