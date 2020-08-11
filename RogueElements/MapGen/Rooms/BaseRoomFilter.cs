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
    public abstract class BaseRoomFilter
    {
        public static bool PassesAllFilters(IRoomPlan plan, List<BaseRoomFilter> filters)
        {
            foreach (BaseRoomFilter filter in filters)
            {
                if (!filter.PassesFilter(plan))
                    return false;
            }

            return true;
        }

        public abstract bool PassesFilter(IRoomPlan plan);
    }
}
