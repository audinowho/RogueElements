// <copyright file="IRoomGridGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IRoomGridGenContext : IFloorPlanGenContext
    {
        GridPlan GridPlan { get; }

        void InitGrid(GridPlan plan);
    }
}
