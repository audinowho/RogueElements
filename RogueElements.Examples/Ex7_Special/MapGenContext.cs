// <copyright file="MapGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using RogueElements;

namespace RogueElements.Examples.Ex7_Special
{
    /// <summary>
    /// Generation context that combines FloorPlan support with item and stair placement.
    /// Extends BaseMapGenContext with IFloorPlanGenContext for room-based generation.
    /// </summary>
    /// <remarks>
    /// See Ex2_Rooms for IFloorPlanGenContext basics, Ex4_Stairs for stair placement,
    /// and Ex6_Items for item placement. This context combines all three.
    /// </remarks>
    public class MapGenContext : BaseMapGenContext<Map>, IFloorPlanGenContext,
        IViewPlaceableGenContext<StairsUp>, IViewPlaceableGenContext<StairsDown>,
        IPlaceableGenContext<Item>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapGenContext"/> class.
        /// </summary>
        public MapGenContext()
            : base()
        {
        }

        /// <summary>
        /// Delegate for obtaining open tiles within a rectangular area.
        /// </summary>
        /// <param name="rect">The area to search for open tiles.</param>
        /// <returns>List of open tile locations.</returns>
        protected delegate List<Loc> GetOpen(Rect rect);

        /// <summary>
        /// Gets the FloorPlan containing room layout information.
        /// Used by SetSpecialRoomStep to find rooms to replace.
        /// </summary>
        public FloorPlan RoomPlan { get; private set; }

        /// <summary>
        /// Gets the list of upward stairs (entrances) on the map.
        /// </summary>
        public List<StairsUp> GenEntrances => this.Map.GenEntrances;

        /// <summary>
        /// Gets the list of downward stairs (exits) on the map.
        /// </summary>
        public List<StairsDown> GenExits => this.Map.GenExits;

        /// <inheritdoc/>
        int IViewPlaceableGenContext<StairsUp>.Count => this.GenEntrances.Count;

        /// <inheritdoc/>
        int IViewPlaceableGenContext<StairsDown>.Count => this.GenExits.Count;

        /// <summary>
        /// Checks if a tile can be set at the specified location.
        /// Prevents overwriting tiles occupied by stairs.
        /// </summary>
        /// <param name="loc">The location to check.</param>
        /// <param name="tile">The tile to potentially place.</param>
        /// <returns>True if the tile can be placed; false if blocked by stairs.</returns>
        public override bool CanSetTile(Loc loc, ITile tile)
        {
            for (int ii = 0; ii < this.GenEntrances.Count; ii++)
            {
                if (this.GenEntrances[ii].Loc == loc)
                    return false;
            }

            for (int ii = 0; ii < this.GenExits.Count; ii++)
            {
                if (this.GenExits[ii].Loc == loc)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Initializes the floor plan for this context.
        /// Called by InitFloorPlanStep before room placement.
        /// </summary>
        /// <param name="plan">The floor plan to use.</param>
        public void InitPlan(FloorPlan plan)
        {
            this.RoomPlan = plan;
        }

        /// <inheritdoc/>
        List<Loc> IPlaceableGenContext<Item>.GetAllFreeTiles() => this.GetAllFreeTiles(this.GetOpenTiles);

        /// <inheritdoc/>
        List<Loc> IPlaceableGenContext<StairsUp>.GetAllFreeTiles() => this.GetAllFreeTiles(this.GetOpenTiles);

        /// <inheritdoc/>
        List<Loc> IPlaceableGenContext<StairsDown>.GetAllFreeTiles() => this.GetAllFreeTiles(this.GetOpenTiles);

        /// <inheritdoc/>
        List<Loc> IPlaceableGenContext<Item>.GetFreeTiles(Rect rect) => this.GetOpenTiles(rect);

        /// <inheritdoc/>
        List<Loc> IPlaceableGenContext<StairsUp>.GetFreeTiles(Rect rect) => this.GetOpenTiles(rect);

        /// <inheritdoc/>
        List<Loc> IPlaceableGenContext<StairsDown>.GetFreeTiles(Rect rect) => this.GetOpenTiles(rect);

        /// <inheritdoc/>
        bool IPlaceableGenContext<Item>.CanPlaceItem(Loc loc) => !this.IsTileOccupied(loc);

        /// <inheritdoc/>
        bool IPlaceableGenContext<StairsUp>.CanPlaceItem(Loc loc) => !this.IsTileOccupied(loc);

        /// <inheritdoc/>
        bool IPlaceableGenContext<StairsDown>.CanPlaceItem(Loc loc) => !this.IsTileOccupied(loc);

        /// <inheritdoc/>
        void IPlaceableGenContext<Item>.PlaceItem(Loc loc, Item item)
        {
            Item newItem = new Item(item.ID, loc);
            this.Map.Items.Add(newItem);
        }

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
        /// Gets all free tiles on the entire map using the specified tile finder.
        /// </summary>
        /// <param name="func">The function to use for finding open tiles.</param>
        /// <returns>List of all free tile locations.</returns>
        protected virtual List<Loc> GetAllFreeTiles(GetOpen func)
        {
            return func?.Invoke(new Rect(0, 0, this.Width, this.Height));
        }

        /// <summary>
        /// Gets open tiles within a rectangular area.
        /// </summary>
        /// <param name="rect">The area to search.</param>
        /// <returns>List of unoccupied floor tile locations.</returns>
        protected List<Loc> GetOpenTiles(Rect rect)
        {
            bool CheckOp(Loc loc) => !this.IsTileOccupied(loc);

            return Grid.FindTilesInBox(rect.Start, rect.Size, CheckOp);
        }

        /// <summary>
        /// Checks if a tile is occupied by non-floor terrain or an item.
        /// </summary>
        /// <param name="loc">The location to check.</param>
        /// <returns>True if occupied; false if available for placement.</returns>
        private bool IsTileOccupied(Loc loc)
        {
            if (this.Map.Tiles[loc.X][loc.Y].ID != Map.ROOM_TERRAIN_ID)
                return true;

            foreach (Item item in this.Map.Items)
            {
                if (item.Loc == loc)
                    return true;
            }

            return false;
        }
    }
}
