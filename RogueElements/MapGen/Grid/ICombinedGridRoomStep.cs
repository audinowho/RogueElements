// <copyright file="ICombinedGridRoomStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{

    public interface ICombinedGridRoomStep: IFloorPlanGenContext
    {
        RandRange MergeRate { get; set; }

        SpawnList<GridCombo> Combos { get; set; }

        List<BaseRoomFilter> Filters { get; set; }

        ComponentCollection RoomComponents { get; set; }
    }
}
