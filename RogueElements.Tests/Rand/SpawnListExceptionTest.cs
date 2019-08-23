// <copyright file="SpawnListExceptionTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Moq;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class SpawnListExceptionTest
    {
        [Test]
        public void SpawnListEmptyChoose()
        {
            // choose when empty
            var spawnList = new SpawnList<string>();
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Assert.That(spawnList.CanPick, Is.EqualTo(false));
            Assert.Throws<InvalidOperationException>(() => { spawnList.Pick(testRand.Object); });
        }

        [Test]
        public void SpawnListZeroChoose()
        {
            // choose when all 0's
            SpawnList<string> spawnList = new SpawnList<string>
            {
                { "apple", 0 },
                { "orange", 0 },
            };
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Assert.That(spawnList.CanPick, Is.EqualTo(false));
            Assert.That(spawnList.SpawnTotal, Is.EqualTo(0));
            Assert.Throws<InvalidOperationException>(() => { spawnList.PickIndex(testRand.Object); });
        }

        [Test]
        public void SpawnListAddNegative()
        {
            // add negative
            var spawnList = new SpawnList<string>();
            Assert.Throws<ArgumentException>(() => { spawnList.Add("apple", -1); });
        }

        [Test]
        public void SpawnListSetNegative()
        {
            // set negative
            SpawnList<string> spawnList = new SpawnList<string> { { "apple", 1 } };
            Assert.Throws<ArgumentException>(() => { spawnList.SetSpawnRate(0, -1); });
        }
    }
}