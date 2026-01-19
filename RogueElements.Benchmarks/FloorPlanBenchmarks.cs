// <copyright file="FloorPlanBenchmarks.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using BenchmarkDotNet.Attributes;

namespace RogueElements.Benchmarks;

/// <summary>
/// Benchmarks for FloorPlan room erasure operations.
/// EraseRoomHall updates ALL room/hall indices - O(rooms + halls) per erasure.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class EraseRoomBenchmarks
{
    private ReRandom _rand = null!;

    [GlobalSetup]
    public void Setup()
    {
        _rand = new ReRandom(12345UL);
    }

    /// <summary>
    /// Erase 5 rooms from a 20-room floor plan.
    /// Each erase is O(remaining rooms + halls).
    /// </summary>
    [Benchmark(Baseline = true)]
    public FloorPlan EraseRooms_5from20()
    {
        var plan = CreateFloorPlanWithConnectedRooms(20);
        // Erase from end to avoid index shifting issues in test
        for (int i = 0; i < 5; i++)
        {
            plan.EraseRoomHall(new RoomHallIndex(plan.RoomCount - 1, false));
        }
        return plan;
    }

    /// <summary>
    /// Erase 10 rooms from a 50-room floor plan.
    /// </summary>
    [Benchmark]
    public FloorPlan EraseRooms_10from50()
    {
        var plan = CreateFloorPlanWithConnectedRooms(50);
        for (int i = 0; i < 10; i++)
        {
            plan.EraseRoomHall(new RoomHallIndex(plan.RoomCount - 1, false));
        }
        return plan;
    }

    /// <summary>
    /// Erase 20 rooms from a 100-room floor plan.
    /// This demonstrates quadratic behavior: 20 × O(80..100) = O(1800) operations.
    /// </summary>
    [Benchmark]
    public FloorPlan EraseRooms_20from100()
    {
        var plan = CreateFloorPlanWithConnectedRooms(100);
        for (int i = 0; i < 20; i++)
        {
            plan.EraseRoomHall(new RoomHallIndex(plan.RoomCount - 1, false));
        }
        return plan;
    }

    private FloorPlan CreateFloorPlanWithConnectedRooms(int roomCount)
    {
        var plan = new FloorPlan();
        plan.InitSize(new Loc(500, 500));

        int gridSize = (int)Math.Ceiling(Math.Sqrt(roomCount));
        int cellSize = 500 / gridSize;
        int roomSize = Math.Max(4, cellSize / 3);

        for (int i = 0; i < roomCount; i++)
        {
            int gridX = i % gridSize;
            int gridY = i / gridSize;

            int x = gridX * cellSize + (cellSize - roomSize) / 2;
            int y = gridY * cellSize + (cellSize - roomSize) / 2;

            var roomGen = new RoomGenSquare<BenchmarkMapGenContext>(
                new RandRange(roomSize, roomSize + 1),
                new RandRange(roomSize, roomSize + 1));
            roomGen.PrepareSize(_rand, new Loc(roomSize, roomSize));
            roomGen.SetLoc(new Loc(x, y));

            // Connect to previous room if exists
            if (i > 0)
            {
                plan.AddRoom(roomGen, new ComponentCollection(), new RoomHallIndex(i - 1, false));
            }
            else
            {
                plan.AddRoom(roomGen, new ComponentCollection());
            }
        }

        return plan;
    }
}

