// <copyright file="MapGenerationBenchmarks.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using BenchmarkDotNet.Attributes;

namespace RogueElements.Benchmarks;

/// <summary>
/// Benchmarks for map generation operations.
/// Tests various map sizes and generation pipeline configurations.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class MapGenerationBenchmarks
{
    private MapGen<BenchmarkMapGenContext> _smallGridLayout = null!;
    private MapGen<BenchmarkMapGenContext> _mediumGridLayout = null!;
    private MapGen<BenchmarkMapGenContext> _largeGridLayout = null!;
    private ulong _seed;

    [GlobalSetup]
    public void Setup()
    {
        _seed = 12345UL;

        // Small grid: 4x3 cells, 7x7 each (~28x21 tiles)
        _smallGridLayout = CreateGridLayout(4, 3, 7, 7);

        // Medium grid: 6x4 cells, 9x9 each (~54x36 tiles)
        _mediumGridLayout = CreateGridLayout(6, 4, 9, 9);

        // Large grid: 10x8 cells, 11x11 each (~110x88 tiles)
        _largeGridLayout = CreateGridLayout(10, 8, 11, 11);
    }

    private static MapGen<BenchmarkMapGenContext> CreateGridLayout(int cellX, int cellY, int cellWidth, int cellHeight)
    {
        var layout = new MapGen<BenchmarkMapGenContext>();

        // Initialize grid
        var startGen = new InitGridPlanStep<BenchmarkMapGenContext>(1)
        {
            CellX = cellX,
            CellY = cellY,
            CellWidth = cellWidth,
            CellHeight = cellHeight,
        };
        layout.GenSteps.Add(-4, startGen);

        // Create branching path
        var path = new GridPathBranch<BenchmarkMapGenContext>
        {
            RoomRatio = new RandRange(70),
            BranchRatio = new RandRange(0, 50),
        };

        var genericRooms = new SpawnList<RoomGen<BenchmarkMapGenContext>>
        {
            { new RoomGenSquare<BenchmarkMapGenContext>(new RandRange(4, 8), new RandRange(4, 8)), 10 },
            { new RoomGenRound<BenchmarkMapGenContext>(new RandRange(5, 9), new RandRange(5, 9)), 10 },
        };
        path.GenericRooms = genericRooms;

        var genericHalls = new SpawnList<PermissiveRoomGen<BenchmarkMapGenContext>>
        {
            { new RoomGenAngledHall<BenchmarkMapGenContext>(50), 10 },
        };
        path.GenericHalls = genericHalls;

        layout.GenSteps.Add(-4, path);

        // Convert to floor plan
        layout.GenSteps.Add(-2, new DrawGridToFloorStep<BenchmarkMapGenContext>());

        // Draw to tiles
        layout.GenSteps.Add(0, new DrawFloorToTileStep<BenchmarkMapGenContext>(1));

        return layout;
    }

    [Benchmark(Baseline = true)]
    public BenchmarkMapGenContext SmallGrid_4x3()
    {
        return _smallGridLayout.GenMap(_seed++);
    }

    [Benchmark]
    public BenchmarkMapGenContext MediumGrid_6x4()
    {
        return _mediumGridLayout.GenMap(_seed++);
    }

    [Benchmark]
    public BenchmarkMapGenContext LargeGrid_10x8()
    {
        return _largeGridLayout.GenMap(_seed++);
    }
}

/// <summary>
/// Benchmarks for individual room generation operations.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class RoomGenBenchmarks
{
    private BenchmarkMapGenContext _context = null!;
    private RoomGenSquare<BenchmarkMapGenContext> _squareGen = null!;
    private RoomGenRound<BenchmarkMapGenContext> _roundGen = null!;
    private RoomGenCave<BenchmarkMapGenContext> _caveGen = null!;

    [GlobalSetup]
    public void Setup()
    {
        _context = new BenchmarkMapGenContext();
        _context.InitSeed(12345UL);
        _context.CreateNew(100, 100);

        _squareGen = new RoomGenSquare<BenchmarkMapGenContext>(new RandRange(5, 10), new RandRange(5, 10));
        _roundGen = new RoomGenRound<BenchmarkMapGenContext>(new RandRange(5, 10), new RandRange(5, 10));
        _caveGen = new RoomGenCave<BenchmarkMapGenContext>(new RandRange(5, 10), new RandRange(5, 10));
    }

    [Benchmark(Baseline = true)]
    public Rect SquareRoom()
    {
        _squareGen.PrepareSize(_context.Rand, new Loc(10, 10));
        _squareGen.DrawOnMap(_context);
        return _squareGen.Draw;
    }

    [Benchmark]
    public Rect RoundRoom()
    {
        _roundGen.PrepareSize(_context.Rand, new Loc(10, 10));
        _roundGen.DrawOnMap(_context);
        return _roundGen.Draw;
    }

    [Benchmark]
    public Rect CaveRoom()
    {
        _caveGen.PrepareSize(_context.Rand, new Loc(10, 10));
        _caveGen.DrawOnMap(_context);
        return _caveGen.Draw;
    }
}

/// <summary>
/// Benchmarks for random number generation operations.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class RngBenchmarks
{
    private ReRandom _reRandom = null!;
    private RandRange _range;

    [GlobalSetup]
    public void Setup()
    {
        _reRandom = new ReRandom(12345UL);
        _range = new RandRange(1, 100);
    }

    [Benchmark]
    public int ReRandom_Next()
    {
        return _reRandom.Next();
    }

    [Benchmark]
    public int ReRandom_NextRange()
    {
        return _reRandom.Next(1, 100);
    }

    [Benchmark]
    public int RandRange_Pick()
    {
        return _range.Pick(_reRandom);
    }

    [Benchmark]
    public ulong ReRandom_NextUInt64()
    {
        return _reRandom.NextUInt64();
    }
}
