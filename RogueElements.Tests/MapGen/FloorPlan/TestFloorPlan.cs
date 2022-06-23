// <copyright file="TestFloorPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace RogueElements.Tests
{
    public class TestFloorPlan : FloorPlan
    {
        public List<FloorRoomPlan> PublicRooms => this.Rooms;

        public List<FloorHallPlan> PublicHalls => this.Halls;

        public static void CompareFloorPlans(TestFloorPlan floorPlan, TestFloorPlan compareFloorPlan)
        {
            Assert.That(floorPlan.DrawRect, Is.EqualTo(compareFloorPlan.DrawRect));
            Assert.That(floorPlan.RoomCount, Is.EqualTo(compareFloorPlan.RoomCount));
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                FloorRoomPlan plan = floorPlan.PublicRooms[ii];
                FloorRoomPlan comparePlan = compareFloorPlan.PublicRooms[ii];
                Assert.That(plan.RoomGen, Is.EqualTo(comparePlan.RoomGen));
                Assert.That(plan.Adjacents, Is.EqualTo(comparePlan.Adjacents));
                Assert.That(plan.Components, Is.EquivalentTo(comparePlan.Components));
            }

            Assert.That(floorPlan.HallCount, Is.EqualTo(compareFloorPlan.HallCount));
            for (int ii = 0; ii < floorPlan.HallCount; ii++)
            {
                FloorHallPlan plan = floorPlan.PublicHalls[ii];
                FloorHallPlan comparePlan = compareFloorPlan.PublicHalls[ii];
                Assert.That(plan.RoomGen, Is.EqualTo(comparePlan.RoomGen));
                Assert.That(plan.Adjacents, Is.EqualTo(comparePlan.Adjacents));
                Assert.That(plan.Components, Is.EquivalentTo(comparePlan.Components));
            }
        }

        public static TestFloorPlan InitFloorToContext(Loc size, Rect[] rooms, Rect[] halls, Tuple<char, char>[] links)
        {
            var floorPlan = new TestFloorPlan();
            InitFloorToContext(floorPlan, new Rect(Loc.Zero, size), rooms, halls, links);
            return floorPlan;
        }

        public static TestFloorPlan InitFloorToContext(Rect rect, Rect[] rooms, Rect[] halls, Tuple<char, char>[] links)
        {
            var floorPlan = new TestFloorPlan();
            InitFloorToContext(floorPlan, rect, rooms, halls, links);
            return floorPlan;
        }

        public static Mock<TestFloorPlan> InitFloorToMockContext(Loc size, Rect[] rooms, Rect[] halls, Tuple<char, char>[] links)
        {
            var floorPlan = new Mock<TestFloorPlan> { CallBase = true };
            InitFloorToContext(floorPlan.Object, new Rect(Loc.Zero, size), rooms, halls, links);
            return floorPlan;
        }

        private static void InitFloorToContext(TestFloorPlan floorPlan, Rect rect, Rect[] rooms, Rect[] halls, Tuple<char, char>[] links)
        {
            floorPlan.InitRect(rect, false);

            // a quick way to set up rooms, halls, and connections
            // a list of rects for rooms, a list of rects for halls
            for (int ii = 0; ii < rooms.Length; ii++)
            {
                var gen = new TestFloorPlanGen((char)('A' + ii));
                gen.PrepareDraw(rooms[ii]);
                floorPlan.PublicRooms.Add(new FloorRoomPlan(gen, new ComponentCollection()));
            }

            for (int ii = 0; ii < halls.Length; ii++)
            {
                var gen = new TestFloorPlanGen((char)('a' + ii));
                gen.PrepareDraw(halls[ii]);
                floorPlan.PublicHalls.Add(new FloorHallPlan(gen, new ComponentCollection()));
            }

            // and finally a list of tuples that link rooms to rooms and halls to halls
            for (int ii = 0; ii < links.Length; ii++)
            {
                bool hall1 = links[ii].Item1 >= 'a';
                int index1 = hall1 ? links[ii].Item1 - 'a' : links[ii].Item1 - 'A';
                bool hall2 = links[ii].Item2 >= 'a';
                int index2 = hall2 ? links[ii].Item2 - 'a' : links[ii].Item2 - 'A';
                var link1 = new RoomHallIndex(index1, hall1);
                var link2 = new RoomHallIndex(index2, hall2);
                IFloorRoomPlan from1 = floorPlan.GetRoomHall(link1);
                IFloorRoomPlan from2 = floorPlan.GetRoomHall(link2);
                from1.Adjacents.Add(link2);
                from2.Adjacents.Add(link1);
            }
        }
    }
}