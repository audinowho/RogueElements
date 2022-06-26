// <copyright file="TestGridFloorPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace RogueElements.Tests
{
    public class TestGridFloorPlan : GridPlan
    {
        public List<GridRoomPlan> PublicArrayRooms => this.ArrayRooms;

        public int[][] PublicRooms => this.Rooms;

        public GridHallGroup[][] PublicVHalls => this.VHalls;

        public GridHallGroup[][] PublicHHalls => this.HHalls;

        public static void CompareFloorPlans(TestGridFloorPlan floorPlan, TestGridFloorPlan compareFloorPlan)
        {
            // check the rooms
            Assert.That(floorPlan.RoomCount, Is.EqualTo(compareFloorPlan.RoomCount));
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                GridRoomPlan plan = floorPlan.GetRoomPlan(ii);
                GridRoomPlan comparePlan = compareFloorPlan.GetRoomPlan(ii);
                Assert.That(plan.RoomGen, Is.EqualTo(comparePlan.RoomGen));
                Assert.That(plan.Bounds, Is.EqualTo(comparePlan.Bounds));
                Assert.That(plan.Components, Is.EquivalentTo(comparePlan.Components));
            }

            // check positions
            Assert.That(floorPlan.PublicRooms, Is.EqualTo(compareFloorPlan.PublicRooms));
            Assert.That(floorPlan.PublicVHalls.Length, Is.EqualTo(compareFloorPlan.PublicVHalls.Length));
            for (int xx = 0; xx < floorPlan.PublicVHalls.Length; xx++)
            {
                Assert.That(floorPlan.PublicVHalls[xx].Length, Is.EqualTo(compareFloorPlan.PublicVHalls[xx].Length));
                for (int yy = 0; yy < floorPlan.PublicVHalls[xx].Length; yy++)
                {
                    Assert.That(floorPlan.PublicVHalls[xx][yy].HallParts.Count, Is.EqualTo(compareFloorPlan.PublicVHalls[xx][yy].HallParts.Count));
                    for (int nn = 0; nn < floorPlan.PublicVHalls[xx][yy].HallParts.Count; nn++)
                    {
                        Assert.That(floorPlan.PublicVHalls[xx][yy].HallParts[nn].RoomGen, Is.EqualTo(compareFloorPlan.PublicVHalls[xx][yy].HallParts[nn].RoomGen));
                        Assert.That(floorPlan.PublicVHalls[xx][yy].HallParts[nn].Components, Is.EquivalentTo(compareFloorPlan.PublicVHalls[xx][yy].HallParts[nn].Components));
                    }
                }
            }

            Assert.That(floorPlan.PublicHHalls.Length, Is.EqualTo(compareFloorPlan.PublicHHalls.Length));
            for (int xx = 0; xx < floorPlan.PublicHHalls.Length; xx++)
            {
                Assert.That(floorPlan.PublicHHalls[xx].Length, Is.EqualTo(compareFloorPlan.PublicHHalls[xx].Length));
                for (int yy = 0; yy < floorPlan.PublicVHalls[xx].Length; yy++)
                {
                    Assert.That(floorPlan.PublicHHalls[xx][yy].HallParts.Count, Is.EqualTo(compareFloorPlan.PublicHHalls[xx][yy].HallParts.Count));
                    for (int nn = 0; nn < floorPlan.PublicHHalls[xx][yy].HallParts.Count; nn++)
                    {
                        Assert.That(floorPlan.PublicHHalls[xx][yy].HallParts[nn].RoomGen, Is.EqualTo(compareFloorPlan.PublicHHalls[xx][yy].HallParts[nn].RoomGen));
                        Assert.That(floorPlan.PublicHHalls[xx][yy].HallParts[nn].Components, Is.EquivalentTo(compareFloorPlan.PublicHHalls[xx][yy].HallParts[nn].Components));
                    }
                }
            }
        }

        public static TestGridFloorPlan InitGridToContext(string[] inGrid)
        {
            return InitGridToContext(inGrid, 0, 0);
        }

        public static TestGridFloorPlan InitGridToContext(string[] inGrid, int widthPerCell, int heightPerCell)
        {
            // transposes
            bool wrap;
            if (inGrid.Length % 2 == 0 && inGrid[0].Length % 2 == 0)
                wrap = true;
            else if (inGrid.Length % 2 == 1 && inGrid[0].Length % 2 == 1)
                wrap = false;
            else
                throw new ArgumentException("Bad input grid!");
            var floorPlan = new TestGridFloorPlan();
            floorPlan.InitSize((inGrid[0].Length + 1) / 2, (inGrid.Length + 1) / 2, widthPerCell, heightPerCell, 1, wrap);
            GridRoomPlan[] addedRooms = new GridRoomPlan[26];

            for (int xx = 0; xx < inGrid[0].Length; xx++)
            {
                for (int yy = 0; yy < inGrid.Length; yy++)
                {
                    char val = inGrid[yy][xx];
                    int x = xx / 2;
                    int y = yy / 2;

                    // rooms
                    if (xx % 2 == 0 && yy % 2 == 0)
                    {
                        if (val >= 'A' && val <= 'Z')
                        {
                            floorPlan.Rooms[x][y] = val - 'A';
                            if (addedRooms[val - 'A'] == null)
                                addedRooms[val - 'A'] = new GridRoomPlan(new Rect(x, y, 1, 1), new TestGridRoomGen(val), new ComponentCollection());
                            GridRoomPlan roomPlan = addedRooms[val - 'A'];

                            if (roomPlan.Bounds.End.X < x)
                                roomPlan.Bounds = new Rect(new Loc(x, roomPlan.Bounds.Y), new Loc(floorPlan.GridWidth - x + roomPlan.Bounds.Size.X, roomPlan.Bounds.Size.Y));
                            else if (roomPlan.Bounds.End.X == x)
                                roomPlan.Bounds = new Rect(roomPlan.Bounds.Start, roomPlan.Bounds.Size + new Loc(1, 0));

                            if (roomPlan.Bounds.End.Y < y)
                                roomPlan.Bounds = new Rect(new Loc(roomPlan.Bounds.X, y), new Loc(roomPlan.Bounds.Size.X, floorPlan.GridHeight - y + roomPlan.Bounds.Size.Y));
                            else if (roomPlan.Bounds.End.Y == y)
                                roomPlan.Bounds = new Rect(roomPlan.Bounds.Start, roomPlan.Bounds.Size + new Loc(0, 1));
                        }
                        else if (val == '0')
                        {
                            floorPlan.Rooms[x][y] = -1;
                        }
                        else
                        {
                            throw new ArgumentException($"Bad input grid val at room {x},{y}!");
                        }
                    }
                    else if (xx % 2 == 0 && yy % 2 == 1)
                    {
                        // vhalls
                        if (val == '#')
                            floorPlan.VHalls[x][y].SetHall(new GridHallPlan(new TestGridRoomGen(), new ComponentCollection()));
                        else if (val == '.')
                            floorPlan.VHalls[x][y].SetHall(null);
                        else
                            throw new ArgumentException($"Bad input grid val at vertical hall {x},{y}!");
                    }
                    else if (xx % 2 == 1 && yy % 2 == 0)
                    {
                        // hhalls
                        if (val == '#')
                            floorPlan.HHalls[x][y].SetHall(new GridHallPlan(new TestGridRoomGen(), new ComponentCollection()));
                        else if (val == '.')
                            floorPlan.HHalls[x][y].SetHall(null);
                        else
                            throw new ArgumentException($"Bad input grid val at horizontal hall {x},{y}!");
                    }
                    else if (xx % 2 == 1 && yy % 2 == 1)
                    {
                        // blank
                        if (val != ' ')
                            throw new ArgumentException("Bad input grid val at blank zone!");
                    }
                }
            }

            for (int ii = 0; ii < 26; ii++)
            {
                if (addedRooms[ii] != null)
                    floorPlan.ArrayRooms.Add(addedRooms[ii]);
            }

            return floorPlan;
        }
    }
}