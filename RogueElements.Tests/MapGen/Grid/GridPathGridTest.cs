// <copyright file="GridPathGridTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class GridPathGridTest
    {
        [Test]
        public void CreateError()
        {
            string[] inGrid =
            {
                "0.0.0",
                ". . .",
                "0.0.0",
            };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            var pathGen = new GridPathGrid<IGridPathTestContext, TestTile>();

            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            pathGen.GenericHalls = mockHalls.Object;
            var mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            pathGen.GenericRooms = mockRooms.Object;

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);

            Assert.Throws<InvalidOperationException>(() => { pathGen.ApplyToPath(testRand.Object, floorPlan); });
        }

        [Test]
        [Ignore("TODO")]
        public void CreatePathSize()
        {
            // min size
            // bigger size
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void CreatePathRoom()
        {
            // 0 ratio
            // 100 ratio
            // some ratio
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void CreatePathHall()
        {
            // 0 ratio
            // 100 ratio
            // some ratio
            throw new NotImplementedException();
        }
    }
}
