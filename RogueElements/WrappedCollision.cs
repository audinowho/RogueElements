// <copyright file="WrappedCollision.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements
{
    public static class WrappedCollision
    {
        public static bool Collides(Loc wrapSize, Rect bound1, Rect bound2)
        {
            return Collides(wrapSize, bound1.Start, bound1.Size, bound2.Start, bound2.Size);
        }

        public static bool Collides(Loc wrapSize, Loc start1, Loc size1, Loc start2, Loc size2)
        {
            return Collides(wrapSize.X, start1.X, size1.X, start2.X, size2.X) &&
                Collides(wrapSize.Y, start1.Y, size1.Y, start2.Y, size2.Y);
        }

        /// <summary>
        /// Checks if two bounds collide, within a wrapped area.
        /// </summary>
        /// <param name="wrapSize">Size of wrapped area</param>
        /// <param name="start1">Unwrapped start of bounds 1</param>
        /// <param name="size1">Size of bounds 1</param>
        /// <param name="start2">Unwrapped start of bounds 2</param>
        /// <param name="size2">Size of bounds 2</param>
        /// <returns></returns>
        public static bool Collides(int wrapSize, int start1, int size1, int start2, int size2)
        {
            start1 = MathUtils.Wrap(start1, wrapSize);
            start2 = MathUtils.Wrap(start2, wrapSize);

            if (Collision.Collides(start1, size1, start2, size2))
                return true;
            else if (start1 < start2)
                return Collision.Collides(start1 + wrapSize, size1, start2, size2);
            else
                return Collision.Collides(start1, size1, start2 + wrapSize, size2);
        }

        public static bool InBounds(Loc wrapSize, Rect rect, Loc point)
        {
            return InBounds(wrapSize.X, rect.Start.X, rect.Size.X, point.X) && InBounds(wrapSize.Y, rect.Start.Y, rect.Size.Y, point.Y);
        }

        public static bool InBounds(Loc wrapSize, Loc start, Loc size, Loc point)
        {
            return InBounds(wrapSize.X, start.X, size.X, point.X) && InBounds(wrapSize.Y, start.Y, size.Y, point.Y);
        }

        /// <summary>
        /// Checks if a point is in bounds, within a wrapped area.
        /// </summary>
        /// <param name="wrapSize">Size of wrapped area</param>
        /// <param name="start">Unwrapped start of bounds</param>
        /// <param name="size">Size of bounds</param>
        /// <param name="pt">Unwrapped position</param>
        /// <returns></returns>
        public static bool InBounds(int wrapSize, int start, int size, int pt)
        {
            start = MathUtils.Wrap(start, wrapSize);
            pt = MathUtils.Wrap(pt, wrapSize);

            return Collision.InBounds(size, pt - start) || Collision.InBounds(size, pt + wrapSize - start);
        }

        /// <summary>
        /// Gets the unwrapped version of pt2 that is closest to pt1.
        /// </summary>
        /// <param name="wrapSize"></param>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public static Loc GetClosestWrap(Loc wrapSize, Loc pt1, Loc pt2)
        {
            return new Loc(GetClosestWrap(wrapSize.X, pt1.X, pt2.X), GetClosestWrap(wrapSize.Y, pt1.Y, pt2.Y));
        }

        /// <summary>
        /// Gets the unwrapped version of pt2 that is closest to pt1.
        /// </summary>
        /// <param name="wrapSize"></param>
        /// <param name="pt1">The point to get close to.  Unwrapped.</param>
        /// <param name="pt2">The point to get a close version of.  Unwrapped.</param>
        /// <returns></returns>
        public static int GetClosestWrap(int wrapSize, int pt1, int pt2)
        {
            int wrapPt1 = MathUtils.Wrap(pt1, wrapSize);
            int wrapPt2 = MathUtils.Wrap(pt2, wrapSize);

            int diff1 = wrapPt2 - wrapPt1;
            if (wrapPt1 < wrapPt2)
            {
                int diff2 = (wrapPt2 - wrapSize) - wrapPt1;
                if (Math.Abs(diff2) < Math.Abs(diff1))
                    return pt1 + diff2;
            }
            else if (wrapPt1 > wrapPt2)
            {
                int diff2 = (wrapPt2 + wrapSize) - wrapPt1;
                if (Math.Abs(diff2) < Math.Abs(diff1))
                    return pt1 + diff2;
            }

            return pt1 + diff1;
        }

        public static IEnumerable<Loc> IteratePointsInBounds(Loc wrapSize, Rect rect, Loc pt)
        {
            foreach (int xx in IteratePointsInBounds(wrapSize.X, rect.X, rect.Size.X, pt.X))
            {
                foreach (int yy in IteratePointsInBounds(wrapSize.Y, rect.Y, rect.Size.Y, pt.Y))
                    yield return new Loc(xx, yy);
            }
        }

        /// <summary>
        /// Returns all unwrapped versions of a point that exist within a region.
        /// </summary>
        /// <param name="wrapSize">Size of the wrapped area</param>
        /// <param name="start">Unwrapped start of the region</param>
        /// <param name="size">Size of the region</param>
        /// <param name="pt">The point to find unwrapped versions.</param>
        /// <returns></returns>
        public static IEnumerable<int> IteratePointsInBounds(int wrapSize, int start, int size, int pt)
        {
            // take the start of the region, round down to the lowest whole map.  this is the earliest map to check
            // take the end of the region, round up to the highest whole map.  this is the bottom-most (exclusive) map to check
            int startBounds = MathUtils.DivDown(start, wrapSize);
            int endBounds = MathUtils.DivUp(start + size, wrapSize);
            int wrapPt = MathUtils.Wrap(pt, wrapSize);

            for (int xx = startBounds; xx < endBounds; xx++)
            {
                int mapStart = xx * wrapSize;
                int testPt = mapStart + wrapPt;
                if (Collision.InBounds(start, size, testPt))
                    yield return testPt;
            }
        }

        /// <summary>
        /// Returns all unwrapped versions of a rect that collides with another rect.
        /// </summary>
        /// <param name="wrapSize"></param>
        /// <param name="rect1">The unwrapped reference region</param>
        /// <param name="rect2">The region to find unwrapped versions of</param>
        /// <returns></returns>
        public static IEnumerable<Rect> IterateRegionsColliding(Loc wrapSize, Rect rect1, Rect rect2)
        {
            foreach (IntRange xx in IterateRegionsColliding(wrapSize.X, rect1.X, rect1.Size.X, rect2.X, rect2.Size.X))
            {
                foreach (IntRange yy in IterateRegionsColliding(wrapSize.Y, rect1.Y, rect1.Size.Y, rect2.Y, rect2.Size.Y))
                    yield return new Rect(new Loc(xx.Min, yy.Min), new Loc(xx.Length, yy.Length));
            }
        }

        /// <summary>
        /// Returns all unwrapped versions of a region that collides with another region.
        /// </summary>
        /// <param name="wrapSize">Size of the wrapped area</param>
        /// <param name="start1">The unwrapped start of the reference region</param>
        /// <param name="size1">The size of the reference region</param>
        /// <param name="start2">The start of the region to find the unwrapped versions of</param>
        /// <param name="size2">The size of the region to find the unwrapped versions of</param>
        /// <returns></returns>
        public static IEnumerable<IntRange> IterateRegionsColliding(int wrapSize, int start1, int size1, int start2, int size2)
        {
            // take the start of the region, round down to the lowest whole map.  this is the earliest map to check
            // take the end of the region, round up to the highest whole map.  this is the bottom-most (exclusive) map to check
            int startBounds = MathUtils.DivDown(start1 - size2, wrapSize);
            int endBounds = MathUtils.DivUp(start1 + size1, wrapSize);
            int wrapPt = MathUtils.Wrap(start2, wrapSize);

            for (int xx = startBounds; xx < endBounds; xx++)
            {
                int mapStart = xx * wrapSize;
                int testStart = mapStart + wrapPt;
                if (Collision.Collides(testStart, size2, start1, size1))
                    yield return new IntRange(testStart, testStart + size2);
            }
        }
    }
}
