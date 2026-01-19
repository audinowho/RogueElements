// <copyright file="MapGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using RogueElements;

namespace RogueElements.Examples.Ex5_Terrain
{
    /// <summary>
    /// Generation context for Example 5 that supports terrain modification.
    /// Identical to Ex4's context - terrain steps work on ITiledGenContext
    /// which is already provided by BaseMapGenContext.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Terrain steps like PerlinWaterStep only require ITiledGenContext to function.
    /// They use SetTile() to modify existing tiles, which is why CanSetTile() is
    /// important - it prevents terrain from overwriting stairs and other placed entities.
    /// </para>
    /// <para>
    /// The MapTerrainStencil passed to terrain steps provides additional filtering
    /// beyond CanSetTile(), controlling which tile types are eligible for modification.
    /// </para>
    /// </remarks>
    public class MapGenContext : BaseMapGenContext<Map>, IRoomGridGenContext, IViewPlaceableGenContext<StairsUp>, IViewPlaceableGenContext<StairsDown>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapGenContext"/> class.
        /// </summary>
        public MapGenContext()
            : base()
        {
        }

        /// <summary>
        /// Delegate type for methods that find open tiles within a rectangular area.
        /// </summary>
        /// <param name="rect">The rectangular area to search.</param>
        /// <returns>List of valid tile locations.</returns>
        protected delegate List<Loc> GetOpen(Rect rect);

        /// <summary>
        /// Gets the floor plan for freeform room-based generation.
        /// </summary>
        public FloorPlan RoomPlan { get; private set; }

        /// <summary>
        /// Gets the grid plan for grid-based room layouts.
        /// </summary>
        public GridPlan GridPlan { get; private set; }

        /// <summary>
        /// Gets the list of upward stairs (entrances) placed on this map.
        /// </summary>
        public List<StairsUp> GenEntrances => this.Map.GenEntrances;

        /// <summary>
        /// Gets the list of downward stairs (exits) placed on this map.
        /// </summary>
        public List<StairsDown> GenExits => this.Map.GenExits;

        /// <inheritdoc/>
        int IViewPlaceableGenContext<StairsUp>.Count => this.GenEntrances.Count;

        /// <inheritdoc/>
        int IViewPlaceableGenContext<StairsDown>.Count => this.GenExits.Count;

        /// <summary>
        /// Determines whether a tile can be set at the specified location.
        /// Prevents terrain steps from overwriting stairs.
        /// </summary>
        /// <param name="loc">The location to check.</param>
        /// <param name="tile">The tile to potentially place.</param>
        /// <returns>True if the tile can be set; false if blocked by stairs.</returns>
        /// <remarks>
        /// This is crucial for terrain generation: without this check, PerlinWaterStep
        /// could place water tiles on top of stairs, making them inaccessible.
        /// The terrain step calls this before each SetTile() operation.
        /// </remarks>
        public override bool CanSetTile(Loc loc, ITile tile)
        {
            // Don't allow terrain to overwrite entrance stairs
            for (int ii = 0; ii < this.GenEntrances.Count; ii++)
            {
                if (this.GenEntrances[ii].Loc == loc)
                    return false;
            }

            // Don't allow terrain to overwrite exit stairs
            for (int ii = 0; ii < this.GenExits.Count; ii++)
            {
                if (this.GenExits[ii].Loc == loc)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Initializes the floor plan for this context.
        /// </summary>
        /// <param name="plan">The floor plan to use.</param>
        public void InitPlan(FloorPlan plan)
        {
            this.RoomPlan = plan;
        }

        /// <summary>
        /// Initializes the grid plan for this context.
        /// </summary>
        /// <param name="plan">The grid plan to use.</param>
        public void InitGrid(GridPlan plan)
        {
            this.GridPlan = plan;
        }

        /// <inheritdoc/>
        List<Loc> IPlaceableGenContext<StairsUp>.GetAllFreeTiles() => this.GetAllFreeTiles(this.GetOpenTiles);

        /// <inheritdoc/>
        List<Loc> IPlaceableGenContext<StairsDown>.GetAllFreeTiles() => this.GetAllFreeTiles(this.GetOpenTiles);

        /// <inheritdoc/>
        List<Loc> IPlaceableGenContext<StairsUp>.GetFreeTiles(Rect rect) => this.GetOpenTiles(rect);

        /// <inheritdoc/>
        List<Loc> IPlaceableGenContext<StairsDown>.GetFreeTiles(Rect rect) => this.GetOpenTiles(rect);

        /// <inheritdoc/>
        bool IPlaceableGenContext<StairsUp>.CanPlaceItem(Loc loc) => !this.IsTileOccupied(loc);

        /// <inheritdoc/>
        bool IPlaceableGenContext<StairsDown>.CanPlaceItem(Loc loc) => !this.IsTileOccupied(loc);

        /// <inheritdoc/>
        void IPlaceableGenContext<StairsUp>.PlaceItem(Loc loc, StairsUp item)
        {
            var stairs = (StairsUp)item.Copy();
            stairs.Loc = loc;
            this.GenEntrances.Add(stairs);
        }

        /// <inheritdoc/>
        void IPlaceableGenContext<StairsDown>.PlaceItem(Loc loc, StairsDown item)
        {
            var stairs = (StairsDown)item.Copy();
            stairs.Loc = loc;
            this.GenExits.Add(stairs);
        }

        /// <inheritdoc/>
        StairsUp IViewPlaceableGenContext<StairsUp>.GetItem(int index) => this.GenEntrances[index];

        /// <inheritdoc/>
        Loc IViewPlaceableGenContext<StairsUp>.GetLoc(int index) => this.GenEntrances[index].Loc;

        /// <inheritdoc/>
        StairsDown IViewPlaceableGenContext<StairsDown>.GetItem(int index) => this.GenExits[index];

        /// <inheritdoc/>
        Loc IViewPlaceableGenContext<StairsDown>.GetLoc(int index) => this.GenExits[index].Loc;

        /// <summary>
        /// Gets all free tiles across the entire map using the specified function.
        /// </summary>
        /// <param name="func">Function to find open tiles in a rectangle.</param>
        /// <returns>List of all free tile locations.</returns>
        protected virtual List<Loc> GetAllFreeTiles(GetOpen func)
        {
            return func?.Invoke(new Rect(0, 0, this.Width, this.Height));
        }

        /// <summary>
        /// Gets all open tiles within the specified rectangular area.
        /// </summary>
        /// <param name="rect">The area to search.</param>
        /// <returns>List of unoccupied tile locations.</returns>
        protected List<Loc> GetOpenTiles(Rect rect)
        {
            bool CheckOp(Loc loc) => !this.IsTileOccupied(loc);

            return Grid.FindTilesInBox(rect.Start, rect.Size, CheckOp);
        }

        /// <summary>
        /// Checks if a tile is occupied (not a valid floor tile).
        /// </summary>
        /// <param name="loc">The location to check.</param>
        /// <returns>True if the tile is occupied or not a floor tile.</returns>
        private bool IsTileOccupied(Loc loc) => this.Map.Tiles[loc.X][loc.Y].ID != Map.ROOM_TERRAIN_ID;
    }
}
