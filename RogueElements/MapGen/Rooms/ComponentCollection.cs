// <copyright file="ComponentCollection.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using System;

namespace RogueElements
{
    /// <summary>
    /// A collection of <see cref="RoomComponent"/> instances, keyed by their type.
    /// Provides type-safe storage and retrieval of room components.
    /// </summary>
    [Serializable]
    public class ComponentCollection : TypeDict<RoomComponent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentCollection"/> class.
        /// </summary>
        public ComponentCollection()
        {
        }

        /// <summary>
        /// Creates a deep copy of this collection and all contained components.
        /// </summary>
        /// <returns>A new <see cref="ComponentCollection"/> with cloned components.</returns>
        public ComponentCollection Clone()
        {
            ComponentCollection newCollection = new ComponentCollection();
            foreach (RoomComponent component in this)
                newCollection.Set(component.Clone());
            return newCollection;
        }
    }
}
