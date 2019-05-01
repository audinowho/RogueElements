using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements
{
    
    public static class DirExt
    {
        public static readonly DirH[] VALID_DIRH = { DirH.Left, DirH.Right };
        public static readonly DirV[] VALID_DIRV = { DirV.Down, DirV.Up };
        public static readonly Dir4[] VALID_DIR4 = { Dir4.Down, Dir4.Left, Dir4.Up, Dir4.Right };
        public static readonly Dir8[] VALID_DIR8 = { Dir8.Down, Dir8.DownLeft, Dir8.Left, Dir8.UpLeft, Dir8.Up, Dir8.UpRight, Dir8.Right, Dir8.DownRight };
        public static readonly Axis4[] VALID_AXIS4 = { Axis4.Vert, Axis4.Horiz };
        public static readonly Axis8[] VALID_AXIS8 = { Axis8.Vert, Axis8.DiagForth, Axis8.Horiz, Axis8.DiagBack };

        public static int ToWrappedInt(this Dir8 dir)
        {
            switch (dir)
            {
                case Dir8.None: return -1;
                case Dir8.Down: return 0;
                case Dir8.Left: return 1;
                case Dir8.Up: return 2;
                case Dir8.Right: return 3;
                case Dir8.DownLeft: return 4;
                case Dir8.UpLeft: return 5;
                case Dir8.UpRight: return 6;
                case Dir8.DownRight: return 7;
                default:
                    throw new ArgumentException("Invalid value to convert.");
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
                    throw new ArgumentException("Invalid value to convert.");
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
                    throw new ArgumentException("Invalid value to convert.");
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
                    throw new ArgumentException("Invalid value to convert.");
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
                    throw new ArgumentException("Invalid value to convert.");
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
                    throw new ArgumentException("Invalid value to convert.");
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
                    throw new ArgumentException("Invalid value to convert.");
            }
        }
        public static Dir8 ToWrappedDir8(this int n)
        {
            //526
            //1X3
            //407
            switch (n)
            {
                case -1: return Dir8.None;
                case 0: return Dir8.Down;
                case 1: return Dir8.Left;
                case 2: return Dir8.Up;
                case 3: return Dir8.Right;
                case 4: return Dir8.DownLeft;
                case 5: return Dir8.UpLeft;
                case 6: return Dir8.UpRight;
                case 7: return Dir8.DownRight;
                default:
                    throw new ArgumentException("Invalid value to convert.");
            }
        }
        public static Dir8 ToFocusedDir8(this int n)
        {
            //576
            //3X4
            //102
            switch (n)
            {
                case -1: return Dir8.None;
                case 0: return Dir8.Down;
                case 1: return Dir8.DownLeft;
                case 2: return Dir8.DownRight;
                case 3: return Dir8.Left;
                case 4: return Dir8.Right;
                case 5: return Dir8.UpLeft;
                case 6: return Dir8.UpRight;
                case 7: return Dir8.Up;
                default:
                    throw new ArgumentException("Invalid value to convert.");
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
                    throw new ArgumentException("Invalid value to convert.");
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
                    throw new ArgumentException("Invalid value to convert.");
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
                    throw new ArgumentException("Invalid value to convert.");
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
                    throw new ArgumentException("Invalid value to convert.");
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
                    throw new ArgumentException("Invalid value to convert.");
            }
        }

        public static bool IsDiagonal(this Dir8 dir)
        {
            Axis8 axis = dir.ToAxis();
            return (axis == Axis8.DiagBack || axis == Axis8.DiagForth);
        }

        public static Loc GetLoc(this DirH dir)
        {
            switch (dir)
            {
                case DirH.None:
                    return Loc.Zero;
                case DirH.Left:
                    return new Loc(-1, 0);
                case DirH.Right:
                    return new Loc(1, 0);
                default:
                    throw new ArgumentException("Invalid value to convert.");
            }
        }
        public static Loc GetLoc(this DirV dir)
        {
            switch (dir)
            {
                case DirV.None:
                    return Loc.Zero;
                case DirV.Down:
                    return new Loc(0, 1);
                case DirV.Up:
                    return new Loc(0, -1);
                default:
                    throw new ArgumentException("Invalid value to convert.");
            }
        }

        public static Loc GetLoc(this Dir4 dir)
        {
            switch (dir)
            {
                case Dir4.None:
                    return Loc.Zero;
                case Dir4.Down:
                    return new Loc(0, 1);
                case Dir4.Left:
                    return new Loc(-1, 0);
                case Dir4.Up:
                    return new Loc(0, -1);
                case Dir4.Right:
                    return new Loc(1, 0);
                default:
                    throw new ArgumentException("Invalid value to convert.");
            }
        }

        public static Loc GetLoc(this Dir8 dir)
        {
            switch (dir)
            {
                case Dir8.None:
                    return Loc.Zero;
                case Dir8.Down:
                    return new Loc(0, 1);
                case Dir8.DownLeft:
                    return new Loc(-1, 1);
                case Dir8.Left:
                    return new Loc(-1, 0);
                case Dir8.UpLeft:
                    return new Loc(-1, -1);
                case Dir8.Up:
                    return new Loc(0, -1);
                case Dir8.UpRight:
                    return new Loc(1, -1);
                case Dir8.Right:
                    return new Loc(1, 0);
                case Dir8.DownRight:
                    return new Loc(1, 1);
                default:
                    throw new ArgumentException("Invalid value to convert.");
            }
        }


        public static DirH Reverse(this DirH dir)
        {
            switch (dir)
            {
                case DirH.Left:
                    return DirH.Right;
                case DirH.Right:
                    return DirH.Left;
                case DirH.None:
                    return DirH.None;
                default:
                    throw new ArgumentException("Invalid value to convert.");
            }
        }
        public static DirV Reverse(this DirV dir)
        {
            switch (dir)
            {
                case DirV.Down:
                    return DirV.Up;
                case DirV.Up:
                    return DirV.Down;
                case DirV.None:
                    return DirV.None;
                default:
                    throw new ArgumentException("Invalid value to convert.");
            }
        }
        public static Dir4 Reverse(this Dir4 dir)
        {
            switch (dir)
            {
                case Dir4.Down:
                    return Dir4.Up;
                case Dir4.Left:
                    return Dir4.Right;
                case Dir4.Up:
                    return Dir4.Down;
                case Dir4.Right:
                    return Dir4.Left;
                case Dir4.None:
                    return Dir4.None;
                default:
                    throw new ArgumentException("Invalid value to convert.");
            }
        }
        public static Dir8 Reverse(this Dir8 dir)
        {
            switch (dir)
            {
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
                case Dir8.None:
                    return Dir8.None;
                default:
                    throw new ArgumentException("Invalid value to convert.");
            }
        }


        public static Dir8 Combine(DirH horiz, DirV vert)
        {
            if (vert == DirV.Down)
            {
                if (horiz == DirH.Right)
                    return Dir8.DownRight;
                else if (horiz == DirH.Left)
                    return Dir8.DownLeft;
                else if (horiz == DirH.None)
                    return Dir8.Down;
            }
            else if (vert == DirV.Up)
            {
                if (horiz == DirH.Right)
                    return Dir8.UpRight;
                else if (horiz == DirH.Left)
                    return Dir8.UpLeft;
                else if (horiz == DirH.None)
                    return Dir8.Up;
            }
            else if (vert == DirV.None)
            {
                if (horiz == DirH.Right)
                    return Dir8.Right;
                else if (horiz == DirH.Left)
                    return Dir8.Left;
                else if (horiz == DirH.None)
                    return Dir8.None;
            }
            throw new ArgumentException("Invalid value to combine.");
        }

        public static void Separate(this Dir8 dir, out DirH horiz, out DirV vert)
        {
            if (!Enum.IsDefined(typeof(Dir8), dir))
                throw new ArgumentException("Invalid value to separate.");
            switch (dir)
            {
                case Dir8.Down:
                case Dir8.DownLeft:
                case Dir8.DownRight:
                    {
                        vert = DirV.Down;
                    }
                    break;
                case Dir8.Up:
                case Dir8.UpLeft:
                case Dir8.UpRight:
                    {
                        vert = DirV.Up;
                    }
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
                    {
                        horiz = DirH.Left;
                    }
                    break;
                case Dir8.Right:
                case Dir8.UpRight:
                case Dir8.DownRight:
                    {
                        horiz = DirH.Right;
                    }
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
                else if (loc.Y <= -loc.X)//Y is negative; flip X
                    return Dir4.Up;
                else
                    return Dir4.Right;
            }
            else if (loc.X < 0)
            {
                if (loc.Y >= -loc.X)//X is negative; flip X
                    return Dir4.Down;
                else if (loc.Y <= loc.X)//X and Y both negative
                    return Dir4.Up;
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
                //Compare dot products to find the vector this one is closest to.
                Dir8 cardinal, diagonal;
                if (loc.Y >= loc.X)
                {
                    cardinal = Dir8.Down;
                    diagonal = Dir8.DownRight;
                }
                else if (loc.Y <= -loc.X)//Y is negative; flip X
                {
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
                else// == 0
                    return Dir8.Right;

                int dot1 = Loc.Dot(cardinal.GetLoc(), loc);
                int dot2 = Loc.Dot(diagonal.GetLoc(), loc);
                //the length of the diagonal vector is 2
                //to get a proper comparison:
                //square both dot products, divide the diagonal by 2
                if (dot1 * dot1 >= dot2 * dot2 / 2)
                    return cardinal;
                else
                    return diagonal;
            }
            else if (loc.X < 0)
            {
                //Compare dot products to find the vector this one is closest to.
                Dir8 cardinal, diagonal;
                if (loc.Y >= -loc.X)//X is negative; flip X
                {
                    //Down-DownLeft
                    cardinal = Dir8.Down;
                    diagonal = Dir8.DownLeft;
                }
                else if (loc.Y <= loc.X)//X and Y both negative
                {
                    //Up-UpLeft
                    cardinal = Dir8.Up;
                    diagonal = Dir8.UpLeft;
                }
                else if (loc.Y > 0)
                {
                    //Left-DownLeft
                    cardinal = Dir8.Left;
                    diagonal = Dir8.DownLeft;
                }
                else if (loc.Y < 0)
                {
                    //Left-UpLeft
                    cardinal = Dir8.Left;
                    diagonal = Dir8.UpLeft;
                }
                else
                    return Dir8.Left;

                int dot1 = Loc.Dot(cardinal.GetLoc(), loc);
                int dot2 = Loc.Dot(diagonal.GetLoc(), loc);
                //the length of the diagonal vector is 2
                //to get a proper comparison:
                //square both dot products, divide the diagonal by 2
                if (dot1 * dot1 >= dot2 * dot2 / 2)
                    return cardinal;
                else
                    return diagonal;
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

        public static Dir4 AddAngles(Dir4 dir1, Dir4 dir2)
        {
            if (!Enum.IsDefined(typeof(Dir4), dir1) || !Enum.IsDefined(typeof(Dir4), dir2))
                throw new ArgumentException("Invalid value to add.");
            if (dir1 == Dir4.None || dir2 == Dir4.None)
                return Dir4.None;
            return DirExt.VALID_DIR4[((int)dir1 + (int)dir2) % 4];
        }

        public static Dir8 AddAngles(Dir8 dir1, Dir8 dir2)
        {
            if (!Enum.IsDefined(typeof(Dir8), dir1) || !Enum.IsDefined(typeof(Dir8), dir2))
                throw new ArgumentException("Invalid value to add.");
            if (dir1 == Dir8.None || dir2 == Dir8.None)
                return Dir8.None;
            return DirExt.VALID_DIR8[((int)dir1 + (int)dir2) % 8];
        }
        
        public static Loc CreateLoc(this Axis4 axis, int scalar, int orth)
        {
            if (axis == Axis4.None || !Enum.IsDefined(typeof(Axis4), axis))
                throw new ArgumentException("Invalid value to convert.");
            if (axis == Axis4.Horiz)
                return new Loc(scalar, orth);
            else
                return new Loc(orth, scalar);
        }

        public static int GetScalar(this Loc loc, Axis4 axis)
        {
            if (axis == Axis4.None || !Enum.IsDefined(typeof(Axis4), axis))
                throw new ArgumentException("Invalid value to convert.");
            if (axis == Axis4.Horiz)
                return loc.X;
            else
                return loc.Y;
        }

    }
}
