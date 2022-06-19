// <copyright file="GridTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class GridTest
    {
        public static char[][] InitGrid(string[] inGrid)
        {
            // transposes
            char[][] result_map = new char[inGrid[0].Length][];
            for (int xx = 0; xx < result_map.Length; xx++)
            {
                result_map[xx] = new char[inGrid.Length];
                for (int yy = 0; yy < result_map[xx].Length; yy++)
                    result_map[xx][yy] = inGrid[yy][xx];
            }

            return result_map;
        }

        public static bool[][] InitBoolGrid(string[] inGrid)
        {
            // transposes
            bool[][] result_map = new bool[inGrid[0].Length][];
            for (int xx = 0; xx < result_map.Length; xx++)
            {
                result_map[xx] = new bool[inGrid.Length];
                for (int yy = 0; yy < result_map[xx].Length; yy++)
                    result_map[xx][yy] = inGrid[yy][xx] == '.';
            }

            return result_map;
        }

        public static int[][] InitIntGrid(string[] inGrid)
        {
            // transposes
            int[][] result_map = new int[inGrid[0].Length][];
            for (int xx = 0; xx < result_map.Length; xx++)
            {
                result_map[xx] = new int[inGrid.Length];
                for (int yy = 0; yy < result_map[xx].Length; yy++)
                    result_map[xx][yy] = inGrid[yy][xx] - 'A';
            }

            return result_map;
        }

        [Test]
        public void FindAPathStraight()
        {
            string[] inGrid =
            {
                ".......",
                ".......",
                ".......",
                ".......",
                ".......",
                ".A...B.",
                ".......",
            };

            // test against a straight obstacle (1 goal)
            char[][] map = InitGrid(inGrid);
            Loc start = Loc.Zero;
            Loc[] ends = new Loc[1];
            for (int xx = 0; xx < map.Length; xx++)
            {
                for (int yy = 0; yy < map[0].Length; yy++)
                {
                    if (map[xx][yy] == 'A')
                        start = new Loc(xx, yy);
                    else if (map[xx][yy] == 'B')
                        ends[0] = new Loc(xx, yy);
                }
            }

            List<Loc> result_path = new List<Loc>
            {
                new Loc(5, 5),
                new Loc(4, 5),
                new Loc(3, 5),
                new Loc(2, 5),
                new Loc(1, 5),
            };

            // nothing is blocked
            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';
            bool CheckDiag(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';

            List<Loc> path = Grid.FindAPath(Loc.Zero, new Loc(map.Length, map[0].Length), start, ends, CheckBlock, CheckDiag);
            Assert.That(path, Is.EquivalentTo(result_path));
        }

        [Test]
        public void FindAPathCurved()
        {
            string[] inGrid =
            {
                ".......",
                ".......",
                ".......",
                ".XXXXX.",
                "...X...",
                ".A.X.B.",
                "...X...",
            };

            // test against a concave obstacle (1 goal)
            char[][] map = InitGrid(inGrid);
            Loc start = Loc.Zero;
            List<Loc> ends = new List<Loc>();
            for (int xx = 0; xx < map.Length; xx++)
            {
                for (int yy = 0; yy < map[0].Length; yy++)
                {
                    if (map[xx][yy] == 'A')
                        start = new Loc(xx, yy);
                    else if (map[xx][yy] == 'B')
                        ends.Add(new Loc(xx, yy));
                }
            }

            List<Loc> result_path = new List<Loc>
            {
                new Loc(5, 5),
                new Loc(6, 4),
                new Loc(6, 3),
                new Loc(6, 2),
                new Loc(5, 2),
                new Loc(4, 2),
                new Loc(3, 2),
                new Loc(2, 2),
                new Loc(1, 2),
                new Loc(0, 2),
                new Loc(0, 3),
                new Loc(0, 4),
                new Loc(1, 5),
            };

            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';
            bool CheckDiag(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';

            List<Loc> path = Grid.FindAPath(Loc.Zero, new Loc(map.Length, map[0].Length), start, ends.ToArray(), CheckBlock, CheckDiag);
            Assert.That(path, Is.EquivalentTo(result_path));
        }

        [Test]
        public void FindAPathImpossible()
        {
            string[] inGrid =
            {
                "...X...",
                ".A.X...",
                "...X...",
                ".XXXXX.",
                "...X...",
                "...X.B.",
                "...X...",
            };

            // test against an impossible obstacle (1 goal), result in the closest location that works
            char[][] map = InitGrid(inGrid);
            Loc start = Loc.Zero;
            List<Loc> ends = new List<Loc>();
            for (int xx = 0; xx < map.Length; xx++)
            {
                for (int yy = 0; yy < map[0].Length; yy++)
                {
                    if (map[xx][yy] == 'A')
                        start = new Loc(xx, yy);
                    else if (map[xx][yy] == 'B')
                        ends.Add(new Loc(xx, yy));
                }
            }

            List<Loc> result_path = new List<Loc>
            {
                new Loc(2, 5),
                new Loc(1, 5),
                new Loc(0, 4),
                new Loc(0, 3),
                new Loc(0, 2),
                new Loc(1, 1),
            };

            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';
            bool CheckDiag(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';

            List<Loc> path = Grid.FindAPath(Loc.Zero, new Loc(map.Length, map[0].Length), start, ends.ToArray(), CheckBlock, CheckDiag);
            Assert.That(path, Is.EquivalentTo(result_path));
        }

        [Test]
        public void FindAPath2Goals()
        {
            string[] inGrid =
            {
                "...B...",
                ".......",
                ".......",
                ".XXXXX.",
                "...X...",
                ".A.X.B.",
                "...X...",
            };

            // test against 2 goals, the closer one (in terms of steps, not initial heuristic evaluation) should be reached first
            char[][] map = InitGrid(inGrid);
            Loc start = Loc.Zero;
            List<Loc> ends = new List<Loc>();
            for (int xx = 0; xx < map.Length; xx++)
            {
                for (int yy = 0; yy < map[0].Length; yy++)
                {
                    if (map[xx][yy] == 'A')
                        start = new Loc(xx, yy);
                    else if (map[xx][yy] == 'B')
                        ends.Add(new Loc(xx, yy));
                }
            }

            List<Loc> result_path = new List<Loc>
            {
                new Loc(3, 0),
                new Loc(2, 0),
                new Loc(1, 1),
                new Loc(0, 2),
                new Loc(0, 3),
                new Loc(0, 4),
                new Loc(1, 5),
            };

            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';
            bool CheckDiag(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';

            List<Loc> path = Grid.FindAPath(Loc.Zero, new Loc(map.Length, map[0].Length), start, ends.ToArray(), CheckBlock, CheckDiag);
            Assert.That(path, Is.EquivalentTo(result_path));
        }

        [Test]
        public void FindAllPaths2Goals()
        {
            string[] inGrid =
            {
                "...B...",
                ".......",
                ".......",
                ".XXXXX.",
                "...X...",
                ".A.X.B.",
                "...X...",
            };

            // test against 2 goals, the closer one (in terms of steps, not initial heuristic evaluation) should be reached first
            char[][] map = InitGrid(inGrid);
            Loc start = Loc.Zero;
            List<Loc> ends = new List<Loc>();
            for (int xx = 0; xx < map.Length; xx++)
            {
                for (int yy = 0; yy < map[0].Length; yy++)
                {
                    if (map[xx][yy] == 'A')
                        start = new Loc(xx, yy);
                    else if (map[xx][yy] == 'B')
                        ends.Add(new Loc(xx, yy));
                }
            }

            List<Loc> result_path_0 = new List<Loc>
            {
                new Loc(3, 0),
                new Loc(2, 0),
                new Loc(1, 1),
                new Loc(0, 2),
                new Loc(0, 3),
                new Loc(0, 4),
                new Loc(1, 5),
            };

            List<Loc> result_path_1 = new List<Loc>
            {
                new Loc(5, 5),
                new Loc(6, 4),
                new Loc(6, 3),
                new Loc(6, 2),
                new Loc(5, 1),
                new Loc(4, 0),
                new Loc(3, 0),
                new Loc(2, 0),
                new Loc(1, 1),
                new Loc(0, 2),
                new Loc(0, 3),
                new Loc(0, 4),
                new Loc(1, 5),
            };

            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';
            bool CheckDiag(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';

            List<Loc>[] path = Grid.FindAllPaths(Loc.Zero, new Loc(map.Length, map[0].Length), start, ends.ToArray(), CheckBlock, CheckDiag);
            Assert.That(path[0], Is.EquivalentTo(result_path_0));
            Assert.That(path[1], Is.EquivalentTo(result_path_1));
        }

        [Test]
        public void FindBestPaths()
        {
            string[] inGrid =
            {
                ".....B..",
                "....B...",
                "...B....",
                "..B..A..",
                ".B......",
                "BBBBBBBB",
            };

            // test against 2 goals, the closer one (in terms of steps, not initial heuristic evaluation) should be reached first
            char[][] map = InitGrid(inGrid);
            Loc start = Loc.Zero;
            List<Loc> ends = new List<Loc>();
            for (int xx = 0; xx < map.Length; xx++)
            {
                for (int yy = 0; yy < map[0].Length; yy++)
                {
                    if (map[xx][yy] == 'A')
                        start = new Loc(xx, yy);
                    else if (map[xx][yy] == 'B')
                        ends.Add(new Loc(xx, yy));
                }
            }

            List<Loc>[] result_paths = new List<Loc>[ends.Count];
            result_paths[5] = new List<Loc>
            {
                new Loc(3, 2),
                new Loc(4, 2),
                new Loc(5, 3),
            };

            result_paths[6] = new List<Loc>
            {
                new Loc(3, 5),
                new Loc(4, 4),
                new Loc(5, 3),
            };

            result_paths[7] = new List<Loc>
            {
                new Loc(4, 1),
                new Loc(4, 2),
                new Loc(5, 3),
            };

            result_paths[8] = new List<Loc>
            {
                new Loc(4, 5),
                new Loc(5, 4),
                new Loc(5, 3),
            };

            result_paths[10] = new List<Loc>
            {
                new Loc(5, 5),
                new Loc(5, 4),
                new Loc(5, 3),
            };

            result_paths[11] = new List<Loc>
            {
                new Loc(6, 5),
                new Loc(5, 4),
                new Loc(5, 3),
            };

            result_paths[12] = new List<Loc>
            {
                new Loc(7, 5),
                new Loc(6, 4),
                new Loc(5, 3),
            };

            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';
            bool CheckDiag(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';

            List<Loc>[] path = Grid.FindNPaths(Loc.Zero, new Loc(map.Length, map[0].Length), start, ends.ToArray(), CheckBlock, CheckDiag, 1, true);
            for (int ii = 0; ii < ends.Count; ii++)
            {
                if (result_paths[ii] != null)
                    Assert.That(path[ii], Is.EquivalentTo(result_paths[ii]));
                else
                    Assert.That(path[ii], Is.Null);
            }
        }

        [Test]
        public void FindBestPathsBlocked()
        {
            string[] inGrid =
            {
                ".....BX.",
                "....BX..",
                "...BX...",
                "..BX.A..",
                ".BX.....",
                "BXBBBBBB",
            };

            // test against 2 goals, the closer one (in terms of steps, not initial heuristic evaluation) should be reached first
            char[][] map = InitGrid(inGrid);
            Loc start = Loc.Zero;
            List<Loc> ends = new List<Loc>();
            for (int xx = 0; xx < map.Length; xx++)
            {
                for (int yy = 0; yy < map[0].Length; yy++)
                {
                    if (map[xx][yy] == 'A')
                        start = new Loc(xx, yy);
                    else if (map[xx][yy] == 'B')
                        ends.Add(new Loc(xx, yy));
                }
            }

            List<Loc>[] result_paths = new List<Loc>[ends.Count];

            result_paths[5] = new List<Loc>
            {
                new Loc(3, 5),
                new Loc(4, 4),
                new Loc(5, 3),
            };

            result_paths[7] = new List<Loc>
            {
                new Loc(4, 5),
                new Loc(5, 4),
                new Loc(5, 3),
            };

            result_paths[9] = new List<Loc>
            {
                new Loc(5, 5),
                new Loc(5, 4),
                new Loc(5, 3),
            };

            result_paths[10] = new List<Loc>
            {
                new Loc(6, 5),
                new Loc(5, 4),
                new Loc(5, 3),
            };

            result_paths[11] = new List<Loc>
            {
                new Loc(7, 5),
                new Loc(6, 4),
                new Loc(5, 3),
            };

            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';
            bool CheckDiag(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';

            List<Loc>[] path = Grid.FindNPaths(Loc.Zero, new Loc(map.Length, map[0].Length), start, ends.ToArray(), CheckBlock, CheckDiag, 1, true);
            for (int ii = 0; ii < ends.Count; ii++)
            {
                if (result_paths[ii] != null)
                    Assert.That(path[ii], Is.EquivalentTo(result_paths[ii]));
                else
                    Assert.That(path[ii], Is.Null);
            }
        }

        [Test]
        public void FindBestPathsNone()
        {
            string[] inGrid =
            {
                ".......",
                ".......",
                ".......",
                "....A..",
                ".......",
                ".......",
            };

            // test against 2 goals, the closer one (in terms of steps, not initial heuristic evaluation) should be reached first
            char[][] map = InitGrid(inGrid);
            Loc start = Loc.Zero;
            List<Loc> ends = new List<Loc>();
            for (int xx = 0; xx < map.Length; xx++)
            {
                for (int yy = 0; yy < map[0].Length; yy++)
                {
                    if (map[xx][yy] == 'A')
                        start = new Loc(xx, yy);
                    else if (map[xx][yy] == 'B')
                        ends.Add(new Loc(xx, yy));
                }
            }

            List<Loc>[] result_paths = new List<Loc>[ends.Count];

            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';
            bool CheckDiag(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';

            List<Loc>[] path = Grid.FindNPaths(Loc.Zero, new Loc(map.Length, map[0].Length), start, ends.ToArray(), CheckBlock, CheckDiag, 1, true);
            Assert.That(path, Is.EquivalentTo(result_paths));
        }

        [Test]
        public void FloodFillSubRect()
        {
            // filling a sub-rectangle of the main map
            // a 3x3 in 7x7
            string[] inGrid =
            {
                ".......",
                ".......",
                ".......",
                ".......",
                ".......",
                ".......",
                ".......",
            };

            string[] outGrid =
            {
                ".......",
                ".......",
                "..~~~..",
                "..~~~..",
                "..~~~..",
                ".......",
                ".......",
            };

            char[][] map = InitGrid(inGrid);
            char[][] result_map = InitGrid(outGrid);

            // nothing is blocked
            bool CheckBlock(Loc testLoc) => false;
            bool CheckDiag(Loc testLoc) => false;
            void Fill(Loc fillLoc) => map[fillLoc.X][fillLoc.Y] = '~';

            Grid.FloodFill(new Rect(new Loc(2), new Loc(3)), CheckBlock, CheckDiag, Fill, new Loc(2));
            Assert.That(map, Is.EqualTo(result_map));
        }

        [Test]
        public void FloodFillNegative()
        {
            // filling a sub-rectangle of the main map
            // in which the sub-rectangle crosses negative
            // a 3x3 in 7x7
            string[] inGrid =
            {
                ".......",
                ".......",
                ".......",
                ".......",
                ".......",
                ".......",
                ".......",
            };

            string[] outGrid =
            {
                ".......",
                ".......",
                "~~.....",
                "~~.....",
                "~~.....",
                "~~.....",
                ".......",
            };

            char[][] map = InitGrid(inGrid);
            char[][] result_map = InitGrid(outGrid);

            // nothing is blocked
            bool CheckBlock(Loc testLoc) => false;
            bool CheckDiag(Loc testLoc) => false;
            void Fill(Loc fillLoc)
            {
                if (fillLoc.X >= 0)
                    map[fillLoc.X][fillLoc.Y] = '~';
            }

            Grid.FloodFill(new Rect(new Loc(-2, 2), new Loc(4)), CheckBlock, CheckDiag, Fill, new Loc(-2, 2));
            Assert.That(map, Is.EqualTo(result_map));
        }

        [Test]
        [TestCase(6, 3)]
        [TestCase(6, 9)]
        [TestCase(3, 6)]
        [TestCase(9, 6)]
        public void FloodFillRing(int startX, int startY)
        {
            // concave structures
            // a 3x3 block inside a 3x3 wide floor ring
            // inside a 1x1 wide block ring inside a 1x1 floor
            // fill from various places
            string[] inGrid =
            {
                ".............",
                ".XXXXXXXXXXX.",
                ".X.........X.",
                ".X.........X.",
                ".X..X...X..X.",
                ".X...XXX...X.",
                ".X...XXX...X.",
                ".X...XXX...X.",
                ".X..X...X..X.",
                ".X.........X.",
                ".X.........X.",
                ".XXXXXXXXXXX.",
                ".............",
            };

            string[] outGrid =
            {
                ".............",
                ".XXXXXXXXXXX.",
                ".X~~~~~~~~~X.",
                ".X~~~~~~~~~X.",
                ".X~~X~~~X~~X.",
                ".X~~~XXX~~~X.",
                ".X~~~XXX~~~X.",
                ".X~~~XXX~~~X.",
                ".X~~X~~~X~~X.",
                ".X~~~~~~~~~X.",
                ".X~~~~~~~~~X.",
                ".XXXXXXXXXXX.",
                ".............",
            };

            char[][] map = InitGrid(inGrid);
            char[][] result_map = InitGrid(outGrid);

            // nothing is blocked
            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] != '.';
            bool CheckDiag(Loc testLoc) => map[testLoc.X][testLoc.Y] != '.';
            void Fill(Loc fillLoc) => map[fillLoc.X][fillLoc.Y] = '~';

            Grid.FloodFill(new Rect(Loc.Zero, new Loc(map[0].Length, map.Length)), CheckBlock, CheckDiag, Fill, new Loc(startX, startY));
            Assert.That(map, Is.EqualTo(result_map));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void FloodFillDiagonal(bool diagonal)
        {
            // two squares diagonally adjacent to each other
            string[] inGrid =
            {
                "..XXX..",
                "..XXX..",
                "XX...XX",
                "XX...XX",
                "XX...XX",
                "..XXX..",
                "..XXX..",
            };

            string[] outGrid1 =
            {
                "..XXX..",
                "..XXX..",
                "XX~~~XX",
                "XX~~~XX",
                "XX~~~XX",
                "..XXX..",
                "..XXX..",
            };

            string[] outGrid2 =
            {
                "~~XXX~~",
                "~~XXX~~",
                "XX~~~XX",
                "XX~~~XX",
                "XX~~~XX",
                "~~XXX~~",
                "~~XXX~~",
            };

            char[][] map = InitGrid(inGrid);
            char[][] result_map;
            if (diagonal)
                result_map = InitGrid(outGrid2);
            else
                result_map = InitGrid(outGrid1);

            // nothing is blocked
            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] != '.';

            Grid.LocTest checkDiag;
            if (diagonal)
                checkDiag = (Loc testLoc) => false;
            else
                checkDiag = (Loc testLoc) => map[testLoc.X][testLoc.Y] != '.';

            void Fill(Loc fillLoc) => map[fillLoc.X][fillLoc.Y] = '~';

            Grid.FloodFill(new Rect(Loc.Zero, new Loc(map[0].Length, map.Length)), CheckBlock, checkDiag, Fill, new Loc(3, 3));
            Assert.That(map, Is.EqualTo(result_map));
        }

        [Test]
        public void FloodFillInit()
        {
            // a previous error case of one downward left preventing fill of the one tile above
            string[] inGrid =
            {
                "XXX...",
                "......",
                ".X....",
                ".X....",
            };

            string[] outGrid =
            {
                "XXX~~~",
                "~~~~~~",
                "~X~~~~",
                "~X~~~~",
            };

            char[][] map = InitGrid(inGrid);
            char[][] result_map = InitGrid(outGrid);

            // nothing is blocked
            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] != '.';
            bool CheckDiag(Loc testLoc) => map[testLoc.X][testLoc.Y] != '.';
            void Fill(Loc fillLoc) => map[fillLoc.X][fillLoc.Y] = '~';

            Grid.FloodFill(new Rect(Loc.Zero, new Loc(map.Length, map[0].Length)), CheckBlock, CheckDiag, Fill, new Loc(3, 1));
            Assert.That(map, Is.EqualTo(result_map));
        }

        [Test]
        [Ignore("TODO")]
        public void ResizeGrid()
        {
            // resize with an invalid dir
            // resize by adding 1 to down or left
            // resize by adding 1 to center
            // resize by adding 1 to up or right
            // resize by adding 10
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void FindClosestConnectedTiles()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void FindTilesInBox()
        {
            // a 5x5 circle
            // a 5x5 circle cut in two
            // a 5x5 circle with offset
            // 0 or negative rect size
            throw new NotImplementedException();
        }

        [Test]
        [TestCase(0, false)]
        [TestCase(1, false)]
        [TestCase(2, true)]
        [TestCase(3, true)]
        [TestCase(4, true)]
        [TestCase(5, false)]
        public void IsChokePoint(int gridType, bool result)
        {
            string[] inGrid;
            switch (gridType)
            {
                case 0:
                    // attempt one without a fork (no)
                    inGrid = new string[]
                    {
                        ".....",
                        ".....",
                        ".....",
                        ".....",
                        ".....",
                    };
                    break;
                case 1:
                    // then a connected 2-way fork
                    inGrid = new string[]
                    {
                        "XX...",
                        "XX.X.",
                        "XX.X.",
                        "XX.X.",
                        "XX...",
                    };
                    break;
                case 2:
                    // then a disconnected 2-way fork
                    inGrid = new string[]
                    {
                        "XX...",
                        "XX.X.",
                        "XX.XX",
                        "XX.X.",
                        "XX...",
                    };
                    break;
                case 3:
                    // then a 3-connected, 1-disconnected 4-way fork
                    inGrid = new string[]
                    {
                        "XX...",
                        "XX.X.",
                        "X....",
                        "XX.X.",
                        "XX...",
                    };
                    break;
                case 4:
                    // attempt one that is a completely blocked fork (yes)
                    inGrid = new string[]
                    {
                        "XX.XX",
                        "XX.XX",
                        "..X..",
                        "XX.XX",
                        "XX.XX",
                    };
                    break;
                case 5:
                    // attempt center block without a fork (no)
                    inGrid = new string[]
                    {
                        ".....",
                        ".....",
                        "..X..",
                        ".....",
                        ".....",
                    };
                    break;
                default:
                    throw new Exception();
            }

            char[][] map = InitGrid(inGrid);

            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';
            bool CheckDiag(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';

            bool isChoke = Grid.IsChokePoint(Loc.Zero, new Loc(map[0].Length, map.Length), new Loc(2), CheckBlock, CheckDiag);
            Assert.That(isChoke, Is.EqualTo(result));
        }

        [Test]
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void IsChokePointSubRect(int gridType, bool result)
        {
            string[] inGrid;
            switch (gridType)
            {
                case 0:
                    // attempt one without a fork (no)
                    inGrid = new string[]
                    {
                        ".......",
                        ".......",
                        ".......",
                        ".......",
                        ".......",
                        ".......",
                        ".......",
                    };
                    break;
                case 1:
                    // then a connected 2-way fork
                    inGrid = new string[]
                    {
                        "XXX....",
                        "XXXXXX.",
                        "XXXXXX.",
                        "XXX....",
                        "XXX.XX.",
                        "XXX.XX.",
                        "XXX....",
                    };
                    break;
                default:
                    throw new Exception();
            }

            char[][] map = InitGrid(inGrid);

            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';
            bool CheckDiag(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';

            bool isChoke = Grid.IsChokePoint(new Loc(1), new Loc(5), new Loc(3), CheckBlock, CheckDiag);
            Assert.That(isChoke, Is.EqualTo(result));
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(true, true)]
        public void IsChokePointDiagonal(bool diagonal, bool result)
        {
            string[] inGrid =
            {
                ".XXX.",
                "X.X.X",
                "XX.XX",
                "X.X.X",
                ".XXX.",
            };

            char[][] map = InitGrid(inGrid);

            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';

            Grid.LocTest checkDiag;
            if (diagonal)
                checkDiag = (Loc testLoc) => false;
            else
                checkDiag = (Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';

            bool isChoke = Grid.IsChokePoint(Loc.Zero, new Loc(map[0].Length, map.Length), new Loc(2), CheckBlock, checkDiag);
            Assert.That(isChoke, Is.EqualTo(result));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void IsChokePointSubRect(bool diagonal)
        {
            string[] inGrid =
            {
                ".....",
                ".X.X.",
                ".....",
                ".X.X.",
                ".....",
            };

            char[][] map = InitGrid(inGrid);

            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';

            Grid.LocTest checkDiag;
            if (diagonal)
                checkDiag = (Loc testLoc) => false;
            else
                checkDiag = (Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';

            // The blocked tile counts as blocking all, even diagonals.
            bool isChoke = Grid.IsChokePoint(Loc.One, new Loc(map[0].Length, map.Length) - new Loc(2), new Loc(2), CheckBlock, checkDiag);
            Assert.That(isChoke, Is.EqualTo(true));
        }

        [Test]
        [TestCase(0, false)]
        [TestCase(1, false)]
        [TestCase(2, false, Dir8.Down)]
        [TestCase(3, false, Dir8.Left, Dir8.Right)]
        [TestCase(3, true, Dir8.DownLeft, Dir8.UpRight)]
        [TestCase(4, false, Dir8.Left, Dir8.Right)]
        [TestCase(4, true, Dir8.Left, Dir8.Right)]
        [TestCase(5, false, Dir8.Up)]
        [TestCase(5, true, Dir8.DownLeft, Dir8.UpLeft)]
        [TestCase(6, false, Dir8.Down, Dir8.Left, Dir8.Up, Dir8.Right)]
        [TestCase(6, true, Dir8.Down, Dir8.Left, Dir8.Up, Dir8.Right)]
        [TestCase(7, false)]
        [TestCase(7, true, Dir8.DownLeft, Dir8.UpLeft, Dir8.UpRight, Dir8.DownRight)]
        [TestCase(8, false)]
        public void GetForkDirs(int gridType, bool diagonal, params Dir8[] resultList)
        {
            // a fully empty surrounding
            string[] inGrid;
            switch (gridType)
            {
                case 0:
                    inGrid = new string[]
                    {
                        "...",
                        "...",
                        "...",
                    };
                    break;
                case 1:
                    inGrid = new string[]
                    {
                        "...",
                        ".X.",
                        "...",
                    };
                    break;
                case 2:
                    inGrid = new string[]
                    {
                        "...",
                        "...",
                        "..X",
                    };
                    break;
                case 3:
                    inGrid = new string[]
                    {
                        ".X.",
                        "...",
                        ".X.",
                    };
                    break;
                case 4:
                    inGrid = new string[]
                    {
                        "..X",
                        "...",
                        "X..",
                    };
                    break;
                case 5:
                    inGrid = new string[]
                    {
                        "...",
                        "X..",
                        ".X.",
                    };
                    break;
                case 6:
                    inGrid = new string[]
                    {
                        "X.X",
                        "...",
                        "X.X",
                    };
                    break;
                case 7:
                    inGrid = new string[]
                    {
                        ".X.",
                        "X.X",
                        ".X.",
                    };
                    break;
                case 8:
                    inGrid = new string[]
                    {
                        "XXX",
                        "X.X",
                        "XXX",
                    };
                    break;
                default:
                    throw new Exception();
            }

            char[][] map = InitGrid(inGrid);

            List<Dir8> compare = new List<Dir8>();
            foreach (Dir8 dir in resultList)
                compare.Add(dir);

            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';

            Grid.LocTest checkDiag;
            if (diagonal)
                checkDiag = (Loc testLoc) => false;
            else
                checkDiag = (Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';

            List<Dir8> result = Grid.GetForkDirs(new Loc(1), CheckBlock, checkDiag);
            Assert.That(result, Is.EquivalentTo(compare));
        }

        [Test]
        [TestCase(0, 1, false)]
        [TestCase(0, 0, false)]
        [TestCase(1, 1, true)]
        [TestCase(1, 2, false)]
        [TestCase(1, 0, false)]
        [TestCase(5, 0, false)]
        [TestCase(5, 1, true)]
        [TestCase(5, 4, true)]
        [TestCase(5, 5, true)]
        [TestCase(5, 6, false)]
        public void IsDirBlocked(int distance, int blockDistance, bool result)
        {
            string[] inGrid =
            {
                "..........",
                ".01234567.",
                "..........",
            };

            char[][] map = InitGrid(inGrid);
            Loc start = new Loc(1, 1) + (Dir8.Right.GetLoc() * blockDistance);
            map[start.X][start.Y] = 'X';

            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';
            bool CheckDiag(Loc testLoc) => map[testLoc.X][testLoc.Y] == 'X';

            bool blocked = Grid.IsDirBlocked(new Loc(1, 1), Dir8.Right, CheckBlock, CheckDiag, distance);
            Assert.That(blocked, Is.EqualTo(result));
        }

        [Test]
        [TestCase(Dir8.Down, 6, true)]
        [TestCase(Dir8.Down, 5, false)]
        [TestCase(Dir8.DownLeft, 5, true)]
        [TestCase(Dir8.DownLeft, 4, false)]
        [TestCase(Dir8.DownRight, 3, true)]
        [TestCase(Dir8.DownRight, 2, false)]
        [TestCase(Dir8.Up, 2, true)]
        [TestCase(Dir8.None, 100, false)]
        public void IsDirBlockedDir(Dir8 dir, int distance, bool result)
        {
            string[] inGrid =
            {
                "XXXXXXXXXXXXX",
                "XX....X.....X",
                "X.X...X.....X",
                "X..X..X.....X",
                "X.....X.....X",
                "X...........X",
                "XXX...X....XX",
                "X...........X",
                "X...........X",
                "X........X..X",
                "X.........X.X",
                "XX.........XX",
                "XXXXXXXXXXXXX",
            };

            char[][] map = InitGrid(inGrid);

            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] != '.';
            bool CheckDiag(Loc testLoc) => map[testLoc.X][testLoc.Y] != '.';

            bool blocked = Grid.IsDirBlocked(new Loc(6, 6), dir, CheckBlock, CheckDiag, distance);
            Assert.That(blocked, Is.EqualTo(result));
        }

        [Test]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void IsDirBlockedDiagonal(bool diagonal, bool result)
        {
            string[] inGrid =
            {
                "XXXXXX",
                "X.XXXX",
                "XX.XXX",
                "XXX.XX",
                "XXXX.X",
                "XXXXXX",
            };

            char[][] map = InitGrid(inGrid);

            bool CheckBlock(Loc testLoc) => map[testLoc.X][testLoc.Y] != '.';

            Grid.LocTest checkDiag;
            if (diagonal)
                checkDiag = (Loc testLoc) => false;
            else
                checkDiag = (Loc testLoc) => map[testLoc.X][testLoc.Y] != '.';

            bool blocked = Grid.IsDirBlocked(new Loc(1, 1), Dir8.DownRight, CheckBlock, checkDiag, 3);
            Assert.That(blocked, Is.EqualTo(result));
        }

        [Test]
        [TestCase(-2)]
        [TestCase(8)]
        public void IsDirBlockedException(Dir8 dir)
        {
            // nothing is blocked
            bool CheckBlock(Loc testLoc) => false;
            bool CheckDiag(Loc testLoc) => false;

            Assert.Throws<ArgumentException>(() => { Grid.IsDirBlocked(Loc.Zero, dir, CheckBlock, CheckDiag, 0); });
        }
    }
}
