// <copyright file="SpawnRangeListTest.cs" company="Audino">
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
    public class SpawnRangeListTest
    {
        private SpawnRangeList<string> spawnRangeList;

        [SetUp]
        public void SpawnListSetUp()
        {
            this.spawnRangeList = new SpawnRangeList<string>();
            this.spawnRangeList.Add("apple", new IntRange(0, 5), 10);
            this.spawnRangeList.Add("orange", new IntRange(3, 9), 20);
        }

        [Test]
        [TestCase(-1, false)]
        [TestCase(0, true)]
        [TestCase(8, true)]
        [TestCase(9, false)]
        public void SpawnRangeListCanPick(int level, bool result)
        {
            Assert.That(this.spawnRangeList.CanPick(level), Is.EqualTo(result));
        }

        [Test]
        [TestCase(-1, 0, 0, "", true)]
        [TestCase(0, 10, 0, "apple", false)]
        [TestCase(0, 10, 9, "apple", false)]
        [TestCase(2, 10, 0, "apple", false)]
        [TestCase(3, 30, 0, "apple", false)]
        [TestCase(3, 30, 9, "apple", false)]
        [TestCase(3, 30, 10, "orange", false)]
        [TestCase(3, 30, 29, "orange", false)]
        [TestCase(4, 30, 10, "orange", false)]
        [TestCase(5, 20, 0, "orange", false)]
        [TestCase(8, 20, 19, "orange", false)]
        [TestCase(9, 0, 0, "", true)]
        public void SpawnRangeListPick(int level, int spawnTotal, int roll, string result, bool exception)
        {
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(spawnTotal)).Returns(roll);
            if (exception)
            {
                Assert.Throws<InvalidOperationException>(() => { this.spawnRangeList.Pick(testRand.Object, level); });
            }
            else
            {
                Assert.That(this.spawnRangeList.Pick(testRand.Object, level), Is.EqualTo(result));
                testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(1));
            }
        }

        [Test]
        [TestCase(-1, 0, 0, "", 0)]
        [TestCase(0, 1, 0, "apple", 10)]
        [TestCase(2, 1, 0, "apple", 10)]
        [TestCase(3, 2, 0, "apple", 10)]
        [TestCase(3, 2, 1, "orange", 20)]
        [TestCase(4, 2, 0, "apple", 10)]
        [TestCase(4, 2, 1, "orange", 20)]
        [TestCase(5, 1, 0, "orange", 20)]
        [TestCase(8, 1, 0, "orange", 20)]
        [TestCase(9, 0, 0, "", 0)]
        public void SpawnRangeListSpawnList(int level, int count, int index, string result, int rate)
        {
            SpawnList<string> spawnList = this.spawnRangeList.GetSpawnList(level);
            Assert.That(spawnList.Count, Is.EqualTo(count));
            if (count > 0)
            {
                Assert.That(spawnList.GetSpawn(index), Is.EqualTo(result));
                Assert.That(spawnList.GetSpawnRate(index), Is.EqualTo(rate));
            }
        }

        [Test]
        public void SpawnRangeListRemove()
        {
            this.spawnRangeList.Remove("apple");
            IEnumerator<string> enumer = this.spawnRangeList.EnumerateOutcomes().GetEnumerator();
            Assert.That(enumer.MoveNext(), Is.EqualTo(true));
            Assert.That(enumer.Current, Is.EqualTo("orange"));
            Assert.That(enumer.MoveNext(), Is.EqualTo(false));
        }

        [Test]
        public void SpawnRangeListGetSpawns()
        {
            IEnumerator<string> enumer = this.spawnRangeList.EnumerateOutcomes().GetEnumerator();
            Assert.That(enumer.MoveNext(), Is.EqualTo(true));
            Assert.That(enumer.Current, Is.EqualTo("apple"));
            Assert.That(enumer.MoveNext(), Is.EqualTo(true));
            Assert.That(enumer.Current, Is.EqualTo("orange"));
            Assert.That(enumer.MoveNext(), Is.EqualTo(false));
        }

        [Test]
        public void SpawnRangeListRangeError()
        {
            Assert.Throws<ArgumentException>(() => { this.spawnRangeList.Add("pear", new IntRange(3, 3), 10); });
        }

        [Test]
        public void SpawnRangeListRateError()
        {
            Assert.Throws<ArgumentException>(() => { this.spawnRangeList.Add("pear", new IntRange(3, 4), -1); });
        }

        [Test]
        public void SpawnRangeListRemoveError()
        {
            Assert.Throws<InvalidOperationException>(() => { this.spawnRangeList.Remove("pear"); });
        }
    }
}
