// <copyright file="InitFloorPlanStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Initializes an empty floor plan, which is a list of rooms that keep track of their size, position, and connectivity with each other.
    /// Gen Steps that operate on the floor plan can add rooms, delete them, or change the rooms in some way.
    /// Once finished, apply DrawFloorToTileStep to draw the actual tiles of the rooms.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class InitFloorPlanStep<T> : GenStep<T>
        where T : class, IFloorPlanGenContext
    {
        public InitFloorPlanStep()
        {
        }

        public InitFloorPlanStep(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public int Width { get; set; }

        public int Height { get; set; }

        /// <summary>
        /// Determines if the map is wrapped around.
        /// </summary>
        public bool Wrap { get; set; }

        public override void Apply(T map)
        {
            var floorPlan = new FloorPlan();
            floorPlan.InitSize(new Loc(this.Width, this.Height), this.Wrap);

            map.InitPlan(floorPlan);
        }

        public override string ToString()
        {
            return string.Format("{0}: Size:{1}x{2}", this.GetType().Name, this.Width, this.Height);
        }
    }
}
