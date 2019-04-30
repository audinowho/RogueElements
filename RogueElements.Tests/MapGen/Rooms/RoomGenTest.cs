using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;

namespace RogueElements.Tests
{

    [TestFixture]
    public class RoomGenTest
    {

        [Test]
        [TestCase(2, 4, Dir4.Down, 2)]
        [TestCase(2, 4, Dir4.Right, 4)]
        [TestCase(2, 4, Dir4.Up, 2)]
        [TestCase(2, 4, Dir4.Left, 4)]
        public void GetBorderLength(int w, int h, Dir4 dir, int result)
        {
            Mock<ITiledGenContext> testContext = new Mock<ITiledGenContext>(MockBehavior.Strict);
            Mock<RoomGen<ITiledGenContext>> roomGen = new Mock<RoomGen<ITiledGenContext>>() { CallBase = true };
            roomGen.SetupGet(p => p.Draw).Returns(new Rect(0, 0, w, h));
            Assert.That(roomGen.Object.GetBorderLength(dir), Is.EqualTo(result));
        }

        [Test]
        [TestCase(0, 0, true)]
        [TestCase(0, 1, true)]
        [TestCase(1, 0, true)]
        [TestCase(1, -1, true)]
        [TestCase(-1, -1, true)]
        [TestCase(1, 1, false)]
        [TestCase(2, 3, false)]
        public void PrepareSize(int x, int y, bool exception)
        {
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            TestRoomGen<ITiledGenContext> roomGen = new TestRoomGen<ITiledGenContext>();
            //check size
            //opened border and requestable borders are the correct dimensions
            Loc size = new Loc(x, y);
            if (exception)
                Assert.Throws<ArgumentException>(() => { roomGen.PrepareSize(testRand.Object, size); });
            else
            {
                roomGen.PrepareSize(testRand.Object, size);
                Assert.That(roomGen.Draw.Size, Is.EqualTo(size));
                Assert.That(roomGen.PublicOpenedBorder.Length, Is.EqualTo(4));
                Assert.That(roomGen.PublicFulfillableBorder.Length, Is.EqualTo(4));
                Assert.That(roomGen.PublicOpenedBorder[0].Length, Is.EqualTo(x));
                Assert.That(roomGen.PublicFulfillableBorder[0].Length, Is.EqualTo(x));
                Assert.That(roomGen.PublicOpenedBorder[2].Length, Is.EqualTo(x));
                Assert.That(roomGen.PublicFulfillableBorder[2].Length, Is.EqualTo(x));
                Assert.That(roomGen.PublicOpenedBorder[1].Length, Is.EqualTo(y));
                Assert.That(roomGen.PublicFulfillableBorder[1].Length, Is.EqualTo(y));
                Assert.That(roomGen.PublicOpenedBorder[3].Length, Is.EqualTo(y));
                Assert.That(roomGen.PublicFulfillableBorder[3].Length, Is.EqualTo(y));
            }
        }

        [Test]
        [TestCase(true, true, true, false)]
        [TestCase(false, false, false, false)]
        public void PrepareSizeIncompleteFulfillableException(bool openDown, bool openLeft, bool openUp, bool openRight)
        {
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            TestRoomGenException<ITiledGenContext> roomGen = new TestRoomGenException<ITiledGenContext>();
            roomGen.openDown = openDown;
            roomGen.openLeft = openLeft;
            roomGen.openUp = openUp;
            roomGen.openRight = openRight;
            Assert.Throws<NotImplementedException>(() => { roomGen.PrepareSize(testRand.Object, new Loc(1)); });
        }


        [Test]
        //two rooms that are right next to each other, but offset
        [TestCase(2, 0, 3, 2, 0, 2, 4, 2, Dir4.Down, 2, 4, false)]
        //in another direction
        [TestCase(2, 3, 3, 4, 0, 1, 2, 4, Dir4.Left, 3, 5, false)]
        //two rooms separated by a one tile rift
        [TestCase(2, 0, 3, 2, 0, 3, 4, 2, Dir4.Down, 0, 0, true)]
        //two rooms overlapping each other
        [TestCase(2, 0, 3, 2, 0, 1, 4, 2, Dir4.Down, 0, 0, true)]
        //two rooms totally offset from each other
        [TestCase(8, 0, 3, 2, 0, 2, 4, 2, Dir4.Down, 0, 0, true)]
        public void ReceiveOpenedBorder(int x1, int y1, int w1, int h1, int x2, int y2, int w2, int h2, Dir4 dir, int expectedStart, int expectedEnd, bool exception)
        {
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            TestRoomGen<ITiledGenContext> roomGenTo = new TestRoomGen<ITiledGenContext>();
            TestRoomGen<ITiledGenContext> roomGenFrom = new TestRoomGen<ITiledGenContext>();
            roomGenTo.PrepareSize(testRand.Object, new Loc(w1, h1));
            roomGenTo.SetLoc(new Loc(x1, y1));
            roomGenFrom.PrepareSize(testRand.Object, new Loc(w2, h2));
            roomGenFrom.SetLoc(new Loc(x2, y2));
            for(int ii = 0; ii < roomGenFrom.PublicOpenedBorder[(int)dir.Reverse()].Length; ii++)
                roomGenFrom.PublicOpenedBorder[(int)dir.Reverse()][ii] = true;

            if (exception)
                Assert.Throws<ArgumentException>(() => { roomGenTo.ReceiveOpenedBorder(roomGenFrom, dir); });
            else
            {
                roomGenTo.ReceiveOpenedBorder(roomGenFrom, dir);
                Range newRange = roomGenTo.RoomSideReqs[(int)dir][0];
                Assert.That(newRange, Is.EqualTo(new Range(expectedStart, expectedEnd)));
            }
        }

        
        [Test]
        [TestCase(false, true, false)]
        [TestCase(true, false, true)]
        public void ReceiveOpenedBorderToFulfill(bool firstHalf, bool secondHalf, bool exception)
        {
            //test with offset, proving previous openedborders are properly transferred
            //test error case, in which no bordertofulfill is opened
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            TestRoomGen<ITiledGenContext> roomGenTo = new TestRoomGen<ITiledGenContext>();
            TestRoomGen<ITiledGenContext> roomGenFrom = new TestRoomGen<ITiledGenContext>();
            roomGenTo.PrepareSize(testRand.Object, new Loc(3, 2));
            roomGenTo.SetLoc(new Loc(2, 0));
            roomGenFrom.PrepareSize(testRand.Object, new Loc(4, 2));
            roomGenFrom.SetLoc(new Loc(0, 2));
            roomGenFrom.PublicOpenedBorder[2][0] = firstHalf;
            roomGenFrom.PublicOpenedBorder[2][1] = firstHalf;
            roomGenFrom.PublicOpenedBorder[2][2] = secondHalf;
            roomGenFrom.PublicOpenedBorder[2][3] = secondHalf;

            if (exception)
                Assert.Throws<ArgumentException>(() => { roomGenTo.ReceiveOpenedBorder(roomGenFrom, Dir4.Down); });
            else
            {
                roomGenTo.ReceiveOpenedBorder(roomGenFrom, Dir4.Down);
                bool[][] expectedBorderToFulfill = new bool[4][];
                expectedBorderToFulfill[0] = new bool[3];
                expectedBorderToFulfill[1] = new bool[2];
                expectedBorderToFulfill[2] = new bool[3];
                expectedBorderToFulfill[3] = new bool[2];
                expectedBorderToFulfill[0][0] = true;
                expectedBorderToFulfill[0][1] = true;
                Assert.That(roomGenTo.PublicBorderToFulfill, Is.EqualTo(expectedBorderToFulfill));
            }
        }


