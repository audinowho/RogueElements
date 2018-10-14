using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;

namespace RogueElements.Tests
{
    [TestFixture]
    public class RoomGenSpecificTest
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


    public class TestRoomGenSpecific<T> : RoomGenSpecific<T> where T : ITiledGenContext
    {
        public bool[][] PublicFulfillableBorder { get { return fulfillableBorder; } }
    }
}
