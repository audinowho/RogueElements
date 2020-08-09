// <copyright file="IFloorRoomPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace RogueElements
{
    public interface IFloorRoomPlan : IRoomPlan
    {
        List<RoomHallIndex> Adjacents { get; }
    }
}
