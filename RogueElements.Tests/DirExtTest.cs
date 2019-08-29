// <copyright file="DirExtTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using NUnit.Framework;
using RogueElements;

namespace RogueElements.Tests
{
    [TestFixture]
    public class DirExtTest
    {
        /// <summary>
        /// Not going to cover all of the functions, since many are just switch statements.  Cover them if they ever get changed to or from something else.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="expected"></param>
        /// <param name="exception"></param>
        [Test]
        [TestCase(DirH.None, Dir4.None)]
        [TestCase(DirH.Left, Dir4.Left)]
        [TestCase(DirH.Right, Dir4.Right)]
        [TestCase((DirH)(-2), 0, true)]
        [TestCase((DirH)2, 0, true)]
        public void ToDir4(DirH dir, Dir4 expected, bool exception = false)
        {
            if (exception)
                Assert.Throws<ArgumentOutOfRangeException>(() => { dir.ToDir4(); });
            else
                Assert.That(dir.ToDir4(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(DirV.None, Dir4.None)]
        [TestCase(DirV.Down, Dir4.Down)]
        [TestCase(DirV.Up, Dir4.Up)]
        [TestCase((DirV)(-2), 0, true)]
        [TestCase((DirV)2, 0, true)]
        public void ToDir4(DirV dir, Dir4 expected, bool exception = false)
        {
            if (exception)
                Assert.Throws<ArgumentOutOfRangeException>(() => { dir.ToDir8(); });
            else
                Assert.That(dir.ToDir4(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(DirH.None, Dir8.None)]
        [TestCase(DirH.Left, Dir8.Left)]
        [TestCase(DirH.Right, Dir8.Right)]
        [TestCase((DirH)(-2), 0, true)]
        [TestCase((DirH)2, 0, true)]
        public void ToDir8(DirH dir, Dir8 expected, bool exception = false)
        {
            if (exception)
                Assert.Throws<ArgumentOutOfRangeException>(() => { dir.ToDir8(); });
            else
                Assert.That(dir.ToDir8(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(DirV.None, Dir8.None)]
        [TestCase(DirV.Down, Dir8.Down)]
        [TestCase(DirV.Up, Dir8.Up)]
        [TestCase((DirV)(-2), 0, true)]
        [TestCase((DirV)2, 0, true)]
        public void ToDir8(DirV dir, Dir8 expected, bool exception = false)
        {
            if (exception)
                Assert.Throws<ArgumentOutOfRangeException>(() => { dir.ToDir8(); });
            else
                Assert.That(dir.ToDir8(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(Dir4.None, Dir8.None)]
        [TestCase(Dir4.Left, Dir8.Left)]
        [TestCase(Dir4.Right, Dir8.Right)]
        [TestCase(Dir4.Down, Dir8.Down)]
        [TestCase(Dir4.Up, Dir8.Up)]
        [TestCase((Dir4)(-2), 0, true)]
        [TestCase((Dir4)4, 0, true)]
        public void ToDir8(Dir4 dir, Dir8 expected, bool exception = false)
        {
            if (exception)
                Assert.Throws<ArgumentOutOfRangeException>(() => { dir.ToDir8(); });
            else
                Assert.That(dir.ToDir8(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(Dir8.None, Dir4.None)]
        [TestCase(Dir8.Down, Dir4.Down)]
        [TestCase(Dir8.Left, Dir4.Left)]
        [TestCase(Dir8.Up, Dir4.Up)]
        [TestCase(Dir8.Right, Dir4.Right)]
        [TestCase(Dir8.DownLeft, 0, true)]
        [TestCase(Dir8.UpLeft, 0, true)]
        [TestCase(Dir8.UpRight, 0, true)]
        [TestCase(Dir8.DownRight, 0, true)]
        [TestCase((Dir8)(-2), 0, true)]
        [TestCase((Dir8)8, 0, true)]
        public void ToDir4(Dir8 dir, Dir4 expected, bool exception = false)
        {
            if (exception)
                Assert.Throws<ArgumentOutOfRangeException>(() => { dir.ToDir4(); });
            else
                Assert.That(dir.ToDir4(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(Axis4.None, Axis8.None)]
        [TestCase(Axis4.Horiz, Axis8.Horiz)]
        [TestCase(Axis4.Vert, Axis8.Vert)]
        [TestCase((Axis4)(-2), 0, true)]
        [TestCase((Axis4)2, 0, true)]
        public void ToAxis8(Axis4 axis, Axis8 expected, bool exception = false)
        {
            if (exception)
                Assert.Throws<ArgumentOutOfRangeException>(() => { axis.ToAxis8(); });
            else
                Assert.That(axis.ToAxis8(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(DirH.None, DirH.None)]
        [TestCase(DirH.Left, DirH.Right)]
        [TestCase((DirH)(-2), 0, true)]
        [TestCase((DirH)2, 0, true)]
        public void Reverse(DirH dir, DirH expected, bool exception = false)
        {
            if (exception)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { dir.Reverse(); });
            }
            else
            {
                Assert.That(dir.Reverse(), Is.EqualTo(expected));
                Assert.That(expected.Reverse(), Is.EqualTo(dir));
            }
        }

        [Test]
        [TestCase(DirV.None, DirV.None)]
        [TestCase(DirV.Up, DirV.Down)]
        [TestCase((DirV)(-2), 0, true)]
        [TestCase((DirV)2, 0, true)]
        public void Reverse(DirV dir, DirV expected, bool exception = false)
        {
            if (exception)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { dir.Reverse(); });
            }
            else
            {
                Assert.That(dir.Reverse(), Is.EqualTo(expected));
                Assert.That(expected.Reverse(), Is.EqualTo(dir));
            }
        }

        [Test]
        [TestCase(Dir4.None, Dir4.None)]
        [TestCase(Dir4.Up, Dir4.Down)]
        [TestCase(Dir4.Left, Dir4.Right)]
        [TestCase((Dir4)(-2), 0, true)]
        [TestCase((Dir4)4, 0, true)]
        public void Reverse(Dir4 dir, Dir4 expected, bool exception = false)
        {
            if (exception)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { dir.Reverse(); });
            }
            else
            {
                Assert.That(dir.Reverse(), Is.EqualTo(expected));
                Assert.That(expected.Reverse(), Is.EqualTo(dir));
            }
        }

        [Test]
        [TestCase(Dir8.None, Dir8.None)]
        [TestCase(Dir8.Up, Dir8.Down)]
        [TestCase(Dir8.Left, Dir8.Right)]
        [TestCase(Dir8.UpLeft, Dir8.DownRight)]
        [TestCase(Dir8.DownLeft, Dir8.UpRight)]
        [TestCase((Dir8)(-2), 0, true)]
        [TestCase((Dir8)8, 0, true)]
        public void Reverse(Dir8 dir, Dir8 expected, bool exception = false)
        {
            if (exception)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { dir.Reverse(); });
            }
            else
            {
                Assert.That(dir.Reverse(), Is.EqualTo(expected));
                Assert.That(expected.Reverse(), Is.EqualTo(dir));
            }
        }

        [Test]
        [TestCase(DirH.None, DirV.None, Dir8.None)]
        [TestCase(DirH.None, DirV.Down, Dir8.Down)]
        [TestCase(DirH.Left, DirV.Down, Dir8.DownLeft)]
        [TestCase(DirH.Left, DirV.None, Dir8.Left)]
        [TestCase(DirH.Left, DirV.Up, Dir8.UpLeft)]
        [TestCase(DirH.None, DirV.Up, Dir8.Up)]
        [TestCase(DirH.Right, DirV.Up, Dir8.UpRight)]
        [TestCase(DirH.Right, DirV.None, Dir8.Right)]
        [TestCase(DirH.Right, DirV.Down, Dir8.DownRight)]
        public void CombineAndBreak(DirH horiz, DirV vert, Dir8 dir)
        {
            Assert.That(DirExt.Combine(horiz, vert), Is.EqualTo(dir));
            dir.Separate(out DirH resH, out DirV resV);
            Assert.AreEqual(resH, horiz);
            Assert.AreEqual(resV, vert);
        }

        [Test]
        [TestCase(DirH.None, (DirV)(-2))]
        [TestCase(DirH.None, (DirV)2)]
        [TestCase((DirV)(-2), DirV.None)]
        [TestCase((DirV)2, DirV.None)]
        public void CombineEx(DirH horiz, DirV vert)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => { DirExt.Combine(horiz, vert); });
        }

        [Test]
        [TestCase((Dir8)(-2))]
        [TestCase((Dir8)8)]
        public void SeparateEx(Dir8 dir)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => { dir.Separate(out DirH resH, out DirV resV); });
        }

        [Test]
        [TestCase(0, 0, Dir8.None)]// base cases
        [TestCase(0, 1, Dir8.Down)]
        [TestCase(-1, 1, Dir8.DownLeft)]
        [TestCase(-1, 0, Dir8.Left)]
        [TestCase(-1, -1, Dir8.UpLeft)]
        [TestCase(0, -1, Dir8.Up)]
        [TestCase(1, -1, Dir8.UpRight)]
        [TestCase(1, 0, Dir8.Right)]
        [TestCase(1, 1, Dir8.DownRight)]
        [TestCase(0, 100, Dir8.Down)]// higher distance, edge cases
        [TestCase(-100, 1, Dir8.DownLeft)]
        [TestCase(-1, 100, Dir8.DownLeft)]
        [TestCase(-100, 0, Dir8.Left)]
        [TestCase(-10, -1, Dir8.UpLeft)]
        [TestCase(-1, -100, Dir8.UpLeft)]
        [TestCase(0, -10, Dir8.Up)]
        [TestCase(100, -1, Dir8.UpRight)]
        [TestCase(1, -100, Dir8.UpRight)]
        [TestCase(100, 0, Dir8.Right)]
        [TestCase(100, 1, Dir8.DownRight)]
        [TestCase(1, 100, Dir8.DownRight)]
        public void GetDir(int locX, int locY, Dir8 expected)
        {
            Assert.That(new Loc(locX, locY).GetDir(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(1, 1, 0, 0, Dir8.None)]// base cases
        [TestCase(1, 1, 0, 1, Dir8.Down)]
        [TestCase(1, 1, -1, 1, Dir8.DownLeft)]
        [TestCase(1, 1, -1, 0, Dir8.Left)]
        [TestCase(1, 1, -1, -1, Dir8.UpLeft)]
        [TestCase(1, 1, 0, -1, Dir8.Up)]
        [TestCase(1, 1, 1, -1, Dir8.UpRight)]
        [TestCase(1, 1, 1, 0, Dir8.Right)]
        [TestCase(1, 1, 1, 1, Dir8.DownRight)]
        [TestCase(10, 10, 4, 4, Dir8.None)]// edge cases
        [TestCase(10, 10, 0, 0, Dir8.None)]
        [TestCase(10, 10, 0, 9, Dir8.None)]
        [TestCase(10, 10, 9, 0, Dir8.None)]
        [TestCase(10, 10, 9, 9, Dir8.None)]
        [TestCase(10, 10, 0, 10, Dir8.Down)]
        [TestCase(10, 10, 9, 10, Dir8.Down)]
        [TestCase(10, 10, -1, 10, Dir8.DownLeft)]
        [TestCase(10, 10, -1, 9, Dir8.Left)]
        [TestCase(10, 10, -1, 0, Dir8.Left)]
        [TestCase(10, 10, -1, -1, Dir8.UpLeft)]
        [TestCase(10, 10, 0, -1, Dir8.Up)]
        [TestCase(10, 10, 9, -1, Dir8.Up)]
        [TestCase(10, 10, 10, -1, Dir8.UpRight)]
        [TestCase(10, 10, 10, 0, Dir8.Right)]
        [TestCase(10, 10, 10, 9, Dir8.Right)]
        [TestCase(10, 10, 10, 10, Dir8.DownRight)]
        [TestCase(10, 10, 0, 100, Dir8.Down)]// higher distance
        [TestCase(10, 10, 9, 100, Dir8.Down)]
        [TestCase(10, 10, -1, 100, Dir8.DownLeft)]
        [TestCase(10, 10, -100, 10, Dir8.DownLeft)]
        [TestCase(10, 10, -100, 9, Dir8.Left)]
        [TestCase(10, 10, -100, 0, Dir8.Left)]
        [TestCase(10, 10, -1, -100, Dir8.UpLeft)]
        [TestCase(10, 10, -100, -1, Dir8.UpLeft)]
        [TestCase(10, 10, 0, -100, Dir8.Up)]
        [TestCase(10, 10, 9, -100, Dir8.Up)]
        [TestCase(10, 10, 10, -100, Dir8.UpRight)]
        [TestCase(10, 10, 100, -1, Dir8.UpRight)]
        [TestCase(10, 10, 10, 0, Dir8.Right)]
        [TestCase(10, 10, 10, 9, Dir8.Right)]
        [TestCase(10, 10, 100, 10, Dir8.DownRight)]
        [TestCase(10, 10, 10, 100, Dir8.DownRight)]
        [TestCase(0, 0, 0, 0, Dir8.None, true)]// exceptions
        [TestCase(1, 0, 0, 0, Dir8.None, true)]
        [TestCase(0, 1, 0, 0, Dir8.None, true)]
        [TestCase(-1, -1, 0, 0, Dir8.None, true)]
        public void GetBoundsDir(int sizeX, int sizeY, int pointX, int pointY, Dir8 expected, bool exception = false)
        {
            if (exception)
                Assert.Throws<ArgumentException>(() => { DirExt.GetBoundsDir(new Loc(sizeX, sizeY), new Loc(pointX, pointY)); });
            else
                Assert.That(DirExt.GetBoundsDir(new Loc(sizeX, sizeY), new Loc(pointX, pointY)), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(0, 0, Dir4.None)]// base cases
        [TestCase(0, 1, Dir4.Down)]
        [TestCase(-1, 0, Dir4.Left)]
        [TestCase(0, -1, Dir4.Up)]
        [TestCase(1, 0, Dir4.Right)]
        [TestCase(1, 1, Dir4.Down)]
        [TestCase(-1, 1, Dir4.Down)]
        [TestCase(-1, -1, Dir4.Up)]
        [TestCase(1, -1, Dir4.Up)]
        [TestCase(0, 100, Dir4.Down)]// higher distance
        [TestCase(-100, 0, Dir4.Left)]
        [TestCase(0, -100, Dir4.Up)]
        [TestCase(100, 0, Dir4.Right)]
        [TestCase(100, 100, Dir4.Down)]// border cases
        [TestCase(-100, 100, Dir4.Down)]
        [TestCase(-100, -100, Dir4.Up)]
        [TestCase(100, -100, Dir4.Up)]
        [TestCase(100, 99, Dir4.Right)]
        [TestCase(-100, 99, Dir4.Left)]
        [TestCase(-100, -99, Dir4.Left)]
        [TestCase(100, -99, Dir4.Right)]
        public void ApproximateDir4(int locX, int locY, Dir4 expected)
        {
            Assert.That(new Loc(locX, locY).ApproximateDir4(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(0, 0, Dir8.None)]// base cases
        [TestCase(0, 1, Dir8.Down)]
        [TestCase(-1, 0, Dir8.Left)]
        [TestCase(0, -1, Dir8.Up)]
        [TestCase(1, 0, Dir8.Right)]
        [TestCase(1, 1, Dir8.DownRight)]
        [TestCase(-1, 1, Dir8.DownLeft)]
        [TestCase(-1, -1, Dir8.UpLeft)]
        [TestCase(1, -1, Dir8.UpRight)]
        [TestCase(0, 100, Dir8.Down)]// higher distance
        [TestCase(-100, 0, Dir8.Left)]
        [TestCase(0, -100, Dir8.Up)]
        [TestCase(100, 0, Dir8.Right)]
        [TestCase(100, 100, Dir8.DownRight)]
        [TestCase(-100, 100, Dir8.DownLeft)]
        [TestCase(-100, -100, Dir8.UpLeft)]
        [TestCase(100, -100, Dir8.UpRight)]
        [TestCase(41, 100, Dir8.Down)]// border cases
        [TestCase(-41, 100, Dir8.Down)]
        [TestCase(42, 100, Dir8.DownRight)]
        [TestCase(-42, 100, Dir8.DownLeft)]
        [TestCase(-41, -100, Dir8.Up)]
        [TestCase(41, -100, Dir8.Up)]
        [TestCase(-42, -100, Dir8.UpLeft)]
        [TestCase(42, -100, Dir8.UpRight)]
        [TestCase(100, 41, Dir8.Right)]
        [TestCase(100, -41, Dir8.Right)]
        [TestCase(100, 42, Dir8.DownRight)]
        [TestCase(100, -42, Dir8.UpRight)]
        [TestCase(-100, 41, Dir8.Left)]
        [TestCase(-100, -41, Dir8.Left)]
        [TestCase(-100, 42, Dir8.DownLeft)]
        [TestCase(-100, -42, Dir8.UpLeft)]
        public void ApproximateDir8(int locX, int locY, Dir8 expected)
        {
            Assert.That(new Loc(locX, locY).ApproximateDir8(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(Dir4.Down, 0, Dir4.Down)]
        [TestCase(Dir4.Left, 0, Dir4.Left)]
        [TestCase(Dir4.Left, -4, Dir4.Left)]
        [TestCase(Dir4.Up, 4, Dir4.Up)]
        [TestCase(Dir4.Down, 1, Dir4.Left)]
        [TestCase(Dir4.Right, 1, Dir4.Down)]
        [TestCase(Dir4.Left, 3, Dir4.Down)]
        [TestCase(Dir4.Up, 10, Dir4.Down)]
        [TestCase(Dir4.Right, -1, Dir4.Up)]
        [TestCase(Dir4.Left, -7, Dir4.Up)]
        [TestCase(Dir4.None, 1, Dir4.None)]
        [TestCase(Dir4.None, -1, Dir4.None)]
        [TestCase((Dir4)(-2), 0, Dir4.None, true)]
        [TestCase((Dir4)4, 0, Dir4.None, true)]
        public void Rotate(Dir4 dir, int n, Dir4 expected, bool exception = false)
        {
            if (exception)
                Assert.Throws<ArgumentOutOfRangeException>(() => { dir.Rotate(n); });
            else
                Assert.That(dir.Rotate(n), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(Dir8.Down, 0, Dir8.Down)]
        [TestCase(Dir8.Left, 0, Dir8.Left)]
        [TestCase(Dir8.Left, -8, Dir8.Left)]
        [TestCase(Dir8.Up, 8, Dir8.Up)]
        [TestCase(Dir8.Down, 1, Dir8.DownLeft)]
        [TestCase(Dir8.Right, 1, Dir8.DownRight)]
        [TestCase(Dir8.Left, 3, Dir8.UpRight)]
        [TestCase(Dir8.Up, 10, Dir8.Right)]
        [TestCase(Dir8.Right, -1, Dir8.UpRight)]
        [TestCase(Dir8.Left, -13, Dir8.UpRight)]
        [TestCase(Dir8.None, 1, Dir8.None)]
        [TestCase(Dir8.None, -1, Dir8.None)]
        [TestCase((Dir8)(-2), 0, Dir8.None, true)]
        [TestCase((Dir8)8, 0, Dir8.None, true)]
        public void Rotate(Dir8 dir, int n, Dir8 expected, bool exception = false)
        {
            if (exception)
                Assert.Throws<ArgumentOutOfRangeException>(() => { dir.Rotate(n); });
            else
                Assert.That(dir.Rotate(n), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(Dir4.Down, Dir4.Down, Dir4.Down)]
        [TestCase(Dir4.None, Dir4.Down, Dir4.None)]
        [TestCase(Dir4.Down, Dir4.None, Dir4.None)]
        [TestCase(Dir4.Down, Dir4.Left, Dir4.Left)]
        [TestCase(Dir4.Left, Dir4.Down, Dir4.Left)]
        [TestCase(Dir4.Down, Dir4.Right, Dir4.Right)]
        [TestCase(Dir4.Down, Dir4.Up, Dir4.Up)]
        [TestCase(Dir4.Left, Dir4.Left, Dir4.Up)]
        [TestCase(Dir4.Up, Dir4.Up, Dir4.Down)]
        [TestCase(Dir4.Up, Dir4.Right, Dir4.Left)]
        [TestCase((Dir4)(-2), Dir4.Down, Dir4.None, true)]
        [TestCase(Dir4.Down, (Dir4)(-2), Dir4.None, true)]
        [TestCase((Dir4)4, Dir4.Down, Dir4.None, true)]
        [TestCase(Dir4.Down, (Dir4)4, Dir4.None, true)]
        public void AddAngles(Dir4 dir1, Dir4 dir2, Dir4 expected, bool exception = false)
        {
            if (exception)
                Assert.Throws<ArgumentOutOfRangeException>(() => { DirExt.AddAngles(dir1, dir2); });
            else
                Assert.That(DirExt.AddAngles(dir1, dir2), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(Dir8.Down, Dir8.Down, Dir8.Down)]
        [TestCase(Dir8.None, Dir8.Down, Dir8.None)]
        [TestCase(Dir8.Down, Dir8.None, Dir8.None)]
        [TestCase(Dir8.Down, Dir8.Left, Dir8.Left)]
        [TestCase(Dir8.Left, Dir8.Down, Dir8.Left)]
        [TestCase(Dir8.Down, Dir8.DownLeft, Dir8.DownLeft)]
        [TestCase(Dir8.DownLeft, Dir8.Down, Dir8.DownLeft)]
        [TestCase(Dir8.Down, Dir8.Right, Dir8.Right)]
        [TestCase(Dir8.Down, Dir8.DownRight, Dir8.DownRight)]
        [TestCase(Dir8.Down, Dir8.Up, Dir8.Up)]
        [TestCase(Dir8.Left, Dir8.Left, Dir8.Up)]
        [TestCase(Dir8.Left, Dir8.DownLeft, Dir8.UpLeft)]
        [TestCase(Dir8.Up, Dir8.Up, Dir8.Down)]
        [TestCase(Dir8.Up, Dir8.Right, Dir8.Left)]
        [TestCase(Dir8.Up, Dir8.DownRight, Dir8.UpLeft)]
        [TestCase((Dir8)(-2), Dir8.Down, Dir8.None, true)]
        [TestCase(Dir8.Down, (Dir8)(-2), Dir8.None, true)]
        [TestCase((Dir8)8, Dir8.Down, Dir8.None, true)]
        [TestCase(Dir8.Down, (Dir8)8, Dir8.None, true)]
        public void AddAngles(Dir8 dir1, Dir8 dir2, Dir8 expected, bool exception = false)
        {
            if (exception)
                Assert.Throws<ArgumentOutOfRangeException>(() => { DirExt.AddAngles(dir1, dir2); });
            else
                Assert.That(DirExt.AddAngles(dir1, dir2), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(Axis4.Horiz, 0, 0, 0)]
        [TestCase(Axis4.Vert, 0, 0, 0)]
        [TestCase(Axis4.Horiz, 1, 1, 0)]
        [TestCase(Axis4.Vert, 1, 0, 1)]
        [TestCase(Axis4.Horiz, -1, -1, 0)]
        [TestCase(Axis4.Vert, -1, 0, -1)]
        [TestCase(Axis4.None, 0, 0, 0, true)]
        [TestCase((Axis4)(-2), 0, 0, 0, true)]
        [TestCase((Axis4)2, 0, 0, 0, true)]
        public void FromAxisScalar(Axis4 axis, int scalar, int expectedX, int expectedY, bool exception = false)
        {
            if (exception)
            {
                if (axis == Axis4.None)
                    Assert.Throws<ArgumentException>(() => { axis.CreateLoc(scalar, 0); });
                else
                    Assert.Throws<ArgumentOutOfRangeException>(() => { axis.CreateLoc(scalar, 0); });
            }
            else
            {
                Assert.That(axis.CreateLoc(scalar, 0), Is.EqualTo(new Loc(expectedX, expectedY)));
            }
        }

        [Test]
        [TestCase(Axis4.Horiz, 1, Dir4.Right)]
        [TestCase(Axis4.Horiz, -1, Dir4.Left)]
        [TestCase(Axis4.Vert, 1, Dir4.Down)]
        [TestCase(Axis4.Vert, -1, Dir4.Up)]
        [TestCase(Axis4.Horiz, int.MaxValue, Dir4.Right)]
        [TestCase(Axis4.Vert, int.MinValue, Dir4.Up)]
        [TestCase(Axis4.None, 0, Dir4.None)]
        [TestCase(Axis4.None, 1, Dir4.None)]
        [TestCase(Axis4.None, -1, Dir4.None)]
        [TestCase(Axis4.Horiz, 0, Dir4.None)]
        [TestCase(Axis4.Vert, 0, Dir4.None)]
        [TestCase((Axis4)(-2), 0, Dir4.None, true)]
        [TestCase((Axis4)2, 0, Dir4.None, true)]
        public void DirFromAxis(Axis4 axis, int scalar, Dir4 expected, bool exception = false)
        {
            if (exception)
                Assert.Throws<ArgumentOutOfRangeException>(() => { axis.GetDir(scalar); });
            else
                Assert.That(axis.GetDir(scalar), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(Axis8.Horiz, 1, Dir8.Right)]
        [TestCase(Axis8.Horiz, -1, Dir8.Left)]
        [TestCase(Axis8.Vert, 1, Dir8.Down)]
        [TestCase(Axis8.Vert, -1, Dir8.Up)]
        [TestCase(Axis8.DiagBack, 1, Dir8.DownRight)]
        [TestCase(Axis8.DiagBack, -1, Dir8.UpLeft)]
        [TestCase(Axis8.DiagForth, 1, Dir8.UpRight)]
        [TestCase(Axis8.DiagForth, -1, Dir8.DownLeft)]
        [TestCase(Axis8.Horiz, int.MaxValue, Dir8.Right)]
        [TestCase(Axis8.Vert, int.MinValue, Dir8.Up)]
        [TestCase(Axis8.None, 0, Dir8.None)]
        [TestCase(Axis8.None, 1, Dir8.None)]
        [TestCase(Axis8.None, -1, Dir8.None)]
        [TestCase(Axis8.Horiz, 0, Dir8.None)]
        [TestCase(Axis8.Vert, 0, Dir8.None)]
        [TestCase(Axis8.DiagBack, 0, Dir8.None)]
        [TestCase(Axis8.DiagForth, 0, Dir8.None)]
        [TestCase((Axis8)(-2), 0, Dir8.None, true)]
        [TestCase((Axis8)4, 0, Dir8.None, true)]
        public void DirFromAxis(Axis8 axis, int scalar, Dir8 expected, bool exception = false)
        {
            if (exception)
                Assert.Throws<ArgumentOutOfRangeException>(() => { axis.GetDir(scalar); });
            else
                Assert.That(axis.GetDir(scalar), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(0, 0, Axis4.Horiz, 0)]
        [TestCase(0, 0, Axis4.Vert, 0)]
        [TestCase(1, 0, Axis4.Horiz, 1)]
        [TestCase(0, 1, Axis4.Vert, 1)]
        [TestCase(-1, 0, Axis4.Horiz, -1)]
        [TestCase(0, -1, Axis4.Vert, -1)]
        [TestCase(0, 0, Axis4.None, 0, true)]
        [TestCase(0, 0, (Axis4)(-2), 0, true)]
        [TestCase(0, 0, (Axis4)2, 0, true)]
        public void ScalarFromAxis(int locX, int locY, Axis4 axis, int expected, bool exception = false)
        {
            if (exception)
            {
                if (axis == Axis4.None)
                    Assert.Throws<ArgumentException>(() => { new Loc(locX, locY).GetScalar(axis); });
                else
                    Assert.Throws<ArgumentOutOfRangeException>(() => { new Loc(locX, locY).GetScalar(axis); });
            }
            else
            {
                Assert.That(new Loc(locX, locY).GetScalar(axis), Is.EqualTo(expected));
            }
        }
    }
}
