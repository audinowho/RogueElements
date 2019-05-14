using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;

namespace RogueElements.Tests
{
    [TestFixture]
    public class RoomGenRoundTest
    {

        //TODO: [Test]
        public void ProposeSize()
        {
            //just check for corner cases
            throw new NotImplementedException();
        }

        [Test]
        public void DrawOnMap1x1()
        {
            //normal circle 1x1
            Mock<RoomGenRound<ITiledGenContext>> roomGen = new Mock<RoomGenRound<ITiledGenContext>>() { CallBase = true };
            roomGen.Setup(p => p.SetRoomBorders(It.IsAny<ITiledGenContext>()));
            string[] inGrid =  { "XXXXXXXX",
                                 "XXXXXXXX",
                                 "XXXXXXXX",
                                 "XXXXXXXX",
                                 "XXXXXXXX",
                                 "XXXXXXXX",
                                 "XXXXXXXX" };

            string[] outGrid = {"XXXXXXXX",
                                "XX.XXXXX",
                                "XXXXXXXX",
                                "XXXXXXXX",
                                "XXXXXXXX",
                                "XXXXXXXX",
                                "XXXXXXXX" };
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.Object.PrepareSize(testContext.Rand, new Loc(1, 1));
            roomGen.Object.SetLoc(new Loc(2, 1));

            roomGen.Object.DrawOnMap(testContext);
            
            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            roomGen.Verify(p => p.SetRoomBorders(testContext), Times.Once());
        }

        [Test]
        public void DrawOnMap4x1()
        {
            //normal circle 4x1
            Mock<RoomGenRound<ITiledGenContext>> roomGen = new Mock<RoomGenRound<ITiledGenContext>>() { CallBase = true };
            roomGen.Setup(p => p.SetRoomBorders(It.IsAny<ITiledGenContext>()));
            string[] inGrid =  { "XXXXXXXX",
                                 "XXXXXXXX",
                                 "XXXXXXXX",
                                 "XXXXXXXX",
                                 "XXXXXXXX",
                                 "XXXXXXXX",
                                 "XXXXXXXX" };

            string[] outGrid = {"XXXXXXXX",
                                "XX....XX",
                                "XXXXXXXX",
                                "XXXXXXXX",
                                "XXXXXXXX",
                                "XXXXXXXX",
                                "XXXXXXXX" };
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.Object.PrepareSize(testContext.Rand, new Loc(4, 1));
            roomGen.Object.SetLoc(new Loc(2, 1));

            roomGen.Object.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            roomGen.Verify(p => p.SetRoomBorders(testContext), Times.Once());
        }

        [Test]
        public void DrawOnMap7x7()
        {
            //normal circle 7x7
            Mock<RoomGenRound<ITiledGenContext>> roomGen = new Mock<RoomGenRound<ITiledGenContext>>() { CallBase = true };
            roomGen.Setup(p => p.SetRoomBorders(It.IsAny<ITiledGenContext>()));
            string[] inGrid =  { "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX" };

            string[] outGrid = {"XXXXXXXXXX",
                                "XXXX...XXX",
                                "XXX.....XX",
                                "XX.......X",
                                "XX.......X",
                                "XX.......X",
                                "XXX.....XX",
                                "XXXX...XXX",
                                "XXXXXXXXXX",
                                "XXXXXXXXXX" };
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.Object.PrepareSize(testContext.Rand, new Loc(7, 7));
            roomGen.Object.SetLoc(new Loc(2, 1));

            roomGen.Object.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            roomGen.Verify(p => p.SetRoomBorders(testContext), Times.Once());
        }

        [Test]
        public void DrawOnMap8x8()
        {
            //normal circle 8x8
            Mock<RoomGenRound<ITiledGenContext>> roomGen = new Mock<RoomGenRound<ITiledGenContext>>() { CallBase = true };
            roomGen.Setup(p => p.SetRoomBorders(It.IsAny<ITiledGenContext>()));
            string[] inGrid =  { "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX" };

            string[] outGrid = {"XXXXXXXXXX",
                                "XXX....XXX",
                                "XX......XX",
                                "X........X",
                                "X........X",
                                "X........X",
                                "X........X",
                                "XX......XX",
                                "XXX....XXX",
                                "XXXXXXXXXX" };
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.Object.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.Object.SetLoc(new Loc(1, 1));

            roomGen.Object.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            roomGen.Verify(p => p.SetRoomBorders(testContext), Times.Once());
        }

