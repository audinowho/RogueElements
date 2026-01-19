// <copyright file="IRoomGenDefault.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Marker interface for the default room generator.
    /// Used to identify rooms that should be treated as default/placeholder rooms.
    /// </summary>
    public interface IRoomGenDefault
    {
    }

    /// <summary>
    /// Generates a one-tile room.
    /// Serves as the simplest possible room generator and default placeholder.
    /// </summary>
    /// <typeparam name="T">The type of map context that supports tiled generation.</typeparam>
    [Serializable]
    public class RoomGenDefault<T> : PermissiveRoomGen<T>, IRoomGenDefault
        where T : ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomGenDefault{T}"/> class.
        /// </summary>
        public RoomGenDefault()
        {
        }

        /// <inheritdoc/>
        public override RoomGen<T> Copy() => new RoomGenDefault<T>();

        /// <inheritdoc/>
        public override Loc ProposeSize(IRandom rand)
        {
            return Loc.One;
        }

        /// <inheritdoc/>
        public override void DrawOnMap(T map)
        {
            this.DrawMapDefault(map);
        }
    }
}
