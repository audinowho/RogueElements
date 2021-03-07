// <copyright file="SpawnerTest.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace RogueElements.Tests
{
    [TestFixture]
    public class SpawnerTest
    {
        public interface IPlaceableRoomTestContext : ITiledGenContext, IFloorPlanGenContext, IPlaceableGenContext<SpawnableChar>
        {
        }

        public interface IViewPlaceableRoomTestContext : ITiledGenContext, IFloorPlanGenContext, IPlaceableGenContext<SpawnableChar>, IViewPlaceableGenContext<TestEntryPoint>
        {
        }

        [Test]
        [Ignore("TODO")]
        public void PickerSpawner()
        {
            // mock an IGenContext for the rand
            // verify that the picker spawner result is the same as the picker result
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void PickerSpawnerRepeat()
        {
            // verify that the picker spawner result is the same as the picker result
            // even if the picker's state changes and repeat calls are made
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void ContextSpawner()
        {
            // mock an ISpawningGenContext for the rand
            // verify that the amount spawned is expected
            // verify that the results are the expected
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void ContextSpawnerStateChange()
        {
            // mock an ISpawningGenContext for the rand
            // verify that the state changes for the map
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void ContextSpawnerCutsShort()
        {
            // mock an ISpawningGenContext for the rand
            // verify that the spawns cut short if they cannot pick
            // do it for the first, and then something that is after first
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void RandomSpawnStep()
        {
            // set the spawn to return a set list of spawns
            // set map.getallfreetiles to a set list of tiles
            // set random to choose specific tiles out of them
            // verify the proper tiles are removed
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO")]
        public void RandomTerrainSpawnStep()
        {
            // input a specific grid
            // set random to choose specific tiles out of them
            // verify the proper tiles are removed
            throw new NotImplementedException();
        }

        [Test]
        [TestCase(0, 1, 2)]
        [TestCase(1, 3, 4)]
        [TestCase(2, 5, 5)]
        [TestCase(3, 3, 2)]
        public void RoomSpawnStepSpawnInRoom(int chosenRand, int locX, int locY)
        {
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // choose freetile count
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(4));
            seq = seq.Returns(chosenRand);
            Mock<IPlaceableRoomTestContext> mockMap = new Mock<IPlaceableRoomTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);

            // get free tiles
            var freeLocs = new List<Loc>
            {
                new Loc(1, 2),
                new Loc(3, 4),
                new Loc(5, 5),
                new Loc(3, 2),
            };
            mockMap.Setup(p => p.GetFreeTiles(new Rect(10, 20, 30, 40))).Returns(freeLocs);

            // expect place item
            mockMap.Setup(p => p.PlaceItem(new Loc(locX, locY), new SpawnableChar('a')));

            Mock<IRoomGen> mockRoom = new Mock<IRoomGen>(MockBehavior.Strict);
            mockRoom.SetupGet(p => p.Draw).Returns(new Rect(10, 20, 30, 40));
            var roomPlan = new FloorRoomPlan(mockRoom.Object, new ComponentCollection());
            Mock<FloorPlan> mockFloor = new Mock<FloorPlan>(MockBehavior.Strict);
            mockFloor.Setup(p => p.GetRoomHall(new RoomHallIndex(0, false))).Returns(roomPlan);
            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            var roomSpawner = new Mock<RoomSpawnStep<IPlaceableRoomTestContext, SpawnableChar>>(null) { CallBase = true };

            roomSpawner.Object.SpawnInRoom(mockMap.Object, new RoomHallIndex(0, false), new SpawnableChar('a'));

            // verify the correct placeitem was called
            mockMap.Verify(p => p.PlaceItem(new Loc(locX, locY), new SpawnableChar('a')), Times.Exactly(1));
        }

        [Test]
        [TestCase(0, 1, 2, 0, 2, 1, 1)] // evenly distributed
        [TestCase(0, 0, 0, 0, 4, 0, 0)] // all on one room is possible
        [TestCase(2, 2, 2, 2, 0, 0, 4)] // all on another room is possible
        public void RoomSpawnStepSpawnRandInCandRooms(int seq1, int seq2, int seq3, int seq4, int room1s, int room2s, int room3s)
        {
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // choose freetile count
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(3));
            seq = seq.Returns(seq1);
            seq = seq.Returns(seq2);
            seq = seq.Returns(seq3);
            seq = seq.Returns(seq4);

            Mock<IPlaceableRoomTestContext> mockMap = new Mock<IPlaceableRoomTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);

            var spawningRooms = new SpawnList<RoomHallIndex>
            {
                { new RoomHallIndex(0, false), 1 },
                { new RoomHallIndex(1, false), 1 },
                { new RoomHallIndex(2, false), 1 },
            };

            // get a list of spawns
            List<SpawnableChar> spawns = new List<SpawnableChar>();
            foreach (Dir4 dir in DirExt.VALID_DIR4)
                spawns.Add(new SpawnableChar('a'));

            var roomSpawner = new Mock<RandomRoomSpawnStep<IPlaceableRoomTestContext, SpawnableChar>>(null, false) { CallBase = true };
            roomSpawner.Setup(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(0, false), new SpawnableChar('a'))).Returns(true);
            roomSpawner.Setup(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(1, false), new SpawnableChar('a'))).Returns(true);
            roomSpawner.Setup(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(2, false), new SpawnableChar('a'))).Returns(true);

            roomSpawner.Object.SpawnRandInCandRooms(mockMap.Object, spawningRooms, spawns, 100);

            roomSpawner.Verify(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(0, false), new SpawnableChar('a')), Times.Exactly(room1s));
            roomSpawner.Verify(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(1, false), new SpawnableChar('a')), Times.Exactly(room2s));
            roomSpawner.Verify(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(2, false), new SpawnableChar('a')), Times.Exactly(room3s));

            // assert that the right values have been taken out of the lists
            Assert.That(spawningRooms.Count, Is.EqualTo(3));
            Assert.That(spawns.Count, Is.EqualTo(0));
        }

        [Test]
        [TestCase(false, 100, 0)]
        [TestCase(true, 0, 3)]
        [TestCase(true, -100, 3)]
        public void RoomSpawnStepSpawnRandInCandRoomsRemoval(bool successful, int successPercent, int expectedSpawns)
        {
            // proves that you can run out of space
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // choose freetile count
            testRand.SetupSequence(p => p.Next(3)).Returns(0);
            testRand.SetupSequence(p => p.Next(2)).Returns(0);
            testRand.SetupSequence(p => p.Next(1)).Returns(0);

            Mock<IPlaceableRoomTestContext> mockMap = new Mock<IPlaceableRoomTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);

            SpawnList<RoomHallIndex> spawningRooms = new SpawnList<RoomHallIndex>
            {
                { new RoomHallIndex(0, false), 1 },
                { new RoomHallIndex(1, false), 1 },
                { new RoomHallIndex(2, false), 1 },
            };

            // get a list of spawns
            List<SpawnableChar> spawns = new List<SpawnableChar>();
            for (int ii = 0; ii < 5; ii++)
                spawns.Add(new SpawnableChar('a'));

            var roomSpawner = new Mock<RandomRoomSpawnStep<IPlaceableRoomTestContext, SpawnableChar>>(null, false) { CallBase = true };
            roomSpawner.Setup(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(0, false), new SpawnableChar('a'))).Returns(successful);
            roomSpawner.Setup(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(1, false), new SpawnableChar('a'))).Returns(successful);
            roomSpawner.Setup(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(2, false), new SpawnableChar('a'))).Returns(successful);

            roomSpawner.Object.SpawnRandInCandRooms(mockMap.Object, spawningRooms, spawns, successPercent);

            roomSpawner.Verify(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(0, false), new SpawnableChar('a')), Times.Exactly(1));
            roomSpawner.Verify(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(1, false), new SpawnableChar('a')), Times.Exactly(1));
            roomSpawner.Verify(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(2, false), new SpawnableChar('a')), Times.Exactly(1));

            Assert.That(spawningRooms.Count, Is.EqualTo(0));
            Assert.That(spawns.Count, Is.EqualTo(5 - expectedSpawns));
        }

        [Test]
        public void RoomSpawnStepSpawnRandInCandRoomsChangeChance()
        {
            // proves that the probability diminishes with each success
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);

            // choose freetile count
            testRand.SetupSequence(p => p.Next(12)).Returns(0);
            testRand.SetupSequence(p => p.Next(10)).Returns(2);
            testRand.SetupSequence(p => p.Next(8)).Returns(0);
            testRand.SetupSequence(p => p.Next(7)).Returns(0);

            Mock<IPlaceableRoomTestContext> mockMap = new Mock<IPlaceableRoomTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);

            SpawnList<RoomHallIndex> spawningRooms = new SpawnList<RoomHallIndex>
            {
                { new RoomHallIndex(0, false), 4 },
                { new RoomHallIndex(1, false), 4 },
                { new RoomHallIndex(2, false), 4 },
            };

            // get a list of spawns
            List<SpawnableChar> spawns = new List<SpawnableChar>();
            for (int ii = 0; ii < 5; ii++)
                spawns.Add(new SpawnableChar('a'));

            var roomSpawner = new Mock<RandomRoomSpawnStep<IPlaceableRoomTestContext, SpawnableChar>>(null, false) { CallBase = true };
            roomSpawner.Setup(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(0, false), new SpawnableChar('a'))).Returns(true);
            roomSpawner.Setup(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(1, false), new SpawnableChar('a'))).Returns(true);
            roomSpawner.Setup(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(2, false), new SpawnableChar('a'))).Returns(true);

            roomSpawner.Object.SpawnRandInCandRooms(mockMap.Object, spawningRooms, spawns, 50);

            roomSpawner.Verify(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(0, false), new SpawnableChar('a')), Times.Exactly(4));
            roomSpawner.Verify(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(1, false), new SpawnableChar('a')), Times.Exactly(1));
            roomSpawner.Verify(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(2, false), new SpawnableChar('a')), Times.Exactly(0));

            Assert.That(spawningRooms.Count, Is.EqualTo(3));
            Assert.That(spawningRooms.GetSpawnRate(0), Is.EqualTo(1));
            Assert.That(spawningRooms.GetSpawnRate(1), Is.EqualTo(2));
            Assert.That(spawningRooms.GetSpawnRate(2), Is.EqualTo(4));
            Assert.That(spawns.Count, Is.EqualTo(0));
        }

        [Test]
        public void RandomRoomSpawnStep()
        {
            Mock<IPlaceableRoomTestContext> mockMap = new Mock<IPlaceableRoomTestContext>(MockBehavior.Strict);

            Mock<FloorPlan> mockFloor = new Mock<FloorPlan>(MockBehavior.Strict);
            mockFloor.SetupGet(p => p.RoomCount).Returns(3);
            mockFloor.Setup(p => p.GetRoomPlan(0)).Returns(new FloorRoomPlan(new TestFloorPlanGen('A'), new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomPlan(1)).Returns(new FloorRoomPlan(new TestFloorPlanGen('B'), new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomPlan(2)).Returns(new FloorRoomPlan(new TestFloorPlanGen('C'), new ComponentCollection()));
            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            Mock<List<SpawnableChar>> mockSpawns = new Mock<List<SpawnableChar>>(MockBehavior.Strict);

            var roomSpawner = new Mock<RandomRoomSpawnStep<IPlaceableRoomTestContext, SpawnableChar>>(null, false) { CallBase = true };

            SpawnList<RoomHallIndex> compare = new SpawnList<RoomHallIndex>
            {
                { new RoomHallIndex(0, false), 10 },
                { new RoomHallIndex(1, false), 10 },
                { new RoomHallIndex(2, false), 10 },
            };

            roomSpawner.Setup(p => p.SpawnRandInCandRooms(mockMap.Object, It.IsAny<SpawnList<RoomHallIndex>>(), mockSpawns.Object, 100));

            roomSpawner.Object.DistributeSpawns(mockMap.Object, mockSpawns.Object);

            roomSpawner.Verify(p => p.SpawnRandInCandRooms(mockMap.Object, It.Is<SpawnList<RoomHallIndex>>(s => s.Equals(compare)), mockSpawns.Object, 100), Times.Exactly(1));
        }

        [Test]
        public void TerminalSpawnStep()
        {
            // tests eligibility of terminals
            Mock<IPlaceableRoomTestContext> mockMap = new Mock<IPlaceableRoomTestContext>(MockBehavior.Strict);

            Mock<FloorPlan> mockFloor = new Mock<FloorPlan>(MockBehavior.Strict);
            mockFloor.SetupGet(p => p.RoomCount).Returns(4);
            mockFloor.Setup(p => p.GetRoomPlan(0)).Returns(new FloorRoomPlan(new TestFloorPlanGen('A'), new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomPlan(1)).Returns(new FloorRoomPlan(new TestFloorPlanGen('B'), new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomPlan(2)).Returns(new FloorRoomPlan(new TestFloorPlanGen('C'), new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomPlan(3)).Returns(new FloorRoomPlan(new TestFloorPlanGen('D'), new ComponentCollection()));
            List<int> adjacents = new List<int>();
            mockFloor.Setup(p => p.GetAdjacentRooms(0)).Returns(adjacents);
            adjacents = new List<int> { 2 };
            mockFloor.Setup(p => p.GetAdjacentRooms(1)).Returns(adjacents);
            adjacents = new List<int> { 1, 3 };
            mockFloor.Setup(p => p.GetAdjacentRooms(2)).Returns(adjacents);
            adjacents = new List<int> { 2 };
            mockFloor.Setup(p => p.GetAdjacentRooms(3)).Returns(adjacents);
            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            Mock<List<SpawnableChar>> mockSpawns = new Mock<List<SpawnableChar>>(MockBehavior.Strict);

            var roomSpawner = new Mock<TerminalSpawnStep<IPlaceableRoomTestContext, SpawnableChar>>(null, false) { CallBase = true };

            var compare1 = new SpawnList<RoomHallIndex>
            {
                { new RoomHallIndex(1, false), 10 },
                { new RoomHallIndex(3, false), 10 },
            };
            var compare2 = new SpawnList<RoomHallIndex>
            {
                { new RoomHallIndex(0, false), 10 },
                { new RoomHallIndex(1, false), 10 },
                { new RoomHallIndex(2, false), 10 },
                { new RoomHallIndex(3, false), 10 },
            };

            roomSpawner.Setup(p => p.SpawnRandInCandRooms(mockMap.Object, It.IsAny<SpawnList<RoomHallIndex>>(), mockSpawns.Object, It.IsAny<int>()));

            roomSpawner.Object.DistributeSpawns(mockMap.Object, mockSpawns.Object);

            roomSpawner.Verify(p => p.SpawnRandInCandRooms(mockMap.Object, It.Is<SpawnList<RoomHallIndex>>(s => s.Equals(compare1)), mockSpawns.Object, 0), Times.Exactly(1));
            roomSpawner.Verify(p => p.SpawnRandInCandRooms(mockMap.Object, It.Is<SpawnList<RoomHallIndex>>(s => s.Equals(compare2)), mockSpawns.Object, 100), Times.Exactly(1));
        }

        [Test]
        public void DueSpawnStepStraightNoHall()
        {
            // order of rooms
            // A=>0=>B
            Mock<IViewPlaceableRoomTestContext> mockMap = new Mock<IViewPlaceableRoomTestContext>(MockBehavior.Strict);

            Mock<FloorPlan> mockFloor = new Mock<FloorPlan>(MockBehavior.Strict);
            mockFloor.SetupGet(p => p.RoomCount).Returns(2);
            Mock<TestFloorPlanGen> startRoom = new Mock<TestFloorPlanGen>(MockBehavior.Strict);
            startRoom.SetupProperty(p => p.Draw);
            startRoom.SetupGet(p => p.Draw).Returns(new Rect(2, 2, 4, 4));
            startRoom.Object.Identifier = 'A';
            mockFloor.Setup(p => p.GetRoomPlan(0)).Returns(new FloorRoomPlan(startRoom.Object, new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomPlan(1)).Returns(new FloorRoomPlan(new TestFloorPlanGen('B'), new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomHall(new RoomHallIndex(0, false))).Returns(new FloorRoomPlan(startRoom.Object, new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomHall(new RoomHallIndex(0, true))).Returns(new FloorHallPlan(new TestFloorPlanGen('0'), new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomHall(new RoomHallIndex(1, false))).Returns(new FloorRoomPlan(new TestFloorPlanGen('B'), new ComponentCollection()));

            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            List<RoomHallIndex> adjacents = new List<RoomHallIndex> { new RoomHallIndex(0, true) };
            mockFloor.Setup(p => p.GetAdjacents(new RoomHallIndex(0, false))).Returns(adjacents);
            adjacents = new List<RoomHallIndex> { new RoomHallIndex(0, false), new RoomHallIndex(1, false) };
            mockFloor.Setup(p => p.GetAdjacents(new RoomHallIndex(0, true))).Returns(adjacents);
            adjacents = new List<RoomHallIndex> { new RoomHallIndex(0, true) };
            mockFloor.Setup(p => p.GetAdjacents(new RoomHallIndex(1, false))).Returns(adjacents);
            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            mockMap.Setup(p => p.GetLoc(0)).Returns(new Loc(3, 3));

            Mock<List<SpawnableChar>> mockSpawns = new Mock<List<SpawnableChar>>(MockBehavior.Strict);

            var roomSpawner = new Mock<DueSpawnStep<IViewPlaceableRoomTestContext, SpawnableChar, TestEntryPoint>>(null, 100, false) { CallBase = true };

            const int maxVal = 3;
            const int rooms = 3;
            SpawnList<RoomHallIndex> compare = new SpawnList<RoomHallIndex>
            {
                { new RoomHallIndex(0, false), int.MaxValue / maxVal / rooms * 1 },
                { new RoomHallIndex(1, false), int.MaxValue / maxVal / rooms * 3 },
            };

            roomSpawner.Setup(p => p.SpawnRandInCandRooms(mockMap.Object, It.IsAny<SpawnList<RoomHallIndex>>(), mockSpawns.Object, 100));

            roomSpawner.Object.DistributeSpawns(mockMap.Object, mockSpawns.Object);

            roomSpawner.Verify(p => p.SpawnRandInCandRooms(mockMap.Object, It.Is<SpawnList<RoomHallIndex>>(s => s.Equals(compare)), mockSpawns.Object, 100), Times.Exactly(1));
        }

        [Test]
        public void DueSpawnStepStraightWithHall()
        {
            // order of rooms
            // A=>0=>B
            Mock<IViewPlaceableRoomTestContext> mockMap = new Mock<IViewPlaceableRoomTestContext>(MockBehavior.Strict);

            Mock<FloorPlan> mockFloor = new Mock<FloorPlan>(MockBehavior.Strict);
            mockFloor.SetupGet(p => p.RoomCount).Returns(2);
            Mock<TestFloorPlanGen> startRoom = new Mock<TestFloorPlanGen>(MockBehavior.Strict);
            startRoom.SetupProperty(p => p.Draw);
            startRoom.SetupGet(p => p.Draw).Returns(new Rect(2, 2, 4, 4));
            startRoom.Object.Identifier = 'A';
            mockFloor.Setup(p => p.GetRoomPlan(0)).Returns(new FloorRoomPlan(startRoom.Object, new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomPlan(1)).Returns(new FloorRoomPlan(new TestFloorPlanGen('B'), new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomHall(new RoomHallIndex(0, false))).Returns(new FloorRoomPlan(startRoom.Object, new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomHall(new RoomHallIndex(0, true))).Returns(new FloorHallPlan(new TestFloorPlanGen('0'), new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomHall(new RoomHallIndex(1, false))).Returns(new FloorRoomPlan(new TestFloorPlanGen('B'), new ComponentCollection()));

            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            List<RoomHallIndex> adjacents = new List<RoomHallIndex> { new RoomHallIndex(0, true) };
            mockFloor.Setup(p => p.GetAdjacents(new RoomHallIndex(0, false))).Returns(adjacents);
            adjacents = new List<RoomHallIndex> { new RoomHallIndex(0, false), new RoomHallIndex(1, false) };
            mockFloor.Setup(p => p.GetAdjacents(new RoomHallIndex(0, true))).Returns(adjacents);
            adjacents = new List<RoomHallIndex> { new RoomHallIndex(0, true) };
            mockFloor.Setup(p => p.GetAdjacents(new RoomHallIndex(1, false))).Returns(adjacents);
            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            mockMap.Setup(p => p.GetLoc(0)).Returns(new Loc(3, 3));

            Mock<List<SpawnableChar>> mockSpawns = new Mock<List<SpawnableChar>>(MockBehavior.Strict);

            var roomSpawner = new Mock<DueSpawnStep<IViewPlaceableRoomTestContext, SpawnableChar, TestEntryPoint>>(null, 100, true) { CallBase = true };

            const int maxVal = 3;
            const int rooms = 3;
            SpawnList<RoomHallIndex> compare = new SpawnList<RoomHallIndex>
            {
                { new RoomHallIndex(0, false), int.MaxValue / maxVal / rooms * 1 },
                { new RoomHallIndex(0, true), int.MaxValue / maxVal / rooms * 2 },
                { new RoomHallIndex(1, false), int.MaxValue / maxVal / rooms * 3 },
            };

            roomSpawner.Setup(p => p.SpawnRandInCandRooms(mockMap.Object, It.IsAny<SpawnList<RoomHallIndex>>(), mockSpawns.Object, 100));

            roomSpawner.Object.DistributeSpawns(mockMap.Object, mockSpawns.Object);

            roomSpawner.Verify(p => p.SpawnRandInCandRooms(mockMap.Object, It.Is<SpawnList<RoomHallIndex>>(s => s.Equals(compare)), mockSpawns.Object, 100), Times.Exactly(1));
        }

        [Test]
        public void DueSpawnStepStraightABC()
        {
            // order of rooms
            // A=>B=>C
            Mock<IViewPlaceableRoomTestContext> mockMap = new Mock<IViewPlaceableRoomTestContext>(MockBehavior.Strict);

            Mock<FloorPlan> mockFloor = new Mock<FloorPlan>(MockBehavior.Strict);
            mockFloor.SetupGet(p => p.RoomCount).Returns(3);
            Mock<TestFloorPlanGen> startRoom = new Mock<TestFloorPlanGen>(MockBehavior.Strict);
            startRoom.SetupProperty(p => p.Draw);
            startRoom.SetupGet(p => p.Draw).Returns(new Rect(2, 2, 4, 4));
            startRoom.Object.Identifier = 'A';
            mockFloor.Setup(p => p.GetRoomPlan(0)).Returns(new FloorRoomPlan(startRoom.Object, new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomPlan(1)).Returns(new FloorRoomPlan(new TestFloorPlanGen('B'), new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomPlan(2)).Returns(new FloorRoomPlan(new TestFloorPlanGen('C'), new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomHall(new RoomHallIndex(0, false))).Returns(new FloorRoomPlan(startRoom.Object, new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomHall(new RoomHallIndex(1, false))).Returns(new FloorRoomPlan(new TestFloorPlanGen('B'), new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomHall(new RoomHallIndex(2, false))).Returns(new FloorRoomPlan(new TestFloorPlanGen('C'), new ComponentCollection()));

            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            List<RoomHallIndex> adjacents = new List<RoomHallIndex> { new RoomHallIndex(1, false) };
            mockFloor.Setup(p => p.GetAdjacents(new RoomHallIndex(0, false))).Returns(adjacents);
            adjacents = new List<RoomHallIndex> { new RoomHallIndex(0, false), new RoomHallIndex(2, false) };
            mockFloor.Setup(p => p.GetAdjacents(new RoomHallIndex(1, false))).Returns(adjacents);
            adjacents = new List<RoomHallIndex> { new RoomHallIndex(1, false) };
            mockFloor.Setup(p => p.GetAdjacents(new RoomHallIndex(2, false))).Returns(adjacents);
            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            mockMap.Setup(p => p.GetLoc(0)).Returns(new Loc(3, 3));

            Mock<List<SpawnableChar>> mockSpawns = new Mock<List<SpawnableChar>>(MockBehavior.Strict);

            var roomSpawner = new Mock<DueSpawnStep<IViewPlaceableRoomTestContext, SpawnableChar, TestEntryPoint>>(null, 100, false) { CallBase = true };

            const int maxVal = 3;
            const int rooms = 3;
            SpawnList<RoomHallIndex> compare = new SpawnList<RoomHallIndex>
            {
                { new RoomHallIndex(0, false), int.MaxValue / maxVal / rooms * 1 },
                { new RoomHallIndex(1, false), int.MaxValue / maxVal / rooms * 2 },
                { new RoomHallIndex(2, false), int.MaxValue / maxVal / rooms * 3 },
            };

            roomSpawner.Setup(p => p.SpawnRandInCandRooms(mockMap.Object, It.IsAny<SpawnList<RoomHallIndex>>(), mockSpawns.Object, 100));

            roomSpawner.Object.DistributeSpawns(mockMap.Object, mockSpawns.Object);

            roomSpawner.Verify(p => p.SpawnRandInCandRooms(mockMap.Object, It.Is<SpawnList<RoomHallIndex>>(s => s.Equals(compare)), mockSpawns.Object, 100), Times.Exactly(1));
        }

        [Test]
        public void DueSpawnStepMiddleABC()
        {
            // order of rooms
            // A<=B=>C
            Mock<IViewPlaceableRoomTestContext> mockMap = new Mock<IViewPlaceableRoomTestContext>(MockBehavior.Strict);

            Mock<FloorPlan> mockFloor = new Mock<FloorPlan>(MockBehavior.Strict);
            mockFloor.SetupGet(p => p.RoomCount).Returns(3);
            mockFloor.Setup(p => p.GetRoomPlan(0)).Returns(new FloorRoomPlan(new TestFloorPlanGen('A'), new ComponentCollection()));
            Mock<TestFloorPlanGen> startRoom = new Mock<TestFloorPlanGen>(MockBehavior.Strict);
            startRoom.SetupProperty(p => p.Draw);
            startRoom.SetupGet(p => p.Draw).Returns(new Rect(2, 2, 4, 4));
            startRoom.Object.Identifier = 'B';
            mockFloor.Setup(p => p.GetRoomPlan(1)).Returns(new FloorRoomPlan(startRoom.Object, new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomPlan(2)).Returns(new FloorRoomPlan(new TestFloorPlanGen('C'), new ComponentCollection()));

            mockFloor.Setup(p => p.GetRoomHall(new RoomHallIndex(0, false))).Returns(new FloorRoomPlan(new TestFloorPlanGen('A'), new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomHall(new RoomHallIndex(1, false))).Returns(new FloorRoomPlan(startRoom.Object, new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomHall(new RoomHallIndex(2, false))).Returns(new FloorRoomPlan(new TestFloorPlanGen('C'), new ComponentCollection()));

            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            List<RoomHallIndex> adjacents = new List<RoomHallIndex> { new RoomHallIndex(1, false) };
            mockFloor.Setup(p => p.GetAdjacents(new RoomHallIndex(0, false))).Returns(adjacents);
            adjacents = new List<RoomHallIndex> { new RoomHallIndex(0, false), new RoomHallIndex(2, false) };
            mockFloor.Setup(p => p.GetAdjacents(new RoomHallIndex(1, false))).Returns(adjacents);
            adjacents = new List<RoomHallIndex> { new RoomHallIndex(1, false) };
            mockFloor.Setup(p => p.GetAdjacents(new RoomHallIndex(2, false))).Returns(adjacents);
            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            mockMap.Setup(p => p.GetLoc(0)).Returns(new Loc(3, 3));

            Mock<List<SpawnableChar>> mockSpawns = new Mock<List<SpawnableChar>>(MockBehavior.Strict);

            var roomSpawner = new Mock<DueSpawnStep<IViewPlaceableRoomTestContext, SpawnableChar, TestEntryPoint>>(null, 100, false) { CallBase = true };

            const int maxVal = 2;
            const int rooms = 3;
            SpawnList<RoomHallIndex> compare = new SpawnList<RoomHallIndex>
            {
                { new RoomHallIndex(1, false), int.MaxValue / maxVal / rooms * 1 },
                { new RoomHallIndex(0, false), int.MaxValue / maxVal / rooms * 2 },
                { new RoomHallIndex(2, false), int.MaxValue / maxVal / rooms * 2 },
            };

            roomSpawner.Setup(p => p.SpawnRandInCandRooms(mockMap.Object, It.IsAny<SpawnList<RoomHallIndex>>(), mockSpawns.Object, 100));

            roomSpawner.Object.DistributeSpawns(mockMap.Object, mockSpawns.Object);

            roomSpawner.Verify(p => p.SpawnRandInCandRooms(mockMap.Object, It.Is<SpawnList<RoomHallIndex>>(s => s.Equals(compare)), mockSpawns.Object, 100), Times.Exactly(1));
        }

        [Test]
        public void DueSpawnStepLoopABCD()
        {
            // order of rooms
            /* A=>B
               v  v
               C=>D */
            Mock<IViewPlaceableRoomTestContext> mockMap = new Mock<IViewPlaceableRoomTestContext>(MockBehavior.Strict);

            Mock<FloorPlan> mockFloor = new Mock<FloorPlan>(MockBehavior.Strict);
            mockFloor.SetupGet(p => p.RoomCount).Returns(4);
            Mock<TestFloorPlanGen> startRoom = new Mock<TestFloorPlanGen>(MockBehavior.Strict);
            startRoom.SetupProperty(p => p.Draw);
            startRoom.SetupGet(p => p.Draw).Returns(new Rect(2, 2, 4, 4));
            startRoom.Object.Identifier = 'A';
            mockFloor.Setup(p => p.GetRoomPlan(0)).Returns(new FloorRoomPlan(startRoom.Object, new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomPlan(1)).Returns(new FloorRoomPlan(new TestFloorPlanGen('B'), new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomPlan(2)).Returns(new FloorRoomPlan(new TestFloorPlanGen('C'), new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomPlan(3)).Returns(new FloorRoomPlan(new TestFloorPlanGen('D'), new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomHall(new RoomHallIndex(0, false))).Returns(new FloorRoomPlan(startRoom.Object, new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomHall(new RoomHallIndex(1, false))).Returns(new FloorRoomPlan(new TestFloorPlanGen('B'), new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomHall(new RoomHallIndex(2, false))).Returns(new FloorRoomPlan(new TestFloorPlanGen('C'), new ComponentCollection()));
            mockFloor.Setup(p => p.GetRoomHall(new RoomHallIndex(3, false))).Returns(new FloorRoomPlan(new TestFloorPlanGen('D'), new ComponentCollection()));
            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            List<RoomHallIndex> adjacents = new List<RoomHallIndex> { new RoomHallIndex(1, false), new RoomHallIndex(2, false) };
            mockFloor.Setup(p => p.GetAdjacents(new RoomHallIndex(0, false))).Returns(adjacents);
            adjacents = new List<RoomHallIndex> { new RoomHallIndex(0, false), new RoomHallIndex(3, false) };
            mockFloor.Setup(p => p.GetAdjacents(new RoomHallIndex(1, false))).Returns(adjacents);
            adjacents = new List<RoomHallIndex> { new RoomHallIndex(0, false), new RoomHallIndex(3, false) };
            mockFloor.Setup(p => p.GetAdjacents(new RoomHallIndex(2, false))).Returns(adjacents);
            adjacents = new List<RoomHallIndex> { new RoomHallIndex(2, false), new RoomHallIndex(1, false) };
            mockFloor.Setup(p => p.GetAdjacents(new RoomHallIndex(3, false))).Returns(adjacents);
            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            mockMap.Setup(p => p.GetLoc(0)).Returns(new Loc(3, 3));

            Mock<List<SpawnableChar>> mockSpawns = new Mock<List<SpawnableChar>>(MockBehavior.Strict);

            var roomSpawner = new Mock<DueSpawnStep<IViewPlaceableRoomTestContext, SpawnableChar, TestEntryPoint>>(null, 100, false) { CallBase = true };

            const int maxVal = 3;
            const int rooms = 4;
            var compare = new SpawnList<RoomHallIndex>
            {
                { new RoomHallIndex(0, false), int.MaxValue / maxVal / rooms * 1 },
                { new RoomHallIndex(1, false), int.MaxValue / maxVal / rooms * 2 },
                { new RoomHallIndex(2, false), int.MaxValue / maxVal / rooms * 2 },
                { new RoomHallIndex(3, false), int.MaxValue / maxVal / rooms * 3 },
            };

            roomSpawner.Setup(p => p.SpawnRandInCandRooms(mockMap.Object, It.IsAny<SpawnList<RoomHallIndex>>(), mockSpawns.Object, 100));

            roomSpawner.Object.DistributeSpawns(mockMap.Object, mockSpawns.Object);

            roomSpawner.Verify(p => p.SpawnRandInCandRooms(mockMap.Object, It.Is<SpawnList<RoomHallIndex>>(s => s.Equals(compare)), mockSpawns.Object, 100), Times.Exactly(1));
        }

        public struct SpawnableChar : ISpawnable
        {
            public char ID;

            public SpawnableChar(char value)
            {
                this.ID = value;
            }

            public SpawnableChar(SpawnableChar other)
            {
                this.ID = other.ID;
            }

            public ISpawnable Copy() => new SpawnableChar(this);
        }

        public struct TestEntryPoint : IEntrance, IExit
        {
            public TestEntryPoint(int value)
            {
                this.ID = value;
            }

            public TestEntryPoint(TestEntryPoint other)
            {
                this.ID = other.ID;
            }

            public int ID { get; set; }

            public ISpawnable Copy() => new TestEntryPoint(this);
        }
    }
}
