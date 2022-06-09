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
        [TestCase(0, 0, 1)]
        [TestCase(1, 0, 1)]
        [TestCase(0, 1, 0)]
        [TestCase(0, 2, 0)]
        [TestCase(1, 1, 1)]
        [TestCase(1, 2, 1)]
        [TestCase(2, 1, 2)]
        [TestCase(2, 2, 4)]
        [TestCase(2, 3, 8)]
        public void IntPow(int a, int b, int result)
        {
            Assert.That(MathUtils.IntPow(a, b), Is.EqualTo(result));
        }

        [Test]
        [TestCase(0, 3, 0)]
        [TestCase(1, 3, 0)]
        [TestCase(2, 3, 0)]
        [TestCase(3, 3, 1)]
        [TestCase(4, 3, 1)]
        [TestCase(-1, 3, -1)]
        [TestCase(-2, 3, -1)]
        [TestCase(-3, 3, -1)]
        [TestCase(-4, 3, -2)]
        [TestCase(0, -3, 0)]
        [TestCase(1, -3, -1)]
        [TestCase(2, -3, -1)]
        [TestCase(3, -3, -1)]
        [TestCase(4, -3, -2)]
        [TestCase(-1, -3, 0)]
        [TestCase(-2, -3, 0)]
        [TestCase(-3, -3, 1)]
        [TestCase(-4, -3, 1)]
        public void DivDown(int num, int den, int result)
        {
            Assert.That(MathUtils.DivDown(num, den), Is.EqualTo(result));
        }

        [Test]
        [TestCase(0, 3, 0)]
        [TestCase(1, 3, 1)]
        [TestCase(2, 3, 1)]
        [TestCase(3, 3, 1)]
        [TestCase(4, 3, 2)]
        [TestCase(-1, 3, 0)]
        [TestCase(-2, 3, 0)]
        [TestCase(-3, 3, -1)]
        [TestCase(-4, 3, -1)]
        [TestCase(0, -3, 0)]
        [TestCase(1, -3, 0)]
        [TestCase(2, -3, 0)]
        [TestCase(3, -3, -1)]
        [TestCase(4, -3, -1)]
        [TestCase(-1, -3, 1)]
        [TestCase(-2, -3, 1)]
        [TestCase(-3, -3, 1)]
        [TestCase(-4, -3, 2)]
        public void DivUp(int num, int den, int result)
        {
            Assert.That(MathUtils.DivUp(num, den), Is.EqualTo(result));
        }

        [Test]
        [TestCase(0, 3, 0)]
        [TestCase(1, 3, 1)]
        [TestCase(2, 3, 2)]
        [TestCase(3, 3, 0)]
        [TestCase(4, 3, 1)]
        [TestCase(7, 3, 1)]
        [TestCase(-1, 3, 2)]
        [TestCase(-2, 3, 1)]
        [TestCase(-3, 3, 0)]
        [TestCase(-4, 3, 2)]
        [TestCase(-7, 3, 2)]
        public void Wrap(int num, int size, int result)
        {
            Assert.That(MathUtils.Wrap(num, size), Is.EqualTo(result));
        }
    }
}
