# Grid Tests

## Overview

Tests for grid-based room layout generation using `GridFloorPlan`. Unlike freeform FloorPlans, GridPlans organize rooms in a regular grid where each cell can contain a room and hallways connect adjacent cells. This enables more structured dungeon layouts.

## Test Classes

| Class | Description |
|-------|-------------|
| `GridFloorPlanTest.cs` | Core GridFloorPlan operations: room placement, grid navigation |
| `GridPathBranchTest.cs` | Path branching algorithm for grid-based room placement |
| `TestGridFloorPlan.cs` | Test helper class for grid plan initialization and comparison |
| `IGridPathTestContext.cs` | Interface for grid path test contexts |

## Key Test Patterns

### ASCII Grid Visualization
Tests use a unique string format where:
- Letters (`A`, `B`, `C`...) represent rooms
- `#` represents hallway connections
- `0` represents empty cells
- `.` represents potential connection points

```csharp
string[] inGrid =
{
    "A#B#C",
    "# . .",
    "D.0.0",
};
TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
```

### Floor Plan Comparison
Test helper compares expected vs actual grid states:
```csharp
TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);
TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
```

### Branch Expansion Testing
Tests verify which directions can expand from existing rooms:
```csharp
[Test]
[TestCase(false)]
[TestCase(true)]
public void GetPossibleExpansionsAlone(bool branch)
{
    string[] inGrid =
    {
        "0.0.0",
        ". . .",
        "0.A.0",
        ". . .",
        "0.0.0",
    };

    TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
    List<LocRay4> rays = GridPathBranch<IGridPathTestContext>.GetPossibleExpansions(floorPlan, branch);

    // Verify 4 expansion directions when alone in center
}
```

## Example Test

```csharp
[Test]
public void CreatePath100Percent()
{
    string[] inGrid =
    {
        "0.0.0",
        ". . .",
        "0.0.0",
        ". . .",
        "0.0.0",
    };

    string[] outGrid =
    {
        "A#B#C",
        ". . #",
        "F#E#D",
        "# . .",
        "G#H#I",
    };

    Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
    // Setup random sequence for path decisions...

    var pathGen = new GridPathBranch<IGridPathTestContext>
    {
        RoomRatio = new RandRange(100),
        BranchRatio = new RandRange(0),
    };

    TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
    TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

    pathGen.ApplyToPath(testRand.Object, floorPlan);

    TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
}
```

## Behaviors Tested

### Grid Room Placement
- Adding rooms to specific grid cells
- Grid boundary validation
- Cell occupancy tracking

### Path Generation
- Branching paths with configurable ratios
- Room quota fulfillment (0%, 50%, 100%)
- Forced branching vs. linear paths

### Expansion Logic
- Valid expansion directions from terminals
- Branch point detection
- Corner and edge case handling

### Hallway Connections
- Horizontal and vertical hallway placement
- Connection between adjacent grid cells

## Running Tests

```bash
dotnet test --filter "FullyQualifiedName~GridFloorPlanTest"
dotnet test --filter "FullyQualifiedName~GridPathBranchTest"
```

## Adding Tests

1. Use `TestGridFloorPlan.InitGridToContext()` to create test grids
2. Define expected output using the same string format
3. Mock `IRandom` sequences for deterministic path generation
4. Use `CompareFloorPlans()` to verify results match expectations
5. Test edge cases: corners, edges, full grids, single rooms
