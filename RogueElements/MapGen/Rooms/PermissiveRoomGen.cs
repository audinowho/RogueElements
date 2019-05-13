// <copyright file="PermissiveRoomGen.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Subclass of RoomGen that can deal with any combination of paths leading into it.  Its RequestedBorder can be disobeyed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class PermissiveRoomGen<T> : RoomGen<T>, IPermissiveRoomGen where T : ITiledGenContext
    {

        protected PermissiveRoomGen() { }

        protected override void PrepareFulfillableBorders(IRandom rand)
        {
            for (int ii = 0; ii < 4; ii++)
            {
                for(int jj = 0; jj < fulfillableBorder[ii].Length; jj++)
                    fulfillableBorder[ii][jj] = true;
            }
        }

    }

    public interface IPermissiveRoomGen : IRoomGen
    {
    }

}
