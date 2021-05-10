// <copyright file="RoomFilterComponent.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements
{
    [Serializable]
    public class RoomFilterComponent : BaseRoomFilter
    {
        public RoomFilterComponent()
        {
            this.Components = new ComponentCollection();
        }

        public RoomFilterComponent(bool negate, params RoomComponent[] components)
        {
            this.Negate = negate;
            this.Components = new ComponentCollection();
            foreach (RoomComponent component in components)
                this.Components.Set(component);
        }

        public bool Negate { get; set; }

        public ComponentCollection Components { get; set; }

        public override bool PassesFilter(IRoomPlan plan)
        {
            foreach (RoomComponent component in this.Components)
            {
                if (plan.Components.Contains(component.GetType()) == this.Negate)
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            if (this.Negate)
                return string.Format("{0}: ^{1}", this.GetType().Name, this.Components.ToString());
            else
                return string.Format("{0}: {1}", this.GetType().Name, this.Components.ToString());
        }
    }
}
