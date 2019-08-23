// <copyright file="ConnectBranchTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Moq;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class ConnectBranchTest
    {
        [Test]
        public void ConnectSelf()
        {
            /* A D
               B-C */
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2), new Rect(3, 5, 2, 3), new Rect(7, 5, 2, 3), new Rect(7, 3, 2, 2) },
                new Rect[] { new Rect(5, 6, 2, 2) },
                new Tuple<char, char>[] { Tuple.Create('A', 'B'), Tuple.Create('B', 'a'), Tuple.Create('a', 'C'), Tuple.Create('C', 'D') });

            TestFloorPlan compareFloorPlan;
            {
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('A', 'B'),
                    Tuple.Create('B', 'a'),
                    Tuple.Create('a', 'C'),
                    Tuple.Create('C', 'D'),
                    Tuple.Create('A', 'b'),
                    Tuple.Create('b', 'D'),
                };
                compareFloorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(22, 14),
                    new Rect[] { new Rect(3, 3, 2, 2), new Rect(3, 5, 2, 3), new Rect(7, 5, 2, 3), new Rect(7, 3, 2, 2) },
                    new Rect[] { new Rect(5, 6, 2, 2), new Rect(5, 3, 2, 2) },
                    links);
            }

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestFloorPlanGen('b'));

            var pathGen = new ConnectBranchTestStep(mockHalls.Object) { ConnectPercent = 100 };
            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void ConnectOther()
        {
            /* A D
               B-CE */
            TestFloorPlan floorPlan;
            {
            var links = new Tuple<char, char>[]
            {
                Tuple.Create('A', 'B'),
                Tuple.Create('B', 'a'),
                Tuple.Create('a', 'C'),
                Tuple.Create('C', 'D'),
                Tuple.Create('C', 'E'),
            };
            floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2), new Rect(3, 5, 2, 3), new Rect(7, 5, 2, 3), new Rect(7, 3, 2, 2), new Rect(9, 6, 2, 2) },
                new Rect[] { new Rect(5, 6, 2, 2) },
                links);
            }

            TestFloorPlan compareFloorPlan;
            {
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('A', 'B'),
                    Tuple.Create('B', 'a'),
                    Tuple.Create('a', 'C'),
                    Tuple.Create('C', 'D'),
                    Tuple.Create('C', 'E'),
                    Tuple.Create('A', 'b'),
                    Tuple.Create('b', 'D'),
                };
                compareFloorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(22, 14),
                    new Rect[] { new Rect(3, 3, 2, 2), new Rect(3, 5, 2, 3), new Rect(7, 5, 2, 3), new Rect(7, 3, 2, 2), new Rect(9, 6, 2, 2) },
                    new Rect[] { new Rect(5, 6, 2, 2), new Rect(5, 3, 2, 2) },
                    links);
            }

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            mockHalls.Setup(p => p.Pick(testRand.Object)).Returns(new TestFloorPlanGen('b'));

            var pathGen = new ConnectBranchTestStep(mockHalls.Object) { ConnectPercent = 100 };
            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void ConnectNonArm()
        {
            /*   E G
               A D-F
               B-C   */
            TestFloorPlan floorPlan;
            {
                Rect[] rooms = new Rect[]
                {
                    new Rect(3, 3, 2, 4),
                    new Rect(3, 7, 2, 3),
                    new Rect(7, 7, 2, 3),
                    new Rect(7, 3, 2, 4),
                    new Rect(7, 1, 2, 2),
                    new Rect(11, 3, 2, 4),
                    new Rect(11, 1, 2, 2),
                };
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('A', 'B'),
                    Tuple.Create('B', 'a'),
                    Tuple.Create('a', 'C'),
                    Tuple.Create('C', 'D'),
                    Tuple.Create('D', 'E'),
                    Tuple.Create('D', 'b'),
                    Tuple.Create('b', 'F'),
                    Tuple.Create('F', 'G'),
                };
                floorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(22, 14),
                    rooms,
                    new Rect[] { new Rect(5, 8, 2, 2), new Rect(9, 4, 2, 2) },
                    links);
            }

            TestFloorPlan compareFloorPlan;
            {
                Rect[] rooms = new Rect[]
                {
                    new Rect(3, 3, 2, 4),
                    new Rect(3, 7, 2, 3),
                    new Rect(7, 7, 2, 3),
                    new Rect(7, 3, 2, 4),
                    new Rect(7, 1, 2, 2),
                    new Rect(11, 3, 2, 4),
                    new Rect(11, 1, 2, 2),
                };
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('A', 'B'),
                    Tuple.Create('B', 'a'),
                    Tuple.Create('a', 'C'),
                    Tuple.Create('C', 'D'),
                    Tuple.Create('D', 'E'),
                    Tuple.Create('D', 'b'),
                    Tuple.Create('b', 'F'),
                    Tuple.Create('F', 'G'),
                    Tuple.Create('A', 'c'),
                    Tuple.Create('c', 'D'),
                    Tuple.Create('E', 'd'),
                    Tuple.Create('d', 'G'),
                };
                compareFloorPlan = TestFloorPlan.InitFloorToContext(
                    new Loc(22, 14),
                    rooms,
                    new Rect[] { new Rect(5, 8, 2, 2), new Rect(9, 4, 2, 2), new Rect(5, 3, 2, 4), new Rect(9, 1, 2, 2) },
                    links);
            }

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);
            Moq.Language.ISetupSequentialResult<PermissiveRoomGen<IFloorPlanTestContext>> hallSeq = mockHalls.SetupSequence(p => p.Pick(testRand.Object));
            hallSeq = hallSeq.Returns(new TestFloorPlanGen('c'));
            hallSeq = hallSeq.Returns(new TestFloorPlanGen('d'));

            var pathGen = new ConnectBranchTestStep(mockHalls.Object) { ConnectPercent = 100 };
            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        [TestCase(Ignore = "TODO")]
        [Ignore("TODO")]
        [SuppressMessage("CodeCracker.CSharp.Usage", "CC0057:UnusedParameter", Justification = "TODO")]
        public void ConnectComb(int randResult)
        {
            /* A D F H
               B-C-E-G */
            throw new NotImplementedException();
        }

        [Test]
        public void ConnectFail()
        {
            // A-B-C-D-E
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2), new Rect(5, 3, 2, 2), new Rect(7, 3, 2, 2), new Rect(9, 3, 2, 2), new Rect(11, 3, 2, 2) },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { Tuple.Create('A', 'B'), Tuple.Create('B', 'C'), Tuple.Create('C', 'D'), Tuple.Create('D', 'E') });
            TestFloorPlan compareFloorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2), new Rect(5, 3, 2, 2), new Rect(7, 3, 2, 2), new Rect(9, 3, 2, 2), new Rect(11, 3, 2, 2) },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { Tuple.Create('A', 'B'), Tuple.Create('B', 'C'), Tuple.Create('C', 'D'), Tuple.Create('D', 'E') });

            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            testRand.Setup(p => p.Next(It.IsAny<int>())).Returns(0);

            var mockHalls = new Mock<IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>>>(MockBehavior.Strict);

            var pathGen = new ConnectBranchTestStep(mockHalls.Object);
            pathGen.ApplyToPath(testRand.Object, floorPlan);

            TestFloorPlan.CompareFloorPlans(floorPlan, compareFloorPlan);
        }

        [Test]
        public void GetBranchArmsSingle()
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2) },
                Array.Empty<Rect>(),
                Array.Empty<Tuple<char, char>>());

            List<List<RoomHallIndex>> expectedArms = new List<List<RoomHallIndex>>();

            List<List<RoomHallIndex>> arms = ConnectBranchTestStep.GetBranchArms(floorPlan);

            Assert.That(arms, Is.EqualTo(expectedArms));
        }

        [Test]
        public void GetBranchArmsLine()
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2), new Rect(3, 5, 2, 2), new Rect(3, 7, 2, 2) },
                Array.Empty<Rect>(),
                new Tuple<char, char>[] { Tuple.Create('A', 'B'), Tuple.Create('B', 'C') });

            var expectedArms = new List<List<RoomHallIndex>>();
            var arm = new List<RoomHallIndex>
            {
                new RoomHallIndex(0, false),
                new RoomHallIndex(1, false),
                new RoomHallIndex(2, false),
            };
            expectedArms.Add(arm);

            List<List<RoomHallIndex>> arms = ConnectBranchTestStep.GetBranchArms(floorPlan);

            Assert.That(arms, Is.EqualTo(expectedArms));
        }

        [Test]
        public void GetBranchArmsHall()
        {
            TestFloorPlan floorPlan = TestFloorPlan.InitFloorToContext(
                new Loc(22, 14),
                new Rect[] { new Rect(3, 3, 2, 2), new Rect(3, 7, 2, 2) },
                new Rect[] { new Rect(3, 5, 2, 2) },
                new Tuple<char, char>[] { Tuple.Create('A', 'a'), Tuple.Create('a', 'B') });

            var expectedArms = new List<List<RoomHallIndex>>();
            var arm = new List<RoomHallIndex>
            {
                new RoomHallIndex(0, false),
                new RoomHallIndex(0, true),
                new RoomHallIndex(1, false),
            };
            expectedArms.Add(arm);

            List<List<RoomHallIndex>> arms = ConnectBranchTestStep.GetBranchArms(floorPlan);

            Assert.That(arms, Is.EqualTo(expectedArms));
        }

        [Test]
        public void GetBranchArmsT()
        {
            TestFloorPlan floorPlan;
            {
                Rect[] rooms = new Rect[]
                {
                    new Rect(3, 7, 2, 2),
                    new Rect(3, 5, 2, 2),
                    new Rect(3, 3, 2, 2),
                    new Rect(3, 9, 2, 2),
                    new Rect(3, 11, 2, 2),
                    new Rect(5, 7, 2, 2),
                    new Rect(7, 7, 2, 2),
                };
                var links = new Tuple<char, char>[]
                {
                    Tuple.Create('A', 'B'),
                    Tuple.Create('B', 'C'),
                    Tuple.Create('A', 'D'),
                    Tuple.Create('D', 'E'),
                    Tuple.Create('A', 'F'),
                    Tuple.Create('F', 'G'),
                };
                floorPlan = TestFloorPlan.InitFloorToContext(new Loc(22, 14), rooms, Array.Empty<Rect>(), links);
            }

            var expectedArms = new List<List<RoomHallIndex>>();
            var arm = new List<RoomHallIndex>
            {
                new RoomHallIndex(2, false),
                new RoomHallIndex(1, false),
            };
            expectedArms.Add(arm);
            arm = new List<RoomHallIndex>
            {
                new RoomHallIndex(4, false),
                new RoomHallIndex(3, false),
            };
            expectedArms.Add(arm);
            arm = new List<RoomHallIndex>
            {
                new RoomHallIndex(6, false),
                new RoomHallIndex(5, false),
            };
            expectedArms.Add(arm);

            List<List<RoomHallIndex>> arms = ConnectBranchTestStep.GetBranchArms(floorPlan);

            Assert.That(arms, Is.EqualTo(expectedArms));
        }

        private class ConnectBranchTestStep : ConnectBranchStep<IFloorPlanTestContext>
        {
            public ConnectBranchTestStep(IRandPicker<PermissiveRoomGen<IFloorPlanTestContext>> genericHalls)
                : base(genericHalls)
            {
            }

            public static List<List<RoomHallIndex>> GetBranchArms(TestFloorPlan floorPlan) => ConnectBranchStep<IFloorPlanTestContext>.GetBranchArms(floorPlan);
        }
    }
}
