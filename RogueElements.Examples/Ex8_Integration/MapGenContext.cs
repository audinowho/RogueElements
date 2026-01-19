// <copyright file="MapGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using RogueSharp;

namespace RogueElements.Examples.Ex8_Integration
{
    /// <summary>
    /// Complete context implementation bridging RogueElements with RogueSharp.
    /// Implements both ITiledGenContext and IRoomGridGenContext to support full pipeline.
    /// </summary>
    /// <remarks>
    /// This context demonstrates integrating with an external map library (RogueSharp).
    /// Key differences from BaseMapGenContext:
    /// - Uses RogueSharp's Map instead of custom tile storage
    /// - Tiles are RogueSharp Cells wrapped in CellTile
    /// - No inheritance from BaseMapGenContext - implements interfaces directly
    ///
    /// Interface breakdown (see earlier examples for details):
    /// - ITiledGenContext (Ex1_Tiles): Tile operations, dimensions, terrain types
    /// - IRoomGridGenContext (Ex3_Grid): Grid-based room layout support
    /// - IFloorPlanGenContext (Ex2_Rooms): Freeform room placement (via IRoomGridGenContext)
    /// </remarks>
    public class MapGenContext : ITiledGenContext, IRoomGridGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapGenContext"/> class.
        /// </summary>
        public MapGenContext()
        {
            this.Map = new Map();
        }

        /// <summary>
        /// Gets or sets the RogueSharp Map being generated.
        /// </summary>
        public Map Map { get; set; }

        /// <summary>
        /// Gets the random number generator for this generation run.
        /// Initialized via InitSeed() at generation start.
        /// </summary>
        public IRandom Rand { get; private set; }

        /// <summary>
        /// Gets the FloorPlan for room-based operations.
        /// Populated by DrawGridToFloorStep from the GridPlan.
        /// </summary>
        public FloorPlan RoomPlan { get; private set; }

        /// <summary>
        /// Gets the GridPlan for grid-based room layout.
        /// Populated by InitGridPlanStep at generation start.
        /// </summary>
        public GridPlan GridPlan { get; private set; }

        /// <summary>
        /// Gets a value indicating whether tiles have been initialized.
        /// </summary>
        public bool TilesInitialized => this.Map.Width > 0 && this.Map.Height > 0;

        /// <summary>
        /// Gets the map width in tiles.
        /// </summary>
        public int Width => this.Map.Width;

        /// <summary>
        /// Gets the map height in tiles.
        /// </summary>
        public int Height => this.Map.Height;

        /// <summary>
        /// Gets a value indicating whether the map wraps at edges.
        /// </summary>
        public bool Wrap => false;

        /// <summary>
        /// Gets the default floor/room terrain (walkable, transparent).
        /// </summary>
        public ITile RoomTerrain => new CellTile(0, 0, true, true, false);

        /// <summary>
        /// Gets the default wall terrain (non-walkable, non-transparent).
        /// </summary>
        public ITile WallTerrain => new CellTile(0, 0, false, false, false);

        /// <summary>
        /// Gets the tile at the specified location.
        /// </summary>
        /// <param name="loc">The tile location.</param>
        /// <returns>The tile as a CellTile wrapper around RogueSharp's Cell.</returns>
        public ITile GetTile(Loc loc) => CellTile.FromCell(this.Map.GetCell(loc.X, loc.Y));

        /// <summary>
        /// Checks if a tile can be placed at the specified location.
        /// Always returns true - RogueSharp maps have no placement restrictions.
        /// </summary>
        /// <param name="loc">The location to check.</param>
        /// <param name="tile">The tile to potentially place.</param>
        /// <returns>Always true for this implementation.</returns>
        public bool CanSetTile(Loc loc, ITile tile) => true;

        /// <summary>
        /// Attempts to set a tile at the specified location.
        /// </summary>
        /// <param name="loc">The location to set.</param>
        /// <param name="tile">The tile to place (must be castable to Cell).</param>
        /// <returns>True if successful; false if CanSetTile returns false.</returns>
        public bool TrySetTile(Loc loc, ITile tile)
        {
            if (!this.CanSetTile(loc, tile))
                return false;

            // Cast ITile to RogueSharp Cell and apply properties
            Cell cell = (Cell)tile;
            this.Map.SetCellProperties(loc.X, loc.Y, cell.IsTransparent, cell.IsWalkable, cell.IsExplored);
            return true;
        }

        /// <summary>
        /// Sets a tile at the specified location, throwing if placement fails.
        /// </summary>
        /// <param name="loc">The location to set.</param>
        /// <param name="tile">The tile to place.</param>
        /// <exception cref="InvalidOperationException">Thrown if tile cannot be placed.</exception>
        public void SetTile(Loc loc, ITile tile)
        {
            if (!this.TrySetTile(loc, tile))
                throw new InvalidOperationException("Can't place tile!");
        }

        /// <summary>
        /// Initializes the random seed for this generation run.
        /// Called by MapGen at the start of GenMap().
        /// </summary>
        /// <param name="seed">The seed value for reproducible generation.</param>
        public void InitSeed(ulong seed)
        {
            this.Rand = new ReRandom(seed);
        }

        /// <inheritdoc/>
        bool ITiledGenContext.TileBlocked(Loc loc)
        {
            return !this.Map.IsWalkable(loc.X, loc.Y);
        }

        /// <inheritdoc/>
        bool ITiledGenContext.TileBlocked(Loc loc, bool diagonal)
        {
            return !this.Map.IsWalkable(loc.X, loc.Y);
        }

        /// <summary>
        /// Creates the tile storage with the specified dimensions.
        /// Initializes the RogueSharp Map.
        /// </summary>
        /// <param name="width">Map width in tiles.</param>
        /// <param name="height">Map height in tiles.</param>
        /// <param name="wrap">Whether map wraps (ignored for RogueSharp).</param>
        public virtual void CreateNew(int width, int height, bool wrap = false)
        {
            this.Map.Initialize(width, height);
        }

        /// <summary>
        /// Called when generation is complete.
        /// No-op for this implementation.
        /// </summary>
        public void FinishGen()
        {
        }

        /// <summary>
        /// Initializes the FloorPlan for room-based operations.
        /// Called by DrawGridToFloorStep or InitFloorPlanStep.
        /// </summary>
        /// <param name="plan">The floor plan to use.</param>
        public void InitPlan(FloorPlan plan)
        {
            this.RoomPlan = plan;
        }

        /// <summary>
        /// Initializes the GridPlan for grid-based layout.
        /// Called by InitGridPlanStep.
        /// </summary>
        /// <param name="plan">The grid plan to use.</param>
        public void InitGrid(GridPlan plan)
        {
            this.GridPlan = plan;
        }
    }
}
