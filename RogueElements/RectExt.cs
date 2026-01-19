// <copyright file="RectExt.cs" company="Audino">
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
    /// Provides extension methods for the <see cref="Rect"/> struct.
    /// </summary>
    public static class RectExt
    {
        /// <summary>
        /// Gets the location to put an adjacent rectangle with specified parameters
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="dir"></param>
        /// <param name="size"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Loc GetEdgeRectLoc(this Rect rect, Dir4 dir, Loc size, int scalar)
        {
            switch (dir)
            {
                case Dir4.Down:
                    return new Loc(scalar, rect.End.Y);
                case Dir4.Left:
                    return new Loc(rect.X - size.X, scalar);
                case Dir4.Up:
                    return new Loc(scalar, rect.Y - size.Y);
                case Dir4.Right:
                    return new Loc(rect.End.X, scalar);
                case Dir4.None:
                    throw new ArgumentException($"No edge for dir {nameof(Dir4.None)}");
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), "Invalid enum value.");
            }
        }

        /// <summary>
        /// Gets the loc just inside the rect, from the specified direction, with the specified scalar.  The scalar determines X if it's a vertical, and Y if it's a horizontal side.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="dir"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Loc GetEdgeLoc(this Rect rect, Dir4 dir, int scalar)
        {
            switch (dir)
            {
                case Dir4.Down:
                    return new Loc(scalar, rect.End.Y - 1);
                case Dir4.Left:
                    return new Loc(rect.X, scalar);
                case Dir4.Up:
                    return new Loc(scalar, rect.Y);
                case Dir4.Right:
                    return new Loc(rect.End.X - 1, scalar);
                case Dir4.None:
                    throw new ArgumentException($"No edge for dir {nameof(Dir4.None)}");
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), "Invalid enum value.");
            }
        }

        /// <summary>
        /// Gets the length of the border in the specified direction.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="dir">The direction of the border.</param>
        /// <returns>The length of the border perpendicular to the direction.</returns>
        public static int GetBorderLength(this Rect rect, Dir4 dir)
        {
            return rect.GetSide(dir.ToAxis()).Length;
        }
    }
}
