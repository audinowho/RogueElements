// <copyright file="PriorityTest.cs" company="Audino">
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
    public class PriorityTest
    {
        [Test]
        [TestCase(0, false, false, 1, true)]// 0 < 1
        [TestCase(0, true, false, 1, false)]// 0 > 1 false
        [TestCase(1, true, false, 0, true)]// 1 > 0
        [TestCase(1, false, false, 0, false)]// 1 < 0 false
        [TestCase(0, false, true, 1, true)]// 0 <= 1
        [TestCase(0, true, true, 1, false)]// 0 >= 1 false
        [TestCase(1, true, true, 0, true)]// 1 >= 0
        [TestCase(1, false, true, 0, false)]// 1 <= 0 false
        [TestCase(1, false, true, 1, true)]// 1 <= 1
        [TestCase(1, true, true, 1, true)]// 1 >= 1
        [TestCase(1, false, false, 1, false)]// 1 < 1 false
        [TestCase(1, true, false, 1, false)]// 1 > 1 false
        public void TestOp1(int lhs1, bool gt, bool eq, int rhs1, bool res)
        {
            Priority lhs = new Priority(lhs1);
            Priority rhs = new Priority(rhs1);

            if (eq)
            {
                if (gt)
                    Assert.That(lhs >= rhs, Is.EqualTo(res));
                else
                    Assert.That(lhs <= rhs, Is.EqualTo(res));
            }
            else
            {
                if (gt)
                    Assert.That(lhs > rhs, Is.EqualTo(res));
                else
                    Assert.That(lhs < rhs, Is.EqualTo(res));
            }
        }

        [Test]
        [TestCase(0, 1, false, 1, 0, true)]// 0.1 < 1.0
        [TestCase(0, 1, true, 1, 0, false)]// 0.1 > 1.0 false
        [TestCase(0, -1, false, 0, 0, true)]// 0.-1 < 0.0
        [TestCase(0, -1, true, 0, 0, false)]// 0.-1 > 0.0 false
        [TestCase(0, 0, false, 0, 1, true)]// 0.0 < 0.1
        [TestCase(0, 0, true, 0, 1, false)]// 0.0 > 0.1 false
        [TestCase(0, 3, true, 0, 12, false)]// 0.3 < 0.12 true
        public void TestOp2(int lhs1, int lhs2, bool gt, int rhs1, int rhs2, bool res)
        {
            Priority lhs = new Priority(lhs1, lhs2);
            Priority rhs = new Priority(rhs1, rhs2);

            if (gt)
                Assert.That(lhs > rhs, Is.EqualTo(res));
            else
                Assert.That(lhs < rhs, Is.EqualTo(res));
        }

        [Test]
        [TestCase(0, 1, true, 0, 1, true)]// 0.1 = 0.1
        [TestCase(0, 1, false, 0, 1, false)]// 0.1 != 0.1 false
        [TestCase(1, 0, false, 0, 1, true)]// 1.0 != 0.1
        [TestCase(1, 0, true, 0, 1, false)]// 1.0 == 0.1 false
        public void TestEq2(int lhs1, int lhs2, bool eq, int rhs1, int rhs2, bool res)
        {
            Priority lhs = new Priority(lhs1, lhs2);
            Priority rhs = new Priority(rhs1, rhs2);

            if (eq)
                Assert.That(lhs == rhs, Is.EqualTo(res));
            else
                Assert.That(lhs != rhs, Is.EqualTo(res));
        }

        [Test]
        public void Test0ne1()
        {
            Assert.IsTrue(new Priority(0) != new Priority(1));
        }

        [Test]
        public void TestNeg1Lt0Neg1()
        {
            Assert.IsTrue(new Priority(-1) < new Priority(0, -1));
        }

        [Test]
        public void Test0p1Lt1()
        {
            Assert.IsTrue(new Priority(0, 1) < new Priority(1));
        }

        [Test]
        public void Test0Eq0p0()
        {
            Assert.IsTrue(new Priority(0) == new Priority(0, 0));
        }

        [Test]
        public void Test0Gt0pNeg1()
        {
            Assert.IsTrue(new Priority(0) > new Priority(0, -1));
        }

        [Test]
        public void Test0Lt0p0p1()
        {
            Assert.IsTrue(new Priority(0) < new Priority(0, 0, 1));
        }

        [Test]
        public void Test1p0p1Gt1p0p0p1()
        {
            Assert.IsTrue(new Priority(1, 0, 1) > new Priority(1, 0, 0, 1));
        }

        [Test]
        public void TestInvalidEqInvalid()
        {
            Priority lhs = Priority.Invalid;
            Priority rhs = Priority.Invalid;
            Assert.IsTrue(lhs == rhs);
        }

        [Test]
        public void TestNullEqEmpty()
        {
            Priority lhs = new Priority(null);
            Priority rhs = new Priority(new int[] { });
            Assert.IsTrue(lhs == rhs);
        }

        [Test]
        public void TestNullEqInvalid()
        {
            Priority lhs = new Priority(null);
            Priority rhs = Priority.Invalid;
            Assert.IsTrue(lhs == rhs);
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void TestInvalidCp0(bool gt, bool eq)
        {
            Priority lhs = Priority.Invalid;
            Priority rhs = new Priority(0);

            if (eq)
            {
                if (gt)
                    Assert.IsFalse(lhs >= rhs);
                else
                    Assert.IsFalse(lhs <= rhs);
            }
            else
            {
                if (gt)
                    Assert.IsFalse(lhs > rhs);
                else
                    Assert.IsFalse(lhs < rhs);
            }
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(6, 13)]
        public void TestIdx(int idx, int val)
        {
            Priority priority = new Priority(0, 1, 2, 3, 5, 8, 13);
            Assert.AreEqual(priority[idx], val);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(7)]
        public void TestIdxEx(int idx)
        {
            Priority priority = new Priority(0, 1, 2, 3, 5, 8, 13);
            Assert.Throws<IndexOutOfRangeException>(() => { int a = priority[idx]; });
        }

        [Test]
        public void TestNullLength()
        {
            Priority priority = new Priority(null);
            Assert.AreEqual(priority.Length, 0);
        }

        [Test]
        public void Test0Length()
        {
            Priority priority = new Priority(0, 0, 0);
            Assert.AreEqual(priority.Length, 1);
        }

        [Test]
        public void Test2Length()
        {
            Priority priority = new Priority(2, 1, 0);
            Assert.AreEqual(priority.Length, 2);
        }

        [Test]
        public void TestLength()
        {
            Priority priority = new Priority(0, 1, 2, 3, 5, 8, 13);
            Assert.AreEqual(priority.Length, 7);
        }
    }
}