        [Test]
        [TestCase(false, true, false)]
        [TestCase(true, false, true)]
        public void ReceiveFulfillableBorderToFulfill(bool firstHalf, bool secondHalf, bool exception)
        {
            //test with offset, proving previous openedborders are properly transferred
            //test error case, in which no bordertofulfill is opened
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            TestRoomGen<ITiledGenContext> roomGenTo = new TestRoomGen<ITiledGenContext>();
            TestRoomGen<ITiledGenContext> roomGenFrom = new TestRoomGen<ITiledGenContext>();
            roomGenTo.PrepareSize(testRand.Object, new Loc(3, 2));
            roomGenTo.SetLoc(new Loc(2, 0));
            roomGenFrom.PrepareSize(testRand.Object, new Loc(4, 2));
            roomGenFrom.SetLoc(new Loc(0, 2));
            roomGenFrom.PublicFulfillableBorder[2][0] = firstHalf;
            roomGenFrom.PublicFulfillableBorder[2][1] = firstHalf;
            roomGenFrom.PublicFulfillableBorder[2][2] = secondHalf;
            roomGenFrom.PublicFulfillableBorder[2][3] = secondHalf;

            if (exception)
                Assert.Throws<ArgumentException>(() => { roomGenTo.ReceiveFulfillableBorder(roomGenFrom, Dir4.Down); });
            else
            {
                roomGenTo.ReceiveFulfillableBorder(roomGenFrom, Dir4.Down);
                bool[][] expectedBorderToFulfill = new bool[4][];
                expectedBorderToFulfill[0] = new bool[3];
                expectedBorderToFulfill[1] = new bool[2];
                expectedBorderToFulfill[2] = new bool[3];
                expectedBorderToFulfill[3] = new bool[2];
                expectedBorderToFulfill[0][0] = true;
                expectedBorderToFulfill[0][1] = true;
                Assert.That(roomGenTo.PublicBorderToFulfill, Is.EqualTo(expectedBorderToFulfill));
            }
        }


        [Test]
        public void ReceiveOpenedBorderToFulfillAlreadyFilled()
        {
            //test error case, in which no borderfill is opened by the openedborder but the tiles already exist
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            TestRoomGen<ITiledGenContext> roomGenTo = new TestRoomGen<ITiledGenContext>();
            TestRoomGen<ITiledGenContext> roomGenFrom = new TestRoomGen<ITiledGenContext>();
            roomGenTo.PrepareSize(testRand.Object, new Loc(3, 2));
            roomGenTo.SetLoc(new Loc(2, 0));
            roomGenFrom.PrepareSize(testRand.Object, new Loc(4, 2));
            roomGenFrom.SetLoc(new Loc(0, 2));
            roomGenFrom.PublicOpenedBorder[2][0] = true;
            roomGenFrom.PublicOpenedBorder[2][1] = true;
            roomGenTo.PublicBorderToFulfill[0][0] = true;
            roomGenTo.PublicBorderToFulfill[0][1] = true;

            Assert.Throws<ArgumentException>(() => { roomGenTo.ReceiveOpenedBorder(roomGenFrom, Dir4.Down); });
        }


        [Test]
        [TestCase(0, -1, Dir4.Down, 0, 0, true)]
        [TestCase(0, 0, Dir4.Down, 0, 0, true)]
        [TestCase(0, 1, Dir4.Down, 0, 0, true)]
        [TestCase(6, 10, Dir4.Down, 0, 0, true)]
        [TestCase(0, 1, Dir4.Up, 0, 0, true)]
        [TestCase(0, 4, Dir4.Up, 1, 4, false)]
        [TestCase(0, 10, Dir4.Up, 1, 6, false)]
        [TestCase(4, 10, Dir4.Up, 4, 6, false)]
        [TestCase(0, 2, Dir4.Left, 0, 0, true)]
        [TestCase(9, 10, Dir4.Left, 0, 0, true)]
        [TestCase(0, 2, Dir4.Left, 0, 0, true)]
        [TestCase(0, 4, Dir4.Left, 2, 4, false)]
        [TestCase(0, 10, Dir4.Left, 2, 9, false)]
        [TestCase(4, 10, Dir4.Left, 4, 9, false)]
        [TestCase(4, 8, Dir4.Left, 4, 8, false)]
        [TestCase(4, 8, Dir4.Right, 4, 8, false)]
        [TestCase(3, 5, Dir4.None, 0, 0, true)]
        [TestCase(3, 5, -1, 0, 0, true)]
        [TestCase(3, 5, 4, 0, 0, true)]
        public void ReceiveBorderRange(int rangeStart, int rangeEnd, Dir4 dir, int expectedStart, int expectedEnd, bool exception)
        {
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            TestRoomGen<ITiledGenContext> roomGen = new TestRoomGen<ITiledGenContext>();
            roomGen.PrepareSize(testRand.Object, new Loc(5, 7));
            roomGen.SetLoc(new Loc(1, 2));

            if (exception)
                Assert.Throws<ArgumentException>(() => { roomGen.ReceiveBorderRange(new Range(rangeStart, rangeEnd), dir); });
            else
            {
                roomGen.ReceiveBorderRange(new Range(rangeStart, rangeEnd), dir);
                Range newRange = roomGen.RoomSideReqs[(int)dir][0];
                Assert.That(newRange, Is.EqualTo(new Range(expectedStart, expectedEnd)));
            }
        }


