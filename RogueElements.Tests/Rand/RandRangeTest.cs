// <copyright file="RandRangeTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class RandRangeTest
    {
        [Test]
        public void LoopedRand()
        {
            var amountRange = new RandRange(3, 6);
            var valueRange = new RandRange(0, 4);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.SetupSequence(p => p.Next(3, 6)).Returns(4);
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(0, 4));
            seq = seq.Returns(0);
            seq = seq.Returns(1);
            seq = seq.Returns(2);
            seq = seq.Returns(3);
            LoopedRand<int> looped = new LoopedRand<int>(valueRange, amountRange);
            List<int> result = looped.Roll(testRand.Object);
            List<int> compare = new List<int> { 0, 1, 2, 3 };
            Assert.That(result, Is.EquivalentTo(compare));
            testRand.Verify(p => p.Next(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(5));
        }

        [Test]
        public void PresetMultiRand()
        {
            PresetMultiRand<int> testPicker = new PresetMultiRand<int>(5, 3, 1, 8);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            List<int> compare = new List<int> { 5, 3, 1, 8 };
            Assert.That(testPicker.Roll(testRand.Object), Is.EquivalentTo(compare));
        }

        [Test]
        public void PresetPicker()
        {
            PresetPicker<int> testPicker = new PresetPicker<int>(5);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Assert.That(testPicker.Pick(testRand.Object), Is.EqualTo(5));
        }

        [Test]
        public void RandBagRemovable()
        {
            RandBag<int> testPicker = new RandBag<int>(8, 5, 2, 3, 4, 1, 6, 7) { RemoveOnRoll = true };
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(8)).Returns(3);
            testRand.Setup(p => p.Next(7)).Returns(6);
            testRand.Setup(p => p.Next(6)).Returns(5);
            testRand.Setup(p => p.Next(5)).Returns(0);
            testRand.Setup(p => p.Next(4)).Returns(2);
            testRand.Setup(p => p.Next(3)).Returns(2);
            testRand.Setup(p => p.Next(2)).Returns(1);
            testRand.Setup(p => p.Next(1)).Returns(0);
            Assert.That(testPicker.Pick(testRand.Object), Is.EqualTo(3));
            Assert.That(testPicker.Pick(testRand.Object), Is.EqualTo(7));
            Assert.That(testPicker.Pick(testRand.Object), Is.EqualTo(6));
            Assert.That(testPicker.Pick(testRand.Object), Is.EqualTo(8));
            Assert.That(testPicker.Pick(testRand.Object), Is.EqualTo(4));
            Assert.That(testPicker.Pick(testRand.Object), Is.EqualTo(1));
            Assert.That(testPicker.Pick(testRand.Object), Is.EqualTo(2));
            Assert.That(testPicker.Pick(testRand.Object), Is.EqualTo(5));
            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(8));
        }

        [Test]
        [TestCase(0, 0, 0, 0)]
        [TestCase(0, 0, 99, 0)]
        [TestCase(0, 50, 50, 0)]
        [TestCase(0, 50, 49, 5)]
        [TestCase(0, 100, 99, 5)]
        [TestCase(0, 100, 0, 5)]
        [TestCase(5, 50, 50, 5)]
        [TestCase(5, 50, 49, 10)]
        public void RandBinomial(int offset, int percent, int rollResult, int result)
        {
            var testPicker = new RandBinomial(5, percent, offset);
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(100)).Returns(rollResult);
            Assert.That(testPicker.Pick(testRand.Object), Is.EqualTo(result));
        }

        [Test]
        public void RandRangeCanPick0()
        {
            var testPicker = RandRange.Empty;
            var rand = new ReRandom(0);
            Assert.That(testPicker.CanPick, Is.EqualTo(true));
            Assert.That(testPicker.Pick(rand), Is.EqualTo(0));
        }
    }
}