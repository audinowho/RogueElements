// <copyright file="RoomGenCrossTest.cs" company="Audino">
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
    public class RoomGenCrossTest
    {
        [Test]
        public void ProposeSize()
        {
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(3, 5)).Returns(3);
            testRand.Setup(p => p.Next(4, 7)).Returns(4);
            var roomGen = new RoomGenCross<ITiledGenContext>(new RandRange(3, 5), new RandRange(4, 7), new RandRange(3, 5), new RandRange(4, 7));

            Loc compare = roomGen.ProposeSize(testRand.Object);

            Assert.That(compare, Is.EqualTo(new Loc(3, 4)));
            testRand.Verify(p => p.Next(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
        }

        [Test]
        public void DrawOnMap()
        {
            // verify pieces stay in contact even with adversarial rolls
            Mock<RoomGenCross<ITiledGenContext>> roomGen = new Mock<RoomGenCross<ITiledGenContext>> { CallBase = true };
            roomGen.Setup(p => p.SetRoomBorders(It.IsAny<ITiledGenContext>()));
            roomGen.Object.MajorWidth = new RandRange(2, 6);
            roomGen.Object.MajorHeight = new RandRange(3, 9);
            roomGen.Object.MinorWidth = new RandRange(1, 2);
            roomGen.Object.MinorHeight = new RandRange(1, 2);
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
                "XX.....X",
                "XXXXXX.X",
                "XXXXXX.X",
                "XXXXXX.X",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(1, 2));
            seq = seq.Returns(1);
            seq = seq.Returns(1);
            testRand.Setup(p => p.Next(5)).Returns(4);
            testRand.Setup(p => p.Next(4)).Returns(0);
            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            testContext.SetTestRand(testRand.Object);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            roomGen.Object.PrepareSize(testContext.Rand, new Loc(5, 4));
            roomGen.Object.SetLoc(new Loc(2, 1));

            roomGen.Object.DrawOnMap(testContext);

            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
            roomGen.Verify(p => p.SetRoomBorders(testContext), Times.Once());
            testRand.Verify(p => p.Next(1, 2), Times.Exactly(2));
            testRand.Verify(p => p.Next(5), Times.Exactly(1));
            testRand.Verify(p => p.Next(4), Times.Exactly(1));
        }

        [Test]
        public void PrepareFulfillableBorders()
        {
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(1, 3)).Returns(1);
            testRand.Setup(p => p.Next(1, 7)).Returns(2);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(2));
            seq = seq.Returns(1);
            seq = seq.Returns(0);
            var roomGen = new TestRoomGenCross<ITiledGenContext>
            {
                MajorWidth = new RandRange(2, 4),
                MajorHeight = new RandRange(3, 9),
                MinorWidth = new RandRange(1, 3),
                MinorHeight = new RandRange(1, 7),
            };

            var expectedFulfillable = new Dictionary<Dir4, bool[]>
            {
                [Dir4.Down] = new bool[] { false, true },
                [Dir4.Left] = new bool[] { true, true, false },
                [Dir4.Up] = new bool[] { false, true },
                [Dir4.Right] = new bool[] { true, true, false },
            };

            roomGen.PrepareSize(testRand.Object, new Loc(2, 3));

            Assert.That(roomGen.PublicFulfillableBorder, Is.EqualTo(expectedFulfillable));
            testRand.Verify(p => p.Next(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(2));
        }

        public class TestRoomGenCross<T> : RoomGenCross<T>
            where T : ITiledGenContext
        {
            public Dictionary<Dir4, bool[]> PublicFulfillableBorder => this.FulfillableBorder;
        }
    }
}