        [Test]
        [TestCase(0, 3, Dir4.Down, 0, 0, true)]
        [TestCase(0, 4, Dir4.Down, 1, 4, false)]
        [TestCase(1, 3, Dir4.Down, 0, 0, true)]
        [TestCase(0, 10, Dir4.Down, 1, 6, false)]
        [TestCase(3, 10, Dir4.Down, 3, 6, false)]
        [TestCase(4, 10, Dir4.Down, 0, 0, true)]
        public void ReceiveBorderRangeFulfilledTile(int rangeStart, int rangeEnd, Dir4 dir, int expectedStart, int expectedEnd, bool exception)
        {
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            TestRoomGenException<ITiledGenContext> roomGen = new TestRoomGenException<ITiledGenContext>();
            roomGen.openDown = true;
            roomGen.openLeft = true;
            roomGen.openUp = true;
            roomGen.openRight = true;
            roomGen.PrepareSize(testRand.Object, new Loc(5, 7));
            roomGen.SetLoc(new Loc(1, 2));

            if (exception)
                Assert.Throws<ArgumentException>(() => { roomGen.ReceiveBorderRange(new Range(rangeStart, rangeEnd), dir); });
            else
            {
                roomGen.ReceiveBorderRange(new Range(rangeStart, rangeEnd), dir);
                Range newRange = roomGen.RoomSideReqs[(int)dir][0];
                Assert.That(newRange, Is.EqualTo(new Range(expectedStart, expectedEnd)));
            }
        }


        [Test]
        public void ReceiveBorderRangeToFulfill()
        {
            //test with offset, proving previous openedborders are properly transferred
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            TestRoomGen<ITiledGenContext> roomGen = new TestRoomGen<ITiledGenContext>();
            roomGen.PrepareSize(testRand.Object, new Loc(5, 7));
            roomGen.SetLoc(new Loc(2, 3));

            roomGen.ReceiveBorderRange(new Range(1, 6), Dir4.Right);
            bool[][] expectedBorderToFulfill = new bool[4][];
            expectedBorderToFulfill[0] = new bool[5];
            expectedBorderToFulfill[1] = new bool[7];
            expectedBorderToFulfill[2] = new bool[5];
            expectedBorderToFulfill[3] = new bool[7];
            expectedBorderToFulfill[3][0] = true;
            expectedBorderToFulfill[3][1] = true;
            expectedBorderToFulfill[3][2] = true;
            Assert.That(roomGen.PublicBorderToFulfill, Is.EqualTo(expectedBorderToFulfill));
        }

        [Test]
        [TestCase(0, 0, 1, 1, Dir4.None, 0, 0, 0, true)]
        [TestCase(2, 4, 5, 7, Dir4.Up, 6, 6, 4, false)]
        [TestCase(2, 4, 5, 7, Dir4.Down, 6, 6, 10, false)]
        [TestCase(2, 8, 5, 7, Dir4.Down, 6, 6, 14, false)]
        [TestCase(2, 8, 5, 7, Dir4.Up, 6, 6, 8, false)]
        [TestCase(2, 4, 5, 7, Dir4.Left, 3, 2, 3, false)]
        [TestCase(2, 4, 5, 7, Dir4.Right, 3, 6, 3, false)]
        [TestCase(2, 4, 8, 7, Dir4.Right, 3, 9, 3, false)]
        public void GetEdgeLoc(int x, int y, int width, int length, Dir4 dir, int scalar, int expectedX, int expectedY, bool exception)
        {
            Mock<ITiledGenContext> testContext = new Mock<ITiledGenContext>(MockBehavior.Strict);
            Mock<RoomGen<ITiledGenContext>> roomGen = new Mock<RoomGen<ITiledGenContext>>() { CallBase = true };
            roomGen.SetupGet(p => p.Draw).Returns(new Rect(x, y, width, length));

            if (exception)
                Assert.Throws<ArgumentException>(() => { roomGen.Object.GetEdgeLoc(dir, scalar); });
            else
                Assert.That(roomGen.Object.GetEdgeLoc(dir, scalar), Is.EqualTo(new Loc(expectedX, expectedY)));
        }

