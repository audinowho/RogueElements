// <copyright file="MathUtilsTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class MathUtilsTest
    {
        [Test]
        [Ignore("TODO")]
        public void BiInterpolate()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void Interpolate()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void AddToSortedListNone()
        {
            List<int> sortedList = new List<int>();

            MathUtils.AddToSortedList(sortedList, 5, (int a, int b) => { return b - a; });

            Assert.That(sortedList.Count, Is.EqualTo(1));
            Assert.That(sortedList[0], Is.EqualTo(5));
        }

        [Test]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void AddToSortedListOne(int addedValue)
        {
            List<int> sortedList = new List<int> { 5 };

            MathUtils.AddToSortedList(sortedList, addedValue, (int a, int b) => { return b - a; });

            Assert.That(sortedList.Count, Is.EqualTo(2));
            for (int ii = 1; ii < sortedList.Count; ii++)
                Assert.That(sortedList[ii], Is.GreaterThanOrEqualTo(sortedList[ii - 1]));
        }
    }
}
