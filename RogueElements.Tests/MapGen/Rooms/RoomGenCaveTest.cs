using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;

namespace RogueElements.Tests
{
    [TestFixture]
    public class RoomGenCaveTest
    {

        //TODO: [Test]
        public void ProposeSize()
        {
            throw new NotImplementedException();
        }

        //TODO: [Test]
        public void DrawOnMap()
        {
            throw new NotImplementedException();
        }

        //TODO: [Test]
        public void PrepareFulfillableBorders()
        {
            throw new NotImplementedException();
        }
    }


    public class TestRoomGenCave<T> : RoomGenCave<T> where T : ITiledGenContext
    {
        public Dictionary<Dir4, bool[]> PublicFulfillableBorder { get { return fulfillableBorder; } }
    }
}
