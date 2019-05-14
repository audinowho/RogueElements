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
    public enum DirV
    {
        None = -1,
        Down = 0,
        Up = 1,
    }

    public enum DirH
    {
        None = -1,
        Left = 0,
        Right = 1,
    }

    public enum Dir4
    {
        None = -1,
        Down = 0,
        Left = 1,
        Up = 2,
        Right = 3,
    }

    public enum Dir8
    {
        None = -1,
        Down = 0,
        DownLeft = 1,
        Left = 2,
        UpLeft = 3,
        Up = 4,
        UpRight = 5,
        Right = 6,
        DownRight = 7,
    }

    public enum Axis4
    {
        None = -1,
        Vert = 0,
        Horiz = 1,
    }

    public enum Axis8
    {
        None = -1,
        Vert = 0,
        DiagForth = 1,
        Horiz = 2,
        DiagBack = 3,
    }
}
