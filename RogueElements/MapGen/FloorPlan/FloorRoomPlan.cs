﻿// <copyright file="FloorRoomPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System;

namespace RogueElements
{
    /// <summary>
    /// Contains data about which cells a room occupies in a GridFloorPlan.
    /// </summary>
    public class FloorRoomPlan : BaseFloorRoomPlan
    {
        public bool Immutable;
        public IRoomGen RoomGen;
        public override IRoomGen Gen { get { return RoomGen; } }

        public FloorRoomPlan(IRoomGen roomGen)
        {
            RoomGen = roomGen;
        }


    }

    public class FloorHallPlan : BaseFloorRoomPlan
    {
        public IPermissiveRoomGen RoomGen;
        public override IRoomGen Gen { get { return RoomGen; } }

        public FloorHallPlan(IPermissiveRoomGen roomGen)
        {
            RoomGen = roomGen;
        }
    }


    public abstract class BaseFloorRoomPlan
    {
        public abstract IRoomGen Gen { get; }
        public List<RoomHallIndex> Adjacents;

        protected BaseFloorRoomPlan()
        {
            Adjacents = new List<RoomHallIndex>();
        }
    }

}
