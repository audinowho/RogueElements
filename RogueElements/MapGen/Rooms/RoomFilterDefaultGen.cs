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
    /// </summary>
    [Serializable]
    public class RoomFilterDefaultGen : BaseRoomFilter
    {
        public RoomFilterDefaultGen()
        {
        }

        public RoomFilterDefaultGen(bool negate)
        {
            this.Negate = negate;
        }

        public bool Negate { get; set; }

        public override bool PassesFilter(IRoomPlan plan)
        {
            if (plan.RoomGen is IRoomGenDefault)
                return !this.Negate;

            return this.Negate;
        }

        public override string ToString()
        {
            if (this.Negate)
                return string.Format("{0}^", this.GetType().Name);
            else
                return string.Format("{0}", this.GetType().Name);
        }
    }
}
