// <copyright file="RoomGenSquareTest.cs" company="Audino">
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
    public class RoomGenSquareTest
    {
        [Test]
        public void ProposeSize()
        {
            // just check for corner cases
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(3, 5)).Returns(3);
            testRand.Setup(p => p.Next(4, 7)).Returns(4);
            RoomGenSquare<ITiledGenContext> roomGen = new RoomGenSquare<ITiledGenContext>(new RandRange(3, 5), new RandRange(4, 7));

            Loc compare = roomGen.ProposeSize(testRand.Object);

            Assert.That(compare, Is.EqualTo(new Loc(3, 4)));
            testRand.Verify(p => p.Next(3, 5), Times.Exactly(1));
            testRand.Verify(p => p.Next(4, 7), Times.Exactly(1));
        }

        [Test]
        public void DrawOnMap()
        {
            // verify it fills up the entire square area!
            var roomGen = new RoomGenSquare<ITiledGenContext>();
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
                "XX.....X",
                "XX.....X",
                "XX.....X",
                "XXXXXXXX",
                "XXXXXXXX",
            };

            TestGenContext testContext = TestGenContext.InitGridToContext(inGrid);
            TestGenContext resultContext = TestGenContext.InitGridToContext(outGrid);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            roomGen.PrepareSize(testRand.Object, new Loc(5, 4));
            roomGen.SetLoc(new Loc(2, 1));

            roomGen.DrawOnMap(testContext);
            Assert.That(testContext.Tiles, Is.EqualTo(resultContext.Tiles));
        }
    }
}
