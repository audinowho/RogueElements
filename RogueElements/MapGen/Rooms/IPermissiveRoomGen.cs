// <copyright file="IPermissiveRoomGen.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RogueElements
{
    public interface IPermissiveRoomGen : IRoomGen
    {
    }

    public interface ISizedRoomGen : IRoomGen
    {
        RandRange Width { get; set; }

        RandRange Height { get; set; }
    }
}