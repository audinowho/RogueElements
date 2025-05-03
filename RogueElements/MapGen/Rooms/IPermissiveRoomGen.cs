// <copyright file="IPermissiveRoomGen.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RogueElements
{
    public interface IPermissiveRoomGen<TTile> : IRoomGen<TTile>
        where TTile : ITile<TTile>
    {
    }

    public interface ISizedRoomGen<TTile> : IRoomGen<TTile>
        where TTile : ITile<TTile>
    {
        RandRange Width { get; set; }

        RandRange Height { get; set; }
    }
}