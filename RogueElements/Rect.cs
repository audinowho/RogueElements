// <copyright file="Rect.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements
{
    [Serializable]
    public struct Rect : IEquatable<Rect>
    {
        public int X;

        public int Y;

        public int Width;

        public int Height;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rect"/> struct, with the specified
        /// position, width, and height.
        /// </summary>
        /// <param name="x">The x coordinate of the top-left corner of the created <see cref="Rect"/>.</param>
        /// <param name="y">The y coordinate of the top-left corner of the created <see cref="Rect"/>.</param>
        /// <param name="width">The width of the created<see cref="Rect"/>.</param>
        /// <param name="height">The height of the created<see cref="Rect"/>.</param>
        public Rect(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rect"/> struct that contains the two given rectangles.
        /// </summary>
        /// <param name="one">One.</param>
        /// <param name="two">Two.</param>
        public Rect(Rect one, Rect two)
        {
            var left = Math.Min(one.Left, two.Left);
            var right = Math.Max(one.Right, two.Right);
            var top = Math.Min(one.Top, two.Top);
            var bottom = Math.Max(one.Bottom, two.Bottom);

            this.X = left;
            this.Y = top;
            this.Width = right - left;
            this.Height = bottom - top;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rect"/> struct, with the specified
        /// location and size.
        /// </summary>
        /// <param name="location">The x and y coordinates of the top-left corner of the created <see cref="Rect"/>.</param>
        /// <param name="size">The width and height of the created <see cref="Rect"/>.</param>
        public Rect(Loc location, Loc size)
        {
            this.X = location.X;
            this.Y = location.Y;
            this.Width = size.X;
            this.Height = size.Y;
        }

        public static Rect Empty => new Rect(0, 0, 0, 0);

        public int Left => this.X;

        public int Right => this.X + this.Width;

        public int Top => this.Y;

        // TODO: sniff out off-by-ones with this...
        public int Bottom => this.Y + this.Height;

        public int Area => this.Width * this.Height;

        public int Perimeter => (this.Width * 2) + (this.Height * 2);

        public bool IsEmpty => this == Empty;

        public Loc Start
        {
            get => new Loc(this.X, this.Y);
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }

        public Loc Size
        {
            get => new Loc(this.Width, this.Height);
            set
            {
                this.Width = value.X;
                this.Height = value.Y;
            }
        }

        public Loc End => this.Start + this.Size;

        public Loc Center => new Loc(this.X + (this.Width / 2), this.Y + (this.Height / 2));

        internal string DebugDisplayString => $"{this.X}  {this.Y}  {this.Width}  {this.Height}";

        /// <summary>
        /// Compares whether two <see cref="Rect"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="Rect"/> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="Rect"/> instance on the right of the equal sign.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(Rect a, Rect b)
        {
            return (a.X == b.X) && (a.Y == b.Y) && (a.Width == b.Width) && (a.Height == b.Height);
        }

        /// <summary>
        /// Compares whether two <see cref="Rect"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="Rect"/> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="Rect"/> instance on the right of the not equal sign.</param>
        /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(Rect a, Rect b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Creates a new <see cref="Rect"/> that contains overlapping region of two other rectangles.
        /// </summary>
        /// <param name="value1">The first <see cref="Rect"/>.</param>
        /// <param name="value2">The second <see cref="Rect"/>.</param>
        /// <returns>Overlapping region of the two rectangles.</returns>
        public static Rect Intersect(Rect value1, Rect value2)
        {
            Intersect(ref value1, ref value2, out Rect rectangle);
            return rectangle;
        }

        /// <summary>
        /// Creates a new <see cref="Rect"/> that contains overlapping region of two other rectangles.
        /// </summary>
        /// <param name="value1">The first <see cref="Rect"/>.</param>
        /// <param name="value2">The second <see cref="Rect"/>.</param>
        /// <param name="result">Overlapping region of the two rectangles as an output parameter.</param>
        public static void Intersect(ref Rect value1, ref Rect value2, out Rect result)
        {
            if (value1.Intersects(value2))
            {
                var rightSide = Math.Min(value1.X + value1.Width, value2.X + value2.Width);
                var leftSide = Math.Max(value1.X, value2.X);
                var topSide = Math.Max(value1.Y, value2.Y);
                var bottomSide = Math.Min(value1.Y + value1.Height, value2.Y + value2.Height);
                result = new Rect(leftSide, topSide, rightSide - leftSide, bottomSide - topSide);
            }
            else
            {
                result = Rect.Empty;
            }
        }

        /// <summary>
        /// Creates a new <see cref="Rect"/> that completely contains two other rectangles.
        /// </summary>
        /// <param name="value1">The first <see cref="Rect"/>.</param>
        /// <param name="value2">The second <see cref="Rect"/>.</param>
        /// <returns>The union of the two rectangles.</returns>
        public static Rect Union(Rect value1, Rect value2)
        {
            var x = Math.Min(value1.X, value2.X);
            var y = Math.Min(value1.Y, value2.Y);
            return new Rect(
                x,
                y,
                Math.Max(value1.Right, value2.Right) - x,
                Math.Max(value1.Bottom, value2.Bottom) - y);
        }

        /// <summary>
        /// Creates a new <see cref="Rect"/> that completely contains two other rectangles.
        /// </summary>
        /// <param name="value1">The first <see cref="Rect"/>.</param>
        /// <param name="value2">The second <see cref="Rect"/>.</param>
        /// <param name="result">The union of the two rectangles as an output parameter.</param>
        public static void Union(ref Rect value1, ref Rect value2, out Rect result)
        {
            result.X = Math.Min(value1.X, value2.X);
            result.Y = Math.Min(value1.Y, value2.Y);
            result.Width = Math.Max(value1.Right, value2.Right) - result.X;
            result.Height = Math.Max(value1.Bottom, value2.Bottom) - result.Y;
        }

        /// <summary>
        /// Creates a new <see cref="Rect"/> from two points.
        /// </summary>
        /// <param name="point0">The top left or bottom right corner</param>
        /// <param name="point1">The bottom left or top right corner</param>
        /// <returns></returns>
        public static Rect FromPoints(Loc point0, Loc point1)
        {
            var x = Math.Min(point0.X, point1.X);
            var y = Math.Min(point0.Y, point1.Y);
            var width = Math.Abs(point0.X - point1.X);
            var height = Math.Abs(point0.Y - point1.Y);
            var rectangle = new Rect(x, y, width, height);
            return rectangle;
        }

        /// <summary>
        /// Creates a new <see cref="Rect"/> from one point.
        /// </summary>
        /// <param name="point">The point contained by the rect</param>
        /// <returns></returns>
        public static Rect FromPoint(Loc point)
        {
            return new Rect(point.X, point.Y, 1, 1);
        }

        /// <summary>
        /// Creates a new <see cref="Rect"/> from a point and radius.  Will always be an odd number in width/height.
        /// </summary>
        /// <param name="point">The point to be used as the center of the rect</param>
        /// <param name="radius">The radius of the rectangle</param>
        /// <returns></returns>
        public static Rect FromPointRadius(Loc point, int radius)
        {
            return new Rect(point - new Loc(radius), new Loc((radius * 2) + 1));
        }

        // TODO: test this; one point from every quadrant
        public static Rect IncludeLoc(Rect bounds, Loc point)
        {
            int minX = Math.Min(bounds.X, point.X);
            int minY = Math.Min(bounds.Y, point.Y);
            int maxX = Math.Max(bounds.End.X, point.X + 1);
            int maxY = Math.Max(bounds.End.Y, point.Y + 1);
            return FromPoints(new Loc(minX, minY), new Loc(maxX, maxY));
        }

        public Rect GetBoundingRectangle()
        {
            return this;
        }

        /// <summary>
        /// Gets whether or not the provided coordinates lie within the bounds of this <see cref="Rect"/>.
        /// </summary>
        /// <param name="x">The x coordinate of the point to check for containment.</param>
        /// <param name="y">The y coordinate of the point to check for containment.</param>
        /// <returns><c>true</c> if the provided coordinates lie inside this <see cref="Rect"/>; <c>false</c> otherwise.</returns>
        public bool Contains(int x, int y)
        {
            return this.X <= x && x < this.X + this.Width && this.Y <= y && y < this.Y + this.Height;
        }

        /// <summary>
        /// Gets whether or not the provided <see cref="Loc"/> lies within the bounds of this <see cref="Rect"/>.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="Rect"/>.</param>
        /// <returns><c>true</c> if the provided <see cref="Loc"/> lies inside this <see cref="Rect"/>; <c>false</c> otherwise.</returns>
        public bool Contains(Loc value)
        {
            return this.X <= value.X && value.X < this.X + this.Width && this.Y <= value.Y && value.Y < this.Y + this.Height;
        }

        /// <summary>
        /// Gets whether or not the provided <see cref="Loc"/> lies within the bounds of this <see cref="Rect"/>.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="Rect"/>.</param>
        /// <param name="result"><c>true</c> if the provided <see cref="Loc"/> lies inside this <see cref="Rect"/>; <c>false</c> otherwise. As an output parameter.</param>
        public void Contains(ref Loc value, out bool result)
        {
            result = (this.X <= value.X) && (value.X < this.X + this.Width) && (this.Y <= value.Y) && (value.Y < this.Y + this.Height);
        }

        /// <summary>
        /// Gets whether or not the provided <see cref="Rect"/> lies entirely within the bounds of this <see cref="Rect"/>.
        /// </summary>
        /// <param name="value">The <see cref="Rect"/> to check for inclusion in this <see cref="Rect"/>.</param>
        /// <returns><c>true</c> if the provided <see cref="Rect"/>'s bounds lie entirely inside this <see cref="Rect"/>; <c>false</c> otherwise.</returns>
        public bool Contains(Rect value)
        {
            return (this.X <= value.X) && (value.X + value.Width <= this.X + this.Width) && (this.Y <= value.Y) && (value.Y + value.Height <= this.Y + this.Height);
        }

        /// <summary>
        /// Gets whether or not the provided <see cref="Rect"/> lies within the bounds of this <see cref="Rect"/>.
        /// </summary>
        /// <param name="value">The <see cref="Rect"/> to check for inclusion in this <see cref="Rect"/>.</param>
        /// <param name="result"><c>true</c> if the provided <see cref="Rect"/>'s bounds lie entirely inside this <see cref="Rect"/>; <c>false</c> otherwise. As an output parameter.</param>
        public void Contains(ref Rect value, out bool result)
        {
            result = (this.X <= value.X) && (value.X + value.Width <= this.X + this.Width) && (this.Y <= value.Y) && (value.Y + value.Height <= this.Y + this.Height);
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is Rect && this == (Rect)obj;
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Rect"/>.
        /// </summary>
        /// <param name="other">The <see cref="Rect"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public bool Equals(Rect other)
        {
            return this == other;
        }

        /// <summary>
        /// Gets the hash code of this <see cref="Rect"/>.
        /// </summary>
        /// <returns>Hash code of this <see cref="Rect"/>.</returns>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Width.GetHashCode() ^ this.Height.GetHashCode();
        }

        /// <summary>
        /// Adjusts the edges of this <see cref="Rect"/> by specified horizontal and vertical amounts.
        /// </summary>
        /// <param name="horizontalAmount">Value to adjust the left and right edges.</param>
        /// <param name="verticalAmount">Value to adjust the top and bottom edges.</param>
        public void Inflate(int horizontalAmount, int verticalAmount)
        {
            this.X -= horizontalAmount;
            this.Y -= verticalAmount;
            this.Width += horizontalAmount * 2;
            this.Height += verticalAmount * 2;
        }

        /// <summary>
        /// Adjusts the edges of this <see cref="Rect"/> in a specified direction.
        /// </summary>
        /// <param name="direction">Direction to expand in.</param>
        /// <param name="amount">Value to expand by.</param>
        public void Expand(Dir4 direction, int amount)
        {
            Axis4 axis = direction.ToAxis();
            if (axis == Axis4.Horiz)
            {
                this.Width += amount;
                if (direction == Dir4.Left)
                    this.X -= amount;
            }
            else if (axis == Axis4.Vert)
            {
                this.Height += amount;
                if (direction == Dir4.Up)
                    this.Y -= amount;
            }
        }

        public IntRange GetSide(Axis4 axis)
        {
            switch (axis)
            {
                case Axis4.Vert:
                    return new IntRange(this.X, this.X + this.Width);
                case Axis4.Horiz:
                    return new IntRange(this.Y, this.Y + this.Height);
                default:
                    throw new ArgumentException("Invalid value to get.");
            }
        }

        public int GetScalar(Dir4 direction)
        {
            switch (direction)
            {
                case Dir4.Down:
                    return this.Bottom;
                case Dir4.Left:
                    return this.Left;
                case Dir4.Up:
                    return this.Top;
                case Dir4.Right:
                    return this.Right;
                default:
                    throw new ArgumentException("Invalid value to get.");
            }
        }

        public void SetScalar(Dir4 direction, int value)
        {
            switch (direction)
            {
                case Dir4.Down:
                    this.Height = value - this.Y;
                    return;
                case Dir4.Left:
                    this.Width = this.Right - value;
                    this.X = value;
                    return;
                case Dir4.Up:
                    this.Height = this.Bottom - value;
                    this.Y = value;
                    return;
                case Dir4.Right:
                    this.Width = value - this.X;
                    return;
                default:
                    throw new ArgumentException("Invalid value to get.");
            }
        }

        /// <summary>
        /// Gets whether or not the other <see cref="Rect"/> intersects with this RectangleF.
        /// </summary>
        /// <param name="value">The other rectangle for testing.</param>
        /// <returns><c>true</c> if other <see cref="Rect"/> intersects with this rectangle; <c>false</c> otherwise.</returns>
        public bool Intersects(Rect value)
        {
            return value.Left < this.Right && this.Left < value.Right &&
                   value.Top < this.Bottom && this.Top < value.Bottom;
        }

        /// <summary>
        /// Gets whether or not the other <see cref="Rect"/> intersects with this rectangle.
        /// </summary>
        /// <param name="value">The other rectangle for testing.</param>
        /// <param name="result"><c>true</c> if other <see cref="Rect"/> intersects with this rectangle; <c>false</c> otherwise. As an output parameter.</param>
        public void Intersects(ref Rect value, out bool result)
        {
            result = value.Left < this.Right && this.Left < value.Right &&
                     value.Top < this.Bottom && this.Top < value.Bottom;
        }

        /// <summary>
        /// Changes the <see cref="Start"/> of this <see cref="Rect"/>.
        /// </summary>
        /// <param name="offsetX">The x coordinate to add to this <see cref="Rect"/>.</param>
        /// <param name="offsetY">The y coordinate to add to this <see cref="Rect"/>.</param>
        public void Offset(int offsetX, int offsetY)
        {
            this.X += offsetX;
            this.Y += offsetY;
        }

        /// <summary>
        /// Changes the <see cref="Start"/> of this <see cref="Rect"/>.
        /// </summary>
        /// <param name="amount">The x and y components to add to this <see cref="Rect"/>.</param>
        public void Offset(Loc amount)
        {
            this.X += amount.X;
            this.Y += amount.Y;
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Rect"/> in the format:
        /// {X:[<see cref="X"/>] Y:[<see cref="Y"/>] Width:[<see cref="Width"/>] Height:[<see cref="Height"/>]}
        /// </summary>
        /// <returns><see cref="string"/> representation of this <see cref="Rect"/>.</returns>
        public override string ToString()
        {
            return $"{{X:{this.X} Y:{this.Y} Width:{this.Width} Height:{this.Height}}}";
        }

        /// <summary>
        /// Calculates the signed depth of intersection between two rectangles.
        /// </summary>
        /// <param name="other">todo: describe other parameter on IntersectionDepth</param>
        /// <returns>
        /// The amount of overlap between two intersecting rectangles. These
        /// depth values can be negative depending on which wides the rectangles
        /// intersect. This allows callers to determine the correct direction
        /// to push objects in order to resolve collisions.
        /// If the rectangles are not intersecting, Loc.Zero is returned.
        /// </returns>
        public Loc IntersectionDepth(Rect other)
        {
            // Calculate half sizes.
            var thisHalfWidth = this.Width / 2;
            var thisHalfHeight = this.Height / 2;
            var otherHalfWidth = other.Width / 2;
            var otherHalfHeight = other.Height / 2;

            // Calculate centers.
            var centerA = new Loc(this.Left + thisHalfWidth, this.Top + thisHalfHeight);
            var centerB = new Loc(other.Left + otherHalfWidth, other.Top + otherHalfHeight);

            // Calculate current and minimum-non-intersecting distances between centers.
            var distanceX = centerA.X - centerB.X;
            var distanceY = centerA.Y - centerB.Y;
            var minDistanceX = thisHalfWidth + otherHalfWidth;
            var minDistanceY = thisHalfHeight + otherHalfHeight;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Loc.Zero;

            // Calculate and return intersection depths.
            var depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            var depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Loc(depthX, depthY);
        }
    }
}
