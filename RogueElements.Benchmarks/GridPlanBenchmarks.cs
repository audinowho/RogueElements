// <copyright file="GridPlanBenchmarks.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using BenchmarkDotNet.Attributes;

namespace RogueElements.Benchmarks;

/// <summary>
/// Benchmarks for GridPlan.GetAdjacentRooms.
/// Currently uses List.Contains which is O(n) - should use HashSet for O(1).
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class GridAdjacencyBenchmarks
{
    private GridPlan _smallGrid = null!;
    private GridPlan _mediumGrid = null!;
    private GridPlan _largeGrid = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Small: 4x4 grid
        _smallGrid = CreateGridPlan(4, 4);

        // Medium: 8x8 grid
        _mediumGrid = CreateGridPlan(8, 8);

        // Large: 12x12 grid
        _largeGrid = CreateGridPlan(12, 12);
    }

    private static GridPlan CreateGridPlan(int width, int height)
    {
        var plan = new GridPlan();
        plan.InitSize(width, height, 8, 8, 1);

        var rand = new ReRandom(12345UL);
        var roomGen = new RoomGenSquare<BenchmarkMapGenContext>(
            new RandRange(4, 6), new RandRange(4, 6));
        var hallGen = new RoomGenAngledHall<BenchmarkMapGenContext>(50);

        // Add rooms in a connected pattern
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                plan.AddRoom(new Loc(x, y), roomGen.Copy(), new ComponentCollection());

                // Add horizontal halls
                if (x > 0)
                {
                    plan.SetHall(new LocRay4(x, y, Dir4.Left), (IPermissiveRoomGen)hallGen.Copy(), new ComponentCollection());
                }

                // Add vertical halls
                if (y > 0)
                {
                    plan.SetHall(new LocRay4(x, y, Dir4.Up), (IPermissiveRoomGen)hallGen.Copy(), new ComponentCollection());
                }
            }
        }

        return plan;
    }

    [Benchmark(Baseline = true)]
    public int GetAdjacentRooms_4x4Grid()
    {
        int total = 0;
        for (int i = 0; i < _smallGrid.RoomCount; i++)
        {
            total += _smallGrid.GetAdjacentRooms(i).Count;
        }
        return total;
    }

    [Benchmark]
    public int GetAdjacentRooms_8x8Grid()
    {
        int total = 0;
        for (int i = 0; i < _mediumGrid.RoomCount; i++)
        {
            total += _mediumGrid.GetAdjacentRooms(i).Count;
        }
        return total;
    }

    [Benchmark]
    public int GetAdjacentRooms_12x12Grid()
    {
        int total = 0;
        for (int i = 0; i < _largeGrid.RoomCount; i++)
        {
            total += _largeGrid.GetAdjacentRooms(i).Count;
        }
        return total;
    }

    /// <summary>
    /// Simulates the current List.Contains approach for comparison.
    /// </summary>
    [Benchmark]
    public List<int> GetAdjacentRooms_ListContains_SingleRoom()
    {
        return _mediumGrid.GetAdjacentRooms(_mediumGrid.RoomCount / 2);
    }

    /// <summary>
    /// Simulates optimized HashSet approach.
    /// </summary>
    [Benchmark]
    public HashSet<int> GetAdjacentRooms_HashSet_SingleRoom()
    {
        return GetAdjacentRoomsWithHashSet(_mediumGrid, _mediumGrid.RoomCount / 2);
    }

    private static HashSet<int> GetAdjacentRoomsWithHashSet(GridPlan plan, int roomIndex)
    {
        var returnSet = new HashSet<int>();
        var room = plan.GetRoomPlan(roomIndex);
        if (room == null)
            return returnSet;

        var bounds = room.Bounds;

        for (int ii = 0; ii < bounds.Size.X; ii++)
        {
            // above
            int up = plan.GetRoomIndex(new LocRay4(bounds.X + ii, bounds.Y, Dir4.Up));
            if (up > -1)
                returnSet.Add(up);

            // below
            int down = plan.GetRoomIndex(new LocRay4(bounds.X + ii, bounds.End.Y - 1, Dir4.Down));
            if (down > -1)
                returnSet.Add(down);
        }

        for (int ii = 0; ii < bounds.Size.Y; ii++)
        {
            // left
            int left = plan.GetRoomIndex(new LocRay4(bounds.X, bounds.Y + ii, Dir4.Left));
            if (left > -1)
                returnSet.Add(left);

            // right
            int right = plan.GetRoomIndex(new LocRay4(bounds.End.X - 1, bounds.Y + ii, Dir4.Right));
            if (right > -1)
                returnSet.Add(right);
        }

        return returnSet;
    }
}

