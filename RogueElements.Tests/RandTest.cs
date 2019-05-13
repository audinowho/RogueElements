using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;

namespace RogueElements.Tests
{
    [TestFixture]
    public class RandTest
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
            var testPicker = new RandRange();
            var rand = new ReRandom(0);
            Assert.That(testPicker.CanPick, Is.EqualTo(true));
            Assert.That(testPicker.Pick(rand), Is.EqualTo(0));
        }
    }

    [TestFixture]
    public class SpawnListExceptionTest
    {
        [Test]
        public void SpawnListEmptyChoose()
        {
            //choose when empty
            var spawnList = new SpawnList<string>();
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Assert.That(spawnList.CanPick, Is.EqualTo(false));
            Assert.Throws<InvalidOperationException>(() => { spawnList.Pick(testRand.Object); });

        }

        [Test]
        public void SpawnListZeroChoose()
        {
            //choose when all 0's
            SpawnList<string> spawnList = new SpawnList<string>
            {
                { "apple", 0 },
                { "orange", 0 }
            };
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            Assert.That(spawnList.CanPick, Is.EqualTo(false));
            Assert.That(spawnList.SpawnTotal, Is.EqualTo(0));
            Assert.Throws<InvalidOperationException>(() => { spawnList.PickIndex(testRand.Object); });
        }

        [Test]
        public void SpawnListAddNegative()
        {
            //add negative
            var spawnList = new SpawnList<string>();
            Assert.Throws<ArgumentException>(() => { spawnList.Add("apple", -1); });
        }


        [Test]
        public void SpawnListSetNegative()
        {
            //set negative
            SpawnList<string> spawnList = new SpawnList<string> { { "apple", 1 } };
            Assert.Throws<ArgumentException>(() => { spawnList.SetSpawnRate(0, -1); });
        }

    }


    [TestFixture]
    public class SpawnListTest
    {
        SpawnList<string> spawnList;

        [SetUp]
        public void SpawnListSetUp()
        {
            spawnList = new SpawnList<string>
            {
                { "apple", 10 },
                { "orange", 20 },
                { "banana", 30 }
            };
        }


        [Test]
        public void SpawnListCanPickTotal()
        {
            Assert.That(spawnList.CanPick, Is.EqualTo(true));
            Assert.That(spawnList.SpawnTotal, Is.EqualTo(60));

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
            //choose with 1-2-3 probability, with edge cases
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(60)).Returns(roll);
            Assert.That(spawnList.PickIndex(testRand.Object), Is.EqualTo(result));
            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(1));
        }

        [Test]
        public void SpawnListChoose()
        {
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(60)).Returns(0);
            Assert.That(spawnList.Pick(testRand.Object), Is.EqualTo("apple"));
            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(1));
        }

        [Test]
        [TestCase("apple", 10)]
        [TestCase("orange", 20)]
        [TestCase("banana", 30)]
        public void SpawnListGetRate(string spawn, int rate)
        {
            //get spawn rate
            Assert.That(spawnList.GetSpawnRate(spawn), Is.EqualTo(rate));
        }

        [Test]
        [TestCase(5)]
        [TestCase(0)]
        public void SpawnListSetRate(int rate)
        {
            //set spawn rate
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(60 - 10 + rate)).Returns(rate);
            spawnList.SetSpawnRate(0, rate);
            Assert.That(spawnList.GetSpawnRate("apple"), Is.EqualTo(rate));
            Assert.That(spawnList.SpawnTotal, Is.EqualTo(60 - 10 + rate));
            Assert.That(spawnList.Pick(testRand.Object), Is.EqualTo("orange"));
            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(1));
        }

        [Test]
        public void SpawnListRemove()
        {
            //remove a spawn
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(50)).Returns(0);
            spawnList.RemoveAt(0);
            Assert.That(spawnList.GetSpawnRate("apple"), Is.EqualTo(0));
            Assert.That(spawnList.SpawnTotal, Is.EqualTo(50));
            Assert.That(spawnList.Pick(testRand.Object), Is.EqualTo("orange"));
            testRand.Verify(p => p.Next(It.IsAny<int>()), Times.Exactly(1));
        }

        [Test]
        public void SpawnListClear()
        {
            //remove all spawn
            spawnList.Clear();
            Assert.That(spawnList.SpawnTotal, Is.EqualTo(0));
            Assert.That(spawnList.Count, Is.EqualTo(0));
        }

        [Test]
        [TestCase(0, "apple", 10)]
        [TestCase(1, "orange", 20)]
        [TestCase(2, "banana", 30)]
        public void SpawnListGetSpawns(int index, string item, int rate)
        {
            //check all spawns
            Assert.That(spawnList.GetSpawn(index), Is.EqualTo(item));
            Assert.That(spawnList.GetSpawnRate(index), Is.EqualTo(rate));
        }
    }
    
    [TestFixture]
    public class SpawnRangeListTest
    {
        SpawnRangeList<string> spawnRangeList;

        [SetUp]
        public void SpawnListSetUp()
        {
            spawnRangeList = new SpawnRangeList<string>();
            spawnRangeList.Add("apple", new Range(0, 5), 10);
            spawnRangeList.Add("orange", new Range(3, 9), 20);
        }

        [Test]
        [TestCase(-1, false)]
        [TestCase(0, true)]
        [TestCase(8, true)]
        [TestCase(9, false)]
        public void SpawnRangeListCanPick(int level, bool result)
        {
            Assert.That(spawnRangeList.CanPick(level), Is.EqualTo(result));
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
                Assert.Throws<InvalidOperationException>(() => { spawnRangeList.Pick(testRand.Object, level); });
            else
            {
                Assert.That(spawnRangeList.Pick(testRand.Object, level), Is.EqualTo(result));
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
            SpawnList<string> spawnList = spawnRangeList.GetSpawnList(level);
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
            spawnRangeList.Remove("apple");
            IEnumerator<string> enumer = spawnRangeList.GetSpawns().GetEnumerator();
            Assert.That(enumer.MoveNext(), Is.EqualTo(true));
            Assert.That(enumer.Current, Is.EqualTo("orange"));
            Assert.That(enumer.MoveNext(), Is.EqualTo(false));
        }

        [Test]
        public void SpawnRangeListGetSpawns()
        {
            IEnumerator<string> enumer = spawnRangeList.GetSpawns().GetEnumerator();
            Assert.That(enumer.MoveNext(), Is.EqualTo(true));
            Assert.That(enumer.Current, Is.EqualTo("apple"));
            Assert.That(enumer.MoveNext(), Is.EqualTo(true));
            Assert.That(enumer.Current, Is.EqualTo("orange"));
            Assert.That(enumer.MoveNext(), Is.EqualTo(false));
        }

        [Test]
        public void SpawnRangeListRangeError()
        {
            Assert.Throws<ArgumentException>(() => { spawnRangeList.Add("pear", new Range(3, 3), 10); });
        }

        [Test]
        public void SpawnRangeListRateError()
        {
            Assert.Throws<ArgumentException>(() => { spawnRangeList.Add("pear", new Range(3, 4), -1); });
        }

        [Test]
        public void SpawnRangeListRemoveError()
        {
            Assert.Throws<InvalidOperationException>(() => { spawnRangeList.Remove("pear"); });
        }
    }
}
