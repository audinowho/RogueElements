// <copyright file="PermissiveRoomGen.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// Subclass of RoomGen that can fulfill any combination of paths leading into it.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    [Serializable]
    public abstract class PermissiveRoomGen<TGenContext, TTile> : RoomGen<TGenContext, TTile>, IPermissiveRoomGen<TTile>
        where TGenContext : ITiledGenContext<TTile>
        where TTile : ITile<TTile>
    {
        protected PermissiveRoomGen()
        {
        }

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
