// <copyright file="RoomFilterComponent.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements
{
    /// <summary>
    /// Filters for rooms that have a specific group of components.
    /// </summary>
    [Serializable]
    public class RoomFilterComponent : BaseRoomFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomFilterComponent"/> class.
        /// </summary>
        public RoomFilterComponent()
        {
            this.Components = new ComponentCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomFilterComponent"/> class with specified components.
        /// </summary>
        /// <param name="negate">If true, the filter passes rooms that do NOT have the components.</param>
        /// <param name="components">The components to filter by.</param>
        public RoomFilterComponent(bool negate, params RoomComponent[] components)
        {
            this.Negate = negate;
            this.Components = new ComponentCollection();
            foreach (RoomComponent component in components)
                this.Components.Set(component);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to negate the filter result.
        /// When true, the filter passes rooms that do NOT have the specified components.
        /// </summary>
        public bool Negate { get; set; }

        /// <summary>
        /// Gets or sets the collection of components to filter by.
        /// </summary>
        public ComponentCollection Components { get; set; }

        /// <inheritdoc/>
        public override bool PassesFilter(IRoomPlan plan)
        {
            foreach (RoomComponent component in this.Components)
            {
                if (plan.Components.Contains(component.GetType()) == this.Negate)
                    return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.Negate)
                return string.Format("{0}: ^{1}", this.GetType().GetFormattedTypeName(), this.Components.ToString());
            else
                return string.Format("{0}: {1}", this.GetType().GetFormattedTypeName(), this.Components.ToString());
        }
    }
}
