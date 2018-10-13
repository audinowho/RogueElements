using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;

namespace RogueElements.Tests
{
    [TestFixture]
    public class PermissiveRoomGenTest
    {
        

        [Test]
        public void PrepareSize()
        {
            //verify all fulfillableborders set to true
            //as well as bordertofulfill set to correct sizes
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            TestPermissiveRoomGen<ITiledGenContext> roomGen = new TestPermissiveRoomGen<ITiledGenContext>();
            roomGen.PrepareSize(testRand.Object, new Loc(2, 3));


            bool[][] expectedFulfillable = new bool[4][];
            expectedFulfillable[0] = new bool[2];
            expectedFulfillable[0][0] = true;
            expectedFulfillable[0][1] = true;
            expectedFulfillable[1] = new bool[3];
            expectedFulfillable[1][0] = true;
            expectedFulfillable[1][1] = true;
            expectedFulfillable[1][2] = true;
            expectedFulfillable[2] = new bool[2];
            expectedFulfillable[2][0] = true;
            expectedFulfillable[2][1] = true;
            expectedFulfillable[3] = new bool[3];
            expectedFulfillable[3][0] = true;
            expectedFulfillable[3][1] = true;
            expectedFulfillable[3][2] = true;

            bool[][] expectedToFulfill = new bool[4][];
            expectedToFulfill[0] = new bool[2];
            expectedToFulfill[1] = new bool[3];
            expectedToFulfill[2] = new bool[2];
            expectedToFulfill[3] = new bool[3];

            Assert.That(roomGen.PublicFulfillableBorder, Is.EqualTo(expectedFulfillable));
            Assert.That(roomGen.PublicBorderToFulfill, Is.EqualTo(expectedToFulfill));
        }

    }



    public class TestPermissiveRoomGen<T> : PermissiveRoomGen<T> where T : ITiledGenContext
    {
        public bool[][] PublicFulfillableBorder { get { return fulfillableBorder; } }
        public bool[][] PublicOpenedBorder { get { return openedBorder; } }
        public bool[][] PublicBorderToFulfill { get { return borderToFulfill; } }
        public override Loc ProposeSize(IRandom rand) { return new Loc(); }
        public override void DrawOnMap(T map) { }

        public void PublicPrepareFulfillableBorders(IRandom rand)
        {
            PrepareFulfillableBorders(rand);
        }
        
    }
    
}
