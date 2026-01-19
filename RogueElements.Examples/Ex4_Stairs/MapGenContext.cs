// <copyright file="MapGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using RogueElements;

namespace RogueElements.Examples.Ex4_Stairs
{
    /// <summary>
    /// Map generation context that supports entity spawning.
    ///
    /// This context extends Ex3's grid support with entity placement capabilities.
    /// It implements IViewPlaceableGenContext for both StairsUp and StairsDown,
    /// enabling spawn steps to place these entities on the map.
    ///
    /// KEY CONCEPT: IPlaceableGenContext&lt;T&gt;
    /// ========================================
    /// This interface must be implemented SEPARATELY for each spawnable entity type.
    /// There is no generic "spawn anything" mechanism - each type needs explicit support.
    ///
    /// The interface requires:
    /// - GetAllFreeTiles(): Returns all valid spawn locations
    /// - GetFreeTiles(Rect): Returns valid spawn locations within a rectangle
    /// - CanPlaceItem(Loc): Checks if a specific location is valid
    /// - PlaceItem(Loc, T): Actually spawns the entity at the location
    ///
    /// IViewPlaceableGenContext&lt;T&gt; extends this with:
    /// - Count: Number of spawned entities
    /// - GetItem(int): Get entity by index
    /// - GetLoc(int): Get location by index
    ///
    /// This design allows different entity types to have different spawn rules.
    /// For example, stairs might only spawn on floor tiles, while items might
    /// spawn on any non-wall tile including water.
    /// </summary>
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
        /// Delegate type for finding open tiles within a rectangle.
        /// Used internally to share tile-finding logic between stair types.
        /// </summary>
        /// <param name="rect">The rectangular area to search.</param>
        /// <returns>List of valid spawn locations.</returns>
        protected delegate List<Loc> GetOpen(Rect rect);

        /// <summary>
        /// Gets the FloorPlan containing room and hall definitions.
        /// </summary>
        public FloorPlan RoomPlan { get; private set; }

        /// <summary>
        /// Gets the GridPlan containing the grid-based room layout.
        /// </summary>
        public GridPlan GridPlan { get; private set; }

        /// <summary>
        /// Gets the list of entrance stairs (StairsUp) on the map.
        /// Delegates to the Map's GenEntrances collection.
        /// </summary>
        public List<StairsUp> GenEntrances => this.Map.GenEntrances;

        /// <summary>
        /// Gets the list of exit stairs (StairsDown) on the map.
        /// Delegates to the Map's GenExits collection.
        /// </summary>
        public List<StairsDown> GenExits => this.Map.GenExits;

        // ============================================================
        // IViewPlaceableGenContext<T>.Count implementations
        // ============================================================
        // Explicit interface implementations allow the same property name
        // with different return values for different type parameters.

        /// <summary>
        /// Gets the count of entrance stairs.
        /// </summary>
        int IViewPlaceableGenContext<StairsUp>.Count => this.GenEntrances.Count;

        /// <summary>
        /// Gets the count of exit stairs.
        /// </summary>
        int IViewPlaceableGenContext<StairsDown>.Count => this.GenExits.Count;

