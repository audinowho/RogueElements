# Performance Optimization Suggestions

Performance audit conducted 2025-12-31. Benchmarks added to measure hotspots.

## Executive Summary

**Current performance is excellent.** A large 80-room map generates in ~0.5ms. These suggestions are for future reference if scaling to very large maps (200+ rooms) or bulk generation (100+ floors at startup).

---

## Quick Wins

### 1. SwapPop for Spawn Distribution

**File:** `RogueElements/MapGen/Spawning/RandomSpawnStep.cs:54`

**Current code:**
```csharp
freeTiles.RemoveAt(randIndex);  // O(n) - shifts all elements after index
```

**Suggested fix:**
```csharp
// O(1) removal: swap with last element, then remove last
int lastIndex = freeTiles.Count - 1;
if (randIndex != lastIndex)
{
    freeTiles[randIndex] = freeTiles[lastIndex];
}
freeTiles.RemoveAt(lastIndex);
```

**Benchmark result:** 21% faster for 100 items (908µs → 715µs)

**When it matters:** Games spawning 500+ entities per floor

---

## Medium-Term Improvements

### 2. Spatial Indexing for Collision Detection

**Files:**
- `RogueElements/MapGen/FloorPlan/FloorPlan.cs:310-320` (AddRoom)
- `RogueElements/MapGen/FloorPlan/FloorPlan.cs:757-776` (CheckCollision)

**Current behavior:** Linear scan through all rooms/halls for every collision check - O(n) per check, O(n²) total for n rooms.

**Suggested approach:** Implement grid-based spatial hashing or quad-tree.

```csharp
// Example: Grid-based spatial hash
private Dictionary<(int, int), List<int>> _spatialGrid;
private const int CellSize = 32;

private (int, int) GetCell(Loc loc) => (loc.X / CellSize, loc.Y / CellSize);

public List<RoomHallIndex> CheckCollision(Rect rect)
{
    var results = new List<RoomHallIndex>();
    // Only check rooms in overlapping cells instead of all rooms
    for (int x = rect.X / CellSize; x <= rect.End.X / CellSize; x++)
    {
        for (int y = rect.Y / CellSize; y <= rect.End.Y / CellSize; y++)
        {
            if (_spatialGrid.TryGetValue((x, y), out var candidates))
            {
                // Check only candidates in this cell
            }
        }
    }
    return results;
}
```

**Estimated improvement:** O(n) → O(1) average case for collision checks

**When it matters:** Maps with 200+ rooms, or algorithms that attempt many placements

---

### 3. Deferred Room Erasure Index Updates

**File:** `RogueElements/MapGen/FloorPlan/FloorPlan.cs:386-424`

**Current behavior:** After removing a room/hall, scans ALL rooms and ALL halls to update adjacency indices. O(n) per erasure.

**Suggested approach:** Batch erasures and update indices once, or use stable IDs instead of array indices.

**When it matters:** Algorithms that remove many rooms during generation

---

## Not Worth Implementing

### HashSet for GetAdjacentRooms

**File:** `RogueElements/MapGen/Grid/GridPlan.cs:837-868`

**Benchmark result:** HashSet was actually 12% *slower* (45ns vs 41ns) due to overhead. For typical room sizes with ~4 neighbors, List.Contains is faster.

**Recommendation:** Keep current implementation.

---

## Benchmark Coverage

The following benchmarks were added to measure these hotspots:

| Benchmark Class | File | What It Measures |
|-----------------|------|------------------|
| `CollisionBenchmarks` | CollisionBenchmarks.cs | FloorPlan.CheckCollision scaling |
| `AddRoomCollisionBenchmarks` | CollisionBenchmarks.cs | Cumulative AddRoom cost |
| `SpawnBenchmarks` | SpawnBenchmarks.cs | RemoveAt vs SwapPop comparison |
| `FreeTilesBenchmarks` | SpawnBenchmarks.cs | GetAllFreeTiles allocation |
| `EraseRoomBenchmarks` | FloorPlanBenchmarks.cs | Room erasure cost |
| `DrawFloorPlanBenchmarks` | FloorPlanBenchmarks.cs | DrawOnMap performance |
| `AdjacencyLookupBenchmarks` | FloorPlanBenchmarks.cs | FloorPlan adjacency iteration |
| `GridAdjacencyBenchmarks` | GridPlanBenchmarks.cs | List vs HashSet for adjacency |
| `GridEraseRoomBenchmarks` | GridPlanBenchmarks.cs | Grid room erasure |
| `GridToFloorConversionBenchmarks` | GridPlanBenchmarks.cs | PlaceRoomsOnFloor |

### Running Benchmarks

```bash
# Run all benchmarks
dotnet run --project RogueElements.Benchmarks -c Release

# Run specific benchmark
dotnet run --project RogueElements.Benchmarks -c Release -- --filter "*SpawnBenchmarks*"

# Quick dry run
dotnet run --project RogueElements.Benchmarks -c Release -- --filter "*Collision*" --job Dry
```

---

## Baseline Results (2025-12-31)

**Hardware:** Apple M3 Pro, .NET 10.0.1

### Map Generation (End-to-End)

| Map Size | Rooms | Time | Memory |
|----------|-------|------|--------|
| Small (4×3) | 12 | 0.04 ms | 152 KB |
| Medium (6×4) | 24 | 0.10 ms | 378 KB |
| Large (10×8) | 80 | 0.55 ms | 1.5 MB |

### AddRoom Collision Scaling

| Rooms | Time | Ratio |
|-------|------|-------|
| 10 | 4.6 µs | 1.0x |
| 25 | 13.8 µs | 3.0x |
| 50 | 33.8 µs | 7.4x |
| 100 | 75.9 µs | 16.6x |

### Spawn Distribution

| Items | RemoveAt | SwapPop | Improvement |
|-------|----------|---------|-------------|
| 10 | 32.2 µs | 30.9 µs | 4% |
| 50 | 148.9 µs | 131.4 µs | 12% |
| 100 | 908.3 µs | 715.1 µs | 21% |

---

## When to Revisit

Consider implementing optimizations if:

1. Users report slow map generation
2. Targeting maps with 200+ rooms
3. Pre-generating 100+ floors at startup
4. Targeting low-end mobile devices
5. Adding real-time streaming generation

Until then, the library performs well for typical roguelike use cases.
