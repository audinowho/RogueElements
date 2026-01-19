# MapGen Tests

## Overview

This folder contains tests for the map generation pipeline components in RogueElements. The tests are organized into subfolders mirroring the source structure: FloorPlan (freeform room placement), GenSteps (individual generation steps), Grid (grid-based layouts), and Rooms (room shape generators).

## Directory Structure

| Directory | Purpose |
|-----------|---------|
| `FloorPlan/` | Tests for freeform room-based map generation using `FloorPlan` |
| `GenSteps/` | Tests for individual `GenStep` implementations (spawning, paths, water) |
| `Grid/` | Tests for grid-based room layouts using `GridPlan` |
| `Rooms/` | Tests for room shape generators (`RoomGenSquare`, `RoomGenCave`, etc.) |

## Key Test Patterns

### Moq Mocking
All tests heavily use Moq to mock interfaces and verify behavior:
```csharp
Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
testRand.Setup(p => p.Next(60)).Returns(roll);
```

### Deterministic Testing with Seeds
Random behavior is tested using mocked `IRandom` with predetermined sequences:
```csharp
Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(3));
seq = seq.Returns(0);
seq = seq.Returns(1);
```

### String Grid Initialization
Many tests use string arrays to define map/grid states:
```csharp
string[] inGrid =
{
    "A#B",
    ". .",
    "0.0",
};
TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
```

## Running Tests

```bash
# Run all MapGen tests
dotnet test --filter "FullyQualifiedName~RogueElements.Tests" --filter "Namespace~MapGen"

# Run specific subfolder
dotnet test --filter "FullyQualifiedName~RogueElements.Tests.FloorPlanTest"
```

## Adding Tests

1. Create test classes in the appropriate subfolder
2. Use `[TestFixture]` attribute on classes
3. Use `[Test]` for individual tests, `[TestCase]` for parameterized tests
4. Mock `IRandom` for deterministic behavior
5. Use existing test helpers (`TestFloorPlan`, `TestGridFloorPlan`, `TestGenContext`)
