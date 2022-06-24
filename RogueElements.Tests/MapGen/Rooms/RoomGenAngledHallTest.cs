// <copyright file="RoomGenAngledHallTest.cs" company="Audino">
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
    public class RoomGenAngledHallTest
    {
        public static void SetBorderInfo(RoomGenAngledHall<ITiledGenContext> roomGen, string[] inGrid)
        {
            int upStart = -1;
            int downStart = -1;
            for (int ii = 0; ii < inGrid[0].Length; ii++)
            {
                if (inGrid[0][ii] == '.' && upStart == -1)
                {
                    upStart = ii;
                }
                else if (inGrid[0][ii] == 'X' && upStart > -1)
                {
                    roomGen.AskBorderRange(new IntRange(upStart, ii), Dir4.Up);
                    upStart = -1;
                }

                if (inGrid[inGrid.Length - 1][ii] == '.' && downStart == -1)
                {
                    downStart = ii;
                }
                else if (inGrid[inGrid.Length - 1][ii] == 'X' && downStart > -1)
                {
                    roomGen.AskBorderRange(new IntRange(downStart, ii), Dir4.Down);
                    downStart = -1;
                }
            }

            if (upStart > -1)
                roomGen.AskBorderRange(new IntRange(upStart, inGrid[0].Length), Dir4.Up);
            if (downStart > -1)
                roomGen.AskBorderRange(new IntRange(downStart, inGrid[0].Length), Dir4.Down);

            int leftStart = -1;
            int rightStart = -1;
            for (int ii = 0; ii < inGrid.Length; ii++)
            {
                if (inGrid[ii][0] == '.' && leftStart == -1)
                {
                    leftStart = ii;
                }
                else if (inGrid[ii][0] == 'X' && leftStart > -1)
                {
                    roomGen.AskBorderRange(new IntRange(leftStart, ii), Dir4.Left);
                    leftStart = -1;
                }

                if (inGrid[ii][inGrid[0].Length - 1] == '.' && rightStart == -1)
                {
                    rightStart = ii;
                }
                else if (inGrid[ii][inGrid[0].Length - 1] == 'X' && rightStart > -1)
                {
                    roomGen.AskBorderRange(new IntRange(rightStart, ii), Dir4.Right);
                    rightStart = -1;
                }
            }

            if (leftStart > -1)
                roomGen.AskBorderRange(new IntRange(leftStart, inGrid.Length), Dir4.Left);
            if (rightStart > -1)
                roomGen.AskBorderRange(new IntRange(rightStart, inGrid.Length), Dir4.Right);
        }

        [Test]
        [Ignore("TODO")]
        public void ProposeSize()
        {
            // just 1
            throw new NotImplementedException();
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(50, 50)]
        public void DrawOnMapStraightIntersect(int bias, int biasRoll)
        {
            // two overlapping sides (no bias)
            // two overlapping sides (half bias 49)
            Mock<RoomGenAngledHall<ITiledGenContext>> roomGen = new Mock<RoomGenAngledHall<ITiledGenContext>> { CallBase = true };
            roomGen.Object.HallTurnBias = bias;
            roomGen.Object.Brush = new DefaultHallBrush();
            roomGen.Setup(p => p.SetRoomBorders(It.IsAny<ITiledGenContext>()));

            string[] inGrid =
            {
                "XXXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXX.",
                ".XXXXXXXX.",
                ".XXXXXXXX.",
                "XXXXXXXXX.",
                "XXXXXXXXX.",
                "XXXXXXXXX.",
                "XXXXXXXXXX",
            };

            string[] outGrid =
            {
                "XXXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXX.",
                "..........",
                ".XXXXXXXX.",
                "XXXXXXXXX.",
                "XXXXXXXXX.",
                "XXXXXXXXX.",
                "XXXXXXXXXX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(biasRoll);
            seq = seq.Returns(0);
            seq = testRand.SetupSequence(p => p.Next(3));
            seq = seq.Returns(1);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.Object.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.Object.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen.Object, inGrid);

            roomGen.Object.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            roomGen.Verify(p => p.SetRoomBorders(testContext), Times.Once());
            testRand.Verify(p => p.Next(1), Times.Exactly(2));
            testRand.Verify(p => p.Next(100), Times.Exactly(2));
            testRand.Verify(p => p.Next(3), Times.Exactly(1));
        }

        [Test]
        [TestCase(100, 0)]
        [TestCase(50, 49)]
        public void DrawOnMapStraightIntersectTurned(int bias, int biasRoll)
        {
            // two overlapping sides (all bias)
            // two overlapping sides (half bias 50)
            var roomGen = new RoomGenAngledHall<ITiledGenContext>(bias);
            string[] inGrid =
            {
                "XXXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXX.",
                ".XXXXXXXX.",
                ".XXXXXXXX.",
                "XXXXXXXXX.",
                "XXXXXXXXX.",
                "XXXXXXXXX.",
                "XXXXXXXXXX",
            };

            string[] outGrid =
            {
                "XXXXXXXXXX",
                ".XXXXXXXXX",
                "...XXXXXXX",
                ".X.XXXXXX.",
                ".X.XXXXXX.",
                ".X.XXXXXX.",
                "XX........",
                "XXXXXXXXX.",
                "XXXXXXXXX.",
                "XXXXXXXXXX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(biasRoll);
            seq = seq.Returns(0);

            // startside left
            seq = testRand.SetupSequence(p => p.Next(5));
            seq = seq.Returns(1);

            // endside right
            seq = testRand.SetupSequence(p => p.Next(6));
            seq = seq.Returns(3);

            // point of angling
            seq = testRand.SetupSequence(p => p.Next(2, 8));
            seq = seq.Returns(2);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(1), Times.Exactly(2));
            testRand.Verify(p => p.Next(100), Times.Exactly(2));
            testRand.Verify(p => p.Next(5), Times.Exactly(1));
            testRand.Verify(p => p.Next(6), Times.Exactly(1));
            testRand.Verify(p => p.Next(2, 8), Times.Exactly(1));
        }

        [Test]
        public void DrawOnMapStraightIntersectForceTurned()
        {
            // no overlap, force no turn (and result in defaulting to turn)
            var roomGen = new RoomGenAngledHall<ITiledGenContext>(100);
            string[] inGrid =
            {
                "XXXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXX.",
                "XXXXXXXXX.",
                "XXXXXXXXX.",
                "XXXXXXXXXX",
            };

            string[] outGrid =
            {
                "XXXXXXXXXX",
                "........XX",
                ".XXXXXX.XX",
                ".XXXXXX.XX",
                ".XXXXXX.XX",
                "XXXXXXX.XX",
                "XXXXXXX.X.",
                "XXXXXXX.X.",
                "XXXXXXX...",
                "XXXXXXXXXX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // startside left
            seq = testRand.SetupSequence(p => p.Next(4));
            seq = seq.Returns(0);

            // endside right
            seq = testRand.SetupSequence(p => p.Next(3));
            seq = seq.Returns(2);

            // point of angling
            seq = testRand.SetupSequence(p => p.Next(2, 8));
            seq = seq.Returns(7);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(1), Times.Exactly(2));
            testRand.Verify(p => p.Next(100), Times.Exactly(2));
            testRand.Verify(p => p.Next(4), Times.Exactly(1));
            testRand.Verify(p => p.Next(3), Times.Exactly(1));
            testRand.Verify(p => p.Next(2, 8), Times.Exactly(1));
        }

        [Test]
        public void DrawOnMapStraightIntersectForceNoTurn()
        {
            // one-tile overlap, force turn (and result in defaulting to no turn)
            var roomGen = new RoomGenAngledHall<ITiledGenContext>();
            string[] inGrid =
            {
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                ".XXXXXXXX.",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
            };

            string[] outGrid =
            {
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "..........",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(1), Times.Exactly(3));
            testRand.Verify(p => p.Next(100), Times.Exactly(2));
        }

        [Test]
        public void DrawOnMapStraightIntersectBigBrushWithRoom()
        {
            // straight hall but with a larger brush
            var roomGen = new RoomGenAngledHall<ITiledGenContext>(0, new SquareHallBrush(new Loc(2)));
            string[] inGrid =
            {
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                ".XXXXXXXX.",
                ".XXXXXXXX.",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
            };

            string[] outGrid =
            {
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "..........",
                "..........",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(1), Times.Exactly(3));
            testRand.Verify(p => p.Next(100), Times.Exactly(2));
        }

        [Test]
        public void DrawOnMapStraightIntersectBigBrush()
        {
            // straight hall but with a larger brush
            var roomGen = new RoomGenAngledHall<ITiledGenContext>(0, new SquareHallBrush(new Loc(2)));
            string[] inGrid =
            {
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                ".XXXXXXXX.",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
            };

            string[] outGrid =
            {
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "..........",
                "X........X",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(1), Times.Exactly(3));
            testRand.Verify(p => p.Next(100), Times.Exactly(2));
        }

        [Test]
        public void DrawOnMapStraightIntersectBigBrushBorder()
        {
            // straight hall with a large brush, but hitting the border
            var roomGen = new RoomGenAngledHall<ITiledGenContext>(0, new SquareHallBrush(new Loc(2)));
            string[] inGrid =
            {
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                ".XXXXXXXX.",
                "XXXXXXXXXX",
            };

            string[] outGrid =
            {
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "..........",
                "XXXXXXXXXX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(1), Times.Exactly(3));
            testRand.Verify(p => p.Next(100), Times.Exactly(2));
        }

        [Test]
        [TestCase(0)]
        [TestCase(100)]
        public void DrawOnMapAngleIntersect(int bias)
        {
            // bias will not affect the outcome here
            var roomGen = new RoomGenAngledHall<ITiledGenContext>(bias);
            string[] inGrid =
            {
                "XXXXX...XX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
            };

            string[] outGrid =
            {
                "XXXXX...XX",
                "XXXXXX.XXX",
                "XXXXXX.XXX",
                ".......XXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // selected sidereqs
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // calls to place the halls
            seq = testRand.SetupSequence(p => p.Next(4));
            seq = seq.Returns(0);
            seq = testRand.SetupSequence(p => p.Next(3));
            seq = seq.Returns(1);
            seq = testRand.SetupSequence(p => p.Next(2));
            seq = seq.Returns(1);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(1), Times.Exactly(2));
            testRand.Verify(p => p.Next(4), Times.Exactly(1));
            testRand.Verify(p => p.Next(3), Times.Exactly(1));
            testRand.Verify(p => p.Next(2), Times.Exactly(1));
        }

        [Test]
        public void DrawCombinedHall()
        {
            // basic case
            var roomGen = new RoomGenAngledHall<ITiledGenContext>();
            string[] inGrid =
            {
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
            };

            string[] outGrid =
            {
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXX.XXXX",
                "XXXXX.XXXX",
            };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(10, 5));
            roomGen.SetLoc(new Loc(0, 0));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawCombinedHall(testContext, Dir4.Down, 3, new int[] { 5 });

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
        }

        [Test]
        public void DrawOnMap3Intersect()
        {
            // opposite sides: not crooked
            var roomGen = new RoomGenAngledHall<ITiledGenContext>();
            string[] inGrid =
            {
                "XXXX.....X",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXX...XXXX",
            };

            string[] outGrid =
            {
                "XXXX.....X",
                "XXXXX.XXXX",
                "XXXXX.XXXX",
                ".XXXX.XXXX",
                "......XXXX",
                ".XXXX.XXXX",
                ".XXXX.XXXX",
                "XXXXX.XXXX",
                "XXXXX.XXXX",
                "XXX...XXXX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // selected sidereqs
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // selected turn/no turn
            seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // calls to place the halls
            seq = testRand.SetupSequence(p => p.Next(2));
            seq = seq.Returns(1);
            seq = testRand.SetupSequence(p => p.Next(4));
            seq = seq.Returns(1);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);
            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(1), Times.Exactly(3));
            testRand.Verify(p => p.Next(100), Times.Exactly(2));
            testRand.Verify(p => p.Next(2), Times.Exactly(1));
            testRand.Verify(p => p.Next(4), Times.Exactly(1));
        }

        [Test]
        public void DrawOnMap3IntersectTurned()
        {
            // opposite sides: crooked, intersecting on the crooked point
            var roomGen = new RoomGenAngledHall<ITiledGenContext>(100);
            string[] inGrid =
            {
                "XXXX.....X",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXX...XXXX",
            };

            string[] outGrid =
            {
                "XXXX.....X",
                "XXXXXXX.XX",
                "XXXXXXX.XX",
                ".XXXXXX.XX",
                "........XX",
                ".XXXX.XXXX",
                ".XXXX.XXXX",
                "XXXXX.XXXX",
                "XXXXX.XXXX",
                "XXX...XXXX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // selected sidereqs
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // selected turn/no turn
            seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // calls to place the halls
            seq = testRand.SetupSequence(p => p.Next(3));
            seq = seq.Returns(2);
            seq = testRand.SetupSequence(p => p.Next(4));

            // the top sidereq has only 4 choices due to having one tile subtracted
            seq = seq.Returns(2);
            seq = seq.Returns(1);
            seq = testRand.SetupSequence(p => p.Next(2, 8));
            seq = seq.Returns(4);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(1), Times.Exactly(3));
            testRand.Verify(p => p.Next(100), Times.Exactly(2));
            testRand.Verify(p => p.Next(3), Times.Exactly(1));
            testRand.Verify(p => p.Next(4), Times.Exactly(2));
            testRand.Verify(p => p.Next(2, 8), Times.Exactly(1));
        }

        [Test]
        public void DrawOnMap3IntersectTurnedIntersected()
        {
            // opposite sides: crooked, intersecting beside the crooked point
            var roomGen = new RoomGenAngledHall<ITiledGenContext>(100);
            string[] inGrid =
            {
                "XXXX.....X",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXX...XXXX",
            };

            string[] outGrid =
            {
                "XXXX.....X",
                "XXXXXXX.XX",
                "XXXXXXX.XX",
                "........XX",
                ".XXXX...XX",
                ".XXXX.XXXX",
                ".XXXX.XXXX",
                "XXXXX.XXXX",
                "XXXXX.XXXX",
                "XXX...XXXX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // selected sidereqs
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // selected turn/no turn
            seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // calls to place the halls
            seq = testRand.SetupSequence(p => p.Next(3));
            seq = seq.Returns(2);
            seq = testRand.SetupSequence(p => p.Next(4));

            // the top sidereq has only 4 choices due to having one tile subtracted
            seq = seq.Returns(2);
            seq = seq.Returns(0);
            seq = testRand.SetupSequence(p => p.Next(2, 8));
            seq = seq.Returns(4);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(1), Times.Exactly(3));
            testRand.Verify(p => p.Next(100), Times.Exactly(2));
            testRand.Verify(p => p.Next(3), Times.Exactly(1));
            testRand.Verify(p => p.Next(4), Times.Exactly(2));
            testRand.Verify(p => p.Next(2, 8), Times.Exactly(1));
        }

        [Test]
        public void DrawOnMap4IntersectNoTurn()
        {
            // no sides crooked
            var roomGen = new RoomGenAngledHall<ITiledGenContext>(0);
            string[] inGrid =
            {
                "XXXX.....X",
                "XXXXXXXXXX",
                "XXXXXXXXX.",
                ".XXXXXXXX.",
                ".XXXXXXXX.",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXX...XXXX",
            };

            string[] outGrid =
            {
                "XXXX.....X",
                "XXXXX.XXXX",
                "XXXXX.XXX.",
                ".XXXX.XXX.",
                "..........",
                ".XXXX.XXXX",
                ".XXXX.XXXX",
                "XXXXX.XXXX",
                "XXXXX.XXXX",
                "XXX...XXXX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // selected sidereqs
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // selected turn/no turn
            seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // calls to place the halls
            seq = testRand.SetupSequence(p => p.Next(2));
            seq = seq.Returns(1);
            seq = seq.Returns(1);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(Loc.One);
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(1), Times.Exactly(4));
            testRand.Verify(p => p.Next(100), Times.Exactly(2));
            testRand.Verify(p => p.Next(2), Times.Exactly(2));
        }

        [Test]
        public void DrawOnMap4IntersectHorizTurn()
        {
            // horiz sides crooked
            var roomGen = new RoomGenAngledHall<ITiledGenContext>(50);
            string[] inGrid =
            {
                "XXXX.....X",
                "XXXXXXXXXX",
                "XXXXXXXXX.",
                ".XXXXXXXX.",
                ".XXXXXXXX.",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXX...XXXX",
            };

            string[] outGrid =
            {
                "XXXX.....X",
                "XXXXX.XXXX",
                "XXXXX.....",
                ".XXXX.XXX.",
                "......XXX.",
                ".XXXX.XXXX",
                ".XXXX.XXXX",
                "XXXXX.XXXX",
                "XXXXX.XXXX",
                "XXX...XXXX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // selected sidereqs
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // selected turn/no turn
            seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(99);

            // calls to place the halls
            seq = testRand.SetupSequence(p => p.Next(2));
            seq = seq.Returns(1);
            seq = testRand.SetupSequence(p => p.Next(4));
            seq = seq.Returns(1);
            seq = testRand.SetupSequence(p => p.Next(3));
            seq = seq.Returns(0);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(1), Times.Exactly(4));
            testRand.Verify(p => p.Next(100), Times.Exactly(2));
            testRand.Verify(p => p.Next(2), Times.Exactly(1));
            testRand.Verify(p => p.Next(4), Times.Exactly(1));
            testRand.Verify(p => p.Next(3), Times.Exactly(1));
        }

        [Test]
        public void DrawOnMap4IntersectBothTurn()
        {
            // both sides crooked
            var roomGen = new RoomGenAngledHall<ITiledGenContext>(100);
            string[] inGrid =
            {
                "XXXX.....X",
                "XXXXXXXXXX",
                "XXXXXXXXX.",
                ".XXXXXXXX.",
                ".XXXXXXXX.",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXX...XXXX",
            };

            string[] outGrid =
            {
                "XXXX.....X",
                "XXXXXXXX.X",
                "XX........",
                ".X.XX.XXX.",
                "...XX.XXX.",
                ".XXXX.XXXX",
                ".XXXX.XXXX",
                "XXXXX.XXXX",
                "XXXXX.XXXX",
                "XXX...XXXX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // selected sidereqs
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // selected turn/no turn
            seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // calls to place the halls
            seq = testRand.SetupSequence(p => p.Next(3));
            seq = seq.Returns(0);
            seq = seq.Returns(2);
            seq = testRand.SetupSequence(p => p.Next(4));
            seq = seq.Returns(1);
            seq = seq.Returns(3);
            seq = testRand.SetupSequence(p => p.Next(2, 8));
            seq = seq.Returns(2);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(1), Times.Exactly(4));
            testRand.Verify(p => p.Next(100), Times.Exactly(2));
            testRand.Verify(p => p.Next(3), Times.Exactly(2));
            testRand.Verify(p => p.Next(4), Times.Exactly(2));
            testRand.Verify(p => p.Next(2, 8), Times.Exactly(1));
        }

        [Test]
        [TestCase(0)]
        [TestCase(100)]
        public void DrawOnMapStraightFork(int bias)
        {
            // 1 to many
            // turn off turn bias
            var roomGen = new RoomGenAngledHall<ITiledGenContext>(bias);
            string[] inGrid =
            {
                "X..X.....X",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXX...XXXX",
            };

            string[] outGrid =
            {
                "X..X.....X",
                "XX.XX.XXXX",
                "XX.XX.XXXX",
                "XX.XX.XXXX",
                "XX.XX.XXXX",
                "XX....XXXX",
                "XXXX.XXXXX",
                "XXXX.XXXXX",
                "XXXX.XXXXX",
                "XXX...XXXX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // selected sidereqs
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = testRand.SetupSequence(p => p.Next(2));
            seq = seq.Returns(0);
            seq = seq.Returns(1);

            // selected turn/no turn
            seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // calls to place the halls
            seq = testRand.SetupSequence(p => p.Next(3));
            seq = seq.Returns(1);
            seq = testRand.SetupSequence(p => p.Next(5));
            seq = seq.Returns(1);
            seq = testRand.SetupSequence(p => p.Next(2, 8));
            seq = seq.Returns(5);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(1), Times.Exactly(2));
            testRand.Verify(p => p.Next(100), Times.Exactly(2));
            testRand.Verify(p => p.Next(2), Times.Exactly(2));
            testRand.Verify(p => p.Next(3), Times.Exactly(1));
            testRand.Verify(p => p.Next(5), Times.Exactly(1));
            testRand.Verify(p => p.Next(2, 8), Times.Exactly(1));
        }

        [Test]
        [TestCase(0)]
        [TestCase(100)]
        public void DrawOnMapStraightCross(int bias)
        {
            // many to many
            // turn off turn bias
            var roomGen = new RoomGenAngledHall<ITiledGenContext>(bias);
            string[] inGrid =
            {
                "X..X.....X",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "X.X...X..X",
            };

            string[] outGrid =
            {
                "X..X.....X",
                "XX.XXXX.XX",
                "XX.XXXX.XX",
                "XX.XXXX.XX",
                "XX.XXXX.XX",
                "XX.XXXX.XX",
                "X........X",
                "X.XX.XXX.X",
                "X.XX.XXX.X",
                "X.X...X..X",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // selected sidereqs
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(3));
            seq = seq.Returns(0);
            seq = seq.Returns(1);
            seq = testRand.SetupSequence(p => p.Next(2));
            seq = seq.Returns(0);
            seq = seq.Returns(1);
            seq = seq.Returns(1);
            seq = seq.Returns(1);
            seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // selected turn/no turn
            seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // calls to place the halls
            seq = testRand.SetupSequence(p => p.Next(5));
            seq = seq.Returns(3);
            seq = testRand.SetupSequence(p => p.Next(2, 8));
            seq = seq.Returns(6);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(1), Times.Exactly(3));
            testRand.Verify(p => p.Next(2), Times.Exactly(4));
            testRand.Verify(p => p.Next(3), Times.Exactly(2));
            testRand.Verify(p => p.Next(100), Times.Exactly(2));
            testRand.Verify(p => p.Next(5), Times.Exactly(1));
            testRand.Verify(p => p.Next(2, 8), Times.Exactly(1));
        }

        [Test]
        [TestCase(0)]
        [TestCase(100)]
        public void DrawOnMapTurnCross(int bias)
        {
            // many to many
            // turn off turn bias
            var roomGen = new RoomGenAngledHall<ITiledGenContext>(bias);
            string[] inGrid =
            {
                "XXXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                "XXXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                "XXXXXXXXXX",
                ".XXXXXXXXX",
                "XXXXXXXXXX",
                "X.X...X..X",
            };

            string[] outGrid =
            {
                "XXXXXXXXXX",
                ".XXXXXXXXX",
                "........XX",
                "XXXXXXX.XX",
                ".XXXXXX.XX",
                "........XX",
                "XXXXXXX.XX",
                "........XX",
                "X.XX.XX.XX",
                "X.X...X..X",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // selected sidereqs
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(3));
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // start points
            seq = seq.Returns(1);
            seq = testRand.SetupSequence(p => p.Next(2));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // start points
            seq = seq.Returns(1);
            seq = seq.Returns(1);
            seq = seq.Returns(1);
            seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // start points
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // selected turn/no turn
            seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(3), Times.Exactly(3));
            testRand.Verify(p => p.Next(2), Times.Exactly(6));
            testRand.Verify(p => p.Next(1), Times.Exactly(4));
        }

        [Test]
        [TestCase(0)]
        [TestCase(100)]
        public void DrawOnMap3Cross(int bias)
        {
            // many to many
            var roomGen = new RoomGenAngledHall<ITiledGenContext>(bias);
            string[] inGrid =
            {
                "X..X.....X",
                "XXXXXXXXXX",
                "XXXXXXXXX.",
                "XXXXXXXXX.",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXX.",
                "XX..X.X.XX",
            };

            string[] outGrid =
            {
                "X..X.....X",
                "XX.XX.XXXX",
                "XX.XX.XXX.",
                "XX.XX.....",
                "XX.XX.XXXX",
                "XX.XX.XXXX",
                "XX......XX",
                "XXX.X.X.XX",
                "XXX.X.X...",
                "XX..X.X.XX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // selected sidereqs
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(3));
            seq = seq.Returns(0);

            // start points
            seq = testRand.SetupSequence(p => p.Next(2));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // start points
            seq = seq.Returns(1);
            seq = seq.Returns(1);
            seq = seq.Returns(1);
            seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // start points
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = testRand.SetupSequence(p => p.Next(5));

            // start points
            seq = seq.Returns(1);

            // selected turn/no turn
            seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // where to bend the halls
            seq = testRand.SetupSequence(p => p.Next(2, 8));
            seq = seq.Returns(6);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(3), Times.Exactly(1));
            testRand.Verify(p => p.Next(2), Times.Exactly(6));
            testRand.Verify(p => p.Next(1), Times.Exactly(6));
            testRand.Verify(p => p.Next(5), Times.Exactly(1));
            testRand.Verify(p => p.Next(100), Times.Exactly(2));
            testRand.Verify(p => p.Next(2, 8), Times.Exactly(1));
        }

        [Test]
        [TestCase(0)]
        [TestCase(100)]
        public void DrawOnMap4Cross(int bias)
        {
            // many to many
            var roomGen = new RoomGenAngledHall<ITiledGenContext>(bias);
            string[] inGrid =
            {
                "X..X.....X",
                "XXXXXXXXXX",
                ".XXXXXXXX.",
                "XXXXXXXXX.",
                "XXXXXXXXXX",
                ".XXXXXXXXX",
                ".XXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXX.",
                "XX..X.X.XX",
            };

            string[] outGrid =
            {
                "X..X.....X",
                "XX.XX.XXXX",
                "......XXX.",
                "XXXXX.....",
                "XXXXX.XXXX",
                ".XXXX.XXXX",
                "......XXXX",
                "XXX.X.XXXX",
                "XXX.X.....",
                "XX..X.X.XX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // selected sidereqs
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(3));
            seq = seq.Returns(0);

            // start points
            seq = testRand.SetupSequence(p => p.Next(2));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // start points
            seq = seq.Returns(1);
            seq = seq.Returns(1);
            seq = seq.Returns(1);
            seq = seq.Returns(1);
            seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // start points
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = seq.Returns(0);
            seq = testRand.SetupSequence(p => p.Next(5));

            // start points
            seq = seq.Returns(1);

            // selected turn/no turn
            seq = testRand.SetupSequence(p => p.Next(100));
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // where to bend the halls
            seq = testRand.SetupSequence(p => p.Next(2, 8));
            seq = seq.Returns(5);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(3), Times.Exactly(1));
            testRand.Verify(p => p.Next(2), Times.Exactly(8));
            testRand.Verify(p => p.Next(1), Times.Exactly(8));
            testRand.Verify(p => p.Next(5), Times.Exactly(1));
            testRand.Verify(p => p.Next(100), Times.Exactly(2));
            testRand.Verify(p => p.Next(2, 8), Times.Exactly(1));
        }

        [Test]
        [TestCase(0)]
        [TestCase(100)]
        public void DrawOnMap1Single(int bias)
        {
            // many to many
            var roomGen = new RoomGenAngledHall<ITiledGenContext>(bias);
            string[] inGrid =
            {
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XX....XXXX",
            };

            string[] outGrid =
            {
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXX.XXXXX",
                "XXXX.XXXXX",
                "XXXX.XXXXX",
                "XXXX.XXXXX",
                "XX....XXXX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // selected sidereqs
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);

            // start points
            seq = testRand.SetupSequence(p => p.Next(4));
            seq = seq.Returns(2);

            // where to bend the halls
            seq = testRand.SetupSequence(p => p.Next(2, 8));
            seq = seq.Returns(5);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(1), Times.Exactly(1));
            testRand.Verify(p => p.Next(4), Times.Exactly(1));
            testRand.Verify(p => p.Next(2, 8), Times.Exactly(1));
        }

        [Test]
        [TestCase(0)]
        [TestCase(100)]
        public void DrawOnMap1Cross(int bias)
        {
            // many to many
            var roomGen = new RoomGenAngledHall<ITiledGenContext>(bias);
            string[] inGrid =
            {
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XX..X.X.XX",
            };

            string[] outGrid =
            {
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXX.....XX",
                "XXX.X.X.XX",
                "XXX.X.X.XX",
                "XXX.X.X.XX",
                "XXX.X.X.XX",
                "XXX.X.X.XX",
                "XX..X.X.XX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // selected sidereqs
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(3));
            seq = seq.Returns(0);
            seq = testRand.SetupSequence(p => p.Next(2));
            seq = seq.Returns(0);

            // start points
            seq = seq.Returns(1);
            seq = testRand.SetupSequence(p => p.Next(1));
            seq = seq.Returns(0);

            // start points
            seq = seq.Returns(0);
            seq = seq.Returns(0);

            // where to bend the halls
            seq = testRand.SetupSequence(p => p.Next(2, 8));
            seq = seq.Returns(3);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            roomGen.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(3), Times.Exactly(1));
            testRand.Verify(p => p.Next(2), Times.Exactly(2));
            testRand.Verify(p => p.Next(1), Times.Exactly(3));
            testRand.Verify(p => p.Next(2, 8), Times.Exactly(1));
        }

        [Test]
        public void DrawOnMapNone()
        {
            // with no inputs, the generator throws an exception
            var roomGen = new RoomGenAngledHall<ITiledGenContext>(0);
            string[] inGrid =
            {
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            roomGen.PrepareSize(testContext.Rand, new Loc(8, 8));
            roomGen.SetLoc(new Loc(1, 1));
            SetBorderInfo(roomGen, inGrid);

            Assert.Throws<ArgumentException>(() => { roomGen.DrawOnMap(testContext); });
        }
    }
}