        [Test]
        public void DrawOnMap7x4()
        {
            //larger height circle 7x4
            Mock<RoomGenRound<ITiledGenContext>> roomGen = new Mock<RoomGenRound<ITiledGenContext>>() { CallBase = true };
            roomGen.Setup(p => p.SetRoomBorders(It.IsAny<ITiledGenContext>()));
            string[] inGrid =  { "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX" };

            string[] outGrid = {"XXXXXXXXXX",
                                "XXX.....XX",
                                "XX.......X",
                                "XX.......X",
                                "XXX.....XX",
                                "XXXXXXXXXX",
                                "XXXXXXXXXX",
                                "XXXXXXXXXX",
                                "XXXXXXXXXX",
                                "XXXXXXXXXX" };
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.Object.PrepareSize(testContext.Rand, new Loc(7, 4));
            roomGen.Object.SetLoc(new Loc(2, 1));

            roomGen.Object.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            roomGen.Verify(p => p.SetRoomBorders(testContext), Times.Once());
        }

        [Test]
        public void DrawOnMap4x7()
        {
            //larger width circle 4x7
            Mock<RoomGenRound<ITiledGenContext>> roomGen = new Mock<RoomGenRound<ITiledGenContext>>() { CallBase = true };
            roomGen.Setup(p => p.SetRoomBorders(It.IsAny<ITiledGenContext>()));
            string[] inGrid =  { "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX",
                                 "XXXXXXXXXX" };

            string[] outGrid = {"XXXXXXXXXX",
                                "XXX..XXXXX",
                                "XX....XXXX",
                                "XX....XXXX",
                                "XX....XXXX",
                                "XX....XXXX",
                                "XX....XXXX",
                                "XXX..XXXXX",
                                "XXXXXXXXXX",
                                "XXXXXXXXXX" };
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.Object.PrepareSize(testContext.Rand, new Loc(4, 7));
            roomGen.Object.SetLoc(new Loc(2, 1));

            roomGen.Object.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            roomGen.Verify(p => p.SetRoomBorders(testContext), Times.Once());
        }

        [Test]
        public void PrepareRequestedBorders1x1()
        {
            //normal circle 1x1
            Mock<IRandom> mockRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGen = new TestRoomGenRound<ITiledGenContext>();
            roomGen.PrepareSize(mockRand.Object, new Loc(1, 1));
            
            var expectedFulfillable = new Dictionary<Dir4, bool[]>();
            expectedFulfillable[Dir4.Down] = new bool[1];
            expectedFulfillable[Dir4.Down][0] = true;
            expectedFulfillable[Dir4.Left] = new bool[1];
            expectedFulfillable[Dir4.Left][0] = true;
            expectedFulfillable[Dir4.Up] = new bool[1];
            expectedFulfillable[Dir4.Up][0] = true;
            expectedFulfillable[Dir4.Right] = new bool[1];
            expectedFulfillable[Dir4.Right][0] = true;

            Assert.That(roomGen.PublicFulfillableBorder, Is.EqualTo(expectedFulfillable));
        }

        [Test]
        public void PrepareRequestedBorders4x1()
        {
            //normal circle 4x1
            Mock<IRandom> mockRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGen = new TestRoomGenRound<ITiledGenContext>();
            roomGen.PrepareSize(mockRand.Object, new Loc(4, 1));

            var expectedFulfillable = new Dictionary<Dir4, bool[]>();
            expectedFulfillable[Dir4.Down] = new bool[4];
            expectedFulfillable[Dir4.Down][0] = true;
            expectedFulfillable[Dir4.Down][1] = true;
            expectedFulfillable[Dir4.Down][2] = true;
            expectedFulfillable[Dir4.Down][3] = true;
            expectedFulfillable[Dir4.Left] = new bool[1];
            expectedFulfillable[Dir4.Left][0] = true;
            expectedFulfillable[Dir4.Up] = new bool[4];
            expectedFulfillable[Dir4.Up][0] = true;
            expectedFulfillable[Dir4.Up][1] = true;
            expectedFulfillable[Dir4.Up][2] = true;
            expectedFulfillable[Dir4.Up][3] = true;
            expectedFulfillable[Dir4.Right] = new bool[1];
            expectedFulfillable[Dir4.Right][0] = true;

            Assert.That(roomGen.PublicFulfillableBorder, Is.EqualTo(expectedFulfillable));
        }

