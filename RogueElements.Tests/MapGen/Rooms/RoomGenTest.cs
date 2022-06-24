// <copyright file="RoomGenTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class RoomGenTest
    {
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
            var roomGen = new TestRoomGen<ITiledGenContext>();

            // check size
            // opened border and requestable borders are the correct dimensions
            Loc size = new Loc(x, y);
            if (exception)
            {
                Assert.Throws<ArgumentException>(() => { roomGen.PrepareSize(testRand.Object, size); });
            }
            else
            {
                roomGen.PrepareSize(testRand.Object, size);
                Assert.That(roomGen.Draw.Size, Is.EqualTo(size));
                Assert.That(roomGen.PublicOpenedBorder[Dir4.Down].Length, Is.EqualTo(x));
                Assert.That(roomGen.PublicFulfillableBorder[Dir4.Down].Length, Is.EqualTo(x));
                Assert.That(roomGen.PublicOpenedBorder[Dir4.Up].Length, Is.EqualTo(x));
                Assert.That(roomGen.PublicFulfillableBorder[Dir4.Up].Length, Is.EqualTo(x));
                Assert.That(roomGen.PublicOpenedBorder[Dir4.Left].Length, Is.EqualTo(y));
                Assert.That(roomGen.PublicFulfillableBorder[Dir4.Left].Length, Is.EqualTo(y));
                Assert.That(roomGen.PublicOpenedBorder[Dir4.Right].Length, Is.EqualTo(y));
                Assert.That(roomGen.PublicFulfillableBorder[Dir4.Right].Length, Is.EqualTo(y));
            }
        }

        [Test]
        [TestCase(true, true, true, false)]
        [TestCase(false, false, false, false)]
        public void PrepareSizeIncompleteFulfillableException(bool openDown, bool openLeft, bool openUp, bool openRight)
        {
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGen = new TestRoomGenException<ITiledGenContext>
            {
                OpenDown = openDown,
                OpenLeft = openLeft,
                OpenUp = openUp,
                OpenRight = openRight,
            };
            Assert.Throws<ArgumentException>(() => { roomGen.PrepareSize(testRand.Object, new Loc(1)); });
        }

        [Test]

        // two rooms that are right next to each other, but offset
        [TestCase(2, 0, 3, 2, 0, 2, 4, 2, Dir4.Down, 2, 4, false)]

        // in another direction
        [TestCase(2, 3, 3, 4, 0, 1, 2, 4, Dir4.Left, 3, 5, false)]

        // two rooms separated by a one tile rift
        [TestCase(2, 0, 3, 2, 0, 3, 4, 2, Dir4.Down, 0, 0, true)]

        // two rooms overlapping each other
        [TestCase(2, 0, 3, 2, 0, 1, 4, 2, Dir4.Down, 0, 0, true)]

        // two rooms totally offset from each other
        [TestCase(8, 0, 3, 2, 0, 2, 4, 2, Dir4.Down, 0, 0, true)]
        public void ReceiveOpenedBorder(int x1, int y1, int w1, int h1, int x2, int y2, int w2, int h2, Dir4 dir, int expectedStart, int expectedEnd, bool exception)
        {
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGenTo = new TestRoomGen<ITiledGenContext>();
            var roomGenFrom = new TestRoomGen<ITiledGenContext>();
            roomGenTo.PrepareSize(testRand.Object, new Loc(w1, h1));
            roomGenTo.SetLoc(new Loc(x1, y1));
            roomGenFrom.PrepareSize(testRand.Object, new Loc(w2, h2));
            roomGenFrom.SetLoc(new Loc(x2, y2));
            for (int ii = 0; ii < roomGenFrom.PublicOpenedBorder[dir.Reverse()].Length; ii++)
                roomGenFrom.PublicOpenedBorder[dir.Reverse()][ii] = true;

            if (exception)
            {
                Assert.Throws<ArgumentException>(() => { roomGenTo.AskBorderFromRoom(roomGenFrom.Draw, roomGenFrom.GetOpenedBorder, dir); });
            }
            else
            {
                roomGenTo.AskBorderFromRoom(roomGenFrom.Draw, roomGenFrom.GetOpenedBorder, dir);
                IntRange newRange = roomGenTo.RoomSideReqs[dir][0];
                Assert.That(newRange, Is.EqualTo(new IntRange(expectedStart, expectedEnd)));
            }
        }

        [Test]
        [TestCase(false, true, false)]
        [TestCase(true, false, true)]
        public void ReceiveOpenedBorderToFulfill(bool firstHalf, bool secondHalf, bool exception)
        {
            // test with offset, proving previous openedborders are properly transferred
            // test error case, in which no bordertofulfill is opened
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGenTo = new TestRoomGen<ITiledGenContext>();
            var roomGenFrom = new TestRoomGen<ITiledGenContext>();
            roomGenTo.PrepareSize(testRand.Object, new Loc(3, 2));
            roomGenTo.SetLoc(new Loc(2, 0));
            roomGenFrom.PrepareSize(testRand.Object, new Loc(4, 2));
            roomGenFrom.SetLoc(new Loc(0, 2));
            roomGenFrom.PublicOpenedBorder[Dir4.Up][0] = firstHalf;
            roomGenFrom.PublicOpenedBorder[Dir4.Up][1] = firstHalf;
            roomGenFrom.PublicOpenedBorder[Dir4.Up][2] = secondHalf;
            roomGenFrom.PublicOpenedBorder[Dir4.Up][3] = secondHalf;

            if (exception)
            {
                Assert.Throws<ArgumentException>(() => { roomGenTo.AskBorderFromRoom(roomGenFrom.Draw, roomGenFrom.GetOpenedBorder, Dir4.Down); });
            }
            else
            {
                roomGenTo.AskBorderFromRoom(roomGenFrom.Draw, roomGenFrom.GetOpenedBorder, Dir4.Down);
                var expectedBorderToFulfill = new Dictionary<Dir4, bool[]>
                {
                    [Dir4.Down] = new bool[] { true, true, false },
                    [Dir4.Left] = new bool[] { false, false },
                    [Dir4.Up] = new bool[] { false, false, false },
                    [Dir4.Right] = new bool[] { false, false },
                };
                Assert.That(roomGenTo.PublicBorderToFulfill, Is.EqualTo(expectedBorderToFulfill));
            }
        }

        [Test]
        public void ReceiveOpenedBorderToFulfillAlreadyFilled()
        {
            // test error case, in which no borderfill is opened by the openedborder but the tiles already exist
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGenTo = new TestRoomGen<ITiledGenContext>();
            var roomGenFrom = new TestRoomGen<ITiledGenContext>();
            roomGenTo.PrepareSize(testRand.Object, new Loc(3, 2));
            roomGenTo.SetLoc(new Loc(2, 0));
            roomGenFrom.PrepareSize(testRand.Object, new Loc(4, 2));
            roomGenFrom.SetLoc(new Loc(0, 2));
            roomGenFrom.PublicOpenedBorder[Dir4.Up][0] = true;
            roomGenFrom.PublicOpenedBorder[Dir4.Up][1] = true;
            roomGenTo.PublicBorderToFulfill[Dir4.Down][0] = true;
            roomGenTo.PublicBorderToFulfill[Dir4.Down][1] = true;

            Assert.Throws<ArgumentException>(() => { roomGenTo.AskBorderFromRoom(roomGenFrom.Draw, roomGenFrom.GetOpenedBorder, Dir4.Down); });
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
            var roomGen = new TestRoomGen<ITiledGenContext>();
            roomGen.PrepareSize(testRand.Object, new Loc(5, 7));
            roomGen.SetLoc(new Loc(1, 2));

            if (exception)
            {
                Assert.Throws<ArgumentException>(() => { roomGen.AskBorderRange(new IntRange(rangeStart, rangeEnd), dir); });
            }
            else
            {
                roomGen.AskBorderRange(new IntRange(rangeStart, rangeEnd), dir);
                IntRange newRange = roomGen.RoomSideReqs[dir][0];
                Assert.That(newRange, Is.EqualTo(new IntRange(expectedStart, expectedEnd)));
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
            var roomGen = new TestRoomGenException<ITiledGenContext>
            {
                OpenDown = true,
                OpenLeft = true,
                OpenUp = true,
                OpenRight = true,
            };
            roomGen.PrepareSize(testRand.Object, new Loc(5, 7));
            roomGen.SetLoc(new Loc(1, 2));

            if (exception)
            {
                Assert.Throws<ArgumentException>(() => { roomGen.AskBorderRange(new IntRange(rangeStart, rangeEnd), dir); });
            }
            else
            {
                roomGen.AskBorderRange(new IntRange(rangeStart, rangeEnd), dir);
                IntRange newRange = roomGen.RoomSideReqs[dir][0];
                Assert.That(newRange, Is.EqualTo(new IntRange(expectedStart, expectedEnd)));
            }
        }

        [Test]
        public void ReceiveBorderRangeToFulfill()
        {
            // test with offset, proving previous openedborders are properly transferred
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGen = new TestRoomGen<ITiledGenContext>();
            roomGen.PrepareSize(testRand.Object, new Loc(5, 7));
            roomGen.SetLoc(new Loc(2, 3));

            roomGen.AskBorderRange(new IntRange(1, 6), Dir4.Right);
            var expectedBorderToFulfill = new Dictionary<Dir4, bool[]>
            {
                [Dir4.Down] = new bool[] { false, false, false, false, false },
                [Dir4.Left] = new bool[] { false, false, false, false, false, false, false },
                [Dir4.Up] = new bool[] { false, false, false, false, false },
                [Dir4.Right] = new bool[] { true, true, true, false, false, false, false },
            };
            Assert.That(roomGen.PublicBorderToFulfill, Is.EqualTo(expectedBorderToFulfill));
        }

        [Test]
        public void SetRoomBordersClear()
        {
            var roomGen = new TestRoomGen<ITiledGenContext>();
            string[] inGrid =
            {
                "XXXXXXXX",
                "XX.....X",
                "XX.....X",
                "XX.....X",
                "XX.....X",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            roomGen.PrepareSize(testRand.Object, new Loc(5, 4));
            roomGen.SetLoc(new Loc(2, 1));

            roomGen.SetRoomBorders(testContext);

            Assert.That(roomGen.PublicOpenedBorder[Dir4.Down], Is.EqualTo(new bool[] { true, true, true, true, true }));
            Assert.That(roomGen.PublicOpenedBorder[Dir4.Left], Is.EqualTo(new bool[] { true, true, true, true }));
            Assert.That(roomGen.PublicOpenedBorder[Dir4.Up], Is.EqualTo(new bool[] { true, true, true, true, true }));
            Assert.That(roomGen.PublicOpenedBorder[Dir4.Right], Is.EqualTo(new bool[] { true, true, true, true }));
        }

        [Test]
        public void SetRoomBordersSemiBlocked()
        {
            var roomGen = new TestRoomGen<ITiledGenContext>();
            string[] inGrid =
            {
                "XXXXXXXX",
                "XXX..X.X",
                "XX.....X",
                "XX....XX",
                "XX..XXXX",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            roomGen.PrepareSize(testRand.Object, new Loc(5, 4));
            roomGen.SetLoc(new Loc(2, 1));

            roomGen.SetRoomBorders(testContext);

            Assert.That(roomGen.PublicOpenedBorder[Dir4.Down], Is.EqualTo(new bool[] { true, true, false, false, false }));
            Assert.That(roomGen.PublicOpenedBorder[Dir4.Left], Is.EqualTo(new bool[] { false, true, true, true }));
            Assert.That(roomGen.PublicOpenedBorder[Dir4.Up], Is.EqualTo(new bool[] { false, true, true, false, true }));
            Assert.That(roomGen.PublicOpenedBorder[Dir4.Right], Is.EqualTo(new bool[] { true, true, false, false }));
        }

        [Test]
        public void SetRoomBordersSubFulfillable()
        {
            var roomGen = new TestRoomGenException<ITiledGenContext>
            {
                OpenDown = true,
                OpenLeft = true,
                OpenUp = true,
                OpenRight = true,
            };
            string[] inGrid =
            {
                "XXXXXXXX",
                "XX.....X",
                "XX.....X",
                "XX.....X",
                "XX.....X",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            roomGen.PrepareSize(testRand.Object, new Loc(5, 4));
            roomGen.SetLoc(new Loc(2, 1));

            roomGen.SetRoomBorders(testContext);

            Assert.That(roomGen.PublicOpenedBorder[Dir4.Down], Is.EqualTo(new bool[] { false, false, true, false, false }));
            Assert.That(roomGen.PublicOpenedBorder[Dir4.Left], Is.EqualTo(new bool[] { false, false, true, false }));
            Assert.That(roomGen.PublicOpenedBorder[Dir4.Up], Is.EqualTo(new bool[] { false, false, true, false, false }));
            Assert.That(roomGen.PublicOpenedBorder[Dir4.Right], Is.EqualTo(new bool[] { false, false, true, false }));
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
            // one or several sidereqs met with only one tile (do nothing)
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Mock<TestRoomGen<ITiledGenContext>> roomGen = new Mock<TestRoomGen<ITiledGenContext>> { CallBase = true };
            roomGen.Setup(p => p.DigAtBorder(It.IsAny<ITiledGenContext>(), It.IsAny<Dir4>(), It.IsAny<int>()));
            string[] inGrid =
            {
                "XXXXXXXX",
                "XX..X..X",
                "XX....XX",
                "XX....XX",
                "XX...XXX",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            roomGen.Object.PrepareSize(testRand.Object, new Loc(5, 4));
            roomGen.Object.SetLoc(new Loc(2, 1));

            // find where the class chose to dig
            if (reqDown)
                roomGen.Object.AskBorderRange(new IntRange(2, 7), Dir4.Down);
            if (reqLeft)
                roomGen.Object.AskBorderRange(new IntRange(1, 5), Dir4.Left);
            if (reqUp)
                roomGen.Object.AskBorderRange(new IntRange(2, 7), Dir4.Up);
            if (reqRight)
                roomGen.Object.AskBorderRange(new IntRange(1, 5), Dir4.Right);

            roomGen.Object.FulfillRoomBorders(testContext, false);

            roomGen.Verify(p => p.DigAtBorder(It.IsAny<ITiledGenContext>(), It.IsAny<Dir4>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        public void FulfillRoomBordersNoneMissingIntersect()
        {
            // two intersecting sidereqs met with only one tile (do nothing)
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Mock<TestRoomGen<ITiledGenContext>> roomGen = new Mock<TestRoomGen<ITiledGenContext>> { CallBase = true };
            roomGen.Setup(p => p.DigAtBorder(It.IsAny<ITiledGenContext>(), It.IsAny<Dir4>(), It.IsAny<int>()));
            string[] inGrid =
            {
                "XXXXXXXX",
                "XX.....X",
                "XX.....X",
                "XX.....X",
                "XXXX.XXX",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            roomGen.Object.PrepareSize(testRand.Object, new Loc(5, 4));
            roomGen.Object.SetLoc(new Loc(2, 1));

            // find where the class chose to dig
            roomGen.Object.AskBorderRange(new IntRange(4, 7), Dir4.Down);
            roomGen.Object.AskBorderRange(new IntRange(2, 5), Dir4.Down);
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
            // one sidereq not met (with rng)
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(1)).Returns(0);
            testRand.Setup(p => p.Next(4)).Returns(fulfillRoll);
            Mock<TestRoomGen<ITiledGenContext>> roomGen = new Mock<TestRoomGen<ITiledGenContext>> { CallBase = true };
            roomGen.Setup(p => p.DigAtBorder(It.IsAny<ITiledGenContext>(), It.IsAny<Dir4>(), It.IsAny<int>()));

            string[] inGrid =
            {
                "XXXXXXXX",
                "XX.....X",
                "XX.....X",
                "XX.....X",
                "XX.XXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            roomGen.Object.PrepareSize(testRand.Object, new Loc(5, 4));
            roomGen.Object.SetLoc(new Loc(2, 1));

            // find where the class chose to dig
            roomGen.Object.AskBorderRange(new IntRange(3, 7), Dir4.Down);
            roomGen.Object.FulfillRoomBorders(testContext, false);

            roomGen.Verify(p => p.DigAtBorder(testContext, Dir4.Down, expectedX), Times.Once());
            testRand.Verify(p => p.Next(1), Times.Exactly(1));
            testRand.Verify(p => p.Next(4), Times.Exactly(1));
        }

        [Test]
        public void FulfillRoomBordersNoneOneMissingIntersect()
        {
            // two intersecting sidereqs not met
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(2));
            seq = seq.Returns(1);
            seq = seq.Returns(0);
            testRand.Setup(p => p.Next(1)).Returns(0);
            Mock<TestRoomGen<ITiledGenContext>> roomGen = new Mock<TestRoomGen<ITiledGenContext>> { CallBase = true };
            roomGen.Setup(p => p.DigAtBorder(It.IsAny<ITiledGenContext>(), It.IsAny<Dir4>(), It.IsAny<int>()));
            string[] inGrid =
            {
                "XXXXXXXX",
                "XX.....X",
                "XX.....X",
                "XX.....X",
                "XX.XXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            roomGen.Object.PrepareSize(testRand.Object, new Loc(5, 4));
            roomGen.Object.SetLoc(new Loc(2, 1));

            // find where the class chose to dig
            roomGen.Object.AskBorderRange(new IntRange(3, 6), Dir4.Down);
            roomGen.Object.AskBorderRange(new IntRange(4, 7), Dir4.Down);
            roomGen.Object.FulfillRoomBorders(testContext, false);

            roomGen.Verify(p => p.DigAtBorder(testContext, Dir4.Down, 4), Times.Once());
            testRand.Verify(p => p.Next(2), Times.Exactly(2));
            testRand.Verify(p => p.Next(1), Times.Exactly(1));
        }

        [Test]
        public void FulfillRoomBordersNoneOneMissingFillAll()
        {
            // one sidereq not met, fulfilled completely, only fulfillable borders
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Mock<TestRoomGen<ITiledGenContext>> roomGen = new Mock<TestRoomGen<ITiledGenContext>> { CallBase = true };
            roomGen.Setup(p => p.DigAtBorder(It.IsAny<ITiledGenContext>(), It.IsAny<Dir4>(), It.IsAny<int>()));
            string[] inGrid =
            {
                "XXXXXXXX",
                "XX.....X",
                "XX.....X",
                "XX.....X",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            roomGen.Object.PrepareSize(testRand.Object, new Loc(5, 4));
            roomGen.Object.SetLoc(new Loc(2, 1));
            roomGen.Object.PublicFulfillableBorder[Dir4.Down][2] = false;
            roomGen.Object.PublicFulfillableBorder[Dir4.Down][3] = false;

            // find where the class chose to dig
            roomGen.Object.AskBorderRange(new IntRange(2, 7), Dir4.Down);
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
            // full sidereq not met on one side, one adjacent sidereq not met on a corner tile
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(4));
            seq = seq.Returns(roll1);
            seq = testRand.SetupSequence(p => p.Next(3));
            seq = seq.Returns(roll2);
            seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            Mock<TestRoomGen<ITiledGenContext>> roomGen = new Mock<TestRoomGen<ITiledGenContext>> { CallBase = true };
            roomGen.Setup(p => p.DigAtBorder(It.IsAny<ITiledGenContext>(), It.IsAny<Dir4>(), It.IsAny<int>()));
            string[] inGrid =
            {
                "XXXXXXXX",
                "XX.....X",
                "XX....XX",
                "XX....XX",
                "XX.XXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            roomGen.Object.PrepareSize(testRand.Object, new Loc(5, 4));
            roomGen.Object.SetLoc(new Loc(2, 1));

            // find where the class chose to dig
            roomGen.Object.AskBorderRange(new IntRange(3, 7), Dir4.Down);
            roomGen.Object.AskBorderRange(new IntRange(2, 5), Dir4.Right);
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
            var roomGen = new TestRoomGen<ITiledGenContext>();
            string[] inGrid =
            {
                "XXXXXXXX",
                "XX..X..X",
                "XX....XX",
                "XX...XXX",
                "XX....XX",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            string[] outGrid;
            switch (resultGrid)
            {
                case 1:
                    outGrid = new string[]
                    {
                        "XXXXXXXX",
                        "XX.....X",
                        "XX....XX",
                        "XX...XXX",
                        "XX....XX",
                        "XXXXXXXX",
                        "XXXXXXXX",
                    };
                    break;
                case 2:
                    outGrid = new string[]
                    {
                        "XXXXXXXX",
                        "XX..X..X",
                        "XX....XX",
                        "XX.....X",
                        "XX....XX",
                        "XXXXXXXX",
                        "XXXXXXXX",
                    };
                    break;
                case 3:
                    outGrid = new string[]
                    {
                        "XXXXXXXX",
                        "XX..X..X",
                        "XX.....X",
                        "XX...X.X",
                        "XX.....X",
                        "XXXXXX.X",
                        "XXXXXXXX",
                    };
                    break;
                default:
                    outGrid = inGrid;
                    break;
            }

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            roomGen.PrepareSize(testRand.Object, new Loc(5, 5));
            roomGen.SetLoc(new Loc(2, 1));

            if (exception)
            {
                Assert.Throws<ArgumentException>(() => { roomGen.DigAtBorder(testContext, dir, scalar); });
            }
            else
            {
                roomGen.DigAtBorder(testContext, dir, scalar);
                Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            }
        }

        [Test]
        public void ChoosePossibleStartsOneReq()
        {
            // one sidereq, 3 tiles
            var testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(1)).Returns(0);
            var roomGen = new TestRoomGen<ITiledGenContext>();
            bool[] permittedRange = { true, true, true, true, true };
            var sideReqs = new List<IntRange> { new IntRange(5, 8) };
            var compare = new List<HashSet<int>>();
            var set = new HashSet<int> { 5, 6, 7 };
            compare.Add(set);

            List<HashSet<int>> result = roomGen.PublicChoosePossibleStarts(testRand.Object, 4, permittedRange, sideReqs);

            Assert.That(result, Is.EqualTo(compare));
            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(1));
        }

        [Test]
        public void ChoosePossibleStartsOneReqRestricted()
        {
            // one sidereq, 3 tiles, one allowed
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(1)).Returns(0);
            var roomGen = new TestRoomGen<ITiledGenContext>();
            bool[] permittedRange = { true, false, true, false, true };
            List<IntRange> sideReqs = new List<IntRange> { new IntRange(5, 8) };
            List<HashSet<int>> compare = new List<HashSet<int>>();
            HashSet<int> set = new HashSet<int> { 6 };

            List<HashSet<int>> result = roomGen.PublicChoosePossibleStarts(testRand.Object, 4, permittedRange, sideReqs);
            compare.Add(set);

            Assert.That(result, Is.EqualTo(compare));
            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(1));
        }

        [Test]
        public void ChoosePossibleStartsTwoReqs()
        {
            // two non-overlapping sidereqs, 2 tiles each
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(2)).Returns(1);
            testRand.Setup(p => p.Next(1)).Returns(0);
            var roomGen = new TestRoomGen<ITiledGenContext>();
            bool[] permittedRange = { true, true, true, true, true };
            List<IntRange> sideReqs = new List<IntRange>
            {
                new IntRange(4, 6),
                new IntRange(7, 9),
            };
            List<HashSet<int>> compare = new List<HashSet<int>>();
            HashSet<int> set = new HashSet<int> { 7, 8 };
            compare.Add(set);
            set = new HashSet<int> { 4, 5 };
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
            // two overlapping sidereqs, 3 tiles each (rng should make no difference)
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(2)).Returns(roll1);
            testRand.Setup(p => p.Next(1)).Returns(0);
            var roomGen = new TestRoomGen<ITiledGenContext>();
            bool[] permittedRange = { true, true, true, true, true };
            List<IntRange> sideReqs = new List<IntRange>
            {
                new IntRange(4, 7),
                new IntRange(5, 8),
            };
            List<HashSet<int>> compare = new List<HashSet<int>>();
            HashSet<int> set = new HashSet<int> { 5, 6 };
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
            // two overlapping sidereqs, one superset of the other (rng should make no difference)
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(2)).Returns(roll1);
            testRand.Setup(p => p.Next(1)).Returns(0);
            var roomGen = new TestRoomGen<ITiledGenContext>();
            bool[] permittedRange = { true, true, true, true, true };
            List<IntRange> sideReqs = new List<IntRange>
            {
                new IntRange(4, 9),
                new IntRange(6, 8),
            };
            List<HashSet<int>> compare = new List<HashSet<int>>();
            HashSet<int> set = new HashSet<int> { 6, 7 };
            compare.Add(set);

            List<HashSet<int>> result = roomGen.PublicChoosePossibleStarts(testRand.Object, 4, permittedRange, sideReqs);

            Assert.That(result, Is.EqualTo(compare));
            testRand.Verify(p => p.Next(2), Times.Exactly(1));
            testRand.Verify(p => p.Next(1), Times.Exactly(1));
        }

        [Test]
        public void ChoosePossibleStartsTwoReqsOverlapRestricted()
        {
            // two overlapping sidereqs, 3 tiles each, two allowed
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(2)).Returns(0);
            testRand.Setup(p => p.Next(1)).Returns(0);
            var roomGen = new TestRoomGen<ITiledGenContext>();
            bool[] permittedRange = { true, false, false, true, true };
            List<IntRange> sideReqs = new List<IntRange>
            {
                new IntRange(4, 7),
                new IntRange(5, 8),
            };
            List<HashSet<int>> compare = new List<HashSet<int>>();
            HashSet<int> set = new HashSet<int> { 4 };
            compare.Add(set);
            set = new HashSet<int> { 7 };
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
            // two separate sidereqs, with one in the middle connecting (with different rng)
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(3)).Returns(roll1);
            testRand.Setup(p => p.Next(2)).Returns(roll2);
            testRand.Setup(p => p.Next(1)).Returns(0);
            var roomGen = new TestRoomGen<ITiledGenContext>();
            bool[] permittedRange = { true, true, true, true, true };
            List<IntRange> sideReqs = new List<IntRange>
            {
                new IntRange(4, 6),
                new IntRange(5, 8),
                new IntRange(7, 9),
            };
            List<HashSet<int>> compare = new List<HashSet<int>>();
            HashSet<int> set = new HashSet<int> { 5 };
            compare.Add(set);
            set = new HashSet<int> { 7, 8 };
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
            // two separate sidereqs, with one in the middle connecting (with different rng)
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(3)).Returns(roll1);
            testRand.Setup(p => p.Next(2)).Returns(roll2);
            testRand.Setup(p => p.Next(1)).Returns(0);
            var roomGen = new TestRoomGen<ITiledGenContext>();
            bool[] permittedRange = { true, true, true, true, true };
            List<IntRange> sideReqs = new List<IntRange>
            {
                new IntRange(4, 6),
                new IntRange(5, 8),
                new IntRange(7, 9),
            };
            List<HashSet<int>> compare = new List<HashSet<int>>();
            HashSet<int> set = new HashSet<int> { 7 };
            compare.Add(set);
            set = new HashSet<int> { 4, 5 };
            compare.Add(set);

            List<HashSet<int>> result = roomGen.PublicChoosePossibleStarts(testRand.Object, 4, permittedRange, sideReqs);

            Assert.That(result, Is.EqualTo(compare));
            testRand.Verify(p => p.Next(3), Times.Exactly(1));
            testRand.Verify(p => p.Next(2), Times.Exactly(1));
            testRand.Verify(p => p.Next(1), Times.Exactly(1));
        }

        public class TestRoomGenException<T> : TestRoomGen<T>
            where T : ITiledGenContext
        {
            public bool OpenDown { get; set; }

            public bool OpenLeft { get; set; }

            public bool OpenUp { get; set; }

            public bool OpenRight { get; set; }

            protected override void PrepareFulfillableBorders(IRandom rand)
            {
                this.FulfillableBorder[Dir4.Down][this.FulfillableBorder[Dir4.Down].Length / 2] = this.OpenDown;
                this.FulfillableBorder[Dir4.Left][this.FulfillableBorder[Dir4.Left].Length / 2] = this.OpenLeft;
                this.FulfillableBorder[Dir4.Up][this.FulfillableBorder[Dir4.Up].Length / 2] = this.OpenUp;
                this.FulfillableBorder[Dir4.Right][this.FulfillableBorder[Dir4.Right].Length / 2] = this.OpenRight;
            }
        }

        public class TestRoomGen<T> : RoomGen<T>
            where T : ITiledGenContext
        {
            public new Dictionary<Dir4, List<IntRange>> RoomSideReqs => base.RoomSideReqs;

            public Dictionary<Dir4, bool[]> PublicOpenedBorder => this.OpenedBorder;

            public Dictionary<Dir4, bool[]> PublicFulfillableBorder => this.FulfillableBorder;

            public Dictionary<Dir4, bool[]> PublicBorderToFulfill => this.BorderToFulfill;

            public override RoomGen<T> Copy() => new TestRoomGen<T>();

            public override Loc ProposeSize(IRandom rand) => Loc.Zero;

            public override void DrawOnMap(T map)
            {
            }

            public List<HashSet<int>> PublicChoosePossibleStarts(IRandom rand, int scalarStart, bool[] permittedRange, List<IntRange> origSideReqs)
            {
                return this.ChoosePossibleStartRanges(rand, scalarStart, permittedRange, origSideReqs);
            }

            protected override void PrepareFulfillableBorders(IRandom rand)
            {
                foreach (Dir4 dir in DirExt.VALID_DIR4)
                {
                    for (int jj = 0; jj < this.FulfillableBorder[dir].Length; jj++)
                        this.FulfillableBorder[dir][jj] = true;
                }
            }
        }
    }
}
