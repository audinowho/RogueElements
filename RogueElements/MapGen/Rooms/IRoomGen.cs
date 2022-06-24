// <copyright file="IRoomGen.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RogueElements
{
    public interface IRoomGen
    {
        Rect Draw { get; }

        void AskWithOpenedBorder(IRoomGen sourceRoom, Dir4 dir);

        void AskWithFulfillableBorder(IRoomGen sourceRoom, Dir4 dir);

        void AskBorderRange(IntRange range, Dir4 dir);

        bool GetOpenedBorder(Dir4 dir, int index);

        bool GetFulfillableBorder(Dir4 dir, int index);

        Loc ProposeSize(IRandom rand);

        void PrepareSize(IRandom rand, Loc size);

        void SetLoc(Loc loc);

        void DrawOnMap(ITiledGenContext map);

        IRoomGen Copy();
    }
}