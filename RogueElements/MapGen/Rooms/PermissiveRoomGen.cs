// <copyright file="PermissiveRoomGen.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// Subclass of RoomGen that can fulfill any combination of paths leading into it.
    /// All border tiles are marked as fulfillable, allowing connections from any direction.
    /// </summary>
    /// <typeparam name="T">The type of map context that supports tiled generation.</typeparam>
    [Serializable]
    public abstract class PermissiveRoomGen<T> : RoomGen<T>, IPermissiveRoomGen
        where T : ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissiveRoomGen{T}"/> class.
        /// </summary>
        protected PermissiveRoomGen()
        {
        }

        /// <inheritdoc/>
        protected override void PrepareFulfillableBorders(IRandom rand)
        {
            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                for (int jj = 0; jj < this.FulfillableBorder[dir].Length; jj++)
                    this.FulfillableBorder[dir][jj] = true;
            }
        }
    }
}
