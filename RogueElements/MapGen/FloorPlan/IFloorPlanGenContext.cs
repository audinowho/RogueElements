// <copyright file="IFloorPlanGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IFloorPlanGenContext : ITiledGenContext
    {
        FloorPlan RoomPlan { get; }

        void InitPlan(FloorPlan plan);
    }
}
