// <copyright file="WrappedCollisionTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class WrappedCollisionTest
    {
        [Test]
        [TestCase(10, 2, 4, 4, 4, true)]
        [TestCase(10, 4, 4, 2, 4, true)]
        [TestCase(10, 2, 4, 6, 2, false)]
        [TestCase(10, 2, 6, 4, 2, true)]
        [TestCase(10, 4, 2, 2, 6, true)]
        [TestCase(10, 7, 4, -1, 4, true)]
        [TestCase(10, 9, 4, -3, 4, true)]
        [TestCase(10, 7, 4, 1, 2, false)]
        [TestCase(10, 7, 6, -1, 2, true)]
        [TestCase(10, 9, 2, -3, 6, true)]
        public void Collides(int wrappedSize, int start1, int size1, int start2, int size2, bool expected)
        {
            for (int ii = -1; ii <= 1; ii++)
            {
                for (int jj = -1; jj <= 1; jj++)
                    Assert.That(WrappedCollision.Collides(wrappedSize, start1 + (ii * wrappedSize), size1, start2 + (jj * wrappedSize), size2), Is.EqualTo(expected));
            }
        }

        [Test]
        [TestCase(10, 0, 5, 0, true)]
        [TestCase(10, 0, 5, 4, true)]
        [TestCase(10, 0, 5, 5, false)]
        [TestCase(10, 0, 5, -1, false)]
        [TestCase(10, 0, 10, 0, true)]
        [TestCase(10, 0, 10, 9, true)]
        [TestCase(10, 0, 10, 10, true)]
        [TestCase(10, 5, 10, 0, true)]
        [TestCase(10, 5, 10, 5, true)]
        [TestCase(10, 5, 10, 9, true)]
        [TestCase(10, 5, 10, 10, true)]
        [TestCase(10, 8, 5, 7, false)]
        [TestCase(10, 8, 5, 8, true)]
        [TestCase(10, 8, 5, 9, true)]
        [TestCase(10, 8, 5, 10, true)]
        [TestCase(10, 8, 5, 12, true)]
        [TestCase(10, 8, 5, 13, false)]
        public void InBounds(int wrappedSize, int start, int size, int pt, bool expected)
        {
            for (int ii = -1; ii <= 1; ii++)
            {
                for (int jj = -1; jj <= 1; jj++)
                    Assert.That(WrappedCollision.InBounds(wrappedSize, start + (ii * wrappedSize), size, pt + (jj * wrappedSize)), Is.EqualTo(expected));
            }
        }
    }
}
