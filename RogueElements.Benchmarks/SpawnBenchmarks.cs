// <copyright file="SpawnBenchmarks.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using BenchmarkDotNet.Attributes;

namespace RogueElements.Benchmarks;

/// <summary>
/// Benchmarks for spawn distribution operations.
/// Measures the O(n²) impact of RemoveAt in RandomSpawnStep.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class SpawnBenchmarks
{
    private SpawnableBenchmarkContext _smallContext = null!;
    private SpawnableBenchmarkContext _mediumContext = null!;
    private SpawnableBenchmarkContext _largeContext = null!;
    private List<BenchmarkSpawnable> _spawns10 = null!;
    private List<BenchmarkSpawnable> _spawns50 = null!;
    private List<BenchmarkSpawnable> _spawns100 = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Small: 50x50 = 2,500 tiles
        _smallContext = CreateSpawnableContext(50, 50);

        // Medium: 100x100 = 10,000 tiles
        _mediumContext = CreateSpawnableContext(100, 100);

        // Large: 200x200 = 40,000 tiles
        _largeContext = CreateSpawnableContext(200, 200);

        // Pre-create spawn lists
        _spawns10 = CreateSpawnList(10);
        _spawns50 = CreateSpawnList(50);
        _spawns100 = CreateSpawnList(100);
    }

    private static SpawnableBenchmarkContext CreateSpawnableContext(int width, int height)
    {
        var context = new SpawnableBenchmarkContext();
        context.InitSeed(12345UL);
        context.CreateNew(width, height);

        // Create some floor tiles (checkerboard pattern for variety)
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if ((x + y) % 3 != 0) // ~66% floor tiles
                {
                    context.SetTile(new Loc(x, y), new BenchmarkTile(BenchmarkTile.ROOM_ID));
                }
            }
        }

        return context;
    }

    private static List<BenchmarkSpawnable> CreateSpawnList(int count)
    {
        var spawns = new List<BenchmarkSpawnable>();
        for (int i = 0; i < count; i++)
        {
            spawns.Add(new BenchmarkSpawnable { ID = i });
        }
        return spawns;
    }

    // Benchmark: Current RemoveAt approach
    [Benchmark(Baseline = true)]
    public int Spawn_10Items_SmallMap_RemoveAt()
    {
        return DistributeWithRemoveAt(_smallContext.Clone(), new List<BenchmarkSpawnable>(_spawns10));
    }

    [Benchmark]
    public int Spawn_50Items_MediumMap_RemoveAt()
    {
        return DistributeWithRemoveAt(_mediumContext.Clone(), new List<BenchmarkSpawnable>(_spawns50));
    }

    [Benchmark]
    public int Spawn_100Items_LargeMap_RemoveAt()
    {
        return DistributeWithRemoveAt(_largeContext.Clone(), new List<BenchmarkSpawnable>(_spawns100));
    }

    // Benchmark: Optimized swap-and-pop approach
    [Benchmark]
    public int Spawn_10Items_SmallMap_SwapPop()
    {
        return DistributeWithSwapPop(_smallContext.Clone(), new List<BenchmarkSpawnable>(_spawns10));
    }

    [Benchmark]
    public int Spawn_50Items_MediumMap_SwapPop()
    {
        return DistributeWithSwapPop(_mediumContext.Clone(), new List<BenchmarkSpawnable>(_spawns50));
    }

    [Benchmark]
    public int Spawn_100Items_LargeMap_SwapPop()
    {
        return DistributeWithSwapPop(_largeContext.Clone(), new List<BenchmarkSpawnable>(_spawns100));
    }

    /// <summary>
    /// Current implementation: O(n) RemoveAt for each spawn.
    /// </summary>
    private static int DistributeWithRemoveAt(SpawnableBenchmarkContext map, List<BenchmarkSpawnable> spawns)
    {
        List<Loc> freeTiles = map.GetAllFreeTiles();
        int placed = 0;

        for (int ii = 0; ii < spawns.Count && freeTiles.Count > 0; ii++)
        {
            int randIndex = map.Rand.Next(freeTiles.Count);
            map.PlaceItem(freeTiles[randIndex], spawns[ii]);
            freeTiles.RemoveAt(randIndex); // O(n) shift
            placed++;
        }

        return placed;
    }

    /// <summary>
    /// Optimized: O(1) swap-and-pop for each spawn.
    /// </summary>
    private static int DistributeWithSwapPop(SpawnableBenchmarkContext map, List<BenchmarkSpawnable> spawns)
    {
        List<Loc> freeTiles = map.GetAllFreeTiles();
        int placed = 0;

        for (int ii = 0; ii < spawns.Count && freeTiles.Count > 0; ii++)
        {
            int randIndex = map.Rand.Next(freeTiles.Count);
            map.PlaceItem(freeTiles[randIndex], spawns[ii]);

            // O(1) removal: swap with last, then remove last
            int lastIndex = freeTiles.Count - 1;
            if (randIndex != lastIndex)
            {
                freeTiles[randIndex] = freeTiles[lastIndex];
            }
            freeTiles.RemoveAt(lastIndex);
            placed++;
        }

        return placed;
    }
}

