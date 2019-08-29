// <copyright file="SpawnListTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Moq;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class SpawnListTest
    {
        private SpawnList<string> spawnList;

        [SetUp]
        public void SpawnListSetUp()
        {
            this.spawnList = new SpawnList<string>
            {
                { "apple", 10 },
                { "orange", 20 },
                { "banana", 30 },
            };
        }

        [Test]
        public void SpawnListCanPickTotal()
        {
            Assert.That(this.spawnList.CanPick, Is.EqualTo(true));
            Assert.That(this.spawnList.SpawnTotal, Is.EqualTo(60));
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(9, 0)]
        [TestCase(10, 1)]
        [TestCase(29, 1)]
        [TestCase(30, 2)]
        [TestCase(59, 2)]
        public void SpawnListChooseIndex(int roll, int result)
        {
            // choose with 1-2-3 probability, with edge cases
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(60)).Returns(roll);
            Assert.That(this.spawnList.PickIndex(testRand.Object), Is.EqualTo(result));
            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(1));
        }

        [Test]
        public void SpawnListChoose()
        {
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(60)).Returns(0);
            Assert.That(this.spawnList.Pick(testRand.Object), Is.EqualTo("apple"));
            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(1));
        }

        [Test]
        [TestCase("apple", 10)]
        [TestCase("orange", 20)]
        [TestCase("banana", 30)]
        public void SpawnListGetRate(string spawn, int rate)
        {
            // get spawn rate
            Assert.That(this.spawnList.GetSpawnRate(spawn), Is.EqualTo(rate));
        }

        [Test]
        [TestCase(5)]
        [TestCase(0)]
        public void SpawnListSetRate(int rate)
        {
            // set spawn rate
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(60 - 10 + rate)).Returns(rate);
            this.spawnList.SetSpawnRate(0, rate);
            Assert.That(this.spawnList.GetSpawnRate("apple"), Is.EqualTo(rate));
            Assert.That(this.spawnList.SpawnTotal, Is.EqualTo(60 - 10 + rate));
            Assert.That(this.spawnList.Pick(testRand.Object), Is.EqualTo("orange"));
            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(1));
        }

        [Test]
        public void SpawnListRemove()
        {
            // remove a spawn
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(50)).Returns(0);
            this.spawnList.RemoveAt(0);
            Assert.That(this.spawnList.GetSpawnRate("apple"), Is.EqualTo(0));
            Assert.That(this.spawnList.SpawnTotal, Is.EqualTo(50));
            Assert.That(this.spawnList.Pick(testRand.Object), Is.EqualTo("orange"));
            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(1));
        }

        [Test]
        public void SpawnListClear()
        {
            // remove all spawn
            this.spawnList.Clear();
            Assert.That(this.spawnList.SpawnTotal, Is.EqualTo(0));
            Assert.That(this.spawnList.Count, Is.EqualTo(0));
        }

        [Test]
        [TestCase(0, "apple", 10)]
        [TestCase(1, "orange", 20)]
        [TestCase(2, "banana", 30)]
        public void SpawnListGetSpawns(int index, string item, int rate)
        {
            // check all spawns
            Assert.That(this.spawnList.GetSpawn(index), Is.EqualTo(item));
            Assert.That(this.spawnList.GetSpawnRate(index), Is.EqualTo(rate));
        }
    }
}