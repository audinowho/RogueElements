using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;

namespace RogueElements.Tests
{
    [TestFixture]
    public class GridPathStartStepTest
    {
        [Test]
        public void CreateErrorPath()
        {
            string[] inGrid = { "0.0",
                                ". .",
                                "0.0" };

            string[] outGrid = { "A.0",
                                 ". .",
                                 "0.0" };
            
            Mock<GridPathStartStepGeneric<IGridPathTestContext>> pathGen = new Mock<GridPathStartStepGeneric<IGridPathTestContext>>();
            pathGen.CallBase = true;

            Moq.Language.ISetupSequentialResult<RoomGen<IGridPathTestContext>> defaultSeq = pathGen.SetupSequence(p => p.GetDefaultGen());
            defaultSeq = defaultSeq.Returns(new TestGridRoomGen('A'));

            Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            pathGen.Object.GenericHalls = mockHalls.Object;
            Mock<IRandPicker<RoomGen<IGridPathTestContext>>> mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            pathGen.Object.GenericRooms = mockRooms.Object;

            Mock<IRandom> mockRand = new Mock<IRandom>(MockBehavior.Strict);
            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);
            TestGridFloorPlan compareFloorPlan = TestGridFloorPlan.InitGridToContext(outGrid);

            pathGen.Object.CreateErrorPath(mockRand.Object, floorPlan);

            //check the rooms
            Assert.That(floorPlan.RoomCount, Is.EqualTo(compareFloorPlan.RoomCount));
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                GridRoomPlan plan = floorPlan.GetRoomPlan(ii);
                GridRoomPlan comparePlan = compareFloorPlan.GetRoomPlan(ii);
                Assert.That(plan.RoomGen, Is.EqualTo(comparePlan.RoomGen));
                Assert.That(plan.Bounds, Is.EqualTo(comparePlan.Bounds));
            }

            Assert.That(floorPlan.PublicRooms, Is.EqualTo(compareFloorPlan.PublicRooms));
            Assert.That(floorPlan.PublicVHalls, Is.EqualTo(compareFloorPlan.PublicVHalls));
            Assert.That(floorPlan.PublicHHalls, Is.EqualTo(compareFloorPlan.PublicHHalls));

            pathGen.Verify(p => p.GetDefaultGen(), Times.Exactly(1));
        }

        //TODO: [Test]
        public void RollRatio()
        {
            throw new NotImplementedException();
        }

        //TODO: [Test]
        public void SafeAddHall()
        {
            throw new NotImplementedException();
        }

        //TODO: [Test]
        public void SelectSpecialRooms()
        {
            throw new NotImplementedException();
        }
    }
}
