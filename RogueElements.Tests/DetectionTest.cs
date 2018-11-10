using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class DetectionTest
    {
        [Test]
        public void DetectBlobsNone()
        {
            //no blob
            string[] inGrid = { "XXXXXX",
                                "XXXXXX",
                                "XXXXXX",
                                "XXXXXX" };

            string[] outGrid = {"@@@@@@",
                                "@@@@@@",
                                "@@@@@@",
                                "@@@@@@" };


            bool[][] map = GridTest.InitBoolGrid(inGrid);
            int[][] blob = GridTest.InitIntGrid(outGrid);

            List<MapBlob> compareBlobs = new List<MapBlob>();

            BlobMap result = Detection.DetectBlobs(map);
            Assert.That(result.Map, Is.EqualTo(blob));
            Assert.That(result.Blobs, Is.EqualTo(compareBlobs));
        }
        [Test]
        public void DetectBlobsSingle()
        {
            //single blob
            string[] inGrid = { "XXXXXX",
                                "X..XXX",
                                "X....X",
                                "X.XXXX",
                                "XXXXXX" };

            string[] outGrid = {"@@@@@@",
                                "@AA@@@",
                                "@AAAA@",
                                "@A@@@@",
                                "@@@@@@" };


            bool[][] map = GridTest.InitBoolGrid(inGrid);
            int[][] blob = GridTest.InitIntGrid(outGrid);

            List<MapBlob> compareBlobs = new List<MapBlob>();
            compareBlobs.Add(new MapBlob(new Rect(1,1,4,3), 7));

            BlobMap result = Detection.DetectBlobs(map);
            Assert.That(result.Map, Is.EqualTo(blob));
            Assert.That(result.Blobs, Is.EqualTo(compareBlobs));
        }

        [Test]
        public void DetectBlobsDiagonal()
        {
            //blobs at corners
            string[] inGrid = { "XXXXXX",
                                "X..XXX",
                                "X..XXX",
                                "XXX..X",
                                "XXX..X",
                                "XXXXXX" };

            string[] outGrid = {"@@@@@@",
                                "@AA@@@",
                                "@AA@@@",
                                "@@@BB@",
                                "@@@BB@",
                                "@@@@@@" };


            bool[][] map = GridTest.InitBoolGrid(inGrid);
            int[][] blob = GridTest.InitIntGrid(outGrid);

            List<MapBlob> compareBlobs = new List<MapBlob>();
            compareBlobs.Add(new MapBlob(new Rect(1, 1, 2, 2), 4));
            compareBlobs.Add(new MapBlob(new Rect(3, 3, 2, 2), 4));

            BlobMap result = Detection.DetectBlobs(map);
            Assert.That(result.Map, Is.EqualTo(blob));
            Assert.That(result.Blobs, Is.EqualTo(compareBlobs));
        }

        [Test]
        public void DetectBlobsCorners()
        {
            //diagonal attached blobs
            string[] inGrid = { "..XX..",
                                "..XX..",
                                "XXXXXX",
                                "XXXXXX" };

            string[] outGrid = {"AA@@BB",
                                "AA@@BB",
                                "@@@@@@",
                                "@@@@@@" };


            bool[][] map = GridTest.InitBoolGrid(inGrid);
            int[][] blob = GridTest.InitIntGrid(outGrid);

            List<MapBlob> compareBlobs = new List<MapBlob>();
            compareBlobs.Add(new MapBlob(new Rect(0, 0, 2, 2), 4));
            compareBlobs.Add(new MapBlob(new Rect(4, 0, 2, 2), 4));

            BlobMap result = Detection.DetectBlobs(map);
            Assert.That(result.Map, Is.EqualTo(blob));
            Assert.That(result.Blobs, Is.EqualTo(compareBlobs));
        }

        [Test]
        public void DetectDisconnectNone()
        {
            //No disconnect
            string[] inGrid = { "XXXXXX",
                                "X....X",
                                "X....X",
                                "XXXXXX" };

            string[] blobGrid =   { "..",
                                    ".." };

            bool[][] map = GridTest.InitBoolGrid(inGrid);
            bool[][] blob = GridTest.InitBoolGrid(blobGrid);

            bool result = Detection.DetectDisconnect(map, blob, new Loc(), false);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void DetectDisconnectSome()
        {
            //disconnect
            string[] inGrid = { "XXXXXX",
                                "X....X",
                                "X....X",
                                "XXXXXX" };

            string[] blobGrid =   { "..",
                                    ".." };

            bool[][] map = GridTest.InitBoolGrid(inGrid);
            bool[][] blob = GridTest.InitBoolGrid(blobGrid);

            bool result = Detection.DetectDisconnect(map, blob, new Loc(2, 1), false);
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void DetectDisconnectErasure()
        {
            //total erasure (without tolerance)
            string[] inGrid = { "XXXXXX",
                                "X....X",
                                "X....X",
                                "XXXXXX" };

            string[] blobGrid =   { "....",
                                    "...." };

            bool[][] map = GridTest.InitBoolGrid(inGrid);
            bool[][] blob = GridTest.InitBoolGrid(blobGrid);

            bool result = Detection.DetectDisconnect(map, blob, new Loc(1, 1), true);
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void DetectDisconnectErasureTolerant()
        {
            //total erasure (with tolerance)
            string[] inGrid = { "XXXXXX",
                                "X....X",
                                "X....X",
                                "XXXXXX" };

            string[] blobGrid =   { "....",
                                    "...." };


            bool[][] map = GridTest.InitBoolGrid(inGrid);
            bool[][] blob = GridTest.InitBoolGrid(blobGrid);

            bool result = Detection.DetectDisconnect(map, blob, new Loc(1, 1), false);
            Assert.That(result, Is.EqualTo(false));
        }

        //TODO: [Test]
        public void DetectConnectedWalls()
        {
            //4 sides
            //concave
            //convex

            //requires floodfill
            //requires getlocray
            throw new NotImplementedException();
        }

        //TODO: [Test]
        public void DetectWalls()
        {
            //requires getlocray
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
            //no correct grounds
            string[] inGrid =  { "XXX",
                                 "XXX",
                                 "XXX" };
            if (gridType == 1)
            {
                //one correct ground, all valid block
                inGrid = new string[] { "XXX",
                                        "XXX",
                                        "X.X" };
            }
            else if (gridType == 2)
            {
                inGrid = new string[] { "XXX",
                                        ".XX",
                                        "XXX" };
            }
            else if (gridType == 3)
            {
                //multiple correct ground
                inGrid = new string[] { "XXX",
                                        ".XX",
                                        "X.X" };
            }
            else if (gridType == 4)
            {
                //one correct ground, but one invalid
                inGrid = new string[] { "XXX",
                                        "~XX",
                                        "X.X" };
            }
            else if (gridType == 5)
            {
                //one correct ground, one crucial diagonal an invalid block
                inGrid = new string[] { "XX~",
                                        "XXX",
                                        "X.X" };
            }
            else if (gridType == 6)
            {
                //one correct ground, both noncrucial diagonal an invalid block
                inGrid = new string[] { "XXX",
                                        "XXX",
                                        "~.~" };
            }

            char[][] map = GridTest.InitGrid(inGrid);

            Grid.LocTest checkBlock = (Loc testLoc) => { return map[testLoc.X][testLoc.Y] == 'X'; };
            Grid.LocTest checkGround = (Loc testLoc) => { return map[testLoc.X][testLoc.Y] == '.'; };
            
            LocRay4 locRay = Detection.GetWallDir(new Loc(1), checkBlock, checkGround);
            Assert.That(locRay, Is.EqualTo(new LocRay4(new Loc(1), result)));
        }


        [Test]
        public void FindAllRectsSingle()
        {
            //single rect
            string[] inGrid =  { "......XXXX",
                                 "......XXXX",
                                 "......XXXX",
                                 "..........",
                                 ".........." };

            bool[][] map = InitGrid(inGrid);

            List<Rect> result = Detection.FindAllRects(map);
            List<Rect> compare = new List<Rect>();
            compare.Add(new Rect(6, 0, 4, 3));
            Assert.That(result, Is.EqualTo(compare));
        }

        [Test]
        public void FindAllRects2Separate()
        {
            //2 rects separate
            string[] inGrid =  { "XX........",
                                 "XX........",
                                 "XX........",
                                 "........XX",
                                 "........XX" };

            bool[][] map = InitGrid(inGrid);

            List<Rect> result = Detection.FindAllRects(map);
            List<Rect> compare = new List<Rect>();
            compare.Add(new Rect(0, 0, 2, 3));
            compare.Add(new Rect(8, 3, 2, 2));
            Assert.That(result, Is.EqualTo(compare));
        }

        [Test]
        public void FindAllRects2Combined()
        {
            //2 rects combined
            string[] inGrid =  { "..........",
                                 "....XX....",
                                 "....XXX...",
                                 "....XXX...",
                                 ".........." };

            bool[][] map = InitGrid(inGrid);

            List<Rect> result = Detection.FindAllRects(map);
            List<Rect> compare = new List<Rect>();
            compare.Add(new Rect(4, 1, 2, 3));
            compare.Add(new Rect(4, 2, 3, 2));
            Assert.That(result, Is.EqualTo(compare));
        }

        [Test]
        public void FindAllRectsSlope()
        {
            //3 rects together in slope
            string[] inGrid =  { ".......XX.",
                                 "....XXXXX.",
                                 "....XXXXX.",
                                 "......XXX.",
                                 ".......XX." };

            bool[][] map = InitGrid(inGrid);

            List<Rect> result = Detection.FindAllRects(map);
            List<Rect> compare = new List<Rect>();
            compare.Add(new Rect(7, 0, 2, 5));
            compare.Add(new Rect(6, 1, 3, 3));
            compare.Add(new Rect(4, 1, 5, 2));
            Assert.That(result, Is.EqualTo(compare));
        }

        [Test]
        public void FindAllRectsHorns()
        {
            //3 rects together in "horn" formation
            string[] inGrid =  { "..........",
                                 "XX........",
                                 "XX.XX.....",
                                 "XXXXX.....",
                                 "XXXXX....." };

            bool[][] map = InitGrid(inGrid);

            List<Rect> result = Detection.FindAllRects(map);
            List<Rect> compare = new List<Rect>();
            compare.Add(new Rect(0, 1, 2, 4));
            compare.Add(new Rect(3, 2, 2, 3));
            compare.Add(new Rect(0, 3, 5, 2));
            Assert.That(result, Is.EqualTo(compare));
        }


        [Test]
        public void FindAllRectsFractal()
        {
            //fractal rect
            string[] inGrid =  { ".XX.XX.XX.",
                                 ".XXXXXXXX.",
                                 "..XXXXXX..",
                                 ".XXXXXXXX.",
                                 ".XXXXXXXX.",
                                 "..XXXXXX..",
                                 ".XXXXXXXX.",
                                 ".XX.XX.XX." };

            bool[][] map = InitGrid(inGrid);

            List<Rect> result = Detection.FindAllRects(map);
            List<Rect> compare = new List<Rect>();
            compare.Add(new Rect(2, 0, 1, 8));
            compare.Add(new Rect(1, 0, 2, 2));
            compare.Add(new Rect(4, 0, 2, 8));
            compare.Add(new Rect(7, 0, 1, 8));
            compare.Add(new Rect(7, 0, 2, 2));
            compare.Add(new Rect(2, 1, 6, 6));
            compare.Add(new Rect(1, 1, 8, 1));
            compare.Add(new Rect(1, 3, 8, 2));
            compare.Add(new Rect(1, 6, 2, 2));
            compare.Add(new Rect(7, 6, 2, 2));
            compare.Add(new Rect(1, 6, 8, 1));
            Assert.That(result, Is.EqualTo(compare));
        }


        public static bool[][] InitGrid(string[] inGrid)
        {
            //transposes
            bool[][] result_map = new bool[inGrid[0].Length][];
            for (int xx = 0; xx < result_map.Length; xx++)
            {
                result_map[xx] = new bool[inGrid.Length];
                for (int yy = 0; yy < result_map[xx].Length; yy++)
                    result_map[xx][yy] = inGrid[yy][xx] == 'X';
            }
            return result_map;
        }
    }
}
