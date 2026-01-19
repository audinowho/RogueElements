// <copyright file="BaseRoomFilter.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements
{
    /// <summary>
    /// Base class for filters that determine whether a room plan meets certain criteria.
    /// Used to selectively apply operations to specific rooms in a floor layout.
    /// </summary>
    [Serializable]
    public abstract class BaseRoomFilter
    {
        /// <summary>
        /// Checks whether a room plan passes all filters in a list.
        /// </summary>
        /// <param name="plan">The room plan to check.</param>
        /// <param name="filters">The list of filters to apply.</param>
        /// <returns>True if the room passes all filters; otherwise, false.</returns>
        public static bool PassesAllFilters(IRoomPlan plan, List<BaseRoomFilter> filters)
        {
            foreach (BaseRoomFilter filter in filters)
            {
                if (!filter.PassesFilter(plan))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether a room plan passes this filter.
        /// </summary>
        /// <param name="plan">The room plan to check.</param>
        /// <returns>True if the room passes the filter; otherwise, false.</returns>
        public abstract bool PassesFilter(IRoomPlan plan);
    }
}
