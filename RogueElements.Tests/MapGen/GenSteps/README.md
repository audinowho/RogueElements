# GenSteps Tests

## Overview

Tests for individual generation step implementations (`GenStep<T>`). Each GenStep performs a specific map generation task like spawning items, creating water features, or placing entry/exit points. These tests verify that steps correctly modify map contexts.

## Test Classes

| Class | Description |
|-------|-------------|
| `SpawnerTest.cs` | Tests for item/entity spawning on maps |
| `WaterStepTest.cs` | Tests for water blob placement and terrain modification |
| `PathStepTest.cs` | Tests for path generation steps |
| `IsolatedStepTest.cs` | Tests for handling isolated room scenarios |
| `StartEndStepTest.cs` | Tests for entrance/exit placement (TODO) |

## Key Test Patterns

### Mocking Map Contexts
GenSteps operate on map contexts, which are mocked for testing:
```csharp
Mock<IFloorPlanGenContext> mockContext = new Mock<IFloorPlanGenContext>(MockBehavior.Strict);
mockContext.Setup(p => p.Rand).Returns(testRand.Object);
mockContext.Setup(p => p.RoomPlan).Returns(floorPlan);
```

### SpawnList Testing
Spawners use weighted spawn lists:
```csharp
var spawnList = new SpawnList<ItemSpawn>();
spawnList.Add(new ItemSpawn("potion"), 10);
spawnList.Add(new ItemSpawn("scroll"), 5);

Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
testRand.Setup(p => p.Next(15)).Returns(0); // Always picks "potion"
```

### Verifying Step Application
Tests verify that steps modify maps correctly:
```csharp
[Test]
public void ApplyWaterStep()
{
    // Setup map with specific tiles
    string[] inGrid = { "....", "....", "...." };
    var context = TestGenContext.InitGridToContext(inGrid);

    // Apply water step
    var waterStep = new WaterStep<TestGenContext>(waterTerrain, 50);
    waterStep.Apply(context);

    // Verify water was placed
    Assert.That(context.GetTile(new Loc(1, 1)).ID, Is.EqualTo(waterTerrain.ID));
}
```

## Behaviors Tested

### Spawning
- Weighted random selection from spawn lists
- Spawn count based on room/map size
- Spawn placement validation (not on walls, etc.)

### Water/Terrain
- Water blob placement using noise algorithms
- Terrain modification respecting boundaries
- Percentage-based coverage

### Paths
- Path step application to room plans
- Integration with room generators

### Isolation Handling
- Detection of isolated (unreachable) rooms
- Fallback behavior when rooms can't connect

## Example Test

```csharp
[Test]
public void SpawnItemAtValidLocation()
{
    Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
    testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

    var context = CreateTestContext();
    var spawner = new PickerSpawner<TestContext, Item>(spawnList);

    spawner.Apply(context);

    Assert.That(context.Items.Count, Is.EqualTo(1));
    testRand.Verify(p => p.Next(It.IsAny<int>()), Times.AtLeastOnce);
}
```

## Running Tests

```bash
dotnet test --filter "FullyQualifiedName~SpawnerTest"
dotnet test --filter "FullyQualifiedName~WaterStepTest"
dotnet test --filter "FullyQualifiedName~PathStepTest"
```

## Adding Tests

1. Create a test class matching `{StepName}Test.cs`
2. Mock the appropriate context interface (`IFloorPlanGenContext`, `ITiledGenContext`, etc.)
3. Setup required mocks for `IRandom` and any dependencies
4. Call `Apply()` on the step and verify the context was modified correctly
5. Use `[Ignore("TODO")]` for tests under development
