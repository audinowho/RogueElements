// <copyright file="IRoomGen.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    public interface IRoomGen
    {
        Rect Draw { get; }

        void AskBorderRange(IntRange range, Dir4 dir);

        void AskBorderFromRoom(Rect sourceDraw, Func<Dir4, int, bool> borderQuery, Dir4 dir);

        bool GetOpenedBorder(Dir4 dir, int index);

        bool GetFulfillableBorder(Dir4 dir, int index);

        Loc ProposeSize(IRandom rand);

        void PrepareSize(IRandom rand, Loc size);

        void SetLoc(Loc loc);

        void DrawOnMap(ITiledGenContext map);

        IRoomGen Copy();
    }
}