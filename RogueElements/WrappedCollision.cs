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
    }
}
