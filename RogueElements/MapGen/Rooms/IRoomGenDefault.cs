// <copyright file="IRoomGenDefault.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IRoomGenDefault
    {
    }

    /// <summary>
    /// Generates a one-tile room.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    [Serializable]
    public class RoomGenDefault<TGenContext, TTile> : PermissiveRoomGen<TGenContext, TTile>, IRoomGenDefault
        where TGenContext : ITiledGenContext<TTile>
        where TTile : ITile<TTile>
    {
        public RoomGenDefault()
        {
        }

        public override RoomGen<TGenContext, TTile> Copy() => new RoomGenDefault<TGenContext, TTile>();

        public override Loc ProposeSize(IRandom rand)
        {
            return Loc.One;
        }

        public override void DrawOnMap(TGenContext map)
        {
            this.DrawMapDefault(map);
        }
    }
}
