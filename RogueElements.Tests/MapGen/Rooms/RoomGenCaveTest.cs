// <copyright file="RoomGenCaveTest.cs" company="Audino">
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
    public class RoomGenCaveTest
    {
        [Test]
        [Ignore("Statistics")]
        public void ProposeSizeStats()
        {
            ReRandom rand = new ReRandom(0);
            Console.WriteLine(string.Format("Dims\tMin%\t10Pct%\tMed%\tMax%"));
            for (int xx = 3; xx < 21; xx++)
            {
                for (int yy = 3; yy <= xx; yy++)
                {
                    RoomGenCave<ITiledGenContext> roomGen = new RoomGenCave<ITiledGenContext>(new RandRange(xx, xx + 3), new RandRange(yy, yy + 3));

                    List<int> rolledW = new List<int>();
                    List<int> rolledH = new List<int>();
                    List<int> rolledA = new List<int>();
                    for (int ii = 0; ii < 1000; ii++)
                    {
                        Loc size = roomGen.ProposeSize(rand);
                        rolledW.Add(size.X);
                        rolledH.Add(size.Y);
                        rolledA.Add(size.X * size.Y);
                    }

                    rolledW.Sort();
                    rolledH.Sort();
                    rolledA.Sort();

                    // Console.WriteLine(String.Format("Orig Size: {0}x{1}", xx, yy));
                    // Console.WriteLine(String.Format("Height Min: {0}    10pct: {1}    25pct: {2}    Med: {3}    Max: {4}", rolledH[0], rolledH[rolledH.Count / 10], rolledH[rolledH.Count / 4], rolledH[rolledH.Count / 2], rolledH[rolledH.Count - 1]));
                    // Console.WriteLine(String.Format("Width Min: {0}    10pct: {1}    25pct: {2}    Med: {3}    Max: {4}", rolledW[0], rolledW[rolledW.Count / 10], rolledW[rolledW.Count / 4], rolledW[rolledW.Count / 2], rolledW[rolledW.Count - 1]));
                    // Console.WriteLine(String.Format(
                    //    "Area% Min: {0}%    10pct: {1}%    25pct: {2}%    Med: {3}%    Max: {4}%",
                    //    rolledA[0] * 100 / (xx * yy),
                    //    rolledA[rolledA.Count / 10] * 100 / (xx * yy),
                    //    rolledA[rolledA.Count / 4] * 100 / (xx * yy),
                    //    rolledA[rolledA.Count / 2] * 100 / (xx * yy),
                    //    rolledA[rolledA.Count - 1] * 100 / (xx * yy)));
                    Console.WriteLine(string.Format(
                        "{0}x{1}\t{2}\t{3}\t{4}\t{5}",
                        xx,
                        yy,
                        rolledA[0] * 1000 / (xx * yy),
                        rolledA[rolledA.Count / 10] * 1000 / (xx * yy),
                        rolledA[rolledA.Count / 2] * 1000 / (xx * yy),
                        rolledA[rolledA.Count - 1] * 1000 / (xx * yy)));
                }
            }
        }

        [Test]
        [Ignore("TODO")]
        public void ProposeSize()
        {
        }

        [Test]
        [Ignore("TODO")]
        public void DrawOnMap()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void PrepareFulfillableBorders()
        {
            throw new NotImplementedException();
        }

        public class TestRoomGenCave<T> : RoomGenCave<T>
            where T : ITiledGenContext
        {
            public Dictionary<Dir4, bool[]> PublicFulfillableBorder => this.FulfillableBorder;
        }
    }
}
