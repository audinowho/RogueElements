// <copyright file="RoomHallIndex.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Represents a unique identifier for a room or hall within a <see cref="FloorPlan"/>.
    /// </summary>
    /// <remarks>
    /// Since rooms and halls are stored in separate lists within a FloorPlan, this struct
    /// combines the list index with a flag indicating whether the element is a hall.
    /// This allows unified referencing of both rooms and halls in adjacency lists and
    /// graph traversal operations.
    /// </remarks>
    public struct RoomHallIndex : IEquatable<RoomHallIndex>
    {
        /// <summary>
        /// Indicates whether this index refers to a hall (true) or a room (false).
        /// </summary>
        public bool IsHall;

        /// <summary>
        /// The zero-based index within the rooms or halls list.
        /// </summary>
        public int Index;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomHallIndex"/> struct.
        /// </summary>
        /// <param name="index">The zero-based index within the appropriate list.</param>
        /// <param name="isHall">True if referring to a hall; false for a room.</param>
        public RoomHallIndex(int index, bool isHall)
        {
            this.Index = index;
            this.IsHall = isHall;
        }

        /// <summary>
        /// Determines whether two <see cref="RoomHallIndex"/> values are equal.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>True if both values are equal; otherwise, false.</returns>
        public static bool operator ==(RoomHallIndex value1, RoomHallIndex value2)
        {
            return value1.Equals(value2);
        }

        /// <summary>
        /// Determines whether two <see cref="RoomHallIndex"/> values are not equal.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>True if the values are not equal; otherwise, false.</returns>
        public static bool operator !=(RoomHallIndex value1, RoomHallIndex value2)
        {
            return !(value1 == value2);
        }

        /// <summary>
        /// Determines whether this instance equals another object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the object is an equal <see cref="RoomHallIndex"/>; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is RoomHallIndex) && this.Equals((RoomHallIndex)obj);
        }

        /// <summary>
        /// Determines whether this instance equals another <see cref="RoomHallIndex"/>.
        /// </summary>
        /// <param name="other">The other value to compare.</param>
        /// <returns>True if both the index and hall flag match; otherwise, false.</returns>
        public bool Equals(RoomHallIndex other)
        {
            return this.IsHall == other.IsHall && this.Index == other.Index;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code combining the index and hall flag.</returns>
        public override int GetHashCode()
        {
            return this.IsHall.GetHashCode() ^ this.Index.GetHashCode();
        }
    }
}
