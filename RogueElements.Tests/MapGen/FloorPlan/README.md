# FloorPlan Tests

## Overview

Tests for freeform room-based map generation using `FloorPlan`. This covers adding rooms to floor plans, connecting rooms via hallways, and managing room adjacencies. The FloorPlan system allows arbitrary room placement without grid constraints.

## Test Classes

| Class | Description |
|-------|-------------|
| `FloorPlanTest.cs` | Core FloorPlan operations: room addition, adjacency, room drawing |
| `AddRoomTest.cs` | Tests for `AddRoom` operations and collision detection |
| `ConnectTest.cs` | Tests for hallway connections between rooms |
| `FloorPathBranchTest.cs` | Path branching algorithm tests for room placement |
| `TestFloorPlan.cs` | Test helper class implementing `IFloorPlanTestContext` |
| `IFloorPlanTestContext.cs` | Interface for floor plan test contexts |

## Key Test Patterns

### String Grid Visualization
Tests use ASCII grids to define room layouts with letters representing rooms and `#` for connections:
```csharp
string[] inGrid =
{
    "A#B#C",
    ". # .",
    "0#D.0",
};
TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(inGrid);
```

### Room Collision Testing
Tests verify rooms don't overlap and respect boundaries:
```csharp
[Test]
public void AddRoomCollide()
{
    // Tests that overlapping rooms throw exceptions
}
```

### Connection Testing
Verifies hallway connections between adjacent rooms:
```csharp
[Test]
public void ConnectRooms()
{
    // Tests bidirectional connections between rooms
    // Verifies adjacency lists are updated correctly
}
```

### Mocking Room Generators
Room generators are mocked to return specific room shapes:
```csharp
Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>> mockRooms =
    new Mock<IRandPicker<RoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
mockRooms.Setup(p => p.Pick(testRand.Object)).Returns(new TestFloorRoomGen('A'));
```

## Example Test

```csharp
[Test]
public void AddRoomToEmptyPlan()
{
    Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
    var floorPlan = new TestFloorPlan();
    var roomGen = new TestFloorRoomGen('A');

    floorPlan.AddRoom(roomGen.Draw, roomGen);

    Assert.That(floorPlan.RoomCount, Is.EqualTo(1));
    Assert.That(floorPlan.GetRoomGen(0).Draw, Is.EqualTo(roomGen.Draw));
}
```

## Behaviors Tested

- **Room Addition**: Adding rooms at valid/invalid positions
- **Collision Detection**: Preventing overlapping rooms
- **Adjacency Management**: Tracking which rooms connect
- **Border Handling**: Managing room borders and entry points
- **Path Generation**: Creating branching paths of rooms

## Running Tests

```bash
dotnet test --filter "FullyQualifiedName~FloorPlanTest"
dotnet test --filter "FullyQualifiedName~AddRoomTest"
dotnet test --filter "FullyQualifiedName~ConnectTest"
```

## Adding Tests

1. Use `TestFloorPlan` and `TestFloorRoomGen` helper classes
2. Initialize plans with `InitFloorToContext()` string arrays
3. Mock `IRandom` for deterministic random behavior
4. Verify room counts, adjacencies, and positions after operations
