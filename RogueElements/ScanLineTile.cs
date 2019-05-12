// <copyright file="ScanLineTile.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public class ScanLineTile
    {

        public int MinX;
        public int MaxX;
        public int Y;
        public DirV Dir;
        public bool GoLeft;
        public bool GoRight;

        public ScanLineTile(int minX, int maxX, int y, DirV dir, bool goLeft, bool goRight)
        {
            MinX = minX;
            MaxX = maxX;
            Y = y;
            Dir = dir;
            GoLeft = goLeft;
            GoRight = goRight;
        }
    }
}
