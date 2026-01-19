// <copyright file="BaseMapGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements.Examples
{
    /// <summary>
    /// Abstract base class providing a reference implementation of <see cref="ITiledGenContext"/>
    /// for tile-based map generation.
    /// </summary>
    /// <typeparam name="TMap">
    /// The concrete map type that stores generation results. Must inherit from <see cref="BaseMap"/>
    /// and have a parameterless constructor.
    /// </typeparam>
    /// <remarks>
    /// <para>
    /// This class bridges the RogueElements generation pipeline with your game's map representation.
    /// It implements <see cref="ITiledGenContext"/> to enable tile-based generation steps such as
    /// room carving, hallway drawing, and terrain placement.
    /// </para>
    /// <para>
    /// <b>Extension Points:</b>
    /// </para>
    /// <list type="bullet">
    /// <item><description>
    /// Override <see cref="CanSetTile"/> to add placement restrictions (e.g., protecting certain areas).
    /// </description></item>
    /// <item><description>
    /// Override <see cref="CreateNew"/> to initialize additional map data structures.
    /// </description></item>
    /// <item><description>
    /// Override <see cref="FinishGen"/> to perform post-generation cleanup or validation.
    /// </description></item>
    /// </list>
    /// <para>
    /// For spawning support, extend this class and implement additional interfaces such as
    /// <see cref="IPlaceableGenContext{T}"/> for items, stairs, or mobs.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create a concrete context for your game
    /// public class MyMapGenContext : BaseMapGenContext&lt;MyMap&gt;, IPlaceableGenContext&lt;Item&gt;
    /// {
    ///     // Add spawning support, custom interfaces, etc.
    /// }
    /// </code>
    /// </example>
    public abstract class BaseMapGenContext<TMap> : ITiledGenContext
        where TMap : BaseMap, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMapGenContext{TMap}"/> class.
        /// </summary>
        /// <remarks>
        /// Creates a new empty map instance. The map tiles are not initialized until
        /// <see cref="CreateNew"/> is called during generation.
        /// </remarks>
        protected BaseMapGenContext()
        {
            this.Map = new TMap();
        }

        /// <summary>
        /// Gets or sets the underlying map data being generated.
        /// </summary>
        /// <value>The map instance that stores tiles and generation results.</value>
        /// <remarks>
        /// After generation completes, access this property to retrieve the finished map
        /// for use in your game.
        /// </remarks>
        public TMap Map { get; set; }

        /// <summary>
        /// Gets a tile instance representing passable floor/room terrain.
        /// </summary>
        /// <value>A new <see cref="Tile"/> with <see cref="BaseMap.ROOM_TERRAIN_ID"/>.</value>
        /// <remarks>
        /// Used by generation steps when carving rooms and hallways.
        /// Returns a new instance each call to avoid shared state issues.
        /// </remarks>
        public ITile RoomTerrain => new Tile(BaseMap.ROOM_TERRAIN_ID);

        /// <summary>
        /// Gets a tile instance representing impassable wall terrain.
        /// </summary>
        /// <value>A new <see cref="Tile"/> with <see cref="BaseMap.WALL_TERRAIN_ID"/>.</value>
        /// <remarks>
        /// Used by generation steps when filling areas with walls.
        /// Returns a new instance each call to avoid shared state issues.
        /// </remarks>
        public ITile WallTerrain => new Tile(BaseMap.WALL_TERRAIN_ID);

        /// <summary>
        /// Gets a value indicating whether the tile array has been initialized.
        /// </summary>
        /// <value><c>true</c> if <see cref="CreateNew"/> has been called; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Generation steps may check this to ensure the map is ready for modification.
        /// </remarks>
        public bool TilesInitialized => this.Map.Tiles != null;

        /// <summary>
        /// Gets the width of the map in tiles.
        /// </summary>
        /// <value>The number of tile columns in the map.</value>
        public int Width => this.Map.Width;

        /// <summary>
        /// Gets the height of the map in tiles.
        /// </summary>
        /// <value>The number of tile rows in the map.</value>
        public int Height => this.Map.Height;

        /// <summary>
        /// Gets a value indicating whether the map wraps at edges (toroidal topology).
        /// </summary>
        /// <value>Always <c>false</c> in this implementation (no wrapping).</value>
        /// <remarks>
        /// Override this property and return <c>true</c> to enable wraparound maps
        /// where walking off one edge appears on the opposite side.
        /// </remarks>
        public bool Wrap => false;

        /// <summary>
        /// Gets the random number generator for this generation context.
        /// </summary>
        /// <value>The <see cref="ReRandom"/> instance seeded during <see cref="InitSeed"/>.</value>
        /// <remarks>
        /// All generation steps should use this RNG to ensure deterministic generation.
        /// Using the same seed will produce identical maps.
        /// </remarks>
        public IRandom Rand => this.Map.Rand;

        /// <summary>
        /// Gets the tile at the specified location.
        /// </summary>
        /// <param name="loc">The map coordinates to query.</param>
        /// <returns>The <see cref="ITile"/> at the specified location.</returns>
        /// <remarks>
        /// Does not perform bounds checking. Ensure <paramref name="loc"/> is within
        /// the map dimensions before calling.
        /// </remarks>
        public ITile GetTile(Loc loc) => this.Map.Tiles[loc.X][loc.Y];

        /// <summary>
        /// Determines whether a tile can be placed at the specified location.
        /// </summary>
        /// <param name="loc">The map coordinates to check.</param>
        /// <param name="tile">The tile to potentially place.</param>
        /// <returns><c>true</c> if the tile can be placed; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Override this method to implement placement restrictions, such as protecting
        /// certain areas from modification or enforcing terrain rules.
        /// The default implementation always returns <c>true</c>.
        /// </remarks>
        public virtual bool CanSetTile(Loc loc, ITile tile) => true;

        /// <summary>
        /// Attempts to place a tile at the specified location.
        /// </summary>
        /// <param name="loc">The map coordinates where the tile should be placed.</param>
        /// <param name="tile">The tile to place.</param>
        /// <returns><c>true</c> if the tile was successfully placed; <c>false</c> if placement was blocked by <see cref="CanSetTile"/>.</returns>
        /// <remarks>
        /// This is the safe way to modify tiles, respecting any placement restrictions.
        /// Prefer this over direct array access.
        /// </remarks>
        public bool TrySetTile(Loc loc, ITile tile)
        {
            if (!this.CanSetTile(loc, tile))
                return false;
            this.Map.Tiles[loc.X][loc.Y] = (Tile)tile;
            return true;
        }

        /// <summary>
        /// Places a tile at the specified location, throwing if placement is blocked.
        /// </summary>
        /// <param name="loc">The map coordinates where the tile should be placed.</param>
        /// <param name="tile">The tile to place.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <see cref="CanSetTile"/> returns <c>false</c> for this location and tile.
        /// </exception>
        /// <remarks>
        /// Use this when tile placement is required and failure indicates a generation error.
        /// For optional placement, use <see cref="TrySetTile"/> instead.
        /// </remarks>
        public void SetTile(Loc loc, ITile tile)
        {
            if (!this.TrySetTile(loc, tile))
                throw new InvalidOperationException("Can't place tile!");
        }

        /// <summary>
        /// Initializes the random number generator with the specified seed.
        /// </summary>
        /// <param name="seed">The seed value for deterministic generation.</param>
        /// <remarks>
        /// Called by <see cref="MapGen{T}"/> at the start of generation. Using the same
        /// seed will produce identical maps, enabling reproducible generation for
        /// testing or sharing map seeds with players.
        /// </remarks>
        public void InitSeed(ulong seed)
        {
            this.Map.Rand = new ReRandom(seed);
        }

        /// <summary>
        /// Determines whether the specified tile blocks movement.
        /// </summary>
        /// <param name="loc">The map coordinates to check.</param>
        /// <returns><c>true</c> if the tile is a wall; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Used by pathfinding and connectivity algorithms in generation steps.
        /// This implementation treats only <see cref="BaseMap.WALL_TERRAIN_ID"/> as blocking.
        /// </remarks>
        bool ITiledGenContext.TileBlocked(Loc loc)
        {
            return this.Map.Tiles[loc.X][loc.Y].ID == BaseMap.WALL_TERRAIN_ID;
        }

        /// <summary>
        /// Determines whether the specified tile blocks movement, with diagonal consideration.
        /// </summary>
        /// <param name="loc">The map coordinates to check.</param>
        /// <param name="diagonal">Whether this is a diagonal movement check.</param>
        /// <returns><c>true</c> if the tile is a wall; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This implementation does not distinguish between cardinal and diagonal movement.
        /// Override to implement different blocking rules for diagonal movement
        /// (e.g., requiring adjacent tiles to also be passable).
        /// </remarks>
        bool ITiledGenContext.TileBlocked(Loc loc, bool diagonal)
        {
            return this.Map.Tiles[loc.X][loc.Y].ID == BaseMap.WALL_TERRAIN_ID;
        }

        /// <summary>
        /// Creates and initializes a new map with the specified dimensions.
        /// </summary>
        /// <param name="width">The width of the map in tiles.</param>
        /// <param name="height">The height of the map in tiles.</param>
        /// <param name="wrap">Whether the map should wrap at edges (ignored in this implementation).</param>
        /// <remarks>
        /// Called by initialization steps (e.g., <c>InitTilesStep</c>) at the start of generation.
        /// Override to initialize additional data structures alongside the tile array.
        /// All tiles are initialized as walls.
        /// </remarks>
        public virtual void CreateNew(int width, int height, bool wrap = false)
        {
            this.Map.InitializeTiles(width, height);
        }

        /// <summary>
        /// Performs any final cleanup or validation after generation completes.
        /// </summary>
        /// <remarks>
        /// Called by <see cref="MapGen{T}"/> after all generation steps have executed.
        /// Override to perform post-generation tasks such as:
        /// <list type="bullet">
        /// <item><description>Validating map connectivity</description></item>
        /// <item><description>Computing pathfinding data</description></item>
        /// <item><description>Generating minimap data</description></item>
        /// <item><description>Releasing temporary generation resources</description></item>
        /// </list>
        /// The default implementation does nothing.
        /// </remarks>
        public virtual void FinishGen()
        {
        }
    }
}
