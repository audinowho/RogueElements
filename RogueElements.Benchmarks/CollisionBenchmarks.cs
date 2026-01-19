// <copyright file="CollisionBenchmarks.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using BenchmarkDotNet.Attributes;

namespace RogueElements.Benchmarks;

/// <summary>
/// Benchmarks for FloorPlan collision detection.
/// Measures how collision checking scales with room count.
/// This is a CRITICAL hotspot - currently O(rooms + halls) per check.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class CollisionBenchmarks
{
    private FloorPlan _smallFloorPlan = null!;
    private FloorPlan _mediumFloorPlan = null!;
    private FloorPlan _largeFloorPlan = null!;
    private Rect _testRect;

    [GlobalSetup]
    public void Setup()
    {
        _testRect = new Rect(50, 50, 5, 5);

        // Small: 10 rooms
        _smallFloorPlan = CreateFloorPlanWithRooms(10);

        // Medium: 50 rooms
        _mediumFloorPlan = CreateFloorPlanWithRooms(50);

        // Large: 200 rooms
        _largeFloorPlan = CreateFloorPlanWithRooms(200);
    }

    private static FloorPlan CreateFloorPlanWithRooms(int roomCount)
    {
        var plan = new FloorPlan();
        plan.InitSize(new Loc(500, 500));

        var rand = new ReRandom(12345UL);

        // Add rooms in a grid pattern to avoid collisions during setup
        int gridSize = (int)Math.Ceiling(Math.Sqrt(roomCount));
        int cellSize = 500 / gridSize;

        for (int i = 0; i < roomCount; i++)
        {
            int gridX = i % gridSize;
            int gridY = i / gridSize;

            int x = gridX * cellSize + 2;
            int y = gridY * cellSize + 2;
            int width = Math.Min(cellSize - 4, rand.Next(4, 10));
            int height = Math.Min(cellSize - 4, rand.Next(4, 10));

            var roomGen = new RoomGenSquare<BenchmarkMapGenContext>(
                new RandRange(width, width + 1),
                new RandRange(height, height + 1));
            roomGen.PrepareSize(rand, new Loc(width, height));
            roomGen.SetLoc(new Loc(x, y));

            plan.AddRoom(roomGen, new ComponentCollection());
        }

        return plan;
    }

    [Benchmark(Baseline = true)]
    public List<RoomHallIndex> CheckCollision_10Rooms()
    {
        return _smallFloorPlan.CheckCollision(_testRect);
    }

    [Benchmark]
    public List<RoomHallIndex> CheckCollision_50Rooms()
    {
        return _mediumFloorPlan.CheckCollision(_testRect);
    }

    [Benchmark]
    public List<RoomHallIndex> CheckCollision_200Rooms()
    {
        return _largeFloorPlan.CheckCollision(_testRect);
    }
}

/// <summary>
/// Benchmarks for AddRoom collision validation.
/// Each AddRoom call checks against ALL existing rooms.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class AddRoomCollisionBenchmarks
{
    private ReRandom _rand = null!;

    [GlobalSetup]
    public void Setup()
    {
        _rand = new ReRandom(12345UL);
    }

    [Benchmark(Baseline = true)]
    public FloorPlan AddRooms_10()
    {
        return AddRoomsToFloorPlan(10);
    }

    [Benchmark]
    public FloorPlan AddRooms_25()
    {
        return AddRoomsToFloorPlan(25);
    }

    [Benchmark]
    public FloorPlan AddRooms_50()
    {
        return AddRoomsToFloorPlan(50);
    }

    [Benchmark]
    public FloorPlan AddRooms_100()
    {
        return AddRoomsToFloorPlan(100);
    }

    private FloorPlan AddRoomsToFloorPlan(int roomCount)
    {
        var plan = new FloorPlan();
        plan.InitSize(new Loc(500, 500));

        int gridSize = (int)Math.Ceiling(Math.Sqrt(roomCount));
        int cellSize = 500 / gridSize;

        for (int i = 0; i < roomCount; i++)
        {
            int gridX = i % gridSize;
            int gridY = i / gridSize;

            int x = gridX * cellSize + 2;
            int y = gridY * cellSize + 2;
            int width = Math.Min(cellSize - 4, 6);
            int height = Math.Min(cellSize - 4, 6);

            var roomGen = new RoomGenSquare<BenchmarkMapGenContext>(
                new RandRange(width, width + 1),
                new RandRange(height, height + 1));
            roomGen.PrepareSize(_rand, new Loc(width, height));
            roomGen.SetLoc(new Loc(x, y));

            plan.AddRoom(roomGen, new ComponentCollection());
        }

        return plan;
    }
}
