// <copyright file="DirExt.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements
{
    public static class DirExt
    {
        public const int DIRH_COUNT = 2;
        public const int DIRV_COUNT = 2;
        public const int DIR4_COUNT = 4;
        public const int DIR8_COUNT = 8;
        public const int AXIS4_COUNT = 2;
        public const int AXIS8_COUNT = 4;

        public static readonly IEnumerable<DirH> VALID_DIRH = new DirH[] { DirH.Left, DirH.Right };
        public static readonly IEnumerable<DirV> VALID_DIRV = new DirV[] { DirV.Up, DirV.Down };
        public static readonly IEnumerable<Dir4> VALID_DIR4 = new Dir4[] { Dir4.Down, Dir4.Left, Dir4.Up, Dir4.Right };
        public static readonly IEnumerable<Dir8> VALID_DIR8 = new Dir8[] { Dir8.Down, Dir8.DownLeft, Dir8.Left, Dir8.UpLeft, Dir8.Up, Dir8.UpRight, Dir8.Right, Dir8.DownRight };
        public static readonly IEnumerable<Axis4> VALID_AXIS4 = new Axis4[] { Axis4.Vert, Axis4.Horiz };
        public static readonly IEnumerable<Axis8> VALID_AXIS8 = new Axis8[] { Axis8.Vert, Axis8.DiagForth, Axis8.Horiz, Axis8.DiagBack };

        public static bool Validate(this DirV dir)
        {
            switch (dir)
            {
                case DirV.None:
                case DirV.Down:
                case DirV.Up:
                    return true;
                default:
                    return false;
            }
        }

        public static bool Validate(this DirH dir)
        {
            switch (dir)
            {
                case DirH.None:
                case DirH.Left:
                case DirH.Right:
                    return true;
                default:
                    return false;
            }
        }

        public static bool Validate(this Dir4 dir)
        {
            switch (dir)
            {
                case Dir4.None:
                case Dir4.Down:
                case Dir4.Left:
                case Dir4.Up:
                case Dir4.Right:
                    return true;
                default:
                    return false;
            }
        }

        public static bool Validate(this Dir8 dir)
        {
            switch (dir)
            {
                case Dir8.None:
                case Dir8.Down:
                case Dir8.DownLeft:
                case Dir8.Left:
                case Dir8.UpLeft:
                case Dir8.Up:
                case Dir8.UpRight:
                case Dir8.Right:
                case Dir8.DownRight:
                    return true;
                default:
                    return false;
            }
        }

        public static bool Validate(this Axis4 axis)
        {
            switch (axis)
            {
                case Axis4.None:
                case Axis4.Horiz:
                case Axis4.Vert:
                    return true;
                default:
                    return false;
            }
        }

        public static bool Validate(this Axis8 axis)
        {
            switch (axis)
            {
                case Axis8.None:
                case Axis8.Horiz:
                case Axis8.Vert:
                case Axis8.DiagBack:
                case Axis8.DiagForth:
                    return true;
                default:
                    return false;
            }
        }

        public static Dir4 ToDir4(this DirH dir)
        {
            switch (dir)
            {
                case DirH.None: return Dir4.None;
                case DirH.Left: return Dir4.Left;
                case DirH.Right: return Dir4.Right;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            }
        }

        public static Dir4 ToDir4(this DirV dir)
        {
            switch (dir)
            {
                case DirV.None: return Dir4.None;
                case DirV.Down: return Dir4.Down;
                case DirV.Up: return Dir4.Up;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            }
        }

        public static Dir8 ToDir8(this DirH dir)
        {
            switch (dir)
            {
                case DirH.None: return Dir8.None;
                case DirH.Left: return Dir8.Left;
                case DirH.Right: return Dir8.Right;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            }
        }

        public static Dir8 ToDir8(this DirV dir)
        {
            switch (dir)
            {
                case DirV.None: return Dir8.None;
                case DirV.Down: return Dir8.Down;
                case DirV.Up: return Dir8.Up;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            }
        }

        public static Dir8 ToDir8(this Dir4 dir)
        {
            switch (dir)
            {
                case Dir4.None: return Dir8.None;
                case Dir4.Down: return Dir8.Down;
                case Dir4.Left: return Dir8.Left;
                case Dir4.Up: return Dir8.Up;
                case Dir4.Right: return Dir8.Right;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            }
        }

        public static Dir4 ToDir4(this Dir8 dir)
        {
            switch (dir)
            {
                case Dir8.None: return Dir4.None;
                case Dir8.Down: return Dir4.Down;
                case Dir8.Left: return Dir4.Left;
                case Dir8.Up: return Dir4.Up;
                case Dir8.Right: return Dir4.Right;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            }
        }

        public static Axis8 ToAxis8(this Axis4 axis)
        {
            switch (axis)
            {
                case Axis4.None:
                    return Axis8.None;
                case Axis4.Horiz:
                    return Axis8.Horiz;
                case Axis4.Vert:
                    return Axis8.Vert;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, "Invalid enum value.");
            }
        }

        public static Axis4 ToAxis4(this Axis8 axis)
        {
            switch (axis)
            {
                case Axis8.None:
                    return Axis4.None;
                case Axis8.Horiz:
                    return Axis4.Horiz;
                case Axis8.Vert:
                    return Axis4.Vert;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, "Invalid enum value.");
            }
        }

        public static Axis4 ToAxis(this Dir4 dir)
        {
            switch (dir)
            {
                case Dir4.None:
                    return Axis4.None;
                case Dir4.Down:
                case Dir4.Up:
                    return Axis4.Vert;
                case Dir4.Left:
                case Dir4.Right:
                    return Axis4.Horiz;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            }
        }

        /// <summary>
        /// Gets the orthogonal axis to this 4-directional axis.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static Axis4 Orth(this Axis4 axis)
        {
            switch (axis)
            {
                case Axis4.None:
                    return Axis4.None;
                case Axis4.Vert:
                    return Axis4.Horiz;
                case Axis4.Horiz:
                    return Axis4.Vert;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, "Invalid enum value.");
            }
        }

        public static Axis8 ToAxis(this Dir8 dir)
        {
            switch (dir)
            {
                case Dir8.None:
                    return Axis8.None;
                case Dir8.Down:
                case Dir8.Up:
                    return Axis8.Vert;
                case Dir8.Left:
                case Dir8.Right:
                    return Axis8.Horiz;
                case Dir8.DownLeft:
                case Dir8.UpRight:
                    return Axis8.DiagForth;
                case Dir8.UpLeft:
                case Dir8.DownRight:
                    return Axis8.DiagBack;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            }
        }

        public static bool IsDiagonal(this Dir8 dir)
        {
            switch (dir)
            {
                case Dir8.DownLeft:
                case Dir8.UpLeft:
                case Dir8.UpRight:
                case Dir8.DownRight:
                    return true;
                case Dir8.Down:
                case Dir8.Left:
                case Dir8.Up:
                case Dir8.Right:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            }
        }

        public static Loc GetLoc(this DirH dir)
        {
            switch (dir)
            {
                case DirH.None:
                    return Loc.Zero;
                case DirH.Left:
                    return -Loc.UnitX;
                case DirH.Right:
                    return Loc.UnitX;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            }
        }

        public static Loc GetLoc(this DirV dir)
        {
            switch (dir)
            {
                case DirV.None:
                    return Loc.Zero;
                case DirV.Down:
                    return Loc.UnitY;
                case DirV.Up:
                    return -Loc.UnitY;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            }
        }

        public static Loc GetLoc(this Dir4 dir)
        {
            switch (dir)
            {
                case Dir4.None:
                    return Loc.Zero;
                case Dir4.Down:
                    return Loc.UnitY;
                case Dir4.Left:
                    return -Loc.UnitX;
                case Dir4.Up:
                    return -Loc.UnitY;
                case Dir4.Right:
                    return Loc.UnitX;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            }
        }

        public static Loc GetLoc(this Dir8 dir)
        {
            switch (dir)
            {
                case Dir8.None:
                    return Loc.Zero;
                case Dir8.Down:
                    return Loc.UnitY;
                case Dir8.DownLeft:
                    return Loc.UnitY - Loc.UnitX;
                case Dir8.Left:
                    return -Loc.UnitX;
                case Dir8.UpLeft:
                    return -Loc.One;
                case Dir8.Up:
                    return -Loc.UnitY;
                case Dir8.UpRight:
                    return Loc.UnitX - Loc.UnitY;
                case Dir8.Right:
                    return Loc.UnitX;
                case Dir8.DownRight:
                    return Loc.One;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            }
        }

        public static DirH Reverse(this DirH dir)
        {
            switch (dir)
            {
                case DirH.None:
                    return DirH.None;
                case DirH.Left:
                    return DirH.Right;
                case DirH.Right:
                    return DirH.Left;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            }
        }

        public static DirV Reverse(this DirV dir)
        {
            switch (dir)
            {
                case DirV.None:
                    return DirV.None;
                case DirV.Down:
                    return DirV.Up;
                case DirV.Up:
                    return DirV.Down;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            }
        }

        public static Dir4 Reverse(this Dir4 dir)
        {
            switch (dir)
            {
                case Dir4.None:
                    return Dir4.None;
                case Dir4.Down:
                    return Dir4.Up;
                case Dir4.Left:
                    return Dir4.Right;
                case Dir4.Up:
                    return Dir4.Down;
                case Dir4.Right:
                    return Dir4.Left;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            }
        }

        public static Dir8 Reverse(this Dir8 dir)
        {
            switch (dir)
            {
                case Dir8.None:
                    return Dir8.None;
                case Dir8.Down:
                    return Dir8.Up;
                case Dir8.DownLeft:
                    return Dir8.UpRight;
                case Dir8.Left:
                    return Dir8.Right;
                case Dir8.UpLeft:
                    return Dir8.DownRight;
                case Dir8.Up:
                    return Dir8.Down;
                case Dir8.UpRight:
                    return Dir8.DownLeft;
                case Dir8.Right:
                    return Dir8.Left;
                case Dir8.DownRight:
                    return Dir8.UpLeft;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            }
        }

        public static Dir8 Combine(DirH horiz, DirV vert)
        {
            switch (vert)
            {
                case DirV.Down:
                    switch (horiz)
                    {
                        case DirH.Right:
                            return Dir8.DownRight;
                        case DirH.Left:
                            return Dir8.DownLeft;
                        case DirH.None:
                            return Dir8.Down;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(horiz), horiz, "Invalid enum value.");
                    }

                case DirV.Up:
                    switch (horiz)
                    {
                        case DirH.Right:
                            return Dir8.UpRight;
                        case DirH.Left:
                            return Dir8.UpLeft;
                        case DirH.None:
                            return Dir8.Up;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(horiz), horiz, "Invalid enum value.");
                    }

                case DirV.None:
                    switch (horiz)
                    {
                        case DirH.Right:
                            return Dir8.Right;
                        case DirH.Left:
                            return Dir8.Left;
                        case DirH.None:
                            return Dir8.None;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(horiz), horiz, "Invalid enum value.");
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(vert), vert, "Invalid enum value.");
            }
        }

        public static void Separate(this Dir8 dir, out DirH horiz, out DirV vert)
        {
            if (!dir.Validate())
                throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            switch (dir)
            {
                case Dir8.Down:
                case Dir8.DownLeft:
                case Dir8.DownRight:
                    vert = DirV.Down;
                    break;
                case Dir8.Up:
                case Dir8.UpLeft:
                case Dir8.UpRight:
                    vert = DirV.Up;
                    break;
                default:
                    vert = DirV.None;
                    break;
            }

            switch (dir)
            {
                case Dir8.Left:
                case Dir8.UpLeft:
                case Dir8.DownLeft:
                    horiz = DirH.Left;
                    break;
                case Dir8.Right:
                case Dir8.UpRight:
                case Dir8.DownRight:
                    horiz = DirH.Right;
                    break;
                default:
                    horiz = DirH.None;
                    break;
            }
        }

        public static Dir8 GetDir(Loc loc1, Loc loc2)
        {
            return GetDir(loc2 - loc1);
        }

        public static Dir8 GetDir(this Loc loc)
        {
            if (loc.Y > 0)
            {
                if (loc.X > 0)
                    return Dir8.DownRight;
                else if (loc.X < 0)
                    return Dir8.DownLeft;
                else
                    return Dir8.Down;
            }
            else if (loc.Y < 0)
            {
                if (loc.X > 0)
                    return Dir8.UpRight;
                else if (loc.X < 0)
                    return Dir8.UpLeft;
                else
                    return Dir8.Up;
            }
            else
            {
                if (loc.X > 0)
                    return Dir8.Right;
                else if (loc.X < 0)
                    return Dir8.Left;
                else
                    return Dir8.None;
            }
        }

        public static Dir8 GetBoundsDir(Loc start, Loc size, Loc point)
        {
            return GetBoundsDir(size, point - start);
        }

        public static Dir8 GetBoundsDir(Loc size, Loc point)
        {
            if (size.X <= 0 || size.Y <= 0)
                throw new ArgumentException("Cannot compute direction using an area at or below 0.");
            if (point.Y < 0)
            {
                if (point.X < 0)
                    return Dir8.UpLeft;
                else if (point.X < size.X)
                    return Dir8.Up;
                else
                    return Dir8.UpRight;
            }
            else if (point.Y < size.Y)
            {
                if (point.X < 0)
                    return Dir8.Left;
                else if (point.X < size.X)
                    return Dir8.None;
                else
                    return Dir8.Right;
            }
            else
            {
                if (point.X < 0)
                    return Dir8.DownLeft;
                else if (point.X < size.X)
                    return Dir8.Down;
                else
                    return Dir8.DownRight;
            }
        }

        /// <summary>
        /// Vertical takes precedent over horizontal
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public static Dir4 ApproximateDir4(this Loc loc)
        {
            if (loc.X > 0)
            {
                if (loc.Y >= loc.X)
                    return Dir4.Down;
                else if (loc.Y <= -loc.X)
                    return Dir4.Up; // Y is negative; flip X
                else
                    return Dir4.Right;
            }
            else if (loc.X < 0)
            {
                if (loc.Y >= -loc.X)
                    return Dir4.Down; // X is negative; flip X
                else if (loc.Y <= loc.X)
                    return Dir4.Up; // X and Y both negative
                else
                    return Dir4.Left;
            }
            else
            {
                if (loc.Y > 0)
                    return Dir4.Down;
                else if (loc.Y < 0)
                    return Dir4.Up;
                else
                    return Dir4.None;
            }
        }

        /// <summary>
        /// Cardinal directions take precedent over diagonal directions.
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public static Dir8 ApproximateDir8(this Loc loc)
        {
            if (loc.X > 0)
            {
                // Compare dot products to find the vector this one is closest to.
                Dir8 cardinal, diagonal;
                if (loc.Y >= loc.X)
                {
                    cardinal = Dir8.Down;
                    diagonal = Dir8.DownRight;
                }
                else if (loc.Y <= -loc.X)
                {
                    // Y is negative; flip X
                    cardinal = Dir8.Up;
                    diagonal = Dir8.UpRight;
                }
                else if (loc.Y > 0)
                {
                    cardinal = Dir8.Right;
                    diagonal = Dir8.DownRight;
                }
                else if (loc.Y < 0)
                {
                    cardinal = Dir8.Right;
                    diagonal = Dir8.UpRight;
                }
                else
                {
                    // == 0
                    return Dir8.Right;
                }

                int dot1 = Loc.Dot(cardinal.GetLoc(), loc);
                int dot2 = Loc.Dot(diagonal.GetLoc(), loc);

                // the length of the diagonal vector is 2
                // to get a proper comparison:
                // square both dot products, divide the diagonal by 2
                if (dot1 * dot1 >= dot2 * dot2 / 2)
                    return cardinal;
                else
                    return diagonal;
            }
            else if (loc.X < 0)
            {
                // Compare dot products to find the vector this one is closest to.
                Dir8 cardinal, diagonal;
                if (loc.Y >= -loc.X)
                {
                    // X is negative; flip X
                    // Down-DownLeft
                    cardinal = Dir8.Down;
                    diagonal = Dir8.DownLeft;
                }
                else if (loc.Y <= loc.X)
                {
                    // X and Y both negative
                    // Up-UpLeft
                    cardinal = Dir8.Up;
                    diagonal = Dir8.UpLeft;
                }
                else if (loc.Y > 0)
                {
                    // Left-DownLeft
                    cardinal = Dir8.Left;
                    diagonal = Dir8.DownLeft;
                }
                else if (loc.Y < 0)
                {
                    // Left-UpLeft
                    cardinal = Dir8.Left;
                    diagonal = Dir8.UpLeft;
                }
                else
                {
                    return Dir8.Left;
                }

                int dot1 = Loc.Dot(cardinal.GetLoc(), loc);
                int dot2 = Loc.Dot(diagonal.GetLoc(), loc);

                // the length of the diagonal vector is 2
                // to get a proper comparison:
                // square both dot products, divide the diagonal by 2
                return dot1 * dot1 >= dot2 * dot2 / 2 ? cardinal : diagonal;
            }
            else
            {
                if (loc.Y > 0)
                    return Dir8.Down;
                else if (loc.Y < 0)
                    return Dir8.Up;
                else
                    return Dir8.None;
            }
        }

        public static Dir4 Rotate(this Dir4 dir, int n)
        {
            if (!dir.Validate())
                throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            if (dir == Dir4.None)
                return Dir4.None;
            return (Dir4)(((int)dir + n) & (DIR4_COUNT - 1));
        }

        public static Dir8 Rotate(this Dir8 dir, int n)
        {
            if (!dir.Validate())
                throw new ArgumentOutOfRangeException(nameof(dir), dir, "Invalid enum value.");
            if (dir == Dir8.None)
                return Dir8.None;
            return (Dir8)(((int)dir + n) & (DIR8_COUNT - 1));
        }

        public static Dir4 AddAngles(Dir4 dir1, Dir4 dir2)
        {
            // dir1 is validated by Dir4.Rotate
            if (!dir2.Validate())
                throw new ArgumentOutOfRangeException(nameof(dir2), dir2, "Invalid enum value.");
            if (dir1 == Dir4.None || dir2 == Dir4.None)
                return Dir4.None;
            return dir1.Rotate((int)dir2);
        }

        public static Dir8 AddAngles(Dir8 dir1, Dir8 dir2)
        {
            // dir1 is validated by Dir8.Rotate
            if (!dir2.Validate())
                throw new ArgumentOutOfRangeException(nameof(dir2), dir2, "Invalid enum value.");
            if (dir1 == Dir8.None || dir2 == Dir8.None)
                return Dir8.None;
            return dir1.Rotate((int)dir2);
        }

        public static Loc CreateLoc(this Axis4 axis, int scalar, int orth)
        {
            switch (axis)
            {
                case Axis4.Vert:
                    return new Loc(orth, scalar);
                case Axis4.Horiz:
                    return new Loc(scalar, orth);
                case Axis4.None:
                    throw new ArgumentException($"Cannot create {nameof(Loc)} from axis {nameof(Axis4.None)}.", nameof(axis));
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, "Invalid enum value.");
            }
        }

        public static Dir4 GetDir(this Axis4 axis, int scalar)
        {
            if (scalar == 0 & axis.Validate())
            {
                return Dir4.None;
            }
            else
            {
                switch (axis)
                {
                    case Axis4.None:
                        return Dir4.None;
                    case Axis4.Horiz:
                        return scalar < 0 ? Dir4.Left : Dir4.Right;
                    case Axis4.Vert:
                        return scalar < 0 ? Dir4.Up : Dir4.Down;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(axis), axis, "Invalid enum value.");
                }
            }
        }

        public static Dir8 GetDir(this Axis8 axis, int scalar)
        {
            if (scalar == 0 & axis.Validate())
            {
                return Dir8.None;
            }
            else
            {
                switch (axis)
                {
                    case Axis8.None:
                        return Dir8.None;
                    case Axis8.Horiz:
                        return scalar < 0 ? Dir8.Left : Dir8.Right;
                    case Axis8.Vert:
                        return scalar < 0 ? Dir8.Up : Dir8.Down;
                    case Axis8.DiagBack:
                        return scalar < 0 ? Dir8.UpLeft : Dir8.DownRight;
                    case Axis8.DiagForth:
                        return scalar < 0 ? Dir8.DownLeft : Dir8.UpRight;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(axis), axis, "Invalid enum value.");
                }
            }
        }

        public static int GetScalar(this Loc loc, Axis4 axis)
        {
            switch (axis)
            {
                case Axis4.Vert:
                    return loc.Y;
                case Axis4.Horiz:
                    return loc.X;
                case Axis4.None:
                    throw new ArgumentException($"Cannot get scalar on axis {nameof(Axis4.None)}.", nameof(axis));
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, "Invalid enum value.");
            }
        }
    }
}