        [Test]
        public void PrepareRequestedBorders7x7()
        {
            //normal circle 7x7
            //normal circle 6x6
            //larger width circle 4x8
            //larger height circle 8x4
            Mock<IRandom> mockRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGen = new TestRoomGenRound<ITiledGenContext>();
            roomGen.PrepareSize(mockRand.Object, new Loc(7, 7));

            var expectedFulfillable = new Dictionary<Dir4, bool[]>();
            expectedFulfillable[Dir4.Down] = new bool[7];
            expectedFulfillable[Dir4.Down][0] = false;
            expectedFulfillable[Dir4.Down][1] = false;
            expectedFulfillable[Dir4.Down][2] = true;
            expectedFulfillable[Dir4.Down][3] = true;
            expectedFulfillable[Dir4.Down][4] = true;
            expectedFulfillable[Dir4.Down][5] = false;
            expectedFulfillable[Dir4.Down][6] = false;
            expectedFulfillable[Dir4.Left] = new bool[7];
            expectedFulfillable[Dir4.Left][0] = false;
            expectedFulfillable[Dir4.Left][1] = false;
            expectedFulfillable[Dir4.Left][2] = true;
            expectedFulfillable[Dir4.Left][3] = true;
            expectedFulfillable[Dir4.Left][4] = true;
            expectedFulfillable[Dir4.Left][5] = false;
            expectedFulfillable[Dir4.Left][6] = false;
            expectedFulfillable[Dir4.Up] = new bool[7];
            expectedFulfillable[Dir4.Up][0] = false;
            expectedFulfillable[Dir4.Up][1] = false;
            expectedFulfillable[Dir4.Up][2] = true;
            expectedFulfillable[Dir4.Up][3] = true;
            expectedFulfillable[Dir4.Up][4] = true;
            expectedFulfillable[Dir4.Up][5] = false;
            expectedFulfillable[Dir4.Up][6] = false;
            expectedFulfillable[Dir4.Right] = new bool[7];
            expectedFulfillable[Dir4.Right][0] = false;
            expectedFulfillable[Dir4.Right][1] = false;
            expectedFulfillable[Dir4.Right][2] = true;
            expectedFulfillable[Dir4.Right][3] = true;
            expectedFulfillable[Dir4.Right][4] = true;
            expectedFulfillable[Dir4.Right][5] = false;
            expectedFulfillable[Dir4.Right][6] = false;

            Assert.That(roomGen.PublicFulfillableBorder, Is.EqualTo(expectedFulfillable));
        }


        [Test]
        public void PrepareRequestedBorders8x8()
        {
            //normal circle 8x8
            //larger width circle 4x8
            //larger height circle 8x4
            Mock<IRandom> mockRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGen = new TestRoomGenRound<ITiledGenContext>();
            roomGen.PrepareSize(mockRand.Object, new Loc(8, 8));

            var expectedFulfillable = new Dictionary<Dir4, bool[]>();
            expectedFulfillable[Dir4.Down] = new bool[8];
            expectedFulfillable[Dir4.Down][0] = false;
            expectedFulfillable[Dir4.Down][1] = false;
            expectedFulfillable[Dir4.Down][2] = true;
            expectedFulfillable[Dir4.Down][3] = true;
            expectedFulfillable[Dir4.Down][4] = true;
            expectedFulfillable[Dir4.Down][5] = true;
            expectedFulfillable[Dir4.Down][6] = false;
            expectedFulfillable[Dir4.Down][7] = false;
            expectedFulfillable[Dir4.Left] = new bool[8];
            expectedFulfillable[Dir4.Left][0] = false;
            expectedFulfillable[Dir4.Left][1] = false;
            expectedFulfillable[Dir4.Left][2] = true;
            expectedFulfillable[Dir4.Left][3] = true;
            expectedFulfillable[Dir4.Left][4] = true;
            expectedFulfillable[Dir4.Left][5] = true;
            expectedFulfillable[Dir4.Left][6] = false;
            expectedFulfillable[Dir4.Left][7] = false;
            expectedFulfillable[Dir4.Up] = new bool[8];
            expectedFulfillable[Dir4.Up][0] = false;
            expectedFulfillable[Dir4.Up][1] = false;
            expectedFulfillable[Dir4.Up][2] = true;
            expectedFulfillable[Dir4.Up][3] = true;
            expectedFulfillable[Dir4.Up][4] = true;
            expectedFulfillable[Dir4.Up][5] = true;
            expectedFulfillable[Dir4.Up][6] = false;
            expectedFulfillable[Dir4.Up][7] = false;
            expectedFulfillable[Dir4.Right] = new bool[8];
            expectedFulfillable[Dir4.Right][0] = false;
            expectedFulfillable[Dir4.Right][1] = false;
            expectedFulfillable[Dir4.Right][2] = true;
            expectedFulfillable[Dir4.Right][3] = true;
            expectedFulfillable[Dir4.Right][4] = true;
            expectedFulfillable[Dir4.Right][5] = true;
            expectedFulfillable[Dir4.Right][6] = false;
            expectedFulfillable[Dir4.Right][7] = false;

            Assert.That(roomGen.PublicFulfillableBorder, Is.EqualTo(expectedFulfillable));
        }


