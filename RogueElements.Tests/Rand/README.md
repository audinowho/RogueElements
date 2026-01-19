# Rand Tests

## Overview

Tests for random number generation utilities, weighted spawn lists, and noise generation. These components are fundamental to procedural generation, ensuring randomness is properly weighted, seeded, and reproducible for testing.

## Test Classes

| Class | Description |
|-------|-------------|
| `SpawnListTest.cs` | Tests for weighted random selection from `SpawnList<T>` |
| `SpawnRangeListTest.cs` | Tests for level-range-based spawn lists |
| `RandRangeTest.cs` | Tests for `RandRange`, `LoopedRand`, `RandBag`, etc. |
| `NoiseGenTest.cs` | Tests for cellular automata and noise generation |

## Key Test Patterns

### Weighted Spawn Selection
Tests verify correct weighted random selection:
```csharp
[SetUp]
public void SpawnListSetUp()
{
    this.spawnList = new SpawnList<string>
    {
        { "apple", 10 },   // 10/60 = 16.7%
        { "orange", 20 },  // 20/60 = 33.3%
        { "banana", 30 },  // 30/60 = 50%
    };
}

[Test]
[TestCase(0, 0)]   // Roll 0-9 -> apple (index 0)
[TestCase(9, 0)]   // Roll 9 -> still apple
[TestCase(10, 1)]  // Roll 10-29 -> orange (index 1)
[TestCase(30, 2)]  // Roll 30-59 -> banana (index 2)
public void SpawnListChooseIndex(int roll, int result)
{
    Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
    testRand.Setup(p => p.Next(60)).Returns(roll);

    Assert.That(this.spawnList.PickIndex(testRand.Object), Is.EqualTo(result));
}
```

### Level-Range Spawn Lists
Tests for spawns valid only in specific level ranges:
```csharp
[SetUp]
public void SpawnListSetUp()
{
    this.spawnRangeList = new SpawnRangeList<string>();
    this.spawnRangeList.Add("apple", new IntRange(0, 5), 10);
    this.spawnRangeList.Add("orange", new IntRange(3, 9), 20);
}

[Test]
[TestCase(-1, false)]  // Before first range
[TestCase(0, true)]    // Apple valid
[TestCase(3, true)]    // Both valid
[TestCase(5, true)]    // Only orange
[TestCase(9, false)]   // After all ranges
public void SpawnRangeListCanPick(int level, bool result)
{
    Assert.That(this.spawnRangeList.CanPick(level), Is.EqualTo(result));
}
```

### Cellular Automata Noise
Tests for iterative automata with cell rules:
```csharp
[Test]
[TestCase(CellRule.None, false)]
[TestCase(CellRule.All, true)]
[TestCase(CellRule.Eq0, true)]    // 0 neighbors = true
[TestCase(CellRule.Lt2, true)]    // <2 neighbors = true
[TestCase(CellRule.Gte1, false)]  // >=1 neighbors = false (has 0)
public void IterateAutomataSingle0(CellRule rule, bool expected)
{
    string[] inGrid =
    {
        "XXX",
        "X.X",  // Center cell has 0 open neighbors
        "XXX",
    };

    bool[][] map = GridTest.InitBoolGrid(inGrid);
    bool[][] result = NoiseGen.IterateAutomata(map, CellRule.None, rule, 1);

    Assert.That(result[1][1], Is.EqualTo(expected));
}
```

## Behaviors Tested

### SpawnList
- **Weighted Selection**: Correct probability distribution
- **Total Calculation**: `SpawnTotal` matches sum of weights
- **Index Selection**: Edge cases at weight boundaries
- **Rate Modification**: Changing spawn rates updates totals
- **Removal**: Removing items updates weights correctly

### SpawnRangeList
- **Range Validation**: Items only spawn within their IntRange
- **Overlapping Ranges**: Multiple items valid at same level
- **Error Handling**: Invalid ranges throw ArgumentException

### RandRange Utilities
- **LoopedRand**: Generate multiple values from a range
- **PresetPicker**: Always return predetermined value
- **RandBag**: Pick without replacement
- **RandBinomial**: Binomial distribution with offset

### NoiseGen
- **Cellular Automata**: Cell rules (Eq0, Lt2, Gte4, etc.)
- **Iteration Count**: 0, 1, 2+ iterations shrink/grow areas
- **Neighbor Counting**: All 8 neighbors influence decisions

## Example Tests

```csharp
// RandBag: pick without replacement
[Test]
public void RandBagRemovable()
{
    RandBag<int> testPicker = new RandBag<int>(true, new List<int> { 8, 5, 2, 3, 4, 1, 6, 7 });
    Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
    testRand.Setup(p => p.Next(8)).Returns(3);  // Pick item at index 3

    Assert.That(testPicker.Pick(testRand.Object), Is.EqualTo(3));
    // Item removed, next pick from remaining 7 items
}

// Cellular automata iteration
[Test]
public void IterateAutomataIterations2()
{
    string[] inGrid = { "XXXXX", "X...X", "X...X", "X...X", "XXXXX" };
    string[] outGrid = { "XXXXX", "XXXXX", "XX.XX", "XXXXX", "XXXXX" };

    bool[][] map = GridTest.InitBoolGrid(inGrid);
    bool[][] compare = GridTest.InitBoolGrid(outGrid);

    bool[][] result = NoiseGen.IterateAutomata(map, CellRule.None, CellRule.Gte4, 2);

    Assert.That(result, Is.EqualTo(compare));
}
```

## Running Tests

```bash
dotnet test --filter "FullyQualifiedName~SpawnListTest"
dotnet test --filter "FullyQualifiedName~SpawnRangeListTest"
dotnet test --filter "FullyQualifiedName~RandRangeTest"
dotnet test --filter "FullyQualifiedName~NoiseGenTest"
```

## Adding Tests

1. Use `Mock<IRandom>` to control random outcomes
2. Test boundary conditions (first/last item, empty lists)
3. Verify weights sum correctly after modifications
4. Use `[TestCase]` for parameterized probability tests
5. Mark incomplete tests with `[Ignore("TODO")]`