/// <summary>
/// Benchmarks for FloorPlan.DrawOnMap which iterates all rooms and halls.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class DrawFloorPlanBenchmarks
{
    private BenchmarkMapGenContext _context = null!;
    private FloorPlan _smallPlan = null!;
    private FloorPlan _mediumPlan = null!;
    private FloorPlan _largePlan = null!;

    [GlobalSetup]
    public void Setup()
    {
        _context = new BenchmarkMapGenContext();
        _context.InitSeed(12345UL);
        _context.CreateNew(200, 200);

        var rand = new ReRandom(12345UL);
        _smallPlan = CreateDrawableFloorPlan(rand, 10, 200, 200);
        _mediumPlan = CreateDrawableFloorPlan(rand, 30, 200, 200);
        _largePlan = CreateDrawableFloorPlan(rand, 60, 200, 200);
    }

    private static FloorPlan CreateDrawableFloorPlan(ReRandom rand, int roomCount, int width, int height)
    {
        var plan = new FloorPlan();
        plan.InitSize(new Loc(width, height));

        int gridSize = (int)Math.Ceiling(Math.Sqrt(roomCount));
        int cellSize = Math.Min(width, height) / gridSize;
        int roomSize = Math.Max(4, cellSize / 2);

        for (int i = 0; i < roomCount; i++)
        {
            int gridX = i % gridSize;
            int gridY = i / gridSize;

            int x = gridX * cellSize + 2;
            int y = gridY * cellSize + 2;

            var roomGen = new RoomGenSquare<BenchmarkMapGenContext>(
                new RandRange(roomSize, roomSize + 1),
                new RandRange(roomSize, roomSize + 1));
            roomGen.PrepareSize(rand, new Loc(roomSize, roomSize));
            roomGen.SetLoc(new Loc(x, y));

            if (i > 0)
            {
                // Add hall connecting to previous room
                int prevGridX = (i - 1) % gridSize;
                int prevGridY = (i - 1) / gridSize;

                if (gridX != prevGridX || gridY != prevGridY)
                {
                    // Create a connecting hall
                    int hallX = Math.Min(x, prevGridX * cellSize + 2 + roomSize);
                    int hallY = Math.Min(y, prevGridY * cellSize + 2 + roomSize);

                    var hallGen = new RoomGenSquare<BenchmarkMapGenContext>(
                        new RandRange(2, 3),
                        new RandRange(2, 3));
                    hallGen.PrepareSize(rand, new Loc(2, 2));
                    hallGen.SetLoc(new Loc(hallX, hallY));

                    plan.AddHall(hallGen, new ComponentCollection(), new RoomHallIndex(i - 1, false));
                }

                plan.AddRoom(roomGen, new ComponentCollection(), new RoomHallIndex(i - 1, false));
            }
            else
            {
                plan.AddRoom(roomGen, new ComponentCollection());
            }
        }

        return plan;
    }

    [Benchmark(Baseline = true)]
    public void DrawFloorPlan_10Rooms()
    {
        ResetContext();
        _smallPlan.DrawOnMap(_context);
    }

    [Benchmark]
    public void DrawFloorPlan_30Rooms()
    {
        ResetContext();
        _mediumPlan.DrawOnMap(_context);
    }

    [Benchmark]
    public void DrawFloorPlan_60Rooms()
    {
        ResetContext();
        _largePlan.DrawOnMap(_context);
    }

    private void ResetContext()
    {
        // Fill with walls before each draw
        for (int x = 0; x < _context.Width; x++)
        {
            for (int y = 0; y < _context.Height; y++)
            {
                _context.Map.Tiles[x][y] = new BenchmarkTile(BenchmarkTile.WALL_ID);
            }
        }
    }
}

/// <summary>
/// Benchmarks for GetDirAdjacent which tries all 4 directions.
/// Called repeatedly during DrawOnMap for border negotiation.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class AdjacencyLookupBenchmarks
{
    private FloorPlan _plan = null!;
    private RoomHallIndex _centerRoom;

    [GlobalSetup]
    public void Setup()
    {
        _plan = CreateConnectedFloorPlan(25);
        _centerRoom = new RoomHallIndex(12, false); // Center room in 5x5 grid
    }

    private static FloorPlan CreateConnectedFloorPlan(int roomCount)
    {
        var plan = new FloorPlan();
        plan.InitSize(new Loc(200, 200));
        var rand = new ReRandom(12345UL);

        int gridSize = (int)Math.Ceiling(Math.Sqrt(roomCount));
        int cellSize = 200 / gridSize;
        int roomSize = cellSize - 4;

        for (int i = 0; i < roomCount; i++)
        {
            int gridX = i % gridSize;
            int gridY = i / gridSize;

            int x = gridX * cellSize + 2;
            int y = gridY * cellSize + 2;

            var roomGen = new RoomGenSquare<BenchmarkMapGenContext>(
                new RandRange(roomSize, roomSize + 1),
                new RandRange(roomSize, roomSize + 1));
            roomGen.PrepareSize(rand, new Loc(roomSize, roomSize));
            roomGen.SetLoc(new Loc(x, y));

            // Connect horizontally
            if (gridX > 0)
            {
                plan.AddRoom(roomGen, new ComponentCollection(), new RoomHallIndex(i - 1, false));
            }
            // Connect vertically
            else if (gridY > 0)
            {
                plan.AddRoom(roomGen, new ComponentCollection(), new RoomHallIndex(i - gridSize, false));
            }
            else
            {
                plan.AddRoom(roomGen, new ComponentCollection());
            }
        }

        return plan;
    }

    [Benchmark]
    public int GetAdjacents_AllRooms()
    {
        int totalAdjacents = 0;
        for (int i = 0; i < _plan.RoomCount; i++)
        {
            var room = _plan.GetRoomHall(new RoomHallIndex(i, false));
            totalAdjacents += room.Adjacents.Count;
        }
        return totalAdjacents;
    }

    [Benchmark]
    public List<int> GetAdjacentRoomHalls_SingleRoom()
    {
        var adjacents = new List<int>();
        var room = _plan.GetRoomHall(_centerRoom);
        foreach (var adj in room.Adjacents)
        {
            adjacents.Add(adj.Index);
        }
        return adjacents;
    }
}
