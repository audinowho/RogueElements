// <copyright file="ComponentCollection.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class ComponentCollection
    {
        public ComponentCollection()
        {
            this.Components = new List<RoomComponent>();
        }

        public ComponentCollection(ComponentCollection other)
        {
            this.Components = new List<RoomComponent>();
            foreach (RoomComponent component in other.Components)
                this.Components.Add(component.Clone());
        }

        public List<RoomComponent> Components { get; }
    }
}
