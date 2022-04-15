// <copyright file="NoiseGenTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class NoiseGenTest
    {
        [Test]
        [Ignore("TODO")]
        public void PerlinNoise()
        {
            // verify that prequency increases
            throw new NotImplementedException();
        }

        [Test]
        [TestCase(CellRule.None, false)]
        [TestCase(CellRule.All, true)]
        [TestCase(CellRule.Eq0, true)]
        [TestCase(CellRule.Lt2, true)]
        [TestCase(CellRule.Gte1, false)]
        public void IterateAutomataSingle0(CellRule rule, bool expected)
        {
            // test to verify all neighbors are influencing decision correctly
            string[] inGrid =
            {
                "XXX",
                "X.X",
                "XXX",
            };

            bool[][] map = GridTest.InitBoolGrid(inGrid);

            bool[][] result = NoiseGen.IterateAutomata(map, CellRule.None, rule, 1);
            Assert.That(result[1][1], Is.EqualTo(expected));
        }

        [Test]
        [TestCase(CellRule.None, false)]
        [TestCase(CellRule.Eq0, false)]
        [TestCase(CellRule.Eq1, true)]
        [TestCase(CellRule.Lt2, true)]
        [TestCase(CellRule.Gte1, true)]
        public void IterateAutomataSingle1(CellRule rule, bool expected)
        {
            // test to verify all neighbors are influencing decision correctly
            string[] inGrid =
            {
                "X.X",
                "X.X",
                "XXX",
            };

            bool[][] map = GridTest.InitBoolGrid(inGrid);

            bool[][] result = NoiseGen.IterateAutomata(map, CellRule.None, rule, 1);
            Assert.That(result[1][1], Is.EqualTo(expected));
        }

        [Test]
        [TestCase(CellRule.Eq1, false)]
        [TestCase(CellRule.Lt2, false)]
        [TestCase(CellRule.Gte1, true)]
        public void IterateAutomataSingle2(CellRule rule, bool expected)
        {
            // test to verify all neighbors are influencing decision correctly
            string[] inGrid =
            {
                "X..",
                "X.X",
                "XXX",
            };

            bool[][] map = GridTest.InitBoolGrid(inGrid);

            bool[][] result = NoiseGen.IterateAutomata(map, CellRule.None, rule, 1);
            Assert.That(result[1][1], Is.EqualTo(expected));
        }

        [Test]
        [TestCase(CellRule.Eq8, false)]
        [TestCase(CellRule.Gte1, true)]
        [TestCase(CellRule.All, true)]
        public void IterateAutomataSingle7(CellRule rule, bool expected)
        {
            // test to verify all neighbors are influencing decision correctly
            string[] inGrid =
            {
                "X..",
                "...",
                "...",
            };

            bool[][] map = GridTest.InitBoolGrid(inGrid);

            bool[][] result = NoiseGen.IterateAutomata(map, CellRule.None, rule, 1);
            Assert.That(result[1][1], Is.EqualTo(expected));
        }

        [Test]
        [TestCase(CellRule.None, false)]
        [TestCase(CellRule.Eq8, true)]
        [TestCase(CellRule.Gte1, true)]
        [TestCase(CellRule.All, true)]
        public void IterateAutomataSingle8(CellRule rule, bool expected)
        {
            // test to verify all neighbors are influencing decision correctly
            string[] inGrid =
            {
                "...",
                "...",
                "...",
            };

            bool[][] map = GridTest.InitBoolGrid(inGrid);

            bool[][] result = NoiseGen.IterateAutomata(map, CellRule.None, rule, 1);
            Assert.That(result[1][1], Is.EqualTo(expected));
        }

        [Test]
        public void IterateAutomataIterations0()
        {
            string[] inGrid =
            {
                "XXXXX",
                "X...X",
                "X...X",
                "X...X",
                "XXXXX",
            };

            bool[][] map = GridTest.InitBoolGrid(inGrid);
            bool[][] compare = GridTest.InitBoolGrid(inGrid);

            bool[][] result = NoiseGen.IterateAutomata(map, CellRule.None, CellRule.Gte4, 0);
            Assert.That(result, Is.EqualTo(compare));
        }

        [Test]
        public void IterateAutomataIterations1()
        {
            string[] inGrid =
            {
                "XXXXX",
                "X...X",
                "X...X",
                "X...X",
                "XXXXX",
            };

            string[] outGrid =
            {
                "XXXXX",
                "XX.XX",
                "X...X",
                "XX.XX",
                "XXXXX",
            };

            bool[][] map = GridTest.InitBoolGrid(inGrid);
            bool[][] compare = GridTest.InitBoolGrid(outGrid);

            bool[][] result = NoiseGen.IterateAutomata(map, CellRule.None, CellRule.Gte4, 1);
            Assert.That(result, Is.EqualTo(compare));
        }

        [Test]
        public void IterateAutomataIterations2()
        {
            string[] inGrid =
            {
                "XXXXX",
                "X...X",
                "X...X",
                "X...X",
                "XXXXX",
            };

            string[] outGrid =
            {
                "XXXXX",
                "XXXXX",
                "XX.XX",
                "XXXXX",
                "XXXXX",
            };

            bool[][] map = GridTest.InitBoolGrid(inGrid);
            bool[][] compare = GridTest.InitBoolGrid(outGrid);

            bool[][] result = NoiseGen.IterateAutomata(map, CellRule.None, CellRule.Gte4, 2);
            Assert.That(result, Is.EqualTo(compare));
        }

        [Test]
        public void IterateAutomataIterations3()
        {
            string[] inGrid =
            {
                "XXXXX",
                "X...X",
                "X...X",
                "X...X",
                "XXXXX",
            };

            string[] outGrid =
            {
                "XXXXX",
                "XXXXX",
                "XXXXX",
                "XXXXX",
                "XXXXX",
            };

            bool[][] map = GridTest.InitBoolGrid(inGrid);
            bool[][] compare = GridTest.InitBoolGrid(outGrid);

            bool[][] result = NoiseGen.IterateAutomata(map, CellRule.None, CellRule.Gte4, 3);
            Assert.That(result, Is.EqualTo(compare));
        }

        [Test]
        [Ignore("TODO")]
        public void RandomDivide()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void Shuffle()
        {
            throw new NotImplementedException();
        }
    }
}
