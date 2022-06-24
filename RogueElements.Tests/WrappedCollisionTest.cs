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

        [Test]
        [TestCase(2, 2, 4, 2, 4, 2)]
        [TestCase(2, 2, 4, 6, 4, 6)]
        [TestCase(2, 2, 4, 8, 4, -2)]
        [TestCase(2, 2, 8, 4, -2, 4)]
        [TestCase(2, 2, 9, 9, -1, -1)]
        public void GetClosestWrapLoc(int pt1x, int pt1y, int pt2x, int pt2y, int expectedx, int expectedy)
        {
            Assert.That(WrappedCollision.GetClosestWrap(new Loc(10), new Loc(pt1x, pt1y), new Loc(pt2x, pt2y)), Is.EqualTo(new Loc(expectedx, expectedy)));
        }

        [Test]
        [TestCase(10, 0, 0, 0)]
        [TestCase(10, 5, 5, 5)]
        [TestCase(10, 10, 10, 10)]
        [TestCase(10, 0, 10, 0)]
        [TestCase(10, 10, 0, 10)]
        [TestCase(10, 1, 3, 3)]
        [TestCase(10, 1, 5, 5)]
        [TestCase(10, 1, 6, 6)]
        [TestCase(10, 1, 7, -3)]
        public void GetClosestWrap(int wrapSize, int pt1, int pt2, int expected)
        {
            for (int ii = -1; ii <= 1; ii++)
            {
                for (int jj = -1; jj <= 1; jj++)
                    Assert.That(WrappedCollision.GetClosestWrap(wrapSize, pt1 + (ii * wrapSize), pt2 + (jj * wrapSize)), Is.EqualTo(expected + (ii * wrapSize)));
            }
        }

        // Normal case: the region is within the wrap bounds.
        [Test]
        [TestCase(10, 0, 3, 0, true)]
        [TestCase(10, 0, 3, 2, true)]
        [TestCase(10, 0, 3, 3, false)]
        [TestCase(10, 0, 3, -1, false)]
        [TestCase(10, 3, 3, 3, true)]
        [TestCase(10, 3, 3, 5, true)]
        [TestCase(10, 3, 3, 2, false)]
        [TestCase(10, 3, 3, 6, false)]

        // Edge case: region intersects the wrap bounds
        [TestCase(10, -2, 5, -2, true)]
        [TestCase(10, -2, 5, 2, true)]
        [TestCase(10, -2, 5, 3, false)]
        [TestCase(10, -2, 5, -3, false)]
        [TestCase(10, 8, 5, 8, true)]
        [TestCase(10, 8, 5, 12, true)]
        [TestCase(10, 8, 5, 13, false)]
        [TestCase(10, 8, 5, 7, false)]
        public void IteratePointsInBounds(int wrapSize, int start, int size, int pt, bool containsResult)
        {
            for (int ii = -1; ii <= 1; ii++)
            {
                int shiftPt = pt + (ii * wrapSize);
                List<int> results = new List<int>();
                List<int> expected = new List<int>();
                if (containsResult)
                    expected.Add(pt);

                foreach (int loc in WrappedCollision.IteratePointsInBounds(wrapSize, start, size, shiftPt))
                    results.Add(loc);

                Assert.That(results, Is.EquivalentTo(expected));
            }
        }

        [Test]
        [TestCase(10, -2, 14, -3, false, false, true)]
        [TestCase(10, -2, 14, -2, false, true, true)]
        [TestCase(10, -2, 14, 0, false, true, true)]
        [TestCase(10, -2, 14, 9, true, true, false)]
        [TestCase(10, -2, 14, 11, true, true, false)]
        [TestCase(10, -2, 14, 12, true, false, false)]
        public void IteratePointsInBoundsExpanded(int wrapSize, int start, int size, int pt, bool containsPreResult, bool containsResult, bool containsPostResult)
        {
            for (int ii = -1; ii <= 1; ii++)
            {
                int shiftPt = pt + (ii * wrapSize);
                List<int> results = new List<int>();
                List<int> expected = new List<int>();

                if (containsPreResult)
                    expected.Add(pt - wrapSize);
                if (containsResult)
                    expected.Add(pt);
                if (containsPostResult)
                    expected.Add(pt + wrapSize);

                foreach (int loc in WrappedCollision.IteratePointsInBounds(wrapSize, start, size, shiftPt))
                    results.Add(loc);

                Assert.That(results, Is.EquivalentTo(expected));
            }
        }

        // Normal case: the region is within the wrap bounds.
        [Test]
        [TestCase(10, 0, 3, -2, 3, true)]
        [TestCase(10, 0, 3, 2, 3, true)]
        [TestCase(10, 0, 3, 3, 3, false)]
        [TestCase(10, 0, 3, -3, 3, false)]

        [TestCase(10, 3, 3, 3, 3, true)]
        [TestCase(10, 3, 3, 5, 3, true)]
        [TestCase(10, 3, 3, 0, 3, false)]
        [TestCase(10, 3, 3, 6, 3, false)]

        // large secondary regions
        [TestCase(10, 0, 3, -2, 7, true)]
        [TestCase(10, 0, 3, 3, 7, false)]

        // Edge case: region intersects the wrap bounds
        [TestCase(10, -2, 5, -4, 3, true)]
        [TestCase(10, -2, 5, 2, 3, true)]
        [TestCase(10, -2, 5, 3, 3, false)]
        [TestCase(10, -2, 5, -5, 3, false)]
        [TestCase(10, 8, 5, 6, 3, true)]
        [TestCase(10, 8, 5, 12, 3, true)]
        [TestCase(10, 8, 5, 13, 3, false)]
        [TestCase(10, 8, 5, 5, 3, false)]

        // large secondary regions
        [TestCase(10, -2, 5, -3, 7, true)]
        [TestCase(10, -2, 5, 3, 5, false)]
        public void IterateRegionsColliding(int wrapSize, int start1, int size1, int start2, int size2, bool containsResult)
        {
            for (int ii = -1; ii <= 1; ii++)
            {
                int shiftPt = start2 + (ii * wrapSize);
                List<IntRange> results = new List<IntRange>();
                List<IntRange> expected = new List<IntRange>();
                if (containsResult)
                    expected.Add(new IntRange(start2, start2 + size2));

                foreach (IntRange loc in WrappedCollision.IterateRegionsColliding(wrapSize, start1, size1, shiftPt, size2))
                    results.Add(loc);

                Assert.That(results, Is.EquivalentTo(expected));
            }
        }

        // large primary regions
        [Test]
        [TestCase(10, -2, 14, -5, 3, false, false, true)]
        [TestCase(10, -2, 14, -4, 3, false, true, true)]
        [TestCase(10, -2, 14, 0, 3, false, true, true)]
        [TestCase(10, -2, 14, 9, 3, true, true, false)]
        [TestCase(10, -2, 14, 11, 3, true, true, false)]
        [TestCase(10, -2, 14, 12, 3, true, false, false)]

        // large secondary regions
        [TestCase(10, 0, 3, 3, 8, true, false, false)]
        [TestCase(10, -2, 5, 3, 6, true, false, false)]

        // large primary and secondary region
        [TestCase(10, -2, 14, -0, 10, true, true, true)]
        public void IterateRegionsCollidingExpanded(int wrapSize, int start1, int size1, int start2, int size2, bool containsPreResult, bool containsResult, bool containsPostResult)
        {
            for (int ii = -1; ii <= 1; ii++)
            {
                int shiftPt = start2 + (ii * wrapSize);
                List<IntRange> results = new List<IntRange>();
                List<IntRange> expected = new List<IntRange>();
                if (containsPreResult)
                    expected.Add(new IntRange(start2 - wrapSize, start2 - wrapSize + size2));
                if (containsResult)
                    expected.Add(new IntRange(start2, start2 + size2));
                if (containsPostResult)
                    expected.Add(new IntRange(start2 + wrapSize, start2 + wrapSize + size2));

                foreach (IntRange loc in WrappedCollision.IterateRegionsColliding(wrapSize, start1, size1, shiftPt, size2))
                    results.Add(loc);

                Assert.That(results, Is.EquivalentTo(expected));
            }
        }
    }
}
