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
            
            bool[][] expectedFulfillable = new bool[4][];
            expectedFulfillable[0] = new bool[1];
            expectedFulfillable[0][0] = true;
            expectedFulfillable[1] = new bool[1];
            expectedFulfillable[1][0] = true;
            expectedFulfillable[2] = new bool[1];
            expectedFulfillable[2][0] = true;
            expectedFulfillable[3] = new bool[1];
            expectedFulfillable[3][0] = true;

            Assert.That(roomGen.PublicFulfillableBorder, Is.EqualTo(expectedFulfillable));
        }

        [Test]
        public void PrepareRequestedBorders4x1()
        {
            //normal circle 4x1
            Mock<IRandom> mockRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGen = new TestRoomGenRound<ITiledGenContext>();
            roomGen.PrepareSize(mockRand.Object, new Loc(4, 1));

            bool[][] expectedFulfillable = new bool[4][];
            expectedFulfillable[0] = new bool[4];
            expectedFulfillable[0][0] = true;
            expectedFulfillable[0][1] = true;
            expectedFulfillable[0][2] = true;
            expectedFulfillable[0][3] = true;
            expectedFulfillable[1] = new bool[1];
            expectedFulfillable[1][0] = true;
            expectedFulfillable[2] = new bool[4];
            expectedFulfillable[2][0] = true;
            expectedFulfillable[2][1] = true;
            expectedFulfillable[2][2] = true;
            expectedFulfillable[2][3] = true;
            expectedFulfillable[3] = new bool[1];
            expectedFulfillable[3][0] = true;

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

            bool[][] expectedFulfillable = new bool[4][];
            expectedFulfillable[0] = new bool[7];
            expectedFulfillable[0][0] = false;
            expectedFulfillable[0][1] = false;
            expectedFulfillable[0][2] = true;
            expectedFulfillable[0][3] = true;
            expectedFulfillable[0][4] = true;
            expectedFulfillable[0][5] = false;
            expectedFulfillable[0][6] = false;
            expectedFulfillable[1] = new bool[7];
            expectedFulfillable[1][0] = false;
            expectedFulfillable[1][1] = false;
            expectedFulfillable[1][2] = true;
            expectedFulfillable[1][3] = true;
            expectedFulfillable[1][4] = true;
            expectedFulfillable[1][5] = false;
            expectedFulfillable[1][6] = false;
            expectedFulfillable[2] = new bool[7];
            expectedFulfillable[2][0] = false;
            expectedFulfillable[2][1] = false;
            expectedFulfillable[2][2] = true;
            expectedFulfillable[2][3] = true;
            expectedFulfillable[2][4] = true;
            expectedFulfillable[2][5] = false;
            expectedFulfillable[2][6] = false;
            expectedFulfillable[3] = new bool[7];
            expectedFulfillable[3][0] = false;
            expectedFulfillable[3][1] = false;
            expectedFulfillable[3][2] = true;
            expectedFulfillable[3][3] = true;
            expectedFulfillable[3][4] = true;
            expectedFulfillable[3][5] = false;
            expectedFulfillable[3][6] = false;

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

            bool[][] expectedFulfillable = new bool[4][];
            expectedFulfillable[0] = new bool[8];
            expectedFulfillable[0][0] = false;
            expectedFulfillable[0][1] = false;
            expectedFulfillable[0][2] = true;
            expectedFulfillable[0][3] = true;
            expectedFulfillable[0][4] = true;
            expectedFulfillable[0][5] = true;
            expectedFulfillable[0][6] = false;
            expectedFulfillable[0][7] = false;
            expectedFulfillable[1] = new bool[8];
            expectedFulfillable[1][0] = false;
            expectedFulfillable[1][1] = false;
            expectedFulfillable[1][2] = true;
            expectedFulfillable[1][3] = true;
            expectedFulfillable[1][4] = true;
            expectedFulfillable[1][5] = true;
            expectedFulfillable[1][6] = false;
            expectedFulfillable[1][7] = false;
            expectedFulfillable[2] = new bool[8];
            expectedFulfillable[2][0] = false;
            expectedFulfillable[2][1] = false;
            expectedFulfillable[2][2] = true;
            expectedFulfillable[2][3] = true;
            expectedFulfillable[2][4] = true;
            expectedFulfillable[2][5] = true;
            expectedFulfillable[2][6] = false;
            expectedFulfillable[2][7] = false;
            expectedFulfillable[3] = new bool[8];
            expectedFulfillable[3][0] = false;
            expectedFulfillable[3][1] = false;
            expectedFulfillable[3][2] = true;
            expectedFulfillable[3][3] = true;
            expectedFulfillable[3][4] = true;
            expectedFulfillable[3][5] = true;
            expectedFulfillable[3][6] = false;
            expectedFulfillable[3][7] = false;

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

            bool[][] expectedFulfillable = new bool[4][];
            expectedFulfillable[0] = new bool[4];
            expectedFulfillable[0][0] = false;
            expectedFulfillable[0][1] = true;
            expectedFulfillable[0][2] = true;
            expectedFulfillable[0][3] = false;
            expectedFulfillable[1] = new bool[7];
            expectedFulfillable[1][0] = false;
            expectedFulfillable[1][1] = true;
            expectedFulfillable[1][2] = true;
            expectedFulfillable[1][3] = true;
            expectedFulfillable[1][4] = true;
            expectedFulfillable[1][5] = true;
            expectedFulfillable[1][6] = false;
            expectedFulfillable[2] = new bool[4];
            expectedFulfillable[2][0] = false;
            expectedFulfillable[2][1] = true;
            expectedFulfillable[2][2] = true;
            expectedFulfillable[2][3] = false;
            expectedFulfillable[3] = new bool[7];
            expectedFulfillable[3][0] = false;
            expectedFulfillable[3][1] = true;
            expectedFulfillable[3][2] = true;
            expectedFulfillable[3][3] = true;
            expectedFulfillable[3][4] = true;
            expectedFulfillable[3][5] = true;
            expectedFulfillable[3][6] = false;

            Assert.That(roomGen.PublicFulfillableBorder, Is.EqualTo(expectedFulfillable));
        }


        [Test]
        public void PrepareRequestedBorders7x4()
        {
            //larger width circle 7x4
            Mock<IRandom> mockRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGen = new TestRoomGenRound<ITiledGenContext>();
            roomGen.PrepareSize(mockRand.Object, new Loc(7, 4));

            bool[][] expectedFulfillable = new bool[4][];
            expectedFulfillable[0] = new bool[7];
            expectedFulfillable[0][0] = false;
            expectedFulfillable[0][1] = true;
            expectedFulfillable[0][2] = true;
            expectedFulfillable[0][3] = true;
            expectedFulfillable[0][4] = true;
            expectedFulfillable[0][5] = true;
            expectedFulfillable[0][6] = false;
            expectedFulfillable[1] = new bool[4];
            expectedFulfillable[1][0] = false;
            expectedFulfillable[1][1] = true;
            expectedFulfillable[1][2] = true;
            expectedFulfillable[1][3] = false;
            expectedFulfillable[2] = new bool[7];
            expectedFulfillable[2][0] = false;
            expectedFulfillable[2][1] = true;
            expectedFulfillable[2][2] = true;
            expectedFulfillable[2][3] = true;
            expectedFulfillable[2][4] = true;
            expectedFulfillable[2][5] = true;
            expectedFulfillable[2][6] = false;
            expectedFulfillable[3] = new bool[4];
            expectedFulfillable[3][0] = false;
            expectedFulfillable[3][1] = true;
            expectedFulfillable[3][2] = true;
            expectedFulfillable[3][3] = false;

            Assert.That(roomGen.PublicFulfillableBorder, Is.EqualTo(expectedFulfillable));
        }
    }



    public class TestRoomGenRound<T> : RoomGenRound<T> where T : ITiledGenContext
    {
        public bool[][] PublicFulfillableBorder { get { return fulfillableBorder; } }
    }
}
