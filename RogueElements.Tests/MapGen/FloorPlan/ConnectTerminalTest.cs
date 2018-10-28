using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;

namespace RogueElements.Tests
{
    [TestFixture]
    public class ConnectTerminalTest
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

            ConnectTerminalStep<IFloorPlanTestContext> pathGen = new ConnectTerminalStep<IFloorPlanTestContext>();
            pathGen.ConnectPercent = 100;

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

            ConnectTerminalStep<IFloorPlanTestContext> pathGen = new ConnectTerminalStep<IFloorPlanTestContext>();
            pathGen.ConnectPercent = 100;

            Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
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

            ConnectTerminalStep<IFloorPlanTestContext> pathGen = new ConnectTerminalStep<IFloorPlanTestContext>();
            pathGen.ConnectPercent = 100;

            Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>> mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
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
                new Rect[] { },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'C'), new Tuple<char, char>('C', 'D'), new Tuple<char, char>('D', 'E') });
            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2), new Rect(5, 3, 2, 2), new Rect(7, 3, 2, 2), new Rect(9, 3, 2, 2), new Rect(11, 3, 2, 2) },
                new Rect[] { },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'C'), new Tuple<char, char>('C', 'D'), new Tuple<char, char>('D', 'E') });


            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            ConnectTerminalStep<IFloorPlanTestContext> pathGen = new ConnectTerminalStep<IFloorPlanTestContext>();
            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void GetBranchArmsSingle()
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2) },
                new Rect[] { },
                new Tuple<char, char>[] { });

            List<List<RoomHallIndex>> expectedArms = new List<List<RoomHallIndex>>();

            ConnectTerminalStep<IFloorPlanTestContext> pathGen = new ConnectTerminalStep<IFloorPlanTestContext>();
            List<List<RoomHallIndex>> arms = pathGen.GetBranchArms(floorPlan);

            Assert.That(arms, Is.EqualTo(expectedArms));
        }

        [Test]
        public void GetBranchArmsLine()
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2), new Rect(3, 5, 2, 2), new Rect(3, 7, 2, 2) },
                new Rect[] { },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'C') });

            List<List<RoomHallIndex>> expectedArms = new List<List<RoomHallIndex>>();
            List<RoomHallIndex> arm = new List<RoomHallIndex>();
            arm.Add(new RoomHallIndex(0, false));
            arm.Add(new RoomHallIndex(1, false));
            arm.Add(new RoomHallIndex(2, false));
            expectedArms.Add(arm);

            ConnectTerminalStep<IFloorPlanTestContext> pathGen = new ConnectTerminalStep<IFloorPlanTestContext>();
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

            List<List<RoomHallIndex>> expectedArms = new List<List<RoomHallIndex>>();
            List<RoomHallIndex> arm = new List<RoomHallIndex>();
            arm.Add(new RoomHallIndex(0, false));
            arm.Add(new RoomHallIndex(0, true));
            arm.Add(new RoomHallIndex(1, false));
            expectedArms.Add(arm);

            ConnectTerminalStep<IFloorPlanTestContext> pathGen = new ConnectTerminalStep<IFloorPlanTestContext>();
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
                new Rect[] { },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'C'),
                                        new Tuple<char, char>('A', 'D'), new Tuple<char, char>('D', 'E'),
                                        new Tuple<char, char>('A', 'F'), new Tuple<char, char>('F', 'G')});

            List<List<RoomHallIndex>> expectedArms = new List<List<RoomHallIndex>>();
            List<RoomHallIndex> arm = new List<RoomHallIndex>();
            arm.Add(new RoomHallIndex(2, false));
            arm.Add(new RoomHallIndex(1, false));
            expectedArms.Add(arm);
            arm = new List<RoomHallIndex>();
            arm.Add(new RoomHallIndex(4, false));
            arm.Add(new RoomHallIndex(3, false));
            expectedArms.Add(arm);
            arm = new List<RoomHallIndex>();
            arm.Add(new RoomHallIndex(6, false));
            arm.Add(new RoomHallIndex(5, false));
            expectedArms.Add(arm);

            ConnectTerminalStep<IFloorPlanTestContext> pathGen = new ConnectTerminalStep<IFloorPlanTestContext>();
            List<List<RoomHallIndex>> arms = pathGen.GetBranchArms(floorPlan);

            Assert.That(arms, Is.EqualTo(expectedArms));
        }

        [Test]
        public void GetPossibleExpansionsOne()
        {
            //two isolated rooms
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2), new Rect(3, 9, 2, 2) },
                new Rect[] { },
                new Tuple<char, char>[] { });

            List<RoomHallIndex> candList = new List<RoomHallIndex>();
            candList.Add(new RoomHallIndex(0, false));

            ConnectTerminalStep <IFloorPlanTestContext> pathGen = new ConnectTerminalStep<IFloorPlanTestContext>();
            SpawnList<ListPathTraversalNode> nodes = pathGen.GetPossibleExpansions(floorPlan, candList);
            
            Assert.That(nodes.Count, Is.EqualTo(1));
            Assert.That(nodes.GetSpawn(0).From, Is.EqualTo(new RoomHallIndex(0, false)));
            Assert.That(nodes.GetSpawn(0).To, Is.EqualTo(new RoomHallIndex(1, false)));
            Assert.That(nodes.GetSpawn(0).Connector, Is.EqualTo(new Rect(3, 5, 2, 4)));
        }

        
        [Test]
        public void GetPossibleExpansionsWeighted()
        {
            //A F
            //B E
            //C-DG
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(3, 1, 2, 2), new Rect(3, 3, 2, 2), new Rect(3, 5, 2, 3),
                            new Rect(7, 5, 2, 3), new Rect(7, 3, 2, 2), new Rect(7, 1, 2, 2),
                            new Rect(9, 6, 2, 2) },
                new Rect[] { new Rect(5, 6, 2, 2) },
                new Tuple<char, char>[] { new Tuple<char, char>('A', 'B'), new Tuple<char, char>('B', 'C'),
                                        new Tuple<char, char>('C', 'a'), new Tuple<char, char>('a', 'D'),
                                        new Tuple<char, char>('D', 'E'), new Tuple<char, char>('E', 'F'),
                                        new Tuple<char, char>('D', 'G')});


            List<RoomHallIndex> candList = new List<RoomHallIndex>();
            candList.Add(new RoomHallIndex(0, false));
            candList.Add(new RoomHallIndex(1, false));
            candList.Add(new RoomHallIndex(2, false));

            ConnectTerminalStep<IFloorPlanTestContext> pathGen = new ConnectTerminalStep<IFloorPlanTestContext>();
            SpawnList<ListPathTraversalNode> nodes = pathGen.GetPossibleExpansions(floorPlan, candList);
            
            Assert.That(nodes.Count, Is.EqualTo(2));
            Assert.That(nodes.GetSpawnRate(0), Is.EqualTo(6));
            Assert.That(nodes.GetSpawn(0).From, Is.EqualTo(new RoomHallIndex(0, false)));
            Assert.That(nodes.GetSpawn(0).To, Is.EqualTo(new RoomHallIndex(5, false)));
            Assert.That(nodes.GetSpawn(0).Connector, Is.EqualTo(new Rect(5, 1, 2, 2)));
            Assert.That(nodes.GetSpawnRate(1), Is.EqualTo(4));
            Assert.That(nodes.GetSpawn(1).From, Is.EqualTo(new RoomHallIndex(1, false)));
            Assert.That(nodes.GetSpawn(1).To, Is.EqualTo(new RoomHallIndex(4, false)));
            Assert.That(nodes.GetSpawn(1).Connector, Is.EqualTo(new Rect(5, 3, 2, 2)));
        }

        [Test]
        public void GetPossibleExpansionsAll()
        {
            //    A B
            //
            //
            //
            //C   D     E
            //    |
            //F   a-G   H
            //
            //
            //    I J
            //go through and refuse all GetRoomToConnect calls
            //measure the GetRoomToConnect Calls
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(5, 1, 1, 2), new Rect(7, 1, 2, 2),
                            new Rect(1, 5, 2, 1), new Rect(5, 5, 2, 2), new Rect(11, 5, 2, 1),
                            new Rect(1, 7, 2, 1), new Rect(7, 7, 2, 2), new Rect(11, 7, 2, 1),
                            new Rect(5, 11, 1, 2), new Rect(7, 11, 1, 2) },
                new Rect[] { new Rect(5, 7, 2, 2) },
                new Tuple<char, char>[] { new Tuple<char, char>('D', 'a'), new Tuple<char, char>('a', 'G') });

            List<RoomHallIndex> candList = new List<RoomHallIndex>();
            candList.Add(new RoomHallIndex(3, false));
            candList.Add(new RoomHallIndex(0, true));
            candList.Add(new RoomHallIndex(6, false));
            
            ConnectTerminalStep<IFloorPlanTestContext> pathGen = new ConnectTerminalStep<IFloorPlanTestContext>();
            SpawnList<ListPathTraversalNode> nodes = pathGen.GetPossibleExpansions(floorPlan, candList);

            Assert.That(nodes.Count, Is.EqualTo(8));
        }

        [Test]
        public void GetRoomToConnectNothingThere()
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2) },
                new Rect[] { },
                new Tuple<char, char>[] { });

            ConnectTerminalStep<IFloorPlanTestContext> pathGen = new ConnectTerminalStep<IFloorPlanTestContext>();
            ListPathTraversalNode node = pathGen.GetRoomToConnect(floorPlan, new RoomHallIndex(0, false), Dir4.Down);

            Assert.That(node, Is.EqualTo(null));
        }

        [Test]
        [TestCase(4, 7, Dir4.Down, 4, 6, 2, 1)]
        [TestCase(4, 9, Dir4.Down, 4, 6, 2, 3)]
        [TestCase(1, 4, Dir4.Left, 3, 4, 1, 2)]
        [TestCase(4, 1, Dir4.Up, 4, 3, 2, 1)]
        [TestCase(7, 4, Dir4.Right, 6, 4, 1, 2)]
        public void GetRoomToConnectBlocked(int blockX, int blockY, Dir4 dir, int rectX, int rectY, int rectW, int rectH)
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(4, 4, 2, 2), new Rect(blockX, blockY, 2, 2) },
                new Rect[] { },
                new Tuple<char, char>[] { });

            ConnectTerminalStep<IFloorPlanTestContext> pathGen = new ConnectTerminalStep<IFloorPlanTestContext>();
            ListPathTraversalNode node = pathGen.GetRoomToConnect(floorPlan, new RoomHallIndex(0, false), dir);

            Assert.That(node.From, Is.EqualTo(new RoomHallIndex(0, false)));
            Assert.That(node.To, Is.EqualTo(new RoomHallIndex(1, false)));
            Assert.That(node.Connector, Is.EqualTo(new Rect(rectX, rectY, rectW, rectH)));
        }
        
        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GetRoomToConnectBlockedMultiple(bool hall)
        {
            List<Rect> rooms = new List<Rect>();
            rooms.Add(new Rect(4, 4, 2, 2));
            rooms.Add(new Rect(4, 10, 2, 2));
            List<Rect> halls = new List<Rect>();
            if (!hall)
                rooms.Add(new Rect(4, 7, 1, 2));
            else
                halls.Add(new Rect(4, 7, 1, 2));
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                rooms.ToArray(),
                halls.ToArray(),
                new Tuple<char, char>[] { });

            ConnectTerminalStep<IFloorPlanTestContext> pathGen = new ConnectTerminalStep<IFloorPlanTestContext>();
            ListPathTraversalNode node = pathGen.GetRoomToConnect(floorPlan, new RoomHallIndex(0, false), Dir4.Down);

            Assert.That(node.From, Is.EqualTo(new RoomHallIndex(0, false)));
            if (!hall)
                Assert.That(node.To, Is.EqualTo(new RoomHallIndex(2, false)));
            else
                Assert.That(node.To, Is.EqualTo(new RoomHallIndex(0, true)));
            Assert.That(node.Connector, Is.EqualTo(new Rect(4, 6, 2, 1)));
        }


        [Test]
        [TestCase(false, true, 4, 2)]
        [TestCase(true, false, 5, 2)]
        [TestCase(true, true, 5, 1)]
        public void GetRoomToConnectRetracted(bool retractLeft, bool retractRight, int rectX, int rectW)
        {
            List<Rect> rooms = new List<Rect>();
            rooms.Add(new Rect(4, 4, 3, 2));
            rooms.Add(new Rect(4, 10, 2, 2));
            if (retractLeft)
                rooms.Add(new Rect(2, 7, 2, 2));
            if (retractRight)
                rooms.Add(new Rect(7, 7, 2, 2));
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                rooms.ToArray(),
                new Rect[] { },
                new Tuple<char, char>[] { });

            ConnectTerminalStep<IFloorPlanTestContext> pathGen = new ConnectTerminalStep<IFloorPlanTestContext>();
            ListPathTraversalNode node = pathGen.GetRoomToConnect(floorPlan, new RoomHallIndex(0, false), Dir4.Down);

            Assert.That(node.From, Is.EqualTo(new RoomHallIndex(0, false)));
            Assert.That(node.To, Is.EqualTo(new RoomHallIndex(1, false)));
            Assert.That(node.Connector, Is.EqualTo(new Rect(rectX, 6, rectW, 4)));
        }

        [Test]
        public void GetRoomToConnectRetractedTooMuch()
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14),
                new Rect[] { new Rect(4, 4, 2, 2), new Rect(4, 10, 2, 2), new Rect(2, 7, 2, 2), new Rect(6, 7, 2, 2) },
                new Rect[] { },
                new Tuple<char, char>[] { });

            ConnectTerminalStep<IFloorPlanTestContext> pathGen = new ConnectTerminalStep<IFloorPlanTestContext>();
            ListPathTraversalNode node = pathGen.GetRoomToConnect(floorPlan, new RoomHallIndex(0, false), Dir4.Down);

            Assert.That(node, Is.EqualTo(null));
        }


        
        [Test]
        [TestCase(-2, false)]
        [TestCase(-1, true)]
        [TestCase(0, true)]
        [TestCase(1, true)]
        [TestCase(2, false)]
        [TestCase(3, true)]
        [TestCase(4, true)]
        [TestCase(5, false)]
        [TestCase(6, false)]
        public void HasBorderOpening(int x, bool expected)
        {
            Dir4 expandTo = Dir4.Up;
            Mock<IRoomGen> mockFrom = new Mock<IRoomGen>(MockBehavior.Strict);
            mockFrom.SetupGet(p => p.Draw).Returns(new Rect(0, 2, 6, 2));
            mockFrom.Setup(p => p.GetBorderLength(expandTo)).Returns(6);
            mockFrom.Setup(p => p.GetFulfillableBorder(expandTo, It.IsIn(0, 1, 4))).Returns(true);
            mockFrom.Setup(p => p.GetFulfillableBorder(expandTo, It.IsIn(2, 3, 5))).Returns(false);
            Rect rectTo = new Rect(x, 0, 2, 2);

            ConnectTerminalStep<IFloorPlanTestContext> pathGen = new ConnectTerminalStep<IFloorPlanTestContext>();
            bool hasOpening = pathGen.HasBorderOpening(mockFrom.Object, rectTo, expandTo);

            Assert.That(hasOpening, Is.EqualTo(expected));
        }
    }
}
