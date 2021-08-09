// <copyright file="DetectionTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class DetectionTest
    {
        public static bool[][] InitGrid(string[] inGrid)
        {
            // transposes
            bool[][] result_map = new bool[inGrid[0].Length][];
            for (int xx = 0; xx < result_map.Length; xx++)
            {
                result_map[xx] = new bool[inGrid.Length];
                for (int yy = 0; yy < result_map[xx].Length; yy++)
                    result_map[xx][yy] = inGrid[yy][xx] == 'X';
            }

            return result_map;
        }

        [Test]
        public void DetectBlobsNone()
        {
            // no blob
            string[] inGrid =
            {
                "XXXXXX",
                "XXXXXX",
                "XXXXXX",
                "XXXXXX",
            };

            string[] outGrid =
            {
                "@@@@@@",
                "@@@@@@",
                "@@@@@@",
                "@@@@@@",
            };

            bool[][] map = GridTest.InitBoolGrid(inGrid);
            int[][] blob = GridTest.InitIntGrid(outGrid);

            List<BlobMap.Blob> compareBlobs = new List<BlobMap.Blob>();

            bool LocTest(Loc loc) => map[loc.X][loc.Y];
            BlobMap result = Detection.DetectBlobs(new Rect(0, 0, map.Length, map[0].Length), LocTest);
            Assert.That(result.Map, Is.EqualTo(blob));
            Assert.That(result.Blobs, Is.EqualTo(compareBlobs));
        }

        [Test]
        public void DetectBlobsSingle()
        {
            // single blob
            string[] inGrid =
            {
                "XXXXXX",
                "X..XXX",
                "X....X",
                "X.XXXX",
                "XXXXXX",
            };

            string[] outGrid =
            {
                "@@@@@@",
                "@AA@@@",
                "@AAAA@",
                "@A@@@@",
                "@@@@@@",
            };

            bool[][] map = GridTest.InitBoolGrid(inGrid);
            int[][] blob = GridTest.InitIntGrid(outGrid);

            var compareBlobs = new List<BlobMap.Blob> { new BlobMap.Blob(new Rect(1, 1, 4, 3), 7) };

            bool LocTest(Loc loc) => map[loc.X][loc.Y];
            BlobMap result = Detection.DetectBlobs(new Rect(0, 0, map.Length, map[0].Length), LocTest);
            Assert.That(result.Map, Is.EqualTo(blob));
            Assert.That(result.Blobs, Is.EqualTo(compareBlobs));
        }

        [Test]
        public void DetectBlobsDiagonal()
        {
            // blobs at corners
            string[] inGrid =
            {
                "XXXXXX",
                "X..XXX",
                "X..XXX",
                "XXX..X",
                "XXX..X",
                "XXXXXX",
            };

            string[] outGrid =
            {
                "@@@@@@",
                "@AA@@@",
                "@AA@@@",
                "@@@BB@",
                "@@@BB@",
                "@@@@@@",
            };

            bool[][] map = GridTest.InitBoolGrid(inGrid);
            int[][] blob = GridTest.InitIntGrid(outGrid);

            var compareBlobs = new List<BlobMap.Blob>
            {
                new BlobMap.Blob(new Rect(1, 1, 2, 2), 4),
                new BlobMap.Blob(new Rect(3, 3, 2, 2), 4),
            };

            bool LocTest(Loc loc) => map[loc.X][loc.Y];
            BlobMap result = Detection.DetectBlobs(new Rect(0, 0, map.Length, map[0].Length), LocTest);
            Assert.That(result.Map, Is.EqualTo(blob));
            Assert.That(result.Blobs, Is.EqualTo(compareBlobs));
        }

        [Test]
        public void DetectBlobsCorners()
        {
            // diagonal attached blobs
            string[] inGrid =
            {
                "..XX..",
                "..XX..",
                "XXXXXX",
                "XXXXXX",
            };

            string[] outGrid =
            {
                "AA@@BB",
                "AA@@BB",
                "@@@@@@",
                "@@@@@@",
            };

            bool[][] map = GridTest.InitBoolGrid(inGrid);
            int[][] blob = GridTest.InitIntGrid(outGrid);

            var compareBlobs = new List<BlobMap.Blob>
            {
                new BlobMap.Blob(new Rect(0, 0, 2, 2), 4),
                new BlobMap.Blob(new Rect(4, 0, 2, 2), 4),
            };

            bool LocTest(Loc loc) => map[loc.X][loc.Y];
            BlobMap result = Detection.DetectBlobs(new Rect(0, 0, map.Length, map[0].Length), LocTest);
            Assert.That(result.Map, Is.EqualTo(blob));
            Assert.That(result.Blobs, Is.EqualTo(compareBlobs));
        }

        [Test]
        public void DetectDisconnectNone()
        {
            // No disconnect
            string[] inGrid =
            {
                "XXXXXX",
                "X....X",
                "X....X",
                "XXXXXX",
            };

            string[] blobGrid =
            {
                "..",
                "..",
            };

            bool[][] map = GridTest.InitBoolGrid(inGrid);
            bool[][] blob = GridTest.InitBoolGrid(blobGrid);

            bool LocTest(Loc loc) => map[loc.X][loc.Y];
            bool BlobTest(Loc loc) => blob[loc.X][loc.Y];
            bool result = Detection.DetectDisconnect(new Rect(0, 0, map.Length, map[0].Length), LocTest, Loc.Zero, new Loc(blob.Length, blob[0].Length), BlobTest, false);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void DetectDisconnectSome()
        {
            // disconnect
            string[] inGrid =
            {
                "XXXXXX",
                "X....X",
                "X....X",
                "XXXXXX",
            };

            string[] blobGrid =
            {
                "..",
                "..",
            };

            bool[][] map = GridTest.InitBoolGrid(inGrid);
            bool[][] blob = GridTest.InitBoolGrid(blobGrid);

            bool LocTest(Loc loc) => map[loc.X][loc.Y];
            bool BlobTest(Loc loc) => blob[loc.X][loc.Y];
            bool result = Detection.DetectDisconnect(new Rect(0, 0, map.Length, map[0].Length), LocTest, new Loc(2, 1), new Loc(blob.Length, blob[0].Length), BlobTest, false);
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void DetectDisconnectErasure()
        {
            // total erasure (without tolerance)
            string[] inGrid =
            {
                "XXXXXX",
                "X....X",
                "X....X",
                "XXXXXX",
            };

            string[] blobGrid =
            {
                "....",
                "....",
            };

            bool[][] map = GridTest.InitBoolGrid(inGrid);
            bool[][] blob = GridTest.InitBoolGrid(blobGrid);

            bool LocTest(Loc loc) => map[loc.X][loc.Y];
            bool BlobTest(Loc loc) => blob[loc.X][loc.Y];
            bool result = Detection.DetectDisconnect(new Rect(0, 0, map.Length, map[0].Length), LocTest, Loc.One, new Loc(blob.Length, blob[0].Length), BlobTest, true);
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void DetectDisconnectErasureTolerant()
        {
            // total erasure (with tolerance)
            string[] inGrid =
            {
                "XXXXXX",
                "X....X",
                "X....X",
                "XXXXXX",
            };

            string[] blobGrid =
            {
                "....",
                "....",
            };

            bool[][] map = GridTest.InitBoolGrid(inGrid);
            bool[][] blob = GridTest.InitBoolGrid(blobGrid);

            bool LocTest(Loc loc) => map[loc.X][loc.Y];
            bool BlobTest(Loc loc) => blob[loc.X][loc.Y];
            bool result = Detection.DetectDisconnect(new Rect(0, 0, map.Length, map[0].Length), LocTest, Loc.One, new Loc(blob.Length, blob[0].Length), BlobTest, false);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        [Ignore("TODO")]
        public void DetectConnectedWalls()
        {
            // 4 sides
            // concave
            // convex

            // requires floodfill
            // requires getlocray
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void DetectWalls()
        {
            // requires getlocray
            throw new NotImplementedException();
        }

        [Test]
        [TestCase(0, Dir4.None)]
        [TestCase(1, Dir4.Up)]
        [TestCase(2, Dir4.Right)]
        [TestCase(3, Dir4.None)]
        [TestCase(4, Dir4.None)]
        [TestCase(5, Dir4.None)]
        [TestCase(6, Dir4.Up)]
        public void GetWallDir(int gridType, Dir4 result)
        {
            // no correct grounds
            string[] inGrid =
            {
                "XXX",
                "XXX",
                "XXX",
            };

            switch (gridType)
            {
                case 1:
                    // one correct ground, all valid block
                    inGrid = new string[]
                    {
                        "XXX",
                        "XXX",
                        "X.X",
                    };
                    break;
                case 2:
                    inGrid = new string[]
                    {
                        "XXX",
                        ".XX",
                        "XXX",
                    };
                    break;
                case 3:
                    // multiple correct ground
                    inGrid = new string[]
                    {
                        "XXX",
                        ".XX",
                        "X.X",
                    };
                    break;
                case 4:
                    // one correct ground, but one invalid
                    inGrid = new string[]
                    {
                        "XXX",
                        "~XX",
                        "X.X",
                    };
                    break;
                case 5:
                    // one correct ground, one crucial diagonal an invalid block
                    inGrid = new string[]
                    {
                        "XX~",
                        "XXX",
                        "X.X",
                    };
                    break;
                case 6:
                    // one correct ground, both noncrucial diagonal an invalid block
                    inGrid = new string[]
                    {
                        "XXX",
                        "XXX",
                        "~.~",
                    };
                    break;
                default:
                    break;
            }

            char[][] map = GridTest.InitGrid(inGrid);

            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';
            bool CheckGround(Loc testLoc) => map[testLoc.X][testLoc.Y] == '.';

            LocRay4 locRay = Detection.GetWallDir(new Loc(1), CheckBlock, CheckGround);
            Assert.That(locRay, Is.EqualTo(new LocRay4(new Loc(1), result)));
        }

        [Test]
        public void FindAllRectsEmpty()
        {
            // single rect
            string[] inGrid =
            {
                "......",
                "......",
                "......",
                "......",
                "......",
            };

            bool[][] map = InitGrid(inGrid);

            List<Rect> result = Detection.FindAllRects(map);
            var compare = new List<Rect>();
            Assert.That(result, Is.EqualTo(compare));
        }

        [Test]
        public void FindAllRectsSingle()
        {
            // single rect
            string[] inGrid =
            {
                "......XXXX",
                "......XXXX",
                "......XXXX",
                "..........",
                "..........",
            };

            bool[][] map = InitGrid(inGrid);

            List<Rect> result = Detection.FindAllRects(map);
            var compare = new List<Rect> { new Rect(6, 0, 4, 3) };
            Assert.That(result, Is.EqualTo(compare));
        }

        [Test]
        public void FindAllRects2Separate()
        {
            // 2 rects separate
            string[] inGrid =
            {
                "XX........",
                "XX........",
                "XX........",
                "........XX",
                "........XX",
            };

            bool[][] map = InitGrid(inGrid);

            List<Rect> result = Detection.FindAllRects(map);
            var compare = new List<Rect>
            {
                new Rect(0, 0, 2, 3),
                new Rect(8, 3, 2, 2),
            };
            Assert.That(result, Is.EqualTo(compare));
        }

        [Test]
        public void FindAllRects2Combined()
        {
            // 2 rects combined
            string[] inGrid =
            {
                "..........",
                "....XX....",
                "....XXX...",
                "....XXX...",
                "..........",
            };

            bool[][] map = InitGrid(inGrid);

            List<Rect> result = Detection.FindAllRects(map);
            var compare = new List<Rect>
            {
                new Rect(4, 1, 2, 3),
                new Rect(4, 2, 3, 2),
            };
            Assert.That(result, Is.EqualTo(compare));
        }

        [Test]
        public void FindAllRectsSlope()
        {
            // 3 rects together in slope
            string[] inGrid =
            {
                ".......XX.",
                "....XXXXX.",
                "....XXXXX.",
                "......XXX.",
                ".......XX.",
            };

            bool[][] map = InitGrid(inGrid);

            List<Rect> result = Detection.FindAllRects(map);
            var compare = new List<Rect>
            {
                new Rect(7, 0, 2, 5),
                new Rect(6, 1, 3, 3),
                new Rect(4, 1, 5, 2),
            };
            Assert.That(result, Is.EqualTo(compare));
        }

        [Test]
        public void FindAllRectsHorns()
        {
            // 3 rects together in "horn" formation
            string[] inGrid =
            {
                "..........",
                "XX........",
                "XX.XX.....",
                "XXXXX.....",
                "XXXXX.....",
            };

            bool[][] map = InitGrid(inGrid);

            List<Rect> result = Detection.FindAllRects(map);
            var compare = new List<Rect>
            {
                new Rect(0, 1, 2, 4),
                new Rect(3, 2, 2, 3),
                new Rect(0, 3, 5, 2),
            };
            Assert.That(result, Is.EqualTo(compare));
        }

        [Test]
        public void FindAllRectsFractal()
        {
            // fractal rect
            string[] inGrid =
            {
                ".XX.XX.XX.",
                ".XXXXXXXX.",
                "..XXXXXX..",
                ".XXXXXXXX.",
                ".XXXXXXXX.",
                "..XXXXXX..",
                ".XXXXXXXX.",
                ".XX.XX.XX.",
            };

            bool[][] map = InitGrid(inGrid);

            List<Rect> result = Detection.FindAllRects(map);
            var compare = new List<Rect>
            {
                new Rect(2, 0, 1, 8),
                new Rect(1, 0, 2, 2),
                new Rect(4, 0, 2, 8),
                new Rect(7, 0, 1, 8),
                new Rect(7, 0, 2, 2),
                new Rect(2, 1, 6, 6),
                new Rect(1, 1, 8, 1),
                new Rect(1, 3, 8, 2),
                new Rect(1, 6, 2, 2),
                new Rect(7, 6, 2, 2),
                new Rect(1, 6, 8, 1),
            };
            Assert.That(result, Is.EqualTo(compare));
        }
    }
}
