# Rooms Tests

## Overview

Tests for room shape generators (`RoomGen<T>` implementations). Room generators are responsible for proposing room sizes, drawing room tiles onto the map, and managing border connectivity. These tests verify that rooms are generated with correct dimensions, shapes, and connection points.

## Test Classes

| Class | Description |
|-------|-------------|
| `RoomGenTest.cs` | Core `RoomGen` base class behavior: size, borders, connections |
| `RoomGenSquareTest.cs` | Tests for rectangular room generation |
| `RoomGenCaveTest.cs` | Tests for cave/irregular room shapes |
| `TestGenContext.cs` | Test helper implementing `ITiledGenContext` |
| `TestRoomGen.cs` | Test double exposing protected members for verification |

## Key Test Patterns

### String Grid Tile Maps
Tests use `X` for walls and `.` for floor tiles:
```csharp
string[] inGrid =
{
    "XXXXXXXX",
    "XX.....X",
    "XX.....X",
    "XX.....X",
    "XXXXXXXX",
};
TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
```

### Size Proposal Testing
Verifies rooms propose sizes within configured ranges:
```csharp
[Test]
public void ProposeSize()
{
    Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
    testRand.Setup(p => p.Next(3, 5)).Returns(3);
    testRand.Setup(p => p.Next(4, 7)).Returns(4);

    RoomGenSquare<ITiledGenContext> roomGen =
        new RoomGenSquare<ITiledGenContext>(new RandRange(3, 5), new RandRange(4, 7));

    Loc size = roomGen.ProposeSize(testRand.Object);

    Assert.That(size, Is.EqualTo(new Loc(3, 4)));
}
```

### Drawing on Map Testing
Verifies room tiles are placed correctly:
```csharp
[Test]
public void DrawOnMap()
{
    var roomGen = new RoomGenSquare<ITiledGenContext>();
    string[] outGrid =
    {
        "XXXXXXXX",
        "XX.....X",
        "XX.....X",
        "XXXXXXXX",
    };

    TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
    roomGen.PrepareSize(testRand.Object, new Loc(5, 2));
    roomGen.SetLoc(new Loc(2, 1));

    roomGen.DrawOnMap(testContext);

    Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
}
```

### Border Connectivity Testing
Tests room border management for hallway connections:
```csharp
[Test]
public void ReceiveOpenedBorder()
{
    roomGenTo.AskBorderFromRoom(roomGenFrom.Draw, roomGenFrom.GetOpenedBorder, Dir4.Down);

    IntRange newRange = roomGenTo.RoomSideReqs[Dir4.Down][0];
    Assert.That(newRange, Is.EqualTo(new IntRange(expectedStart, expectedEnd)));
}
```

## Behaviors Tested

### Size Management
- Size proposal within RandRange bounds
- Invalid size rejection (zero or negative)
- Size preparation with border arrays

### Tile Drawing
- Correct tile placement for room shape
- Respecting room bounds (Draw.Start, Draw.Size)
- Different room shapes (square, cave, etc.)

### Border Handling
- OpenedBorder: which tiles can connect
- FulfillableBorder: which tiles allow entries
- BorderToFulfill: pending connection requirements
- Border range validation and clamping

### Room Connection
- Receiving border requests from adjacent rooms
- Fulfilling border requirements via digging
- Handling partially blocked borders

## Example Test

```csharp
[Test]
public void FulfillRoomBordersNoneOneMissing()
{
    Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
    testRand.Setup(p => p.Next(4)).Returns(2); // Pick middle tile

    Mock<TestRoomGen<ITiledGenContext>> roomGen =
        new Mock<TestRoomGen<ITiledGenContext>> { CallBase = true };
    roomGen.Setup(p => p.DigAtBorder(It.IsAny<ITiledGenContext>(), It.IsAny<Dir4>(), It.IsAny<int>()));

    // Setup room with unfulfilled border requirement
    roomGen.Object.AskBorderRange(new IntRange(3, 7), Dir4.Down);
    roomGen.Object.FulfillRoomBorders(testContext, false);

    // Verify it dug at the randomly selected tile
    roomGen.Verify(p => p.DigAtBorder(testContext, Dir4.Down, 5), Times.Once());
}
```

## Running Tests

```bash
dotnet test --filter "FullyQualifiedName~RoomGenTest"
dotnet test --filter "FullyQualifiedName~RoomGenSquareTest"
dotnet test --filter "FullyQualifiedName~RoomGenCaveTest"
```

## Adding Tests

1. Use `TestGenContext.InitGridToContext()` for tile map setup
2. Use `TestRoomGen` to access protected members for verification
3. Call `PrepareSize()` and `SetLoc()` before testing draw operations
4. Test edge cases: minimum sizes, corner positions, blocked borders
5. Mark incomplete tests with `[Ignore("TODO")]`