/// <summary>
/// Benchmarks for GetAllFreeTiles allocation.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class FreeTilesBenchmarks
{
    private SpawnableBenchmarkContext _smallContext = null!;
    private SpawnableBenchmarkContext _mediumContext = null!;
    private SpawnableBenchmarkContext _largeContext = null!;

    [GlobalSetup]
    public void Setup()
    {
        _smallContext = CreateContext(50, 50);
        _mediumContext = CreateContext(100, 100);
        _largeContext = CreateContext(200, 200);
    }

    private static SpawnableBenchmarkContext CreateContext(int width, int height)
    {
        var context = new SpawnableBenchmarkContext();
        context.InitSeed(12345UL);
        context.CreateNew(width, height);

        // Create floor tiles
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                context.SetTile(new Loc(x, y), new BenchmarkTile(BenchmarkTile.ROOM_ID));
            }
        }

        return context;
    }

    [Benchmark(Baseline = true)]
    public List<Loc> GetFreeTiles_50x50()
    {
        return _smallContext.GetAllFreeTiles();
    }

    [Benchmark]
    public List<Loc> GetFreeTiles_100x100()
    {
        return _mediumContext.GetAllFreeTiles();
    }

    [Benchmark]
    public List<Loc> GetFreeTiles_200x200()
    {
        return _largeContext.GetAllFreeTiles();
    }
}

/// <summary>
/// Simple spawnable for benchmarking.
/// </summary>
public class BenchmarkSpawnable : ISpawnable
{
    public int ID { get; set; }
    public Loc Loc { get; set; }

    public ISpawnable Copy() => new BenchmarkSpawnable { ID = ID, Loc = Loc };
}

/// <summary>
/// Context that supports spawning for benchmarks.
/// </summary>
public class SpawnableBenchmarkContext : BenchmarkMapGenContext, IPlaceableGenContext<BenchmarkSpawnable>
{
    private readonly List<BenchmarkSpawnable> _spawnedItems = new();

    public List<Loc> GetAllFreeTiles()
    {
        var freeTiles = new List<Loc>();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var loc = new Loc(x, y);
                if (CanPlaceItem(loc))
                    freeTiles.Add(loc);
            }
        }
        return freeTiles;
    }

    public List<Loc> GetFreeTiles(Rect rect)
    {
        var freeTiles = new List<Loc>();
        for (int x = rect.X; x < rect.End.X && x < Width; x++)
        {
            for (int y = rect.Y; y < rect.End.Y && y < Height; y++)
            {
                var loc = new Loc(x, y);
                if (CanPlaceItem(loc))
                    freeTiles.Add(loc);
            }
        }
        return freeTiles;
    }

    public bool CanPlaceItem(Loc loc)
    {
        if (loc.X < 0 || loc.X >= Width || loc.Y < 0 || loc.Y >= Height)
            return false;

        // Can only place on floor tiles
        return !TileBlocked(loc);
    }

    public void PlaceItem(Loc loc, BenchmarkSpawnable item)
    {
        item.Loc = loc;
        _spawnedItems.Add(item);
    }

    public SpawnableBenchmarkContext Clone()
    {
        var clone = new SpawnableBenchmarkContext();
        clone.InitSeed((ulong)Rand.Next());
        clone.CreateNew(Width, Height);

        // Copy tiles
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                clone.Map.Tiles[x][y] = new BenchmarkTile(Map.Tiles[x][y].ID);
            }
        }

        return clone;
    }
}
