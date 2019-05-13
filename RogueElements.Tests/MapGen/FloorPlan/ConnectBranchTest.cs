using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;

namespace RogueElements.Tests
{
    [TestFixture]
    public class ConnectBranchTest
    {
        [Test]
        public void ConnectSelf()
        {
            //A D
            //B-C
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2), new Rect(3, 5, 2, 3),
                            new Rect(7, 5, 2, 3), new Rect(7, 3, 2, 2) },
                new Rect[] { new Rect(5, 6, 2, 2) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'a'),
                                        new Tuple<char, char>('a', 'C'), new Tuple<char, char>('C', 'D') });
            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2), new Rect(3, 5, 2, 3),
                            new Rect(7, 5, 2, 3), new Rect(7, 3, 2, 2) },
                new Rect[] { new Rect(5, 6, 2, 2), new Rect(5, 3, 2, 2) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'a'),
                                        new Tuple<char, char>('a', 'C'), new Tuple<char, char>('C', 'D'),
                                        new Tuple<char, char>('A', 'b'), new Tuple<char, char>('b', 'D')});


            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var pathGen = new ConnectBranchStep<IFloorPlanTestContext> { ConnectPercent = 100 };

            Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestFloorPlanGen('b'));
            pathGen.GenericHalls = mockHalls.Object;
            
            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void ConnectOther()
        {
            //A D
            //B-CE
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2), new Rect(3, 5, 2, 3),
                            new Rect(7, 5, 2, 3), new Rect(7, 3, 2, 2), new Rect(9, 6, 2, 2) },
                new Rect[] { new Rect(5, 6, 2, 2) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'a'),
                                        new Tuple<char, char>('a', 'C'), new Tuple<char, char>('C', 'D'),
                                        new Tuple<char, char>('C', 'E')});
            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2), new Rect(3, 5, 2, 3),
                            new Rect(7, 5, 2, 3), new Rect(7, 3, 2, 2), new Rect(9, 6, 2, 2) },
                new Rect[] { new Rect(5, 6, 2, 2), new Rect(5, 3, 2, 2) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'a'),
                                        new Tuple<char, char>('a', 'C'), new Tuple<char, char>('C', 'D'),
                                        new Tuple<char, char>('C', 'E'),
                                        new Tuple<char, char>('A', 'b'), new Tuple<char, char>('b', 'D')});


            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var pathGen = new ConnectBranchStep<IFloorPlanTestContext> { ConnectPercent = 100 };

            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestFloorPlanGen('b'));
            pathGen.GenericHalls = mockHalls.Object;

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void ConnectNonArm()
        {
            //  E G
            //A D-F
            //B-C
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 4), new Rect(3, 7, 2, 3),
                            new Rect(7, 7, 2, 3), new Rect(7, 3, 2, 4), new Rect(7, 1, 2, 2),
                            new Rect(11, 3, 2, 4), new Rect(11, 1, 2, 2) },
                new Rect[] { new Rect(5, 8, 2, 2), new Rect(9, 4, 2, 2) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'a'),
                                        new Tuple<char, char>('a', 'C'),
                                        new Tuple<char, char>('C', 'D'), new Tuple<char, char>('D', 'E'),
                                        new Tuple<char, char>('D', 'b'),
                                        new Tuple<char, char>('b', 'F'), new Tuple<char, char>('F', 'G') });
            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 4), new Rect(3, 7, 2, 3),
                            new Rect(7, 7, 2, 3), new Rect(7, 3, 2, 4), new Rect(7, 1, 2, 2),
                            new Rect(11, 3, 2, 4), new Rect(11, 1, 2, 2) },
                new Rect[] { new Rect(5, 8, 2, 2), new Rect(9, 4, 2, 2),
                            new Rect(5, 3, 2, 4), new Rect(9, 1, 2, 2) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'a'),
                                        new Tuple<char, char>('a', 'C'),
                                        new Tuple<char, char>('C', 'D'), new Tuple<char, char>('D', 'E'),
                                        new Tuple<char, char>('D', 'b'),
                                        new Tuple<char, char>('b', 'F'), new Tuple<char, char>('F', 'G'),
                                        new Tuple<char, char>('A', 'c'), new Tuple<char, char>('c', 'D'),
                                        new Tuple<char, char>('E', 'd'), new Tuple<char, char>('d', 'G') });


            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var pathGen = new ConnectBranchStep<IFloorPlanTestContext> { ConnectPercent = 100 };

            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<PermissiveRoomGen<IFloorPlanTestContext>> hallSeq = mockHalls.SetupSequence(p => p.Pick(testRand.Object));
            hallSeq = hallSeq.Returns(new TestFloorPlanGen('c'));
            hallSeq = hallSeq.Returns(new TestFloorPlanGen('d'));
            pathGen.GenericHalls = mockHalls.Object;

            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }


        //TODO: [Test]
        public void ConnectComb(int randResult)
        {
            //A D F H
            //B-C-E-G
            throw new NotImplementedException();
        }

        [Test]
        public void ConnectFail()
        {
            //A-B-C-D-E
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2), new Rect(5, 3, 2, 2), new Rect(7, 3, 2, 2), new Rect(9, 3, 2, 2), new Rect(11, 3, 2, 2) },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'C'), new Tuple<char, char>('C', 'D'), new Tuple<char, char>('D', 'E') });
            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2), new Rect(5, 3, 2, 2), new Rect(7, 3, 2, 2), new Rect(9, 3, 2, 2), new Rect(11, 3, 2, 2) },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'C'), new Tuple<char, char>('C', 'D'), new Tuple<char, char>('D', 'E') });


            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var pathGen = new ConnectBranchStep<IFloorPlanTestContext>();
            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void GetBranchArmsSingle()
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2) },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            List<List<RoomHallIndex>> expectedArms = new List<List<RoomHallIndex>>();

            var pathGen = new ConnectBranchStep<IFloorPlanTestContext>();
            List<List<RoomHallIndex>> arms = pathGen.GetBranchArms(floorPlan);

            Assert.That(arms, Is.EqualTo(expectedArms));
        }

        [Test]
        public void GetBranchArmsLine()
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2), new Rect(3, 5, 2, 2), new Rect(3, 7, 2, 2) },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'C') });

            var expectedArms = new List<List<RoomHallIndex>>();
            var arm = new List<RoomHallIndex>
            {
                new RoomHallIndex(0, false),
                new RoomHallIndex(1, false),
                new RoomHallIndex(2, false)
            };
            expectedArms.Add(arm);

            var pathGen = new ConnectBranchStep<IFloorPlanTestContext>();
            List<List<RoomHallIndex>> arms = pathGen.GetBranchArms(floorPlan);

            Assert.That(arms, Is.EqualTo(expectedArms));
        }

        [Test]
        public void GetBranchArmsHall()
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2), new Rect(3, 7, 2, 2) },
                new Rect[] { new Rect(3, 5, 2, 2) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'a'), new Tuple<char, char>('a', 'B') });

            var expectedArms = new List<List<RoomHallIndex>>();
            var arm = new List<RoomHallIndex>
            {
                new RoomHallIndex(0, false),
                new RoomHallIndex(0, true),
                new RoomHallIndex(1, false)
            };
            expectedArms.Add(arm);

            var pathGen = new ConnectBranchStep<IFloorPlanTestContext>();
            List<List<RoomHallIndex>> arms = pathGen.GetBranchArms(floorPlan);

            Assert.That(arms, Is.EqualTo(expectedArms));
        }

        [Test]
        public void GetBranchArmsT()
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(3, 7, 2, 2),
                            new Rect(3, 5, 2, 2), new Rect(3, 3, 2, 2), 
                            new Rect(3, 9, 2, 2), new Rect(3, 11, 2, 2),
                            new Rect(5, 7, 2, 2), new Rect(7, 7, 2, 2)},
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'C'),
                                        new Tuple<char, char>('A', 'D'), new Tuple<char, char>('D', 'E'),
                                        new Tuple<char, char>('A', 'F'), new Tuple<char, char>('F', 'G')});

            var expectedArms = new List<List<RoomHallIndex>>();
            var arm = new List<RoomHallIndex>
            {
                new RoomHallIndex(2, false),
                new RoomHallIndex(1, false)
            };
            expectedArms.Add(arm);
            arm = new List<RoomHallIndex>
            {
                new RoomHallIndex(4, false),
                new RoomHallIndex(3, false)
            };
            expectedArms.Add(arm);
            arm = new List<RoomHallIndex>
            {
                new RoomHallIndex(6, false),
                new RoomHallIndex(5, false)
            };
            expectedArms.Add(arm);

            var pathGen = new ConnectBranchStep<IFloorPlanTestContext>();
            List<List<RoomHallIndex>> arms = pathGen.GetBranchArms(floorPlan);

            Assert.That(arms, Is.EqualTo(expectedArms));
        }

    }
}
