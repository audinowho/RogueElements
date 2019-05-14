// <copyright file="Collision.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements
{
    public static class Collision
    {
        public static bool InFront(Loc startLoc, Loc testLoc, Dir8 dir, int range)
        {
            return InFront(testLoc - startLoc, dir, range);
        }

        public static bool InFront(Loc testLoc, Dir8 dir, int range)
        {
            if (dir < Dir8.None || (int)dir >= DirExt.DIR8_COUNT)
                throw new ArgumentException("Invalid value to convert.");
            if (testLoc == Loc.Zero)
                return true;
            int foundRange = testLoc.Dist8();
            if (range >= 0 && foundRange > range)
                return false;
            return dir.GetLoc() * foundRange == testLoc;
        }

        public static bool Collides(Rect bound1, Rect bound2)
        {
            return Collides(bound1.Start, bound1.Size, bound2.Start, bound2.Size);
        }

        public static bool Collides(Loc start1, Loc size1, Loc start2, Loc size2)
        {
            return Collides(start1.X, size1.X, start2.X, size2.X) &&
                Collides(start1.Y, size1.Y, start2.Y, size2.Y);
        }

        public static bool Collides(int start1, int size1, int start2, int size2)
        {
            return start1 + size1 > start2 && start2 + size2 > start1;
        }

        public static bool InBounds(Rect rect, Loc point)
        {
            return InBounds(rect.Size.X, rect.Size.Y, point - rect.Start);
        }

        public static bool InBounds(Loc start, Loc size, Loc point)
        {
            return InBounds(size.X, size.Y, point - start);
        }

        public static bool InBounds(int sizeX, int sizeY, Loc pt)
        {
            return InBounds(sizeX, pt.X) && InBounds(sizeY, pt.Y);
        }

        public static bool InBounds(int start, int size, int pt)
        {
            return InBounds(size, pt - start);
        }

        public static bool InBounds(int size, int pt)
        {
            return pt >= 0 && pt < size;
        }
    }
}
