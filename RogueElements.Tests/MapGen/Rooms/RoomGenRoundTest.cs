// <copyright file="RoomGenRoundTest.cs" company="Audino">
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
    public class RoomGenRoundTest
    {
        [Test]
        [Ignore("TODO")]
        public void ProposeSize()
        {
            // just check for corner cases
            throw new NotImplementedException();
        }

        [Test]
        public void DrawOnMap1x1()
        {
            // normal circle 1x1
            Mock<RoomGenRound<ITiledGenContext>> roomGen = new Mock<RoomGenRound<ITiledGenContext>> { CallBase = true };
            roomGen.Setup(p => p.SetRoomBorders(It.IsAny<ITiledGenContext>()));
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
                "XX.XXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
            };

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
            // normal circle 4x1
            Mock<RoomGenRound<ITiledGenContext>> roomGen = new Mock<RoomGenRound<ITiledGenContext>> { CallBase = true };
            roomGen.Setup(p => p.SetRoomBorders(It.IsAny<ITiledGenContext>()));
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
                "XX....XX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
                "XXXXXXXX",
            };

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
            // normal circle 7x7
            Mock<RoomGenRound<ITiledGenContext>> roomGen = new Mock<RoomGenRound<ITiledGenContext>> { CallBase = true };
            roomGen.Setup(p => p.SetRoomBorders(It.IsAny<ITiledGenContext>()));
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

            string[] outGrid =
            {
                "XXXXXXXXXX",
                "XXXX...XXX",
                "XXX.....XX",
                "XX.......X",
                "XX.......X",
                "XX.......X",
                "XXX.....XX",
                "XXXX...XXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
            };

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
            // normal circle 8x8
            Mock<RoomGenRound<ITiledGenContext>> roomGen = new Mock<RoomGenRound<ITiledGenContext>> { CallBase = true };
            roomGen.Setup(p => p.SetRoomBorders(It.IsAny<ITiledGenContext>()));
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

            string[] outGrid =
            {
                "XXXXXXXXXX",
                "XXX....XXX",
                "XX......XX",
                "X........X",
                "X........X",
                "X........X",
                "X........X",
                "XX......XX",
                "XXX....XXX",
                "XXXXXXXXXX",
            };

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
            // larger height circle 7x4
            Mock<RoomGenRound<ITiledGenContext>> roomGen = new Mock<RoomGenRound<ITiledGenContext>> { CallBase = true };
            roomGen.Setup(p => p.SetRoomBorders(It.IsAny<ITiledGenContext>()));
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

            string[] outGrid =
            {
                "XXXXXXXXXX",
                "XXX.....XX",
                "XX.......X",
                "XX.......X",
                "XXX.....XX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
            };

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
            // larger width circle 4x7
            Mock<RoomGenRound<ITiledGenContext>> roomGen = new Mock<RoomGenRound<ITiledGenContext>> { CallBase = true };
            roomGen.Setup(p => p.SetRoomBorders(It.IsAny<ITiledGenContext>()));
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

            string[] outGrid =
            {
                "XXXXXXXXXX",
                "XXX..XXXXX",
                "XX....XXXX",
                "XX....XXXX",
                "XX....XXXX",
                "XX....XXXX",
                "XX....XXXX",
                "XXX..XXXXX",
                "XXXXXXXXXX",
                "XXXXXXXXXX",
            };

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
            // normal circle 1x1
            Mock<IRandom> mockRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGen = new TestRoomGenRound<ITiledGenContext>();
            roomGen.PrepareSize(mockRand.Object, new Loc(1, 1));

            var expectedFulfillable = new Dictionary<Dir4, bool[]>
            {
                [Dir4.Down] = new bool[] { true },
                [Dir4.Left] = new bool[] { true },
                [Dir4.Up] = new bool[] { true },
                [Dir4.Right] = new bool[] { true },
            };

            Assert.That(roomGen.PublicFulfillableBorder, Is.EqualTo(expectedFulfillable));
        }

        [Test]
        public void PrepareRequestedBorders4x1()
        {
            // normal circle 4x1
            Mock<IRandom> mockRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGen = new TestRoomGenRound<ITiledGenContext>();
            roomGen.PrepareSize(mockRand.Object, new Loc(4, 1));

            var expectedFulfillable = new Dictionary<Dir4, bool[]>
            {
                [Dir4.Down] = new bool[] { true, true, true, true },
                [Dir4.Left] = new bool[] { true },
                [Dir4.Up] = new bool[] { true, true, true, true },
                [Dir4.Right] = new bool[] { true },
            };

            Assert.That(roomGen.PublicFulfillableBorder, Is.EqualTo(expectedFulfillable));
        }

        [Test]
        public void PrepareRequestedBorders7x7()
        {
            // normal circle 7x7
            // normal circle 6x6
            // larger width circle 4x8
            // larger height circle 8x4
            Mock<IRandom> mockRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGen = new TestRoomGenRound<ITiledGenContext>();
            roomGen.PrepareSize(mockRand.Object, new Loc(7, 7));

            var expectedFulfillable = new Dictionary<Dir4, bool[]>
            {
                [Dir4.Down] = new bool[] { false, false, true, true, true, false, false },
                [Dir4.Left] = new bool[] { false, false, true, true, true, false, false },
                [Dir4.Up] = new bool[] { false, false, true, true, true, false, false },
                [Dir4.Right] = new bool[] { false, false, true, true, true, false, false },
            };

            Assert.That(roomGen.PublicFulfillableBorder, Is.EqualTo(expectedFulfillable));
        }

        [Test]
        public void PrepareRequestedBorders8x8()
        {
            // normal circle 8x8
            // larger width circle 4x8
            // larger height circle 8x4
            Mock<IRandom> mockRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGen = new TestRoomGenRound<ITiledGenContext>();
            roomGen.PrepareSize(mockRand.Object, new Loc(8, 8));

            var expectedFulfillable = new Dictionary<Dir4, bool[]>
            {
                [Dir4.Down] = new bool[] { false, false, true, true, true, true, false, false },
                [Dir4.Left] = new bool[] { false, false, true, true, true, true, false, false },
                [Dir4.Up] = new bool[] { false, false, true, true, true, true, false, false },
                [Dir4.Right] = new bool[] { false, false, true, true, true, true, false, false },
            };

            Assert.That(roomGen.PublicFulfillableBorder, Is.EqualTo(expectedFulfillable));
        }

        [Test]
        public void PrepareRequestedBorders4x7()
        {
            // larger height circle 4x7
            // larger width circle 7x4
            Mock<IRandom> mockRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGen = new TestRoomGenRound<ITiledGenContext>();
            roomGen.PrepareSize(mockRand.Object, new Loc(4, 7));

            var expectedFulfillable = new Dictionary<Dir4, bool[]>
            {
                [Dir4.Down] = new bool[] { false, true, true, false },
                [Dir4.Left] = new bool[] { false, true, true, true, true, true, false },
                [Dir4.Up] = new bool[] { false, true, true, false },
                [Dir4.Right] = new bool[] { false, true, true, true, true, true, false },
            };

            Assert.That(roomGen.PublicFulfillableBorder, Is.EqualTo(expectedFulfillable));
        }

        [Test]
        public void PrepareRequestedBorders7x4()
        {
            // larger width circle 7x4
            Mock<IRandom> mockRand = new Mock<IRandom>(MockBehavior.Strict);
            var roomGen = new TestRoomGenRound<ITiledGenContext>();
            roomGen.PrepareSize(mockRand.Object, new Loc(7, 4));

            var expectedFulfillable = new Dictionary<Dir4, bool[]>
            {
                [Dir4.Down] = new bool[] { false, true, true, true, true, true, false },
                [Dir4.Left] = new bool[] { false, true, true, false },
                [Dir4.Up] = new bool[] { false, true, true, true, true, true, false },
                [Dir4.Right] = new bool[] { false, true, true, false },
            };

            Assert.That(roomGen.PublicFulfillableBorder, Is.EqualTo(expectedFulfillable));
        }

        public class TestRoomGenRound<T> : RoomGenRound<T>
            where T : ITiledGenContext
        {
            public Dictionary<Dir4, bool[]> PublicFulfillableBorder => this.FulfillableBorder;
        }
    }
}