        [Test]
        [TestCase(0, 0, 1, 1, Dir4.None, 1, 1, 0, 0, 0, true)]
        [TestCase(2, 4, 5, 7, Dir4.Up, 2, 3, 6, 6, 1, false)]
        [TestCase(2, 4, 5, 7, Dir4.Up, 2, 4, 6, 6, 0, false)]
        [TestCase(2, 4, 5, 7, Dir4.Up, 3, 3, 6, 6, 1, false)]
        [TestCase(2, 4, 5, 7, Dir4.Down, 2, 3, 6, 6, 11, false)]
        [TestCase(2, 8, 5, 7, Dir4.Down, 2, 3, 6, 6, 15, false)]
        [TestCase(2, 8, 5, 7, Dir4.Down, 3, 4, 6, 6, 15, false)]
        [TestCase(2, 8, 5, 7, Dir4.Up, 2, 3, 6, 6, 5, false)]
        [TestCase(2, 4, 5, 7, Dir4.Left, 2, 3, 3, 0, 3, false)]
        [TestCase(2, 4, 5, 7, Dir4.Left, 3, 3, 3, -1, 3, false)]
        [TestCase(2, 4, 5, 7, Dir4.Left, 2, 4, 3, 0, 3, false)]
        [TestCase(2, 4, 5, 7, Dir4.Right, 2, 3, 3, 7, 3, false)]
        [TestCase(2, 4, 8, 7, Dir4.Right, 2, 3, 3, 10, 3, false)]
        [TestCase(2, 4, 8, 7, Dir4.Right, 3, 4, 3, 10, 3, false)]
        public void GetRectEdgeLoc(int x, int y, int width, int length,
            Dir4 dir, int sizeX, int sizeY, int scalar, int expectedX, int expectedY, bool exception)
        {
            Mock<ITiledGenContext> testContext = new Mock<ITiledGenContext>(MockBehavior.Strict);
            Mock<RoomGen<ITiledGenContext>> roomGen = new Mock<RoomGen<ITiledGenContext>>() { CallBase = true };
            roomGen.SetupGet(p => p.Draw).Returns(new Rect(x, y, width, length));

            if (exception)
                Assert.Throws<ArgumentException>(() => { roomGen.Object.GetEdgeRectLoc(dir, new Loc(sizeX, sizeY), scalar); });
            else
                Assert.That(roomGen.Object.GetEdgeRectLoc(dir, new Loc(sizeX, sizeY), scalar), Is.EqualTo(new Loc(expectedX, expectedY)));
        }

        [Test]
        public void SetRoomBordersClear()
        {
            TestRoomGen<ITiledGenContext> roomGen = new TestRoomGen<ITiledGenContext>();
            string[] inGrid =  { "XXXXXXXX",
                                 "XX.....X",
                                 "XX.....X",
                                 "XX.....X",
                                 "XX.....X",
                                 "XXXXXXXX",
                                 "XXXXXXXX" };
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            roomGen.PrepareSize(testRand.Object, new Loc(5, 4));
            roomGen.SetLoc(new Loc(2, 1));

            roomGen.SetRoomBorders(testContext);

            Assert.That(roomGen.PublicOpenedBorder[0], Is.EqualTo(new bool[] { true, true, true, true, true }));
            Assert.That(roomGen.PublicOpenedBorder[1], Is.EqualTo(new bool[] { true, true, true, true }));
            Assert.That(roomGen.PublicOpenedBorder[2], Is.EqualTo(new bool[] { true, true, true, true, true }));
            Assert.That(roomGen.PublicOpenedBorder[3], Is.EqualTo(new bool[] { true, true, true, true }));
        }

        [Test]
        public void SetRoomBordersSemiBlocked()
        {
            TestRoomGen<ITiledGenContext> roomGen = new TestRoomGen<ITiledGenContext>();
            string[] inGrid =  { "XXXXXXXX",
                                 "XXX..X.X",
                                 "XX.....X",
                                 "XX....XX",
                                 "XX..XXXX",
                                 "XXXXXXXX",
                                 "XXXXXXXX" };
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            roomGen.PrepareSize(testRand.Object, new Loc(5, 4));
            roomGen.SetLoc(new Loc(2, 1));

            roomGen.SetRoomBorders(testContext);

            Assert.That(roomGen.PublicOpenedBorder[0], Is.EqualTo(new bool[] { true, true, false, false, false }));
            Assert.That(roomGen.PublicOpenedBorder[1], Is.EqualTo(new bool[] { false, true, true, true }));
            Assert.That(roomGen.PublicOpenedBorder[2], Is.EqualTo(new bool[] { false, true, true, false, true }));
            Assert.That(roomGen.PublicOpenedBorder[3], Is.EqualTo(new bool[] { true, true, false, false }));
        }

        [Test]
        public void SetRoomBordersSubFulfillable()
        {
            TestRoomGenException<ITiledGenContext> roomGen = new TestRoomGenException<ITiledGenContext>();
            roomGen.openDown = true;
            roomGen.openLeft = true;
            roomGen.openUp = true;
            roomGen.openRight = true;
            string[] inGrid =  { "XXXXXXXX",
                                 "XX.....X",
                                 "XX.....X",
                                 "XX.....X",
                                 "XX.....X",
                                 "XXXXXXXX",
                                 "XXXXXXXX" };
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            roomGen.PrepareSize(testRand.Object, new Loc(5, 4));
            roomGen.SetLoc(new Loc(2, 1));

            roomGen.SetRoomBorders(testContext);

            Assert.That(roomGen.PublicOpenedBorder[0], Is.EqualTo(new bool[] { false, false, true, false, false }));
            Assert.That(roomGen.PublicOpenedBorder[1], Is.EqualTo(new bool[] { false, false, true, false }));
            Assert.That(roomGen.PublicOpenedBorder[2], Is.EqualTo(new bool[] { false, false, true, false, false }));
            Assert.That(roomGen.PublicOpenedBorder[3], Is.EqualTo(new bool[] { false, false, true, false }));
        }

