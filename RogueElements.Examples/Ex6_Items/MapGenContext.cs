// <copyright file="MapGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using RogueElements;

namespace RogueElements.Examples.Ex6_Items
{
    /// <summary>
    /// Generation context for Example 6 that supports spawning multiple entity types.
    /// Demonstrates implementing multiple IPlaceableGenContext&lt;T&gt; interfaces
    /// to enable spawning different types of entities (items, mobs, stairs).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Key pattern: A single context can implement IPlaceableGenContext&lt;T&gt; for
    /// multiple types. This allows different RandomSpawnStep instances to place
    /// different entity types using the same context.
    /// </para>
    /// <para>
    /// Each IPlaceableGenContext&lt;T&gt; implementation requires:
    /// - GetAllFreeTiles(): Returns all valid spawn locations
    /// - GetFreeTiles(Rect): Returns valid spawn locations within an area
    /// - CanPlaceItem(Loc): Checks if a specific location is valid
    /// - PlaceItem(Loc, T): Actually places the entity at the location
    /// </para>
    /// <para>
    /// The IsTileOccupied() method is extended in this example to prevent
    /// stacking - items and mobs cannot be placed on the same tile.
    /// </para>
    /// </remarks>
    public class MapGenContext : BaseMapGenContext<Map>, IRoomGridGenContext,
        IViewPlaceableGenContext<StairsUp>, IViewPlaceableGenContext<StairsDown>,
        IPlaceableGenContext<Item>, IPlaceableGenContext<Mob>
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
        /// Prevents terrain from overwriting stairs.
        /// </summary>
        /// <param name="loc">The location to check.</param>
        /// <param name="tile">The tile to potentially place.</param>
        /// <returns>True if the tile can be set; false if blocked by stairs.</returns>
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

        // ===================================================================================
        // IPlaceableGenContext<T> IMPLEMENTATIONS
        // ===================================================================================
        // Each spawnable type needs its own set of interface methods.
        // These tell RandomSpawnStep where entities CAN go and how to place them.

        /// <inheritdoc/>
        /// <remarks>Returns all valid item spawn locations on the map.</remarks>
        List<Loc> IPlaceableGenContext<Item>.GetAllFreeTiles() => this.GetAllFreeTiles(this.GetOpenTiles);

        /// <inheritdoc/>
        /// <remarks>Returns all valid mob spawn locations on the map.</remarks>
        List<Loc> IPlaceableGenContext<Mob>.GetAllFreeTiles() => this.GetAllFreeTiles(this.GetOpenTiles);

        /// <inheritdoc/>
        List<Loc> IPlaceableGenContext<StairsUp>.GetAllFreeTiles() => this.GetAllFreeTiles(this.GetOpenTiles);

        /// <inheritdoc/>
        List<Loc> IPlaceableGenContext<StairsDown>.GetAllFreeTiles() => this.GetAllFreeTiles(this.GetOpenTiles);

        /// <inheritdoc/>
        List<Loc> IPlaceableGenContext<Item>.GetFreeTiles(Rect rect) => this.GetOpenTiles(rect);

        /// <inheritdoc/>
        List<Loc> IPlaceableGenContext<Mob>.GetFreeTiles(Rect rect) => this.GetOpenTiles(rect);

        /// <inheritdoc/>
        List<Loc> IPlaceableGenContext<StairsUp>.GetFreeTiles(Rect rect) => this.GetOpenTiles(rect);

        /// <inheritdoc/>
        List<Loc> IPlaceableGenContext<StairsDown>.GetFreeTiles(Rect rect) => this.GetOpenTiles(rect);

        /// <inheritdoc/>
        bool IPlaceableGenContext<Item>.CanPlaceItem(Loc loc) => !this.IsTileOccupied(loc);

        /// <inheritdoc/>
        bool IPlaceableGenContext<Mob>.CanPlaceItem(Loc loc) => !this.IsTileOccupied(loc);

        /// <inheritdoc/>
        bool IPlaceableGenContext<StairsUp>.CanPlaceItem(Loc loc) => !this.IsTileOccupied(loc);

        /// <inheritdoc/>
        bool IPlaceableGenContext<StairsDown>.CanPlaceItem(Loc loc) => !this.IsTileOccupied(loc);

        // ===================================================================================
        // PlaceItem METHODS - Actually add entities to the map
        // ===================================================================================

        /// <summary>
        /// Places an item at the specified location.
        /// </summary>
        /// <param name="loc">The location to place the item.</param>
        /// <param name="item">The item template to copy and place.</param>
        /// <remarks>
        /// Creates a NEW Item instance at the location rather than modifying the template.
        /// This is important because the same Item template may be used for multiple spawns.
        /// </remarks>
        void IPlaceableGenContext<Item>.PlaceItem(Loc loc, Item item)
        {
            // Create a new item with the location set
            // Don't modify the template - it may be reused!
            Item newItem = new Item(item.ID, loc);
            this.Map.Items.Add(newItem);
        }

        /// <summary>
        /// Places a mob at the specified location.
        /// </summary>
        /// <param name="loc">The location to place the mob.</param>
        /// <param name="item">The mob template to copy and place.</param>
        void IPlaceableGenContext<Mob>.PlaceItem(Loc loc, Mob item)
        {
            Mob newItem = new Mob(item.ID, loc);
            this.Map.Mobs.Add(newItem);
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
        /// Checks if a tile is occupied and cannot accept new entities.
        /// </summary>
        /// <param name="loc">The location to check.</param>
        /// <returns>True if the tile is occupied; false if available for spawning.</returns>
        /// <remarks>
        /// This method prevents entity stacking by checking:
        /// 1. Is the tile walkable floor (not wall or water)?
        /// 2. Is there already an item at this location?
        /// 3. Is there already a mob at this location?
        ///
        /// This ensures items and mobs don't overlap, making the map more readable.
        /// </remarks>
        private bool IsTileOccupied(Loc loc)
        {
            // Can only spawn on floor tiles (ROOM_TERRAIN_ID)
            if (this.Map.Tiles[loc.X][loc.Y].ID != Map.ROOM_TERRAIN_ID)
                return true;

            // Check for existing items at this location
            foreach (Item item in this.Map.Items)
            {
                if (item.Loc == loc)
                    return true;
            }

            // Check for existing mobs at this location
            foreach (Mob item in this.Map.Mobs)
            {
                if (item.Loc == loc)
                    return true;
            }

            return false;
        }
    }
}
