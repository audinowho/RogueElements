// <copyright file="Program.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using BenchmarkDotNet.Running;
using RogueElements.Benchmarks;

// Run all benchmarks when no args provided, or specific ones based on args
if (args.Length == 0)
{
    // Core generation benchmarks
    BenchmarkRunner.Run<MapGenerationBenchmarks>();
    BenchmarkRunner.Run<RoomGenBenchmarks>();
    BenchmarkRunner.Run<RngBenchmarks>();

    // Performance hotspot benchmarks
    BenchmarkRunner.Run<CollisionBenchmarks>();
    BenchmarkRunner.Run<AddRoomCollisionBenchmarks>();
    BenchmarkRunner.Run<SpawnBenchmarks>();
    BenchmarkRunner.Run<FreeTilesBenchmarks>();
    BenchmarkRunner.Run<EraseRoomBenchmarks>();
    BenchmarkRunner.Run<DrawFloorPlanBenchmarks>();
    BenchmarkRunner.Run<AdjacencyLookupBenchmarks>();
    BenchmarkRunner.Run<GridAdjacencyBenchmarks>();
    BenchmarkRunner.Run<GridEraseRoomBenchmarks>();
    BenchmarkRunner.Run<GridToFloorConversionBenchmarks>();
}
else
{
    BenchmarkSwitcher.FromAssembly(typeof(MapGenerationBenchmarks).Assembly).Run(args);
}
