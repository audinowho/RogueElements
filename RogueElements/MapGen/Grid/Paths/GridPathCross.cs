// <copyright file="GridPathCross.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// Creates a grid plan made up of a center room and halls and rooms extending off in the four cardinal directions.
    /// For best results, it is recommended to make grid height and width odd numbers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class GridPathCross<T> : GridPathStartStepGeneric<T>
        where T : class, IRoomGridGenContext
    {
        public GridPathCross()
            : base()
        {
        }

        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            // always clear before trying
            floorPlan.Clear();

            int middleWidth = floorPlan.GridWidth / 2;
            int middleHeight = floorPlan.GridHeight / 2;

            // Create first room in center
            Loc sourceRoom = new Loc(middleWidth, middleHeight);
            floorPlan.AddRoom(sourceRoom, this.GenericRooms.Pick(rand), this.RoomComponents.Clone());
            floorPlan.SetHall(new LocRay4(sourceRoom, Dir4.Left), this.GenericHalls.Pick(rand), this.HallComponents.Clone());
            floorPlan.SetHall(new LocRay4(sourceRoom, Dir4.Up), this.GenericHalls.Pick(rand), this.HallComponents.Clone());

            // Create horizontal rooms extending from the center
            for (int x = 0; x < floorPlan.GridWidth; x++)
            {
                if (x != middleWidth)
                {
                    Loc currentRoom = new Loc(x, middleHeight);
                    floorPlan.AddRoom(currentRoom, this.GenericRooms.Pick(rand), this.RoomComponents.Clone());
                    if (x != 0)
                    {
                        floorPlan.SetHall(new LocRay4(currentRoom, Dir4.Left), this.GenericHalls.Pick(rand), this.HallComponents.Clone());
                    }
                }
            }

            // Create vertical rooms extending from the center
            for (int y = 0; y < floorPlan.GridHeight; y++)
            {
                if (y != middleHeight)
                {
                    Loc currentRoom = new Loc(middleWidth, y);
                    floorPlan.AddRoom(currentRoom, this.GenericRooms.Pick(rand), this.RoomComponents.Clone());
                    if (y != 0)
                    {
                        floorPlan.SetHall(new LocRay4(currentRoom, Dir4.Up), this.GenericHalls.Pick(rand), this.HallComponents.Clone());
                    }
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0}", this.GetType().GetFormattedTypeName());
        }
    }
}