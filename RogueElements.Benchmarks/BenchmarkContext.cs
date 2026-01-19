// <copyright file="BenchmarkContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RogueElements.Benchmarks;

/// <summary>
/// Simple tile wrapper for benchmarking.
/// </summary>
public class BenchmarkTile : ITile
{
    public const int WALL_ID = 0;
    public const int ROOM_ID = 1;
    public const int WATER_ID = 2;

    public int ID { get; set; }

    public BenchmarkTile()
    {
        ID = WALL_ID;
    }

    public BenchmarkTile(int id)
    {
        ID = id;
    }

    public ITile Copy() => new BenchmarkTile(ID);

    public bool TileEquivalent(ITile other) => other is BenchmarkTile tile && tile.ID == ID;
}

/// <summary>
/// Map storage for benchmarking.
/// </summary>
public class BenchmarkMap
{
    public BenchmarkTile[][] Tiles { get; private set; } = null!;
    public int Width => Tiles?.Length ?? 0;
    public int Height => Tiles?.Length > 0 ? Tiles[0].Length : 0;
    public ReRandom Rand { get; set; } = null!;

    public void InitializeTiles(int width, int height)
    {
        Tiles = new BenchmarkTile[width][];
        for (int x = 0; x < width; x++)
        {
            Tiles[x] = new BenchmarkTile[height];
            for (int y = 0; y < height; y++)
            {
                Tiles[x][y] = new BenchmarkTile(BenchmarkTile.WALL_ID);
            }
        }
    }
}

/// <summary>
/// Full-featured map generation context for benchmarking grid-based generation.
/// Implements IRoomGridGenContext to support the complete generation pipeline.
/// </summary>
public class BenchmarkMapGenContext : ITiledGenContext, IRoomGridGenContext
{
    public BenchmarkMap Map { get; set; }

    public ITile RoomTerrain => new BenchmarkTile(BenchmarkTile.ROOM_ID);
    public ITile WallTerrain => new BenchmarkTile(BenchmarkTile.WALL_ID);
    public bool TilesInitialized => Map.Tiles != null;
    public int Width => Map.Width;
    public int Height => Map.Height;
    public bool Wrap => false;
    public IRandom Rand => Map.Rand;
    public FloorPlan RoomPlan { get; private set; } = null!;
    public GridPlan GridPlan { get; private set; } = null!;

    public BenchmarkMapGenContext()
    {
        Map = new BenchmarkMap();
    }

    public ITile GetTile(Loc loc) => Map.Tiles[loc.X][loc.Y];

    public bool CanSetTile(Loc loc, ITile tile) => true;

    public bool TrySetTile(Loc loc, ITile tile)
    {
        if (!CanSetTile(loc, tile))
            return false;
        Map.Tiles[loc.X][loc.Y] = (BenchmarkTile)tile;
        return true;
    }

    public void SetTile(Loc loc, ITile tile)
    {
        if (!TrySetTile(loc, tile))
            throw new InvalidOperationException("Can't place tile!");
    }

    public void InitSeed(ulong seed)
    {
        Map.Rand = new ReRandom(seed);
    }

    public bool TileBlocked(Loc loc) => Map.Tiles[loc.X][loc.Y].ID == BenchmarkTile.WALL_ID;

    public bool TileBlocked(Loc loc, bool diagonal) => Map.Tiles[loc.X][loc.Y].ID == BenchmarkTile.WALL_ID;

    public void CreateNew(int width, int height, bool wrap = false)
    {
        Map.InitializeTiles(width, height);
    }

    public void FinishGen()
    {
    }

    public void InitPlan(FloorPlan plan)
    {
        RoomPlan = plan;
    }

    public void InitGrid(GridPlan plan)
    {
        GridPlan = plan;
    }
}
