// <copyright file="BaseRoomFilter.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements
{
    [Serializable]
    public abstract class BaseRoomFilter<TTile>
        where TTile : ITile<TTile>
    {
        public static bool PassesAllFilters(IRoomPlan<TTile> plan, List<BaseRoomFilter<TTile>> filters)
        {
            foreach (BaseRoomFilter<TTile> filter in filters)
            {
                if (!filter.PassesFilter(plan))
                    return false;
            }

            return true;
        }

        public abstract bool PassesFilter(IRoomPlan<TTile> plan);
    }
}
