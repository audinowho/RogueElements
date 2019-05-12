// <copyright file="RoomGenDefault.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class RoomGenDefault<T> : PermissiveRoomGen<T> where T : ITiledGenContext
    {
        public RoomGenDefault() { }
        public override RoomGen<T> Copy() { return new RoomGenDefault<T>(); }


        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(1);
        }

        public override void DrawOnMap(T map)
        {
            DrawMapDefault(map);
        }
    }

}