        [Test]
        [TestCase(false, false, false, false)]
        [TestCase(true, false, false, false)]
        [TestCase(false, true, false, false)]
        [TestCase(false, false, true, false)]
        [TestCase(false, false, false, true)]
        [TestCase(true, true, true, true)]
        public void FulfillRoomBorders4SideReqsNoneMissing(bool reqDown, bool reqLeft, bool reqUp, bool reqRight)
        {
            //one or several sidereqs met with only one tile (do nothing)
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Mock<TestRoomGen<ITiledGenContext>> roomGen = new Mock<TestRoomGen<ITiledGenContext>>() { CallBase = true };
            roomGen.Setup(p => p.DigAtBorder(It.IsAny<ITiledGenContext>(), It.IsAny<Dir4>(), It.IsAny<int>()));
            string[] inGrid =  { "XXXXXXXX",
                                 "XX..X..X",
                                 "XX....XX",
                                 "XX....XX",
                                 "XX...XXX",
                                 "XXXXXXXX",
                                 "XXXXXXXX" };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            roomGen.Object.PrepareSize(testRand.Object, new Loc(5, 4));
            roomGen.Object.SetLoc(new Loc(2, 1));
            if (reqDown)
                roomGen.Object.ReceiveBorderRange(new Range(2, 7), Dir4.Down);
            if (reqLeft)
                roomGen.Object.ReceiveBorderRange(new Range(1, 5), Dir4.Left);
            if (reqUp)
                roomGen.Object.ReceiveBorderRange(new Range(2, 7), Dir4.Up);
            if (reqRight)
                roomGen.Object.ReceiveBorderRange(new Range(1, 5), Dir4.Right);
            //find where the class chose to dig

            roomGen.Object.FulfillRoomBorders(testContext, false);

            roomGen.Verify(p => p.DigAtBorder(It.IsAny<ITiledGenContext>(), It.IsAny<Dir4>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        public void FulfillRoomBordersNoneMissingIntersect()
        {
            //two intersecting sidereqs met with only one tile (do nothing)
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Mock<TestRoomGen<ITiledGenContext>> roomGen = new Mock<TestRoomGen<ITiledGenContext>>() { CallBase = true };
            roomGen.Setup(p => p.DigAtBorder(It.IsAny<ITiledGenContext>(), It.IsAny<Dir4>(), It.IsAny<int>()));
            string[] inGrid =  { "XXXXXXXX",
                                 "XX.....X",
                                 "XX.....X",
                                 "XX.....X",
                                 "XXXX.XXX",
                                 "XXXXXXXX",
                                 "XXXXXXXX" };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            roomGen.Object.PrepareSize(testRand.Object, new Loc(5, 4));
            roomGen.Object.SetLoc(new Loc(2, 1));
            roomGen.Object.ReceiveBorderRange(new Range(4, 7), Dir4.Down);
            roomGen.Object.ReceiveBorderRange(new Range(2, 5), Dir4.Down);
            //find where the class chose to dig

            roomGen.Object.FulfillRoomBorders(testContext, false);

            roomGen.Verify(p => p.DigAtBorder(It.IsAny<ITiledGenContext>(), It.IsAny<Dir4>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        [TestCase(0, 3)]
        [TestCase(1, 4)]
        [TestCase(2, 5)]
        [TestCase(3, 6)]
        public void FulfillRoomBordersNoneOneMissing(int fulfillRoll, int expectedX)
        {
            //one sidereq not met (with rng)
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(1)).Returns(0);
            testRand.Setup(p => p.Next(4)).Returns(fulfillRoll);
            Mock<TestRoomGen<ITiledGenContext>> roomGen = new Mock<TestRoomGen<ITiledGenContext>>() { CallBase = true };
            roomGen.Setup(p => p.DigAtBorder(It.IsAny<ITiledGenContext>(), It.IsAny<Dir4>(), It.IsAny<int>()));
            //mock the ChoosePossibleStartRanges?
            //roomGen.Setup(p => p.ChoosePossibleStartRanges(It.IsAny<IRandom>(), It.IsAny<int>(), It.IsAny<bool[]>(), It.IsAny<List<Range>>()));
            string[] inGrid =  { "XXXXXXXX",
                                 "XX.....X",
                                 "XX.....X",
                                 "XX.....X",
                                 "XX.XXXXX",
                                 "XXXXXXXX",
                                 "XXXXXXXX" };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            roomGen.Object.PrepareSize(testRand.Object, new Loc(5, 4));
            roomGen.Object.SetLoc(new Loc(2, 1));
            roomGen.Object.ReceiveBorderRange(new Range(3, 7), Dir4.Down);
            //find where the class chose to dig

            roomGen.Object.FulfillRoomBorders(testContext, false);
            
            roomGen.Verify(p => p.DigAtBorder(testContext, Dir4.Down, expectedX), Times.Once());
            testRand.Verify(p => p.Next(1), Times.Exactly(1));
            testRand.Verify(p => p.Next(4), Times.Exactly(1));
        }

        [Test]
        public void FulfillRoomBordersNoneOneMissingIntersect()
        {
            //two intersecting sidereqs not met
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(2));
            seq = seq.Returns(1);
            seq = seq.Returns(0);
            testRand.Setup(p => p.Next(1)).Returns(0);
            Mock<TestRoomGen<ITiledGenContext>> roomGen = new Mock<TestRoomGen<ITiledGenContext>>() { CallBase = true };
            roomGen.Setup(p => p.DigAtBorder(It.IsAny<ITiledGenContext>(), It.IsAny<Dir4>(), It.IsAny<int>()));
            string[] inGrid =  { "XXXXXXXX",
                                 "XX.....X",
                                 "XX.....X",
                                 "XX.....X",
                                 "XX.XXXXX",
                                 "XXXXXXXX",
                                 "XXXXXXXX" };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            roomGen.Object.PrepareSize(testRand.Object, new Loc(5, 4));
            roomGen.Object.SetLoc(new Loc(2, 1));
            roomGen.Object.ReceiveBorderRange(new Range(3, 6), Dir4.Down);
            roomGen.Object.ReceiveBorderRange(new Range(4, 7), Dir4.Down);
            //find where the class chose to dig

            roomGen.Object.FulfillRoomBorders(testContext, false);

            roomGen.Verify(p => p.DigAtBorder(testContext, Dir4.Down, 4), Times.Once());
            testRand.Verify(p => p.Next(2), Times.Exactly(2));
            testRand.Verify(p => p.Next(1), Times.Exactly(1));
        }

        [Test]
        public void FulfillRoomBordersNoneOneMissingFillAll()
        {
            //one sidereq not met, fulfilled completely, only fulfillable borders
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Mock<TestRoomGen<ITiledGenContext>> roomGen = new Mock<TestRoomGen<ITiledGenContext>>() { CallBase = true };
            roomGen.Setup(p => p.DigAtBorder(It.IsAny<ITiledGenContext>(), It.IsAny<Dir4>(), It.IsAny<int>()));
            string[] inGrid =  { "XXXXXXXX",
                                 "XX.....X",
                                 "XX.....X",
                                 "XX.....X",
                                 "XXXXXXXX",
                                 "XXXXXXXX",
                                 "XXXXXXXX" };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            roomGen.Object.PrepareSize(testRand.Object, new Loc(5, 4));
            roomGen.Object.SetLoc(new Loc(2, 1));
            roomGen.Object.PublicFulfillableBorder[(int)Dir4.Down][2] = false;
            roomGen.Object.PublicFulfillableBorder[(int)Dir4.Down][3] = false;
            roomGen.Object.ReceiveBorderRange(new Range(2, 7), Dir4.Down);
            //find where the class chose to dig

            roomGen.Object.FulfillRoomBorders(testContext, true);

            roomGen.Verify(p => p.DigAtBorder(testContext, Dir4.Down, 2), Times.Once());
            roomGen.Verify(p => p.DigAtBorder(testContext, Dir4.Down, 3), Times.Once());
            roomGen.Verify(p => p.DigAtBorder(testContext, Dir4.Down, 6), Times.Once());
        }

        [Test]
        [TestCase(0, 0, 3, 2)]
        [TestCase(3, 2, 6, 4)]
        public void FulfillRoomBordersNoneMultiMissingIntersect(int roll1, int roll2, int expected1, int expected2)
        {
            //full sidereq not met on one side, one adjacent sidereq not met on a corner tile
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(4));
            seq = seq.Returns(roll1);
            seq = testRand.SetupSequence(p => p.Next(3));
            seq = seq.Returns(roll2);
            seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            Mock<TestRoomGen<ITiledGenContext>> roomGen = new Mock<TestRoomGen<ITiledGenContext>>() { CallBase = true };
            roomGen.Setup(p => p.DigAtBorder(It.IsAny<ITiledGenContext>(), It.IsAny<Dir4>(), It.IsAny<int>()));
            string[] inGrid =  { "XXXXXXXX",
                                 "XX.....X",
                                 "XX....XX",
                                 "XX....XX",
                                 "XX.XXXXX",
                                 "XXXXXXXX",
                                 "XXXXXXXX" };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            roomGen.Object.PrepareSize(testRand.Object, new Loc(5, 4));
            roomGen.Object.SetLoc(new Loc(2, 1));
            roomGen.Object.ReceiveBorderRange(new Range(3, 7), Dir4.Down);
            roomGen.Object.ReceiveBorderRange(new Range(2, 5), Dir4.Right);
            //find where the class chose to dig

            roomGen.Object.FulfillRoomBorders(testContext, false);


            roomGen.Verify(p => p.DigAtBorder(testContext, Dir4.Down, expected1), Times.Once());
            roomGen.Verify(p => p.DigAtBorder(testContext, Dir4.Right, expected2), Times.Once());
            testRand.Verify(p => p.Next(4), Times.Exactly(1));
            testRand.Verify(p => p.Next(3), Times.Exactly(1));
            testRand.Verify(p => p.Next(1), Times.Exactly(2));
        }

        [Test]
        [TestCase(Dir4.Left, 3, 0, false)]
        [TestCase(Dir4.Up, 4, 1, false)]
        [TestCase(Dir4.Right, 3, 2, false)]
        [TestCase(Dir4.Down, 6, 3, false)]
        [TestCase(Dir4.Left, 5, 0, true)]
        public void DigAtBorder(Dir4 dir, int scalar, int resultGrid, bool exception)
        {
            TestRoomGen<ITiledGenContext> roomGen = new TestRoomGen<ITiledGenContext>();
            string[] inGrid =  { "XXXXXXXX",
                                 "XX..X..X",
                                 "XX....XX",
                                 "XX...XXX",
                                 "XX....XX",
                                 "XXXXXXXX",
                                 "XXXXXXXX" };
            string[] outGrid;
            if (resultGrid == 1)
            {
                outGrid = new string[] {"XXXXXXXX",
                                        "XX.....X",
                                        "XX....XX",
                                        "XX...XXX",
                                        "XX....XX",
                                        "XXXXXXXX",
                                        "XXXXXXXX" };
            }
            else if (resultGrid == 2)
            {
                outGrid = new string[] {"XXXXXXXX",
                                        "XX..X..X",
                                        "XX....XX",
                                        "XX.....X",
                                        "XX....XX",
                                        "XXXXXXXX",
                                        "XXXXXXXX" };
            }
            else if (resultGrid == 3)
            {
                outGrid = new string[] {"XXXXXXXX",
                                        "XX..X..X",
                                        "XX.....X",
                                        "XX...X.X",
                                        "XX.....X",
                                        "XXXXXX.X",
                                        "XXXXXXXX" };
            }
            else
            {
                outGrid = inGrid;
            }
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            roomGen.PrepareSize(testRand.Object, new Loc(5, 5));
            roomGen.SetLoc(new Loc(2, 1));

            if (exception)
                Assert.Throws<ArgumentException>(() => { roomGen.DigAtBorder(testContext, dir, scalar); });
            else
            {
                roomGen.DigAtBorder(testContext, dir, scalar);
                Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            }
        }

        [Test]
        public void ChoosePossibleStartsOneReq()
        {
            //one sidereq, 3 tiles
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(1)).Returns(0);
            TestRoomGen<ITiledGenContext> roomGen = new TestRoomGen<ITiledGenContext>();
            bool[] permittedRange = { true, true, true, true, true };
            List<Range> sideReqs = new List<Range>();
            sideReqs.Add(new Range(5, 8));
            List<HashSet<int>> compare = new List<HashSet<int>>();
            HashSet<int> set = new HashSet<int>();
            set.Add(5);
            set.Add(6);
            set.Add(7);
            compare.Add(set);

            List<HashSet<int>> result = roomGen.PublicChoosePossibleStarts(testRand.Object, 4, permittedRange, sideReqs);

            Assert.That(result, Is.EqualTo(compare));
            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(1));
        }


        [Test]
        public void ChoosePossibleStartsOneReqRestricted()
        {
            //one sidereq, 3 tiles, one allowed
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(1)).Returns(0);
            TestRoomGen<ITiledGenContext> roomGen = new TestRoomGen<ITiledGenContext>();
            bool[] permittedRange = { true, false, true, false, true };
            List<Range> sideReqs = new List<Range>();
            sideReqs.Add(new Range(5, 8));
            List<HashSet<int>> compare = new List<HashSet<int>>();
            HashSet<int> set = new HashSet<int>();
            set.Add(6);

            List<HashSet<int>> result = roomGen.PublicChoosePossibleStarts(testRand.Object, 4, permittedRange, sideReqs);
            compare.Add(set);

            Assert.That(result, Is.EqualTo(compare));
            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(1));
        }

        [Test]
        public void ChoosePossibleStartsTwoReqs()
        {
            //two non-overlapping sidereqs, 2 tiles each
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(2)).Returns(1);
            testRand.Setup(p => p.Next(1)).Returns(0);
            TestRoomGen<ITiledGenContext> roomGen = new TestRoomGen<ITiledGenContext>();
            bool[] permittedRange = { true, true, true, true, true };
            List<Range> sideReqs = new List<Range>();
            sideReqs.Add(new Range(4, 6));
            sideReqs.Add(new Range(7, 9));
            List<HashSet<int>> compare = new List<HashSet<int>>();
            HashSet<int> set = new HashSet<int>();
            set.Add(7);
            set.Add(8);
            compare.Add(set);
            set = new HashSet<int>();
            set.Add(4);
            set.Add(5);
            compare.Add(set);

            List<HashSet<int>> result = roomGen.PublicChoosePossibleStarts(testRand.Object, 4, permittedRange, sideReqs);

            Assert.That(result, Is.EqualTo(compare));
            testRand.Verify(p => p.Next(2), Times.Exactly(1));
            testRand.Verify(p => p.Next(1), Times.Exactly(1));
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void ChoosePossibleStartsTwoReqsOverlap(int roll1)
        {
            //two overlapping sidereqs, 3 tiles each (rng should make no difference)
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(2)).Returns(roll1);
            testRand.Setup(p => p.Next(1)).Returns(0);
            TestRoomGen<ITiledGenContext> roomGen = new TestRoomGen<ITiledGenContext>();
            bool[] permittedRange = { true, true, true, true, true };
            List<Range> sideReqs = new List<Range>();
            sideReqs.Add(new Range(4, 7));
            sideReqs.Add(new Range(5, 8));
            List<HashSet<int>> compare = new List<HashSet<int>>();
            HashSet<int> set = new HashSet<int>();
            set.Add(5);
            set.Add(6);
            compare.Add(set);

            List<HashSet<int>> result = roomGen.PublicChoosePossibleStarts(testRand.Object, 4, permittedRange, sideReqs);

            Assert.That(result, Is.EqualTo(compare));
            testRand.Verify(p => p.Next(2), Times.Exactly(1));
            testRand.Verify(p => p.Next(1), Times.Exactly(1));
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void ChoosePossibleStartsTwoReqsSuperset(int roll1)
        {
            //two overlapping sidereqs, one superset of the other (rng should make no difference)
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(2)).Returns(roll1);
            testRand.Setup(p => p.Next(1)).Returns(0);
            TestRoomGen<ITiledGenContext> roomGen = new TestRoomGen<ITiledGenContext>();
            bool[] permittedRange = { true, true, true, true, true };
            List<Range> sideReqs = new List<Range>();
            sideReqs.Add(new Range(4, 9));
            sideReqs.Add(new Range(6, 8));
            List<HashSet<int>> compare = new List<HashSet<int>>();
            HashSet<int> set = new HashSet<int>();
            set.Add(6);
            set.Add(7);
            compare.Add(set);

            List<HashSet<int>> result = roomGen.PublicChoosePossibleStarts(testRand.Object, 4, permittedRange, sideReqs);

            Assert.That(result, Is.EqualTo(compare));
            testRand.Verify(p => p.Next(2), Times.Exactly(1));
            testRand.Verify(p => p.Next(1), Times.Exactly(1));
        }

        [Test]
        public void ChoosePossibleStartsTwoReqsOverlapRestricted()
        {
            //two overlapping sidereqs, 3 tiles each, two allowed
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(2)).Returns(0);
            testRand.Setup(p => p.Next(1)).Returns(0);
            TestRoomGen<ITiledGenContext> roomGen = new TestRoomGen<ITiledGenContext>();
            bool[] permittedRange = { true, false, false, true, true };
            List<Range> sideReqs = new List<Range>();
            sideReqs.Add(new Range(4, 7));
            sideReqs.Add(new Range(5, 8));
            List<HashSet<int>> compare = new List<HashSet<int>>();
            HashSet<int> set = new HashSet<int>();
            set.Add(4);
            compare.Add(set);
            set = new HashSet<int>();
            set.Add(7);
            compare.Add(set);

            List<HashSet<int>> result = roomGen.PublicChoosePossibleStarts(testRand.Object, 4, permittedRange, sideReqs);

            Assert.That(result, Is.EqualTo(compare));
            testRand.Verify(p => p.Next(2), Times.Exactly(1));
            testRand.Verify(p => p.Next(1), Times.Exactly(1));
        }



        [Test]
        [TestCase(0, 1)]
        [TestCase(0, 0)]
        [TestCase(1, 0)]
        public void ChoosePossibleStartsThreeReqsOverlapOneLeft(int roll1, int roll2)
        {
            //two separate sidereqs, with one in the middle connecting (with different rng)
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(3)).Returns(roll1);
            testRand.Setup(p => p.Next(2)).Returns(roll2);
            testRand.Setup(p => p.Next(1)).Returns(0);
            TestRoomGen<ITiledGenContext> roomGen = new TestRoomGen<ITiledGenContext>();
            bool[] permittedRange = { true, true, true, true, true };
            List<Range> sideReqs = new List<Range>();
            sideReqs.Add(new Range(4, 6));
            sideReqs.Add(new Range(5, 8));
            sideReqs.Add(new Range(7, 9));
            List<HashSet<int>> compare = new List<HashSet<int>>();
            HashSet<int> set = new HashSet<int>();
            set.Add(5);
            compare.Add(set);
            set = new HashSet<int>();
            set.Add(7);
            set.Add(8);
            compare.Add(set);

            List<HashSet<int>> result = roomGen.PublicChoosePossibleStarts(testRand.Object, 4, permittedRange, sideReqs);

            Assert.That(result, Is.EqualTo(compare));
            testRand.Verify(p => p.Next(3), Times.Exactly(1));
            testRand.Verify(p => p.Next(2), Times.Exactly(1));
            testRand.Verify(p => p.Next(1), Times.Exactly(1));
        }


        [Test]
        [TestCase(2, 1)]
        [TestCase(2, 0)]
        [TestCase(1, 1)]
        public void ChoosePossibleStartsThreeReqsOverlapOneRight(int roll1, int roll2)
        {
            //two separate sidereqs, with one in the middle connecting (with different rng)
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(3)).Returns(roll1);
            testRand.Setup(p => p.Next(2)).Returns(roll2);
            testRand.Setup(p => p.Next(1)).Returns(0);
            TestRoomGen<ITiledGenContext> roomGen = new TestRoomGen<ITiledGenContext>();
            bool[] permittedRange = { true, true, true, true, true };
            List<Range> sideReqs = new List<Range>();
            sideReqs.Add(new Range(4, 6));
            sideReqs.Add(new Range(5, 8));
            sideReqs.Add(new Range(7, 9));
            List<HashSet<int>> compare = new List<HashSet<int>>();
            HashSet<int> set = new HashSet<int>();
            set.Add(7);
            compare.Add(set);
            set = new HashSet<int>();
            set.Add(4);
            set.Add(5);
            compare.Add(set);

            List<HashSet<int>> result = roomGen.PublicChoosePossibleStarts(testRand.Object, 4, permittedRange, sideReqs);

            Assert.That(result, Is.EqualTo(compare));
            testRand.Verify(p => p.Next(3), Times.Exactly(1));
            testRand.Verify(p => p.Next(2), Times.Exactly(1));
            testRand.Verify(p => p.Next(1), Times.Exactly(1));
        }

    }

    public class TestTile : ITile
    {
        public int ID { get; set; }

        public TestTile() { }
        public TestTile(int id) { ID = id; }

        public override bool Equals(object obj)
        {
            TestTile other = obj as TestTile;
            if (other == null)
                return false;
            return other.ID == ID;
        }
        protected TestTile(TestTile other)
        {
            ID = other.ID;
        }
        public ITile Copy() { return new TestTile(this); }

        public bool TileEquivalent(ITile other)
        {
            return Equals(other);
        }

        public override int GetHashCode() { return ID; }
    }

    public class TestGenContext : ITiledGenContext
    {

        public static TestGenContext InitGridToContext(string[] inGrid)
        {
            //transposes
            TestGenContext testContext = new TestGenContext();
            testContext.CreateNew(inGrid[0].Length, inGrid.Length);
            for (int xx = 0; xx < testContext.Width; xx++)
            {
                for (int yy = 0; yy < testContext.Height; yy++)
                {
                    ((TestTile)testContext.Tiles[xx][yy]).ID = (inGrid[yy][xx] == 'X') ? 1 : 0;
                }
            }
            return testContext;
        }

        public IRandom Rand { get; private set; }
        public void InitSeed(ulong seed) { }
        public void SetTestRand(IRandom rand) { Rand = rand; }
        public void FinishGen() { }

        public bool TileBlocked(Loc loc) { return TileBlocked(loc, false); }
        public bool TileBlocked(Loc loc, bool diagonal) { return ((TestTile)Tiles[loc.X][loc.Y]).ID != 0; }

        public ITile RoomTerrain { get { return new TestTile(0); } }
        public ITile WallTerrain { get { return new TestTile(1); } }

        public ITile GetTile(Loc loc) { return Tiles[loc.X][loc.Y]; }
        public void SetTile(Loc loc, ITile tile) { Tiles[loc.X][loc.Y] = tile; }
        public bool TrySetTile(Loc loc, ITile tile) { SetTile(loc, tile); return true; }
        public bool CanSetTile(Loc loc, ITile tile) { return true; }

        public bool TilesInitialized { get { return Tiles != null; } }

        public ITile[][] Tiles { get { return tiles; } }
        private ITile[][] tiles;
        public int Width { get { return Tiles.Length; } }
        public int Height { get { return Tiles[0].Length; } }
        public void CreateNew(int tileWidth, int tileHeight)
        {
            tiles = new ITile[tileWidth][];
            for (int ii = 0; ii < tileWidth; ii++)
            {
                tiles[ii] = new ITile[tileHeight];
                for (int jj = 0; jj < tileHeight; jj++)
                    tiles[ii][jj] = new TestTile();
            }
        }
    }


    public class TestRoomGenException<T> : TestRoomGen<T> where T : ITiledGenContext
    {
        public bool openDown;
        public bool openLeft;
        public bool openUp;
        public bool openRight;
        
        protected override void PrepareFulfillableBorders(IRandom rand)
        {
            fulfillableBorder[0][fulfillableBorder[0].Length/2] = openDown;
            fulfillableBorder[1][fulfillableBorder[1].Length/2] = openLeft;
            fulfillableBorder[2][fulfillableBorder[2].Length/2] = openUp;
            fulfillableBorder[3][fulfillableBorder[3].Length/2] = openRight;
        }
    }

    public class TestRoomGen<T> : RoomGen<T> where T : ITiledGenContext
    {
        public List<Range>[] RoomSideReqs { get { return roomSideReqs; } }
        public bool[][] PublicOpenedBorder { get { return openedBorder; } }
        public bool[][] PublicFulfillableBorder { get { return fulfillableBorder; } }
        public bool[][] PublicBorderToFulfill { get { return borderToFulfill; } }

        public override RoomGen<T> Copy() { return new TestRoomGen<T>(); }

        public override Loc ProposeSize(IRandom rand) { return new Loc(); }
        public override void DrawOnMap(T map) { }
        protected override void PrepareFulfillableBorders(IRandom rand)
        {
            for (int ii = 0; ii < 4; ii++)
                for (int jj = 0; jj < fulfillableBorder[ii].Length; jj++)
                    fulfillableBorder[ii][jj] = true;
        }
        
        public List<HashSet<int>> PublicChoosePossibleStarts(IRandom rand, int scalarStart, bool[] permittedRange, List<Range> origSideReqs)
        {
            return ChoosePossibleStartRanges(rand, scalarStart, permittedRange, origSideReqs);
        }
    }
}
