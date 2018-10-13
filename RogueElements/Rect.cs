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

        public static Rect Empty { get { return new Rect(); } }

        public int Left { get { return X; } }

        public int Right { get { return X + Width; } }

        public int Top { get { return Y; } }

        //TODO: sniff out off-by-ones with this...
        public int Bottom { get { return Y + Height; } }
        
        public int Area { get { return Width * Height; } }

        public int Perimeter { get { return Width * 2 +  Height * 2; } }

        public bool IsEmpty { get { return Width.Equals(0) && Height.Equals(0) && X.Equals(0) && Y.Equals(0); } }

        public Loc Start
        {
            get { return new Loc(X, Y); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Loc Size
        {
            get { return new Loc(Width, Height); }
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }
        public Loc End { get { return Start + Size; } }

        public Loc Center { get { return new Loc(X + Width / 2, Y + Height / 2); } }

        internal string DebugDisplayString { get { return string.Concat(X, "  ", Y, "  ", Width, "  ", Height); } }

        /// <summary>
        /// Creates a new instance of <see cref="Rect"/> struct, with the specified
        /// position, width, and height.
        /// </summary>
        /// <param name="x">The x coordinate of the top-left corner of the created <see cref="Rect"/>.</param>
        /// <param name="y">The y coordinate of the top-left corner of the created <see cref="Rect"/>.</param>
        /// <param name="width">The width of the created <see cref="Rect"/>.</param>
        /// <param name="height">The height of the created <see cref="Rect"/>.</param>
        public Rect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Humper.Base.RectangleF"/> that contains the two given rectangles.
        /// </summary>
        /// <param name="one">One.</param>
        /// <param name="two">Two.</param>
        public Rect(Rect one, Rect two)
        {
            var left = Math.Min(one.Left, two.Left);
            var right = Math.Max(one.Right, two.Right);
            var top = Math.Min(one.Top, two.Top);
            var bottom = Math.Max(one.Bottom, two.Bottom);

            X = left;
            Y = top;
            Width = right - left;
            Height = bottom - top;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Rect"/> struct, with the specified
        /// location and size.
        /// </summary>
        /// <param name="location">The x and y coordinates of the top-left corner of the created <see cref="Rect"/>.</param>
        /// <param name="size">The width and height of the created <see cref="Rect"/>.</param>
        public Rect(Loc location, Loc size)
        {
            X = location.X;
            Y = location.Y;
            Width = size.X;
            Height = size.Y;
        }

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
            return X <= x && x < X + Width && Y <= y && y < Y + Height;
        }

        /// <summary>
        /// Gets whether or not the provided <see cref="Vector2"/> lies within the bounds of this <see cref="Rect"/>.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="Rect"/>.</param>
        /// <returns><c>true</c> if the provided <see cref="Vector2"/> lies inside this <see cref="Rect"/>; <c>false</c> otherwise.</returns>
        public bool Contains(Loc value)
        {
            return X <= value.X && value.X < X + Width && Y <= value.Y && value.Y < Y + Height;
        }

        /// <summary>
        /// Gets whether or not the provided <see cref="Vector2"/> lies within the bounds of this <see cref="Rect"/>.
        /// </summary>
        /// <param name="value">The coordinates to check for inclusion in this <see cref="Rect"/>.</param>
        /// <param name="result"><c>true</c> if the provided <see cref="Vector2"/> lies inside this <see cref="Rect"/>; <c>false</c> otherwise. As an output parameter.</param>
        public void Contains(ref Loc value, out bool result)
        {
            result = (X <= value.X) && (value.X < X + Width) && (Y <= value.Y) && (value.Y < Y + Height);
        }

        /// <summary>
        /// Gets whether or not the provided <see cref="Rect"/> lies within the bounds of this <see cref="Rect"/>.
        /// </summary>
        /// <param name="value">The <see cref="Rect"/> to check for inclusion in this <see cref="Rect"/>.</param>
        /// <returns><c>true</c> if the provided <see cref="Rect"/>'s bounds lie entirely inside this <see cref="Rect"/>; <c>false</c> otherwise.</returns>
        public bool Contains(Rect value)
        {
            return (X <= value.X) && (value.X + value.Width <= X + Width) && (Y <= value.Y) && (value.Y + value.Height <= Y + Height);
        }

        /// <summary>
        /// Gets whether or not the provided <see cref="Rect"/> lies within the bounds of this <see cref="Rect"/>.
        /// </summary>
        /// <param name="value">The <see cref="Rect"/> to check for inclusion in this <see cref="Rect"/>.</param>
        /// <param name="result"><c>true</c> if the provided <see cref="Rect"/>'s bounds lie entirely inside this <see cref="Rect"/>; <c>false</c> otherwise. As an output parameter.</param>
        public void Contains(ref Rect value, out bool result)
        {
            result = (X <= value.X) && (value.X + value.Width <= X + Width) && (Y <= value.Y) && (value.Y + value.Height <= Y + Height);
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to compare.</param>
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
            return X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
        }

        /// <summary>
        /// Adjusts the edges of this <see cref="Rect"/> by specified horizontal and vertical amounts. 
        /// </summary>
        /// <param name="horizontalAmount">Value to adjust the left and right edges.</param>
        /// <param name="verticalAmount">Value to adjust the top and bottom edges.</param>
        public void Inflate(int horizontalAmount, int verticalAmount)
        {
            X -= horizontalAmount;
            Y -= verticalAmount;
            Width += horizontalAmount * 2;
            Height += verticalAmount * 2;
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
                Width += amount;
                if (direction == Dir4.Left)
                    X -= amount;
            }
            else if (axis == Axis4.Vert)
            {
                Height += amount;
                if (direction == Dir4.Up)
                    Y -= amount;
            }
        }
        
        public Range GetSide(Axis4 axis)
        {
            switch (axis)
            {
                case Axis4.Vert:
                    return new Range(X, X + Width);
                case Axis4.Horiz:
                    return new Range(Y, Y + Height);
                default:
                    throw new ArgumentException("Invalid value to get.");
            }
        }


        public int GetScalar(Dir4 direction)
        {
            switch (direction)
            {
                case Dir4.Down:
                    return Bottom;
                case Dir4.Left:
                    return Left;
                case Dir4.Up:
                    return Top;
                case Dir4.Right:
                    return Right;
                default:
                    throw new ArgumentException("Invalid value to get.");
            }
        }

        public void SetScalar(Dir4 direction, int value)
        {
            switch (direction)
            {
                case Dir4.Down:
                    {
                        Height = value - Y;
                        return;
                    }
                case Dir4.Left:
                    {
                        Width = Right - value;
                        X = value;
                        return;
                    }
                case Dir4.Up:
                    {
                        Height = Bottom - value;
                        Y = value;
                        return;
                    }
                case Dir4.Right:
                    {
                        Width = value - X;
                        return;
                    }
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
            return value.Left < Right && Left < value.Right &&
                   value.Top < Bottom && Top < value.Bottom;
        }


        /// <summary>
        /// Gets whether or not the other <see cref="Rect"/> intersects with this rectangle.
        /// </summary>
        /// <param name="value">The other rectangle for testing.</param>
        /// <param name="result"><c>true</c> if other <see cref="Rect"/> intersects with this rectangle; <c>false</c> otherwise. As an output parameter.</param>
        public void Intersects(ref Rect value, out bool result)
        {
            result = value.Left < Right && Left < value.Right &&
                     value.Top < Bottom && Top < value.Bottom;
        }

        /// <summary>
        /// Creates a new <see cref="Rect"/> that contains overlapping region of two other rectangles.
        /// </summary>
        /// <param name="value1">The first <see cref="Rect"/>.</param>
        /// <param name="value2">The second <see cref="Rect"/>.</param>
        /// <returns>Overlapping region of the two rectangles.</returns>
        public static Rect Intersect(Rect value1, Rect value2)
        {
            Rect rectangle;
            Intersect(ref value1, ref value2, out rectangle);
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
                result = new Rect(0, 0, 0, 0);
            }
        }

        /// <summary>
        /// Changes the <see cref="Start"/> of this <see cref="Rect"/>.
        /// </summary>
        /// <param name="offsetX">The x coordinate to add to this <see cref="Rect"/>.</param>
        /// <param name="offsetY">The y coordinate to add to this <see cref="Rect"/>.</param>
        public void Offset(int offsetX, int offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        /// <summary>
        /// Changes the <see cref="Start"/> of this <see cref="Rect"/>.
        /// </summary>
        /// <param name="amount">The x and y components to add to this <see cref="Rect"/>.</param>
        public void Offset(Loc amount)
        {
            X += amount.X;
            Y += amount.Y;
        }

        /// <summary>
        /// Returns a <see cref="String"/> representation of this <see cref="Rect"/> in the format:
        /// {X:[<see cref="X"/>] Y:[<see cref="Y"/>] Width:[<see cref="Width"/>] Height:[<see cref="Height"/>]}
        /// </summary>
        /// <returns><see cref="String"/> representation of this <see cref="Rect"/>.</returns>
        public override string ToString()
        {
            return "{X:" + X + " Y:" + Y + " Width:" + Width + " Height:" + Height + "}";
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
            return new Rect(x, y,
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
        /// Calculates the signed depth of intersection between two rectangles.
        /// </summary>
        /// <returns>
        /// The amount of overlap between two intersecting rectangles. These
        /// depth values can be negative depending on which wides the rectangles
        /// intersect. This allows callers to determine the correct direction
        /// to push objects in order to resolve collisions.
        /// If the rectangles are not intersecting, Vector2.Zero is returned.
        /// </returns>
        public Loc IntersectionDepth(Rect other)
        {
            // Calculate half sizes.
            var thisHalfWidth = Width / 2;
            var thisHalfHeight = Height / 2;
            var otherHalfWidth = other.Width / 2;
            var otherHalfHeight = other.Height / 2;

            // Calculate centers.
            var centerA = new Loc(Left + thisHalfWidth, Top + thisHalfHeight);
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

        //TODO: test this; one point from every quadrant
        public static Rect IncludeLoc(Rect bounds, Loc point)
        {
            int minX = Math.Min(bounds.X, point.X);
            int minY = Math.Min(bounds.Y, point.Y);
            int maxX = Math.Max(bounds.End.X, point.X + 1);
            int maxY = Math.Max(bounds.End.Y, point.Y + 1);
            return FromPoints(new Loc(minX, minY), new Loc(maxX, maxY));
        }
    }
}
