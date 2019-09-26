// <copyright file="RoomGenBlockedTest.cs" company="Audino">
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
    public class RoomGenBlockedTest
    {
        [Test]
        public void ProposeSize()
        {
            // just check for corner cases
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(3, 5)).Returns(3);
            testRand.Setup(p => p.Next(4, 7)).Returns(4);
            var roomGen = new RoomGenBlocked<ITiledGenContext>(new TestTile(1), new RandRange(3, 5), new RandRange(4, 7), RandRange.Empty, RandRange.Empty);

            Loc compare = roomGen.ProposeSize(testRand.Object);

            Assert.That(compare, Is.EqualTo(new Loc(3, 4)));
            testRand.Verify(p => p.Next(3, 5), Times.Exactly(1));
            testRand.Verify(p => p.Next(4, 7), Times.Exactly(1));
        }

        [Test]
        public void DrawOnMapNone()
        {
            // verify it fills up the entire square area!
            var roomGen = new RoomGenBlocked<ITiledGenContext>
            {
                BlockWidth = new RandRange(0),
                BlockHeight = new RandRange(0),
                BlockTerrain = new TestTile(1),
            };
            string[] inGrid =
            {
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            string[] outGrid =
            {
                "XXXXXXXX",
                "X......X",
                "X......X",
                "X......X",
                "X......X",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(0, 0)).Returns(0);
            testRand.Setup(p => p.Next(1, 5)).Returns(1);
            testRand.Setup(p => p.Next(1, 3)).Returns(1);

            testContext.SetTestRand(testRand.Object);
            roomGen.PrepareSize(testRand.Object, new Loc(6, 4));
            roomGen.SetLoc(new Loc(1, 1));

            roomGen.DrawOnMap(testContext);
            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(0, 0), Times.Exactly(2));
            testRand.Verify(p => p.Next(1, 5), Times.Exactly(1));
            testRand.Verify(p => p.Next(1, 3), Times.Exactly(1));
        }

        [Test]
        public void DrawOnMapMin()
        {
            // verify it fills up the entire square area!
            var roomGen = new RoomGenBlocked<ITiledGenContext>
            {
                BlockWidth = new RandRange(2),
                BlockHeight = new RandRange(1),
                BlockTerrain = new TestTile(1),
            };
            string[] inGrid =
            {
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            string[] outGrid =
            {
                "XXXXXXXX",
                "X......X",
                "X.XX...X",
                "X......X",
                "X......X",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(1, 1)).Returns(1);
            testRand.Setup(p => p.Next(2, 2)).Returns(2);
            testRand.Setup(p => p.Next(1, 3)).Returns(1);
            testRand.Setup(p => p.Next(1, 2)).Returns(1);

            testContext.SetTestRand(testRand.Object);
            roomGen.PrepareSize(testRand.Object, new Loc(6, 4));
            roomGen.SetLoc(new Loc(1, 1));

            roomGen.DrawOnMap(testContext);
            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(1, 1), Times.Exactly(1));
            testRand.Verify(p => p.Next(2, 2), Times.Exactly(1));
            testRand.Verify(p => p.Next(1, 3), Times.Exactly(1));
            testRand.Verify(p => p.Next(1, 2), Times.Exactly(1));
        }

        [Test]
        public void DrawOnMapMax()
        {
            // verify it fills up the entire square area!
            var roomGen = new RoomGenBlocked<ITiledGenContext>
            {
                BlockWidth = new RandRange(2),
                BlockHeight = new RandRange(1),
                BlockTerrain = new TestTile(1),
            };
            string[] inGrid =
            {
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            string[] outGrid =
            {
                "XXXXXXXX",
                "X......X",
                "X......X",
                "X...XX.X",
                "X......X",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(1, 1)).Returns(1);
            testRand.Setup(p => p.Next(2, 2)).Returns(2);
            testRand.Setup(p => p.Next(1, 3)).Returns(3);
            testRand.Setup(p => p.Next(1, 2)).Returns(2);

            testContext.SetTestRand(testRand.Object);
            roomGen.PrepareSize(testRand.Object, new Loc(6, 4));
            roomGen.SetLoc(new Loc(1, 1));

            roomGen.DrawOnMap(testContext);
            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(1, 1), Times.Exactly(1));
            testRand.Verify(p => p.Next(2, 2), Times.Exactly(1));
            testRand.Verify(p => p.Next(1, 3), Times.Exactly(1));
            testRand.Verify(p => p.Next(1, 2), Times.Exactly(1));
        }

        [Test]
        public void DrawOnMapOversize()
        {
            // verify it fills up the entire square area!
            var roomGen = new RoomGenBlocked<ITiledGenContext>
            {
                BlockWidth = new RandRange(200),
                BlockHeight = new RandRange(100),
                BlockTerrain = new TestTile(1),
            };
            string[] inGrid =
            {
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            string[] outGrid =
            {
                "XXXXXXXX",
                "X......X",
                "X.XXXX.X",
                "X.XXXX.X",
                "X......X",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(100, 100)).Returns(100);
            testRand.Setup(p => p.Next(200, 200)).Returns(200);
            testRand.Setup(p => p.Next(1, 1)).Returns(1);

            testContext.SetTestRand(testRand.Object);
            roomGen.PrepareSize(testRand.Object, new Loc(6, 4));
            roomGen.SetLoc(new Loc(1, 1));

            roomGen.DrawOnMap(testContext);
            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            testRand.Verify(p => p.Next(100, 100), Times.Exactly(1));
            testRand.Verify(p => p.Next(200, 200), Times.Exactly(1));
            testRand.Verify(p => p.Next(1, 1), Times.Exactly(2));
        }

        [Test]
        [Ignore("TODO")]
        public void DrawOnMapBlock()
        {
            // smallest block
            // largest block
            throw new NotImplementedException();
        }
    }
}