        [Test]
        public void PrepareRequestedBorders4x7()
        {
            //larger height circle 4x7
            //larger width circle 7x4
            Mock<IRandom> mockRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGen = new TestRoomGenRound<ITiledGenContext>();
            roomGen.PrepareSize(mockRand.Object, new Loc(4, 7));

            var expectedFulfillable = new Dictionary<Dir4, bool[]>();
            expectedFulfillable[Dir4.Down] = new bool[4];
            expectedFulfillable[Dir4.Down][0] = false;
            expectedFulfillable[Dir4.Down][1] = true;
            expectedFulfillable[Dir4.Down][2] = true;
            expectedFulfillable[Dir4.Down][3] = false;
            expectedFulfillable[Dir4.Left] = new bool[7];
            expectedFulfillable[Dir4.Left][0] = false;
            expectedFulfillable[Dir4.Left][1] = true;
            expectedFulfillable[Dir4.Left][2] = true;
            expectedFulfillable[Dir4.Left][3] = true;
            expectedFulfillable[Dir4.Left][4] = true;
            expectedFulfillable[Dir4.Left][5] = true;
            expectedFulfillable[Dir4.Left][6] = false;
            expectedFulfillable[Dir4.Up] = new bool[4];
            expectedFulfillable[Dir4.Up][0] = false;
            expectedFulfillable[Dir4.Up][1] = true;
            expectedFulfillable[Dir4.Up][2] = true;
            expectedFulfillable[Dir4.Up][3] = false;
            expectedFulfillable[Dir4.Right] = new bool[7];
            expectedFulfillable[Dir4.Right][0] = false;
            expectedFulfillable[Dir4.Right][1] = true;
            expectedFulfillable[Dir4.Right][2] = true;
            expectedFulfillable[Dir4.Right][3] = true;
            expectedFulfillable[Dir4.Right][4] = true;
            expectedFulfillable[Dir4.Right][5] = true;
            expectedFulfillable[Dir4.Right][6] = false;

            Assert.That(roomGen.PublicFulfillableBorder, Is.EqualTo(expectedFulfillable));
        }


        [Test]
        public void PrepareRequestedBorders7x4()
        {
            //larger width circle 7x4
            Mock<IRandom> mockRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGen = new TestRoomGenRound<ITiledGenContext>();
            roomGen.PrepareSize(mockRand.Object, new Loc(7, 4));

            var expectedFulfillable = new Dictionary<Dir4, bool[]>();
            expectedFulfillable[Dir4.Down] = new bool[7];
            expectedFulfillable[Dir4.Down][0] = false;
            expectedFulfillable[Dir4.Down][1] = true;
            expectedFulfillable[Dir4.Down][2] = true;
            expectedFulfillable[Dir4.Down][3] = true;
            expectedFulfillable[Dir4.Down][4] = true;
            expectedFulfillable[Dir4.Down][5] = true;
            expectedFulfillable[Dir4.Down][6] = false;
            expectedFulfillable[Dir4.Left] = new bool[4];
            expectedFulfillable[Dir4.Left][0] = false;
            expectedFulfillable[Dir4.Left][1] = true;
            expectedFulfillable[Dir4.Left][2] = true;
            expectedFulfillable[Dir4.Left][3] = false;
            expectedFulfillable[Dir4.Up] = new bool[7];
            expectedFulfillable[Dir4.Up][0] = false;
            expectedFulfillable[Dir4.Up][1] = true;
            expectedFulfillable[Dir4.Up][2] = true;
            expectedFulfillable[Dir4.Up][3] = true;
            expectedFulfillable[Dir4.Up][4] = true;
            expectedFulfillable[Dir4.Up][5] = true;
            expectedFulfillable[Dir4.Up][6] = false;
            expectedFulfillable[Dir4.Right] = new bool[4];
            expectedFulfillable[Dir4.Right][0] = false;
            expectedFulfillable[Dir4.Right][1] = true;
            expectedFulfillable[Dir4.Right][2] = true;
            expectedFulfillable[Dir4.Right][3] = false;

            Assert.That(roomGen.PublicFulfillableBorder, Is.EqualTo(expectedFulfillable));
        }
    }



    public class TestRoomGenRound<T> : RoomGenRound<T> where T : ITiledGenContext
    {
        public Dictionary<Dir4, bool[]> PublicFulfillableBorder { get { return fulfillableBorder; } }
    }
}
