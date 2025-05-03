// <copyright file="GridPathStartStepTest.cs" company="Audino">
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
    public class GridPathStartStepTest
    {
        [Test]
        public void CreateErrorPath()
        {
            string[] inGrid =
            {
                "0.0",
                ". .",
                "0.0",
            };

            string[] outGrid =
            {
                "A.0",
                ". .",
                "0.0",
            };

            var pathGen = new Mock<GridPathStartStepGeneric<IGridPathTestContext, TestTile>> { CallBase = true };

            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext, TestTile>> defaultSeq = pathGen.SetupSequence(p => p.GetDefaultGen());
            defaultSeq = defaultSeq.Returns(new TestGridRoomGen('A'));

            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            pathGen.Object.GenericHalls = mockHalls.Object;
            var mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext, TestTile>>>(MockBehavior.Strict);
            pathGen.Object.GenericRooms = mockRooms.Object;

            Mock<IRandom> mockRand = new Mock<IRandom>(MockBehavior.Strict);
            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            pathGen.Object.CreateErrorPath(mockRand.Object, floorPlan);

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

            pathGen.Verify(p => p.GetDefaultGen(), Times.Exactly(1));
        }

        [Test]
        [Ignore("TODO")]
        public void RollRatio()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void SafeAddHall()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void SelectSpecialRooms()
        {
            throw new NotImplementedException();
        }
    }
}
