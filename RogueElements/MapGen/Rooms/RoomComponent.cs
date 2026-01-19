// <copyright file="RoomComponent.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using System;

namespace RogueElements
{
    /// <summary>
    /// Base class for components that can be attached to rooms to provide metadata and behavior.
    /// Components are used to tag rooms with additional information for filtering and spawning.
    /// </summary>
    [Serializable]
    public abstract class RoomComponent
    {
        /// <summary>
        /// Creates a deep copy of this component.
        /// </summary>
        /// <returns>A new instance that is a copy of this component.</returns>
        public abstract RoomComponent Clone();
    }
}
