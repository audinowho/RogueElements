// <copyright file="SpawnListTestRemovable.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Moq;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class SpawnListTestRemovable
    {
        [Test]
        public void SpawnListRemovable()
        {
            // roll with removable. Does not use the setup object.
            SpawnList<string> spawnList = new SpawnList<string>(true)
            {
                { "apple", 10 },
                { "orange", 20 },
                { "banana", 30 },
            };
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(60)).Returns(20);
            Assert.That(spawnList.Pick(testRand.Object), Is.EqualTo("orange"));
            Assert.That(spawnList.GetSpawnRate("orange"), Is.EqualTo(0));
            Assert.That(spawnList.SpawnTotal, Is.EqualTo(40));
            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(1));
        }
    }
}