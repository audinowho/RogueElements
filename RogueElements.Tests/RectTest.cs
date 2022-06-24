// <copyright file="RectTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using NUnit.Framework;
using RogueElements;

namespace RogueElements.Tests
{
    [TestFixture]
    public class RectTest
    {
        [Test]
        [TestCase(2, 4, Dir4.Down, 2)]
        [TestCase(2, 4, Dir4.Right, 4)]
        [TestCase(2, 4, Dir4.Up, 2)]
        [TestCase(2, 4, Dir4.Left, 4)]
        public void GetBorderLength(int w, int h, Dir4 dir, int result)
        {
            Rect rect = new Rect(0, 0, w, h);
            Assert.That(rect.GetBorderLength(dir), Is.EqualTo(result));
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
            Rect rect = new Rect(x, y, width, length);

            if (exception)
                Assert.Throws<ArgumentException>(() => { rect.GetEdgeLoc(dir, scalar); });
            else
                Assert.That(rect.GetEdgeLoc(dir, scalar), Is.EqualTo(new Loc(expectedX, expectedY)));
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
        public void GetRectEdgeLoc(
            int x,
            int y,
            int width,
            int length,
            Dir4 dir,
            int sizeX,
            int sizeY,
            int scalar,
            int expectedX,
            int expectedY,
            bool exception)
        {
            Rect rect = new Rect(x, y, width, length);

            if (exception)
                Assert.Throws<ArgumentException>(() => { rect.GetEdgeRectLoc(dir, new Loc(sizeX, sizeY), scalar); });
            else
                Assert.That(rect.GetEdgeRectLoc(dir, new Loc(sizeX, sizeY), scalar), Is.EqualTo(new Loc(expectedX, expectedY)));
        }
    }
}
