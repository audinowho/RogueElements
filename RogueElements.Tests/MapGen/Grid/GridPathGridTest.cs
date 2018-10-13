using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;

namespace RogueElements.Tests
{
    [TestFixture]
    public class GridPathGridTest
    {
        [Test]
        public void CreateError()
        {
            string[] inGrid = { "0.0.0",
                                ". . .",
                                "0.0.0" };

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            GridPathGrid<IGridPathTestContext> pathGen = new GridPathGrid<IGridPathTestContext>();

            Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            pathGen.GenericHalls = mockHalls.Object;
            Mock<IRandPicker<RoomGen<IGridPathTestContext>>> mockRooms = new Mock<IRandPicker<RoomGen<IGridPathTestContext>>>(MockBehavior.Strict);
            pathGen.GenericRooms = mockRooms.Object;

            TestGridFloorPlan floorPlan = TestGridFloorPlan.InitGridToContext(inGrid);

            Assert.Throws<InvalidOperationException>(() => { pathGen.ApplyToPath(testRand.Object, floorPlan); });
        }

        //TODO: [Test]
        public void CreatePathSize()
        {
            //min size
            //bigger size
            throw new NotImplementedException();
        }

        //TODO: [Test]
        public void CreatePathRoom()
        {
            //0 ratio
            //100 ratio
            //some ratio
            throw new NotImplementedException();
        }

        //TODO: [Test]
        public void CreatePathHall()
        {
            //0 ratio
            //100 ratio
            //some ratio
            throw new NotImplementedException();
        }
    }
}