        /// <summary>
        /// Determines whether a tile can be modified at the given location.
        /// Prevents tiles from being changed where stairs have been placed.
        /// This protects spawned entities from being overwritten by later generation steps.
        /// </summary>
        /// <param name="loc">The location to check.</param>
        /// <param name="tile">The tile that would be set (unused in this check).</param>
        /// <returns>True if the tile can be set; false if stairs occupy this location.</returns>
        public override bool CanSetTile(Loc loc, ITile tile)
        {
            // Check all entrance stairs
            for (int ii = 0; ii < this.GenEntrances.Count; ii++)
            {
                if (this.GenEntrances[ii].Loc == loc)
                    return false;
            }

            // Check all exit stairs
            for (int ii = 0; ii < this.GenExits.Count; ii++)
            {
                if (this.GenExits[ii].Loc == loc)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Initializes the FloorPlan for this context.
        /// </summary>
        /// <param name="plan">The FloorPlan to use for room placement.</param>
        public void InitPlan(FloorPlan plan)
        {
            this.RoomPlan = plan;
        }

        /// <summary>
        /// Initializes the GridPlan for this context.
        /// </summary>
        /// <param name="plan">The GridPlan defining the grid structure.</param>
        public void InitGrid(GridPlan plan)
        {
            this.GridPlan = plan;
        }

        // ============================================================
        // IPlaceableGenContext<StairsUp> implementation
        // ============================================================
        // These methods define how StairsUp entities can be spawned.

        /// <summary>
        /// Gets all free tiles where StairsUp can be placed.
        /// Called by spawn steps to find valid spawn locations.
        /// </summary>
        /// <returns>List of all valid spawn locations for entrance stairs.</returns>
        List<Loc> IPlaceableGenContext<StairsUp>.GetAllFreeTiles() => this.GetAllFreeTiles(this.GetOpenTiles);

        /// <summary>
        /// Gets free tiles within a specific rectangle for StairsUp.
        /// </summary>
        /// <param name="rect">The area to search.</param>
        /// <returns>List of valid spawn locations within the rectangle.</returns>
        List<Loc> IPlaceableGenContext<StairsUp>.GetFreeTiles(Rect rect) => this.GetOpenTiles(rect);

        /// <summary>
        /// Checks if StairsUp can be placed at a specific location.
        /// </summary>
        /// <param name="loc">The location to check.</param>
        /// <returns>True if the location is a valid spawn point.</returns>
        bool IPlaceableGenContext<StairsUp>.CanPlaceItem(Loc loc) => !this.IsTileOccupied(loc);

        /// <summary>
        /// Places a StairsUp entity at the specified location.
        /// Creates a copy of the template and assigns the location.
        /// </summary>
        /// <param name="loc">Where to place the stairs.</param>
        /// <param name="item">Template stairs to copy.</param>
        void IPlaceableGenContext<StairsUp>.PlaceItem(Loc loc, StairsUp item)
        {
            // Copy the template - never modify the original
            var stairs = (StairsUp)item.Copy();
            stairs.Loc = loc;
            this.GenEntrances.Add(stairs);
        }

        // ============================================================
        // IPlaceableGenContext<StairsDown> implementation
        // ============================================================
        // These methods define how StairsDown entities can be spawned.
        // Note: Nearly identical to StairsUp, but uses GenExits list.

        /// <summary>
        /// Gets all free tiles where StairsDown can be placed.
        /// </summary>
        /// <returns>List of all valid spawn locations for exit stairs.</returns>
        List<Loc> IPlaceableGenContext<StairsDown>.GetAllFreeTiles() => this.GetAllFreeTiles(this.GetOpenTiles);

        /// <summary>
        /// Gets free tiles within a specific rectangle for StairsDown.
        /// </summary>
        /// <param name="rect">The area to search.</param>
        /// <returns>List of valid spawn locations within the rectangle.</returns>
        List<Loc> IPlaceableGenContext<StairsDown>.GetFreeTiles(Rect rect) => this.GetOpenTiles(rect);

        /// <summary>
        /// Checks if StairsDown can be placed at a specific location.
        /// </summary>
        /// <param name="loc">The location to check.</param>
        /// <returns>True if the location is a valid spawn point.</returns>
        bool IPlaceableGenContext<StairsDown>.CanPlaceItem(Loc loc) => !this.IsTileOccupied(loc);

        /// <summary>
        /// Places a StairsDown entity at the specified location.
        /// </summary>
        /// <param name="loc">Where to place the stairs.</param>
        /// <param name="item">Template stairs to copy.</param>
        void IPlaceableGenContext<StairsDown>.PlaceItem(Loc loc, StairsDown item)
        {
            // Copy the template - never modify the original
            var stairs = (StairsDown)item.Copy();
            stairs.Loc = loc;
            this.GenExits.Add(stairs);
        }

        // ============================================================
        // IViewPlaceableGenContext<T> implementations
        // ============================================================
        // These methods allow reading back spawned entities by index.

        /// <summary>
        /// Gets a StairsUp entity by index.
        /// </summary>
        /// <param name="index">The index of the stairs.</param>
        /// <returns>The stairs at the given index.</returns>
        StairsUp IViewPlaceableGenContext<StairsUp>.GetItem(int index) => this.GenEntrances[index];

        /// <summary>
        /// Gets the location of a StairsUp entity by index.
        /// </summary>
        /// <param name="index">The index of the stairs.</param>
        /// <returns>The location of the stairs.</returns>
        Loc IViewPlaceableGenContext<StairsUp>.GetLoc(int index) => this.GenEntrances[index].Loc;

        /// <summary>
        /// Gets a StairsDown entity by index.
        /// </summary>
        /// <param name="index">The index of the stairs.</param>
        /// <returns>The stairs at the given index.</returns>
        StairsDown IViewPlaceableGenContext<StairsDown>.GetItem(int index) => this.GenExits[index];

        /// <summary>
        /// Gets the location of a StairsDown entity by index.
        /// </summary>
        /// <param name="index">The index of the stairs.</param>
        /// <returns>The location of the stairs.</returns>
        Loc IViewPlaceableGenContext<StairsDown>.GetLoc(int index) => this.GenExits[index].Loc;

        // ============================================================
        // Helper methods for finding spawn locations
        // ============================================================

        /// <summary>
        /// Gets all free tiles on the entire map using the provided search function.
        /// </summary>
        /// <param name="func">Function to find open tiles within a rectangle.</param>
        /// <returns>List of all valid spawn locations.</returns>
        protected virtual List<Loc> GetAllFreeTiles(GetOpen func)
        {
            // Search the entire map bounds
            return func?.Invoke(new Rect(0, 0, this.Width, this.Height));
        }

        /// <summary>
        /// Finds open tiles within a rectangular area.
        /// A tile is "open" if it's a floor tile (ROOM_TERRAIN_ID).
        /// </summary>
        /// <param name="rect">The area to search.</param>
        /// <returns>List of locations that are valid spawn points.</returns>
        protected List<Loc> GetOpenTiles(Rect rect)
        {
            // Define what makes a tile "open" for spawning
            bool CheckOp(Loc loc) => !this.IsTileOccupied(loc);

            // Grid.FindTilesInBox is a utility that iterates all tiles in the rect
            // and returns those that pass the check function
            return Grid.FindTilesInBox(rect.Start, rect.Size, CheckOp);
        }

        /// <summary>
        /// Checks if a tile is occupied (not a floor tile).
        /// Used to determine valid spawn locations.
        /// </summary>
        /// <param name="loc">The location to check.</param>
        /// <returns>True if the tile is not a floor tile.</returns>
        private bool IsTileOccupied(Loc loc) => this.Map.Tiles[loc.X][loc.Y].ID != Map.ROOM_TERRAIN_ID;
    }
}
