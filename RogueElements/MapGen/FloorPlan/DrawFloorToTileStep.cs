// <copyright file="DrawFloorToTileStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Renders the floor plan to the tile map, converting abstract room layouts into actual tiles.
    /// </summary>
    /// <typeparam name="T">The generation context type, which must implement <see cref="IFloorPlanGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This step is the final phase of floor plan-based generation. It creates the tile map,
    /// fills it with wall terrain, then draws each room and hall according to their generators.
    /// </para>
    /// <para>
    /// This step should only be executed after the floor plan layout is complete - after all rooms
    /// have been added and connected. Subsequent steps can then operate on the tile-based map.
    /// </para>
    /// </remarks>
    /// <seealso cref="InitFloorPlanStep{T}"/>
    /// <seealso cref="FloorPlan.DrawOnMap"/>
    [Serializable]
    public class DrawFloorToTileStep<T> : GenStep<T>
        where T : class, IFloorPlanGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawFloorToTileStep{T}"/> class with no padding.
        /// </summary>
        public DrawFloorToTileStep()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawFloorToTileStep{T}"/> class with specified padding.
        /// </summary>
        /// <param name="padding">The number of wall tiles to add around the map border.</param>
        public DrawFloorToTileStep(int padding = 0)
        {
            this.Padding = padding;
        }

        /// <summary>
        /// Gets or sets the number of tiles to add as a wall border around the map.
        /// </summary>
        public int Padding { get; set; }

        /// <summary>
        /// Applies this step by rendering the floor plan to tiles.
        /// </summary>
        /// <param name="map">The generation context containing the floor plan.</param>
        public override void Apply(T map)
        {
            // draw on map
            map.CreateNew(
                map.RoomPlan.DrawRect.Width + (2 * this.Padding),
                map.RoomPlan.DrawRect.Height + (2 * this.Padding),
                map.RoomPlan.Wrap);

            GenContextDebug.DebugProgress("Initialized Map");

            for (int ii = 0; ii < map.Width; ii++)
            {
                for (int jj = 0; jj < map.Height; jj++)
                    map.SetTile(new Loc(ii, jj), map.WallTerrain.Copy());
            }

            GenContextDebug.DebugProgress("Set Walls");
            map.RoomPlan.MoveStart(new Loc(this.Padding));
            GenContextDebug.DebugProgress("Moved Floor");
            map.RoomPlan.DrawOnMap(map);
        }

        /// <summary>
        /// Returns a string representation of this step.
        /// </summary>
        /// <returns>A string describing this step's configuration.</returns>
        public override string ToString()
        {
            return string.Format("{0}: Padding:{1}", this.GetType().GetFormattedTypeName(), this.Padding);
        }
    }
}
