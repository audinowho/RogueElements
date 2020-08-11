// <copyright file="ComponentCollection.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using System;

namespace RogueElements
{
    [Serializable]
    public class ComponentCollection : TypeDict<RoomComponent>
    {
        public ComponentCollection()
        {
        }

        public ComponentCollection Clone()
        {
            ComponentCollection newCollection = new ComponentCollection();
            foreach (RoomComponent component in this)
                newCollection.Set(component.Clone());
            return newCollection;
        }
    }
}
