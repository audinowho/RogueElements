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

            TestGridFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);

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
