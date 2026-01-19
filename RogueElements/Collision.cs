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
    /// <summary>
    /// Provides methods for collision detection between rectangles and bounds checking.
    /// </summary>
    public static class Collision
    {
        /// <summary>
        /// Given a first entity and its facing direction, determines if the second entity is in front of the first entity.
        /// </summary>
        /// <param name="startLoc">First entity location</param>
        /// <param name="testLoc">Second entity location</param>
        /// <param name="dir">First entity direction</param>
        /// <param name="range">-1 for infinite range.</param>
        /// <returns></returns>
        public static bool InFront(Loc startLoc, Loc testLoc, Dir8 dir, int range)
        {
            return InFront(testLoc - startLoc, dir, range);
        }

        /// <summary>
        /// Given a first entity at 0,0 and its facing direction, determines if the second entity is in front of the first entity.
        /// </summary>
        /// <param name="testLoc">Second entity location</param>
        /// <param name="dir">First entity direction</param>
        /// <param name="range">-1 for infinite range.</param>
        /// <returns></returns>
        public static bool InFront(Loc testLoc, Dir8 dir, int range)
        {
            if (!dir.Validate())
                throw new ArgumentException("Invalid value to convert.");
            if (testLoc == Loc.Zero)
                return true;
            int foundRange = testLoc.Dist8();
            if (range >= 0 && foundRange > range)
                return false;
            return dir.GetLoc() * foundRange == testLoc;
        }

        /// <summary>
        /// Determines if two rectangles overlap.
        /// </summary>
        /// <param name="bound1">The first rectangle.</param>
        /// <param name="bound2">The second rectangle.</param>
        /// <returns><c>true</c> if the rectangles overlap; otherwise <c>false</c>.</returns>
        public static bool Collides(Rect bound1, Rect bound2)
        {
            return Collides(bound1.Start, bound1.Size, bound2.Start, bound2.Size);
        }

        /// <summary>
        /// Determines if two rectangular regions overlap.
        /// </summary>
        /// <param name="start1">Start of the first region.</param>
        /// <param name="size1">Size of the first region.</param>
        /// <param name="start2">Start of the second region.</param>
        /// <param name="size2">Size of the second region.</param>
        /// <returns><c>true</c> if the regions overlap; otherwise <c>false</c>.</returns>
        public static bool Collides(Loc start1, Loc size1, Loc start2, Loc size2)
        {
            return Collides(start1.X, size1.X, start2.X, size2.X) &&
                Collides(start1.Y, size1.Y, start2.Y, size2.Y);
        }

        /// <summary>
        /// Checks if two bounds collide
        /// </summary>
        /// <param name="start1">Start of bounds 1</param>
        /// <param name="size1">Size of bounds 1</param>
        /// <param name="start2">Start of bounds 2</param>
        /// <param name="size2">Size of bounds 2</param>
        /// <returns></returns>
        public static bool Collides(int start1, int size1, int start2, int size2)
        {
            return start1 + size1 > start2 && start2 + size2 > start1;
        }

        /// <summary>
        /// Calculates the amount of intersection between two bounds.
        /// If they don't intersect, the number is negative and represents their distance from intersecting.
        /// </summary>
        /// <param name="start1">Start of bounds 1</param>
        /// <param name="size1">Size of bounds 1</param>
        /// <param name="start2">Start of bounds 2</param>
        /// <param name="size2">Size of bounds 2</param>
        /// <returns></returns>
        public static int GetIntersection(int start1, int size1, int start2, int size2)
        {
            int distLeft = start1 - (start2 + size2);
            int distRight = start2 - (start1 + size1);

            if (distLeft < 0 && distRight < 0)
                return Math.Min(start1 + size1, start2 + size2) - Math.Max(start1, start2);
            else
                return -Math.Max(distLeft, distRight);
        }

        /// <summary>
        /// Determines if a point is within a rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="point">The point to check.</param>
        /// <returns><c>true</c> if the point is within the rectangle; otherwise <c>false</c>.</returns>
        public static bool InBounds(Rect rect, Loc point)
        {
            return InBounds(rect.Size.X, rect.Size.Y, point - rect.Start);
        }

        /// <summary>
        /// Determines if a point is within a rectangular region.
        /// </summary>
        /// <param name="start">Start of the region.</param>
        /// <param name="size">Size of the region.</param>
        /// <param name="point">The point to check.</param>
        /// <returns><c>true</c> if the point is within the region; otherwise <c>false</c>.</returns>
        public static bool InBounds(Loc start, Loc size, Loc point)
        {
            return InBounds(size.X, size.Y, point - start);
        }

        /// <summary>
        /// Determines if a point is within a rectangular region starting at origin.
        /// </summary>
        /// <param name="sizeX">Width of the region.</param>
        /// <param name="sizeY">Height of the region.</param>
        /// <param name="pt">The point to check.</param>
        /// <returns><c>true</c> if the point is within the region; otherwise <c>false</c>.</returns>
        public static bool InBounds(int sizeX, int sizeY, Loc pt)
        {
            return InBounds(sizeX, pt.X) && InBounds(sizeY, pt.Y);
        }

        /// <summary>
        /// Determines if a value is within a 1D range.
        /// </summary>
        /// <param name="start">Start of the range.</param>
        /// <param name="size">Size of the range.</param>
        /// <param name="pt">The value to check.</param>
        /// <returns><c>true</c> if the value is within the range; otherwise <c>false</c>.</returns>
        public static bool InBounds(int start, int size, int pt)
        {
            return InBounds(size, pt - start);
        }

        /// <summary>
        /// Determines if a value is within a 1D range starting at zero.
        /// </summary>
        /// <param name="size">Size of the range.</param>
        /// <param name="pt">The value to check.</param>
        /// <returns><c>true</c> if the value is within [0, size); otherwise <c>false</c>.</returns>
        public static bool InBounds(int size, int pt)
        {
            return pt >= 0 && pt < size;
        }

        /// <summary>
        /// Clamps a point to be within a rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="point">The point to clamp.</param>
        /// <returns>The clamped point.</returns>
        public static Loc ClampToBounds(Rect rect, Loc point)
        {
            return ClampToBounds(rect.Start, rect.Size, point);
        }

        /// <summary>
        /// Clamps a point to be within a rectangular region.
        /// </summary>
        /// <param name="start">Start of the region.</param>
        /// <param name="size">Size of the region.</param>
        /// <param name="point">The point to clamp.</param>
        /// <returns>The clamped point.</returns>
        public static Loc ClampToBounds(Loc start, Loc size, Loc point)
        {
            return ClampToBounds(size.X, size.Y, point - start) + start;
        }

        /// <summary>
        /// Clamps a point to be within a rectangular region starting at origin.
        /// </summary>
        /// <param name="sizeX">Width of the region.</param>
        /// <param name="sizeY">Height of the region.</param>
        /// <param name="pt">The point to clamp.</param>
        /// <returns>The clamped point.</returns>
        public static Loc ClampToBounds(int sizeX, int sizeY, Loc pt)
        {
            return new Loc(Math.Min(Math.Max(0, pt.X), sizeX - 1), Math.Min(Math.Max(0, pt.Y), sizeY - 1));
        }
    }
}
