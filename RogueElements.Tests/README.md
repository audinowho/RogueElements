# RogueElements.Tests

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![NUnit](https://img.shields.io/badge/NUnit-3.10-green.svg)](https://nunit.org/)
[![Moq](https://img.shields.io/badge/Moq-4.8-blue.svg)](https://github.com/moq/moq4)

Unit tests for the RogueElements library using NUnit and Moq. Tests mirror the source structure and verify core algorithms, data structures, and generation logic.

## Overview

This test project ensures the reliability of RogueElements through comprehensive unit tests covering:

- Grid operations and pathfinding algorithms
- Random number generation and weighted selection
- Collision detection and shape algorithms
- Room generation and floor plan construction
- Priority queue behavior

## Running Tests

```bash
# From repository root
dotnet test RogueElements.Tests/RogueElements.Tests.csproj

# With detailed output
dotnet test RogueElements.Tests/RogueElements.Tests.csproj --logger "console;verbosity=detailed"

# Run specific test class
dotnet test --filter "FullyQualifiedName~GridTest"

# Run specific test method
dotnet test --filter "FullyQualifiedName~GridTest.FindAPathStraight"
```

## Testing Philosophy

### Deterministic Testing

Tests use deterministic seeds or mocked random sources to ensure reproducibility:

```csharp
[Test]
public void SpawnListPickMultiple()
{
    // Use controlled randomness via Moq
    var mockRand = new Mock<IRandom>(MockBehavior.Strict);
    mockRand.Setup(r => r.Next(It.IsAny<int>())).Returns(0);

    var spawnList = new SpawnList<int> { { 1, 10 }, { 2, 20 } };
    int result = spawnList.Pick(mockRand.Object);

    Assert.That(result, Is.EqualTo(1));
}
```

### Visual Test Grids

Many tests use ASCII grid representations for clarity:

```csharp
[Test]
public void FindAPathCurved()
{
    string[] inGrid =
    {
        ".......",
        ".......",
        ".......",
        ".XXXXX.",
        "...X...",
        ".A.X.B.",
        "...X...",
    };

    char[][] map = InitGrid(inGrid);
    // Test pathfinding from A to B around obstacles X
    List<Loc> path = Grid.FindAPath(...);
    Assert.That(path, Is.EquivalentTo(expectedPath));
}
```

### Boundary Conditions

Tests cover edge cases and boundary conditions:

```csharp
[Test]
public void SpawnListPickEmpty()
{
    var spawnList = new SpawnList<int>();
    Assert.Throws<InvalidOperationException>(() => spawnList.Pick(rand));
}
```

## Test Structure

Tests mirror the source directory structure:

```
RogueElements.Tests/
├── MapGen/
│   ├── FloorPlan/          # FloorPlan algorithm tests
│   │   ├── FloorPlanTest.cs
│   │   ├── PathTest.cs
│   │   └── ...
│   ├── Grid/               # GridPlan tests
│   │   ├── GridPlanTest.cs
│   │   └── ...
│   ├── Rooms/              # Room generation tests
│   │   ├── RoomGenTest.cs
│   │   └── ...
│   └── GenSteps/           # Individual GenStep tests
│       └── ...
├── Rand/                   # RNG utility tests
│   ├── RandRangeTest.cs
│   ├── SpawnListTest.cs
│   ├── SpawnListExceptionTest.cs
│   └── SpawnRangeListTest.cs
├── Priority/               # Priority queue tests
│   └── PriorityListTest.cs
├── GridTest.cs             # Core Grid algorithm tests
├── CollisionTest.cs        # Collision detection tests
├── DetectionTest.cs        # Shape detection tests
├── DirExtTest.cs           # Direction extension tests
├── MathUtilsTest.cs        # Math utility tests
├── NoiseGenTest.cs         # Perlin noise tests
├── RectTest.cs             # Rectangle operation tests
├── TypeDictTest.cs         # TypeDict tests
└── WrappedCollisionTest.cs # Wrapped collision tests
```

## Key Test Files

### GridTest.cs

Tests grid algorithms including pathfinding:

```csharp
[TestFixture]
public class GridTest
{
    [Test]
    public void FindAPathStraight()
    {
        // Tests A* pathfinding in open space
    }

    [Test]
    public void FindAPathCurved()
    {
        // Tests pathfinding around obstacles
    }

    [Test]
    public void FloodFill()
    {
        // Tests flood fill algorithm
    }
}
```

### SpawnListTest.cs

Tests weighted random selection:

```csharp
[TestFixture]
public class SpawnListTest
{
    [Test]
    public void SpawnListAdd() { }

    [Test]
    public void SpawnListGetSpawnRate() { }

    [Test]
    public void SpawnListPick() { }

    [Test]
    public void SpawnListPickMultiple() { }
}
```

### DetectionTest.cs

Tests shape detection and blob analysis:

```csharp
[TestFixture]
public class DetectionTest
{
    [Test]
    public void DetectBlobs() { }

    [Test]
    public void FindConnectedTiles() { }

    [Test]
    public void FindHoles() { }
}
```

## Adding New Tests

### 1. Create Test Class

Create a new test file mirroring the source structure:

```csharp
using System;
using NUnit.Framework;
using Moq;

namespace RogueElements.Tests
{
    [TestFixture]
    public class MyFeatureTest
    {
        [SetUp]
        public void Setup()
        {
            // Test initialization
        }

        [TearDown]
        public void TearDown()
        {
            // Cleanup
        }

        [Test]
        public void MyFeature_WhenCondition_ShouldBehavior()
        {
            // Arrange
            var input = PrepareInput();

            // Act
            var result = MyFeature.Process(input);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
```

### 2. Use Test Helpers

Leverage existing test utilities:

```csharp
// Convert string grid to char array
char[][] map = GridTest.InitGrid(new[]
{
    "###",
    "#.#",
    "###"
});

// Convert to bool grid (. = true, # = false)
bool[][] boolMap = GridTest.InitBoolGrid(inGrid);

// Convert to int grid (A=0, B=1, C=2, ...)
int[][] intMap = GridTest.InitIntGrid(inGrid);
```

### 3. Mock Dependencies

Use Moq for isolating units:

```csharp
[Test]
public void GenStep_AppliesCorrectly()
{
    // Mock the context
    var mockContext = new Mock<ITiledGenContext>();
    mockContext.Setup(c => c.Width).Returns(10);
    mockContext.Setup(c => c.Height).Returns(10);
    mockContext.Setup(c => c.Rand).Returns(new ReRandom(12345));

    // Create and apply step
    var step = new MyGenStep();
    step.Apply(mockContext.Object);

    // Verify interactions
    mockContext.Verify(c => c.SetTile(It.IsAny<Loc>(), It.IsAny<ITile>()), Times.AtLeastOnce);
}
```

### 4. Test Naming Conventions

Follow the pattern: `MethodName_Condition_ExpectedBehavior`

```csharp
[Test]
public void FindPath_WithBlockedRoute_ReturnsEmpty() { }

[Test]
public void SpawnList_WhenEmpty_ThrowsException() { }

[Test]
public void GridPlan_AddRoom_UpdatesCellCount() { }
```

## Test Data Patterns

### ASCII Grid Pattern

```csharp
string[] input =
{
    "#####",
    "#...#",
    "#.#.#",
    "#...#",
    "#####"
};
// # = wall/blocked
// . = floor/open
// A-Z = special markers
// X = obstacle
```

### Expected Result Verification

```csharp
List<Loc> expectedPath = new List<Loc>
{
    new Loc(5, 5),
    new Loc(4, 5),
    new Loc(3, 5),
    new Loc(2, 5),
    new Loc(1, 5)
};

Assert.That(actualPath, Is.EquivalentTo(expectedPath));
```

## Continuous Integration

Tests run on every commit via Travis CI:

```yaml
# .travis.yml
script:
  - dotnet build RogueElements.sln
  - dotnet test RogueElements.Tests/RogueElements.Tests.csproj
```

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| NUnit | 3.10.1 | Test framework |
| NUnit3TestAdapter | 3.10.0 | VS Test adapter |
| Microsoft.NET.Test.Sdk | 15.8.0 | Test SDK |
| Moq | 4.8.0 | Mocking framework |
| StyleCop.Analyzers | 1.1.118 | Code style enforcement |
| CodeCracker.CSharp | 1.1.0 | Code analysis |

## See Also

- **[RogueElements/](../RogueElements/)** - Core library source
- **[RogueElements.Examples/](../RogueElements.Examples/)** - Usage examples
- **[CLAUDE.md](../CLAUDE.md)** - Full architecture documentation

---

![Repobeats analytics](https://repobeats.axiom.co/api/embed/placeholder.svg "Repobeats analytics image")
