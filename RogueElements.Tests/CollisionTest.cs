// <copyright file="CollisionTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class CollisionTest
    {
        [Test]
        [TestCase(0, 0, Dir8.None, 0, true)]
        [TestCase(0, 0, Dir8.Down, 0, true)]
        [TestCase(0, 1, Dir8.Down, 0, false)]
        [TestCase(0, 1, Dir8.Down, 1, true)]
        [TestCase(0, 10, Dir8.Down, 9, false)]
        [TestCase(0, 10, Dir8.Down, -1, true)]
        [TestCase(1, 1, Dir8.Down, 1, false)]
        [TestCase(0, 1, Dir8.DownRight, 1, false)]
        [TestCase(1, 1, Dir8.DownRight, 1, true)]
        [TestCase(10, 10, Dir8.DownRight, 10, true)]
        [TestCase(9, 10, Dir8.DownRight, 10, false)]
        [TestCase(9, 10, Dir8.Down, 10, false)]
        [TestCase(0, 0, (Dir8)(-2), 0, false, true)]
        [TestCase(0, 0, (Dir8)8, 0, false, true)]
        public void InFront(int testX, int testY, Dir8 dir, int range, bool expected, bool exception = false)
        {
            if (exception)
                Assert.Throws<ArgumentException>(() => { Collision.InFront(new Loc(testX, testY), dir, range); });
            else
                Assert.That(Collision.InFront(new Loc(testX, testY), dir, range), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(2, 4, 4, 4, true)]
        [TestCase(4, 4, 2, 4, true)]
        [TestCase(2, 4, 6, 2, false)]
        [TestCase(2, 6, 4, 2, true)]
        [TestCase(4, 2, 2, 6, true)]
        public void Collides(int start1, int size1, int start2, int size2, bool expected)
        {
            Assert.That(Collision.Collides(start1, size1, start2, size2), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(5, 0, true)]
        [TestCase(5, 4, true)]
        [TestCase(5, 5, false)]
        [TestCase(5, -1, false)]
        [TestCase(-5, 0, false)]
        [TestCase(-5, -1, false)]
        [TestCase(-5, -5, false)]
        [TestCase(-5, -6, false)]
        public void InBounds(int size, int pt, bool expected)
        {
            Assert.That(Collision.InBounds(size, pt), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(2, 3, 0, 0, 0, 0)]
        [TestCase(2, 3, -1, -1, 0, 0)]
        [TestCase(2, 3, 2, 3, 1, 2)]
        [TestCase(2, 3, -1, 3, 0, 2)]
        [TestCase(2, 3, 2, -1, 1, 0)]
        [TestCase(2, 3, 1, 2, 1, 2)]
        public void ClampToBounds(int sizeX, int sizeY, int ptX, int ptY, int expectedX, int expectedY)
        {
            Assert.That(Collision.ClampToBounds(sizeX, sizeY, new Loc(ptX, ptY)), Is.EqualTo(new Loc(expectedX, expectedY)));
        }
    }
}
