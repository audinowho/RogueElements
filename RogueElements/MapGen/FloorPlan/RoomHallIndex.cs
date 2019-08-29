// <copyright file="RoomHallIndex.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public struct RoomHallIndex : IEquatable<RoomHallIndex>
    {
        public bool IsHall;
        public int Index;

        public RoomHallIndex(int index, bool isHall)
        {
            this.Index = index;
            this.IsHall = isHall;
        }

        public static bool operator ==(RoomHallIndex value1, RoomHallIndex value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(RoomHallIndex value1, RoomHallIndex value2)
        {
            return !(value1 == value2);
        }

        public override bool Equals(object obj)
        {
            return (obj is RoomHallIndex) && this.Equals((RoomHallIndex)obj);
        }

        public bool Equals(RoomHallIndex other)
        {
            return this.IsHall == other.IsHall && this.Index == other.Index;
        }

        public override int GetHashCode()
        {
            return this.IsHall.GetHashCode() ^ this.Index.GetHashCode();
        }
    }
}
