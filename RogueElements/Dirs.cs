// <copyright file="Dirs.cs" company="Audino">
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
    /// Represents a vertical direction (up or down).
    /// </summary>
    public enum DirV
    {
        /// <summary>No direction.</summary>
        None = -1,

        /// <summary>Downward direction (positive Y).</summary>
        Down = 0,

        /// <summary>Upward direction (negative Y).</summary>
        Up = 1,
    }

    /// <summary>
    /// Represents a horizontal direction (left or right).
    /// </summary>
    public enum DirH
    {
        /// <summary>No direction.</summary>
        None = -1,

        /// <summary>Leftward direction (negative X).</summary>
        Left = 0,

        /// <summary>Rightward direction (positive X).</summary>
        Right = 1,
    }

    /// <summary>
    /// Represents one of four cardinal directions.
    /// </summary>
    public enum Dir4
    {
        /// <summary>No direction.</summary>
        None = -1,

        /// <summary>Downward direction (positive Y).</summary>
        Down = 0,

        /// <summary>Leftward direction (negative X).</summary>
        Left = 1,

        /// <summary>Upward direction (negative Y).</summary>
        Up = 2,

        /// <summary>Rightward direction (positive X).</summary>
        Right = 3,
    }

    /// <summary>
    /// Represents one of eight directions including diagonals.
    /// </summary>
    public enum Dir8
    {
        /// <summary>No direction.</summary>
        None = -1,

        /// <summary>Downward direction.</summary>
        Down = 0,

        /// <summary>Down-left diagonal direction.</summary>
        DownLeft = 1,

        /// <summary>Leftward direction.</summary>
        Left = 2,

        /// <summary>Up-left diagonal direction.</summary>
        UpLeft = 3,

        /// <summary>Upward direction.</summary>
        Up = 4,

        /// <summary>Up-right diagonal direction.</summary>
        UpRight = 5,

        /// <summary>Rightward direction.</summary>
        Right = 6,

        /// <summary>Down-right diagonal direction.</summary>
        DownRight = 7,
    }

    /// <summary>
    /// Represents one of two axes (vertical or horizontal).
    /// </summary>
    public enum Axis4
    {
        /// <summary>No axis.</summary>
        None = -1,

        /// <summary>Vertical axis (Y).</summary>
        Vert = 0,

        /// <summary>Horizontal axis (X).</summary>
        Horiz = 1,
    }

    /// <summary>
    /// Represents one of four axes including diagonals.
    /// </summary>
    public enum Axis8
    {
        /// <summary>No axis.</summary>
        None = -1,

        /// <summary>Vertical axis (Y).</summary>
        Vert = 0,

        /// <summary>Forward diagonal axis (bottom-left to top-right).</summary>
        DiagForth = 1,

        /// <summary>Horizontal axis (X).</summary>
        Horiz = 2,

        /// <summary>Backward diagonal axis (top-left to bottom-right).</summary>
        DiagBack = 3,
    }
}