/// <summary>
/// Benchmarks for GridPlan room erasure operations.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class GridEraseRoomBenchmarks
{
    private ReRandom _rand = null!;

    [GlobalSetup]
    public void Setup()
    {
        _rand = new ReRandom(12345UL);
    }

    [Benchmark(Baseline = true)]
    public GridPlan EraseRooms_4from16()
    {
        var plan = CreateFullGrid(4, 4);
        // Erase corner rooms
        plan.EraseRoom(new Loc(3, 3));
        plan.EraseRoom(new Loc(3, 0));
        plan.EraseRoom(new Loc(0, 3));
        plan.EraseRoom(new Loc(0, 0));
        return plan;
    }

    [Benchmark]
    public GridPlan EraseRooms_9from64()
    {
        var plan = CreateFullGrid(8, 8);
        // Erase a 3x3 section
        for (int x = 2; x < 5; x++)
        {
            for (int y = 2; y < 5; y++)
            {
                plan.EraseRoom(new Loc(x, y));
            }
        }
        return plan;
    }

    [Benchmark]
    public GridPlan EraseRooms_16from144()
    {
        var plan = CreateFullGrid(12, 12);
        // Erase a 4x4 section
        for (int x = 4; x < 8; x++)
        {
            for (int y = 4; y < 8; y++)
            {
                plan.EraseRoom(new Loc(x, y));
            }
        }
        return plan;
    }

    private GridPlan CreateFullGrid(int width, int height)
    {
        var plan = new GridPlan();
        plan.InitSize(width, height, 8, 8, 1);

        var roomGen = new RoomGenSquare<BenchmarkMapGenContext>(
            new RandRange(4, 6), new RandRange(4, 6));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                plan.AddRoom(new Loc(x, y), roomGen.Copy(), new ComponentCollection());
            }
        }

        return plan;
    }
}

/// <summary>
/// Benchmarks for GridPlan to FloorPlan conversion.
/// PlaceRoomsOnFloor is a complex operation with multiple phases.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class GridToFloorConversionBenchmarks
{
    private BenchmarkMapGenContext _smallContext = null!;
    private BenchmarkMapGenContext _mediumContext = null!;
    private BenchmarkMapGenContext _largeContext = null!;

    [GlobalSetup]
    public void Setup()
    {
        _smallContext = CreateContextWithGrid(4, 4);
        _mediumContext = CreateContextWithGrid(8, 6);
        _largeContext = CreateContextWithGrid(12, 10);
    }

    private static BenchmarkMapGenContext CreateContextWithGrid(int gridWidth, int gridHeight)
    {
        var context = new BenchmarkMapGenContext();
        context.InitSeed(12345UL);

        var gridPlan = new GridPlan();
        gridPlan.InitSize(gridWidth, gridHeight, 10, 10, 2);
        context.InitGrid(gridPlan);

        var rand = new ReRandom(12345UL);
        var roomGen = new RoomGenSquare<BenchmarkMapGenContext>(
            new RandRange(5, 8), new RandRange(5, 8));
        var hallGen = new RoomGenAngledHall<BenchmarkMapGenContext>(50);

        // Create connected grid
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                gridPlan.AddRoom(new Loc(x, y), roomGen.Copy(), new ComponentCollection());

                if (x > 0)
                    gridPlan.SetHall(new LocRay4(x, y, Dir4.Left), (IPermissiveRoomGen)hallGen.Copy(), new ComponentCollection());
                if (y > 0)
                    gridPlan.SetHall(new LocRay4(x, y, Dir4.Up), (IPermissiveRoomGen)hallGen.Copy(), new ComponentCollection());
            }
        }

        return context;
    }

    [Benchmark(Baseline = true)]
    public FloorPlan ConvertGridToFloor_4x4()
    {
        var context = CloneContext(_smallContext);
        var floorPlan = new FloorPlan();
        floorPlan.InitSize(context.GridPlan.Size);
        context.InitPlan(floorPlan);
        context.GridPlan.PlaceRoomsOnFloor(context);
        return floorPlan;
    }

    [Benchmark]
    public FloorPlan ConvertGridToFloor_8x6()
    {
        var context = CloneContext(_mediumContext);
        var floorPlan = new FloorPlan();
        floorPlan.InitSize(context.GridPlan.Size);
        context.InitPlan(floorPlan);
        context.GridPlan.PlaceRoomsOnFloor(context);
        return floorPlan;
    }

    [Benchmark]
    public FloorPlan ConvertGridToFloor_12x10()
    {
        var context = CloneContext(_largeContext);
        var floorPlan = new FloorPlan();
        floorPlan.InitSize(context.GridPlan.Size);
        context.InitPlan(floorPlan);
        context.GridPlan.PlaceRoomsOnFloor(context);
        return floorPlan;
    }

    private static BenchmarkMapGenContext CloneContext(BenchmarkMapGenContext source)
    {
        // Re-create context with same grid structure
        return CreateContextWithGrid(source.GridPlan.GridWidth, source.GridPlan.GridHeight);
    }
}
