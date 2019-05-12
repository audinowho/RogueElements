// <copyright file="RoomHallIndex.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System;

namespace RogueElements
{
    public struct RoomHallIndex : IEquatable<RoomHallIndex>
    {
        public bool IsHall;
        public int Index;

        public RoomHallIndex(int index, bool isHall)
        {
            Index = index;
            IsHall = isHall;
        }

        public override bool Equals(object obj)
        {
            return (obj is RoomHallIndex) && Equals((RoomHallIndex)obj);
        }

        public bool Equals(RoomHallIndex other)
        {
            return (IsHall == other.IsHall && Index == other.Index);
        }


        public static bool operator ==(RoomHallIndex value1, RoomHallIndex value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(RoomHallIndex value1, RoomHallIndex value2)
        {
            return !(value1 == value2);
        }

        public override int GetHashCode()
        {
            return IsHall.GetHashCode() ^ Index.GetHashCode();
        }
    }
}
