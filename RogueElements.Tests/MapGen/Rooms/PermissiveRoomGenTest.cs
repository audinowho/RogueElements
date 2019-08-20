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
            var roomGen = new TestPermissiveRoomGen<ITiledGenContext>();
            roomGen.PrepareSize(testRand.Object, new Loc(2, 3));


            var expectedFulfillable = new Dictionary<Dir4, bool[]>();
            expectedFulfillable[Dir4.Down] = new bool[2];
            expectedFulfillable[Dir4.Down][0] = true;
            expectedFulfillable[Dir4.Down][1] = true;
            expectedFulfillable[Dir4.Left] = new bool[3];
            expectedFulfillable[Dir4.Left][0] = true;
            expectedFulfillable[Dir4.Left][1] = true;
            expectedFulfillable[Dir4.Left][2] = true;
            expectedFulfillable[Dir4.Up] = new bool[2];
            expectedFulfillable[Dir4.Up][0] = true;
            expectedFulfillable[Dir4.Up][1] = true;
            expectedFulfillable[Dir4.Right] = new bool[3];
            expectedFulfillable[Dir4.Right][0] = true;
            expectedFulfillable[Dir4.Right][1] = true;
            expectedFulfillable[Dir4.Right][2] = true;

            var expectedToFulfill = new Dictionary<Dir4, bool[]>();
            expectedToFulfill[Dir4.Down] = new bool[2];
            expectedToFulfill[Dir4.Left] = new bool[3];
            expectedToFulfill[Dir4.Up] = new bool[2];
            expectedToFulfill[Dir4.Right] = new bool[3];

            Assert.That(roomGen.PublicFulfillableBorder, Is.EqualTo(expectedFulfillable));
            Assert.That(roomGen.PublicBorderToFulfill, Is.EqualTo(expectedToFulfill));
        }

    }



    public class TestPermissiveRoomGen<T> : PermissiveRoomGen<T> where T : ITiledGenContext
    {
        public Dictionary<Dir4, bool[]> PublicFulfillableBorder { get { return FulfillableBorder; } }
        public Dictionary<Dir4, bool[]> PublicOpenedBorder { get { return OpenedBorder; } }
        public Dictionary<Dir4, bool[]> PublicBorderToFulfill { get { return BorderToFulfill; } }

        public override RoomGen<T> Copy() { return new TestPermissiveRoomGen<T>(); }

        public override Loc ProposeSize(IRandom rand) { return new Loc(); }
        public override void DrawOnMap(T map) { }

        public void PublicPrepareFulfillableBorders(IRandom rand)
        {
            PrepareFulfillableBorders(rand);
        }
        
    }
    
}
