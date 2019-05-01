using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;

namespace RogueElements.Tests
{
    [TestFixture]
    public class SpawnerTest
    {

        //TODO: [Test]
        public void PickerSpawner()
        {
            //mock an IGenContext for the rand
            //verify that the picker spawner result is the same as the picker result
            throw new NotImplementedException();
        }

        //TODO: [Test]
        public void PickerSpawnerRepeat()
        {
            //verify that the picker spawner result is the same as the picker result
            //even if the picker's state changes and repeat calls are made
            throw new NotImplementedException();
        }

        //TODO: [Test]
        public void ContextSpawner()
        {
            //mock an ISpawningGenContext for the rand
            //verify that the amount spawned is expected
            //verify that the results are the expected
            throw new NotImplementedException();
        }

        //TODO: [Test]
        public void ContextSpawnerStateChange()
        {
            //mock an ISpawningGenContext for the rand
            //verify that the state changes for the map
            throw new NotImplementedException();
        }

        //TODO: [Test]
        public void ContextSpawnerCutsShort()
        {
            //mock an ISpawningGenContext for the rand
            //verify that the spawns cut short if they cannot pick
            //do it for the first, and then something that is after first
            throw new NotImplementedException();
        }


        //TODO: [Test]
        public void RandomSpawnStep()
        {
            //set the spawn to return a set list of spawns
            //set map.getallfreetiles to a set list of tiles
            //set random to choose specific tiles out of them
            //verify the proper tiles are removed
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
            //choose freetile count
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(4));
            seq = seq.Returns(chosenRand);
            Mock<IPlaceableRoomTestContext> mockMap = new Mock<IPlaceableRoomTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);
            //get free tiles
            List<Loc> freeLocs = new List<Loc>();
            freeLocs.Add(new Loc(1, 2));
            freeLocs.Add(new Loc(3, 4));
            freeLocs.Add(new Loc(5, 5));
            freeLocs.Add(new Loc(3, 2));
            mockMap.Setup(p => p.GetFreeTiles(new Rect(10, 20, 30, 40))).Returns(freeLocs);
            //expect place item
            mockMap.Setup(p => p.PlaceItem(new Loc(locX, locY), 'a'));

            Mock<IRoomGen> mockRoom = new Mock<IRoomGen>(MockBehavior.Strict);
            mockRoom.SetupGet(p => p.Draw).Returns(new Rect(10,20,30,40));
            FloorRoomPlan roomPlan = new FloorRoomPlan(mockRoom.Object);
            Mock<FloorPlan> mockFloor = new Mock<FloorPlan>(MockBehavior.Strict);
            mockFloor.Setup(p => p.GetRoomHall(new RoomHallIndex(0, false))).Returns(roomPlan);
            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            Mock<RoomSpawnStep<IPlaceableRoomTestContext, char>> roomSpawner = new Mock<RoomSpawnStep<IPlaceableRoomTestContext, char>>();
            roomSpawner.CallBase = true;

            roomSpawner.Object.SpawnInRoom(mockMap.Object, new RoomHallIndex(0, false), 'a');

            //verify the correct placeitem was called
            mockMap.Verify(p => p.PlaceItem(new Loc(locX, locY), 'a'), Times.Exactly(1));
        }

        [Test]
        [TestCase(0, 1, 2, 0, 2, 1, 1)]//evenly distributed
        [TestCase(0, 0, 0, 0, 4, 0, 0)]//all on one room is possible
        [TestCase(2, 2, 2, 2, 0, 0, 4)]//all on another room is possible
        public void RoomSpawnStepSpawnRandInCandRooms(int seq1, int seq2, int seq3, int seq4, int room1s, int room2s, int room3s)
        {
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            //choose freetile count
            Moq.Language.ISetupSequentialResult<int> seq = testRand.SetupSequence(p => p.Next(3));
            seq = seq.Returns(seq1);
            seq = seq.Returns(seq2);
            seq = seq.Returns(seq3);
            seq = seq.Returns(seq4);

            Mock<IPlaceableRoomTestContext> mockMap = new Mock<IPlaceableRoomTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);

            SpawnList<RoomHallIndex> spawningRooms = new SpawnList<RoomHallIndex>();
            spawningRooms.Add(new RoomHallIndex(0, false), 1);
            spawningRooms.Add(new RoomHallIndex(1, false), 1);
            spawningRooms.Add(new RoomHallIndex(2, false), 1);

            //get a list of spawns
            List<char> spawns = new List<char>();
            for (int ii = 0; ii < DirExt.VALID_DIR4.Length; ii++)
                spawns.Add('a');

            Mock<RandomRoomSpawnStep<IPlaceableRoomTestContext, char>> roomSpawner = new Mock<RandomRoomSpawnStep<IPlaceableRoomTestContext, char>>();
            roomSpawner.CallBase = true;
            roomSpawner.Setup(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(0, false), 'a')).Returns(true);
            roomSpawner.Setup(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(1, false), 'a')).Returns(true);
            roomSpawner.Setup(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(2, false), 'a')).Returns(true);

            roomSpawner.Object.SpawnRandInCandRooms(mockMap.Object, spawningRooms, spawns, 100);

            roomSpawner.Verify(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(0, false), 'a'), Times.Exactly(room1s));
            roomSpawner.Verify(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(1, false), 'a'), Times.Exactly(room2s));
            roomSpawner.Verify(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(2, false), 'a'), Times.Exactly(room3s));

            //assert that the right values have been taken out of the lists
            Assert.That(spawningRooms.Count, Is.EqualTo(3));
            Assert.That(spawns.Count, Is.EqualTo(0));
        }

        [Test]
        [TestCase(false, 100, 0)]
        [TestCase(true, 0, 3)]
        [TestCase(true, -100, 3)]
        public void RoomSpawnStepSpawnRandInCandRoomsRemoval(bool successful, int successPercent, int expectedSpawns)
        {
            //proves that you can run out of space
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            //choose freetile count
            testRand.SetupSequence(p => p.Next(3)).Returns(0);
            testRand.SetupSequence(p => p.Next(2)).Returns(0);
            testRand.SetupSequence(p => p.Next(1)).Returns(0);

            Mock<IPlaceableRoomTestContext> mockMap = new Mock<IPlaceableRoomTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);

            SpawnList<RoomHallIndex> spawningRooms = new SpawnList<RoomHallIndex>();
            spawningRooms.Add(new RoomHallIndex(0, false), 1);
            spawningRooms.Add(new RoomHallIndex(1, false), 1);
            spawningRooms.Add(new RoomHallIndex(2, false), 1);

            //get a list of spawns
            List<char> spawns = new List<char>();
            for (int ii = 0; ii < 5; ii++)
                spawns.Add('a');

            Mock<RandomRoomSpawnStep<IPlaceableRoomTestContext, char>> roomSpawner = new Mock<RandomRoomSpawnStep<IPlaceableRoomTestContext, char>>();
            roomSpawner.CallBase = true;
            roomSpawner.Setup(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(0, false), 'a')).Returns(successful);
            roomSpawner.Setup(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(1, false), 'a')).Returns(successful);
            roomSpawner.Setup(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(2, false), 'a')).Returns(successful);

            roomSpawner.Object.SpawnRandInCandRooms(mockMap.Object, spawningRooms, spawns, successPercent);

            roomSpawner.Verify(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(0, false), 'a'), Times.Exactly(1));
            roomSpawner.Verify(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(1, false), 'a'), Times.Exactly(1));
            roomSpawner.Verify(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(2, false), 'a'), Times.Exactly(1));

            Assert.That(spawningRooms.Count, Is.EqualTo(0));
            Assert.That(spawns.Count, Is.EqualTo(5-expectedSpawns));
        }

        [Test]
        public void RoomSpawnStepSpawnRandInCandRoomsChangeChance()
        {
            //proves that the probability diminishes with each success
            Mock<IRandom> testRand = new Mock<IRandom>(MockBehavior.Strict);
            //choose freetile count
            testRand.SetupSequence(p => p.Next(12)).Returns(0);
            testRand.SetupSequence(p => p.Next(10)).Returns(2);
            testRand.SetupSequence(p => p.Next(8)).Returns(0);
            testRand.SetupSequence(p => p.Next(7)).Returns(0);

            Mock<IPlaceableRoomTestContext> mockMap = new Mock<IPlaceableRoomTestContext>(MockBehavior.Strict);
            mockMap.SetupGet(p => p.Rand).Returns(testRand.Object);

            SpawnList<RoomHallIndex> spawningRooms = new SpawnList<RoomHallIndex>();
            spawningRooms.Add(new RoomHallIndex(0, false), 4);
            spawningRooms.Add(new RoomHallIndex(1, false), 4);
            spawningRooms.Add(new RoomHallIndex(2, false), 4);

            //get a list of spawns
            List<char> spawns = new List<char>();
            for (int ii = 0; ii < 5; ii++)
                spawns.Add('a');

            Mock<RandomRoomSpawnStep<IPlaceableRoomTestContext, char>> roomSpawner = new Mock<RandomRoomSpawnStep<IPlaceableRoomTestContext, char>>();
            roomSpawner.CallBase = true;
            roomSpawner.Setup(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(0, false), 'a')).Returns(true);
            roomSpawner.Setup(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(1, false), 'a')).Returns(true);
            roomSpawner.Setup(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(2, false), 'a')).Returns(true);

            roomSpawner.Object.SpawnRandInCandRooms(mockMap.Object, spawningRooms, spawns, 50);

            roomSpawner.Verify(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(0, false), 'a'), Times.Exactly(4));
            roomSpawner.Verify(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(1, false), 'a'), Times.Exactly(1));
            roomSpawner.Verify(p => p.SpawnInRoom(mockMap.Object, new RoomHallIndex(2, false), 'a'), Times.Exactly(0));

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
            mockFloor.Setup(p => p.GetRoom(0)).Returns(new TestFloorPlanGen('A'));
            mockFloor.Setup(p => p.GetRoom(1)).Returns(new TestFloorPlanGen('B'));
            mockFloor.Setup(p => p.GetRoom(2)).Returns(new TestFloorPlanGen('C'));
            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            Mock<List<char>> mockSpawns = new Mock<List<char>>(MockBehavior.Strict);

            Mock<RandomRoomSpawnStep<IPlaceableRoomTestContext, char>> roomSpawner = new Mock<RandomRoomSpawnStep<IPlaceableRoomTestContext, char>>();
            roomSpawner.CallBase = true;

            SpawnList<RoomHallIndex> compare = new SpawnList<RoomHallIndex>();
            compare.Add(new RoomHallIndex(0, false));
            compare.Add(new RoomHallIndex(1, false));
            compare.Add(new RoomHallIndex(2, false));

            roomSpawner.Setup(p => p.SpawnRandInCandRooms(mockMap.Object, It.IsAny<SpawnList<RoomHallIndex>>(), mockSpawns.Object, 100));
            
            roomSpawner.Object.DistributeSpawns(mockMap.Object, mockSpawns.Object);
            
            roomSpawner.Verify(p => p.SpawnRandInCandRooms(mockMap.Object, It.Is<SpawnList<RoomHallIndex>>(s => s.Equals(compare)), mockSpawns.Object, 100), Times.Exactly(1));
        }
        

        [Test]
        public void TerminalSpawnStep()
        {
            //tests eligibility of terminals
            Mock<IPlaceableRoomTestContext> mockMap = new Mock<IPlaceableRoomTestContext>(MockBehavior.Strict);

            Mock<FloorPlan> mockFloor = new Mock<FloorPlan>(MockBehavior.Strict);
            mockFloor.SetupGet(p => p.RoomCount).Returns(4);
            mockFloor.Setup(p => p.GetRoom(0)).Returns(new TestFloorPlanGen('A'));
            mockFloor.Setup(p => p.GetRoom(1)).Returns(new TestFloorPlanGen('B'));
            mockFloor.Setup(p => p.GetRoom(2)).Returns(new TestFloorPlanGen('C'));
            mockFloor.Setup(p => p.GetRoom(3)).Returns(new TestFloorPlanGen('D'));
            List<int> adjacents = new List<int>();
            mockFloor.Setup(p => p.GetAdjacentRooms(0)).Returns(adjacents);
            adjacents = new List<int>();
            adjacents.Add(2);
            mockFloor.Setup(p => p.GetAdjacentRooms(1)).Returns(adjacents);
            adjacents = new List<int>();
            adjacents.Add(1);
            adjacents.Add(3);
            mockFloor.Setup(p => p.GetAdjacentRooms(2)).Returns(adjacents);
            adjacents = new List<int>();
            adjacents.Add(2);
            mockFloor.Setup(p => p.GetAdjacentRooms(3)).Returns(adjacents);
            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            Mock<List<char>> mockSpawns = new Mock<List<char>>(MockBehavior.Strict);


            Mock<TerminalSpawnStep<IPlaceableRoomTestContext, char>> roomSpawner = new Mock<TerminalSpawnStep<IPlaceableRoomTestContext, char>>();
            roomSpawner.CallBase = true;

            SpawnList<RoomHallIndex> compare1 = new SpawnList<RoomHallIndex>();
            compare1.Add(new RoomHallIndex(1, false));
            compare1.Add(new RoomHallIndex(3, false));
            SpawnList<RoomHallIndex> compare2 = new SpawnList<RoomHallIndex>();
            compare2.Add(new RoomHallIndex(0, false));
            compare2.Add(new RoomHallIndex(1, false));
            compare2.Add(new RoomHallIndex(2, false));
            compare2.Add(new RoomHallIndex(3, false));

            roomSpawner.Setup(p => p.SpawnRandInCandRooms(mockMap.Object, It.IsAny<SpawnList<RoomHallIndex>>(), mockSpawns.Object, It.IsAny<int>()));

            roomSpawner.Object.DistributeSpawns(mockMap.Object, mockSpawns.Object);

            roomSpawner.Verify(p => p.SpawnRandInCandRooms(mockMap.Object, It.Is<SpawnList<RoomHallIndex>>(s => s.Equals(compare1)), mockSpawns.Object, 0), Times.Exactly(1));
            roomSpawner.Verify(p => p.SpawnRandInCandRooms(mockMap.Object, It.Is<SpawnList<RoomHallIndex>>(s => s.Equals(compare2)), mockSpawns.Object, 100), Times.Exactly(1));
        }

        [Test]
        public void DueSpawnStepStraightABC()
        {
            //A=>B=>C order of rooms
            Mock<IViewPlaceableRoomTestContext> mockMap = new Mock<IViewPlaceableRoomTestContext>(MockBehavior.Strict);

            Mock<FloorPlan> mockFloor = new Mock<FloorPlan>(MockBehavior.Strict);
            mockFloor.SetupGet(p => p.RoomCount).Returns(3);
            Mock<TestFloorPlanGen> startRoom = new Mock<TestFloorPlanGen>(MockBehavior.Strict);
            startRoom.Object.Identifier = 'A';
            startRoom.SetupGet(p => p.Draw).Returns(new Rect(2, 2, 4, 4));
            mockFloor.Setup(p => p.GetRoom(0)).Returns(startRoom.Object);
            mockFloor.Setup(p => p.GetRoom(1)).Returns(new TestFloorPlanGen('B'));
            mockFloor.Setup(p => p.GetRoom(2)).Returns(new TestFloorPlanGen('C'));
            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            List<int> adjacents = new List<int>();
            adjacents.Add(1);
            mockFloor.Setup(p => p.GetAdjacentRooms(0)).Returns(adjacents);
            adjacents = new List<int>();
            adjacents.Add(0);
            adjacents.Add(2);
            mockFloor.Setup(p => p.GetAdjacentRooms(1)).Returns(adjacents);
            adjacents = new List<int>();
            adjacents.Add(1);
            mockFloor.Setup(p => p.GetAdjacentRooms(2)).Returns(adjacents);
            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            mockMap.Setup(p => p.GetLoc(0)).Returns(new Loc(3,3));

            Mock<List<char>> mockSpawns = new Mock<List<char>>(MockBehavior.Strict);


            Mock<DueSpawnStep<IViewPlaceableRoomTestContext, char, int>> roomSpawner = new Mock<DueSpawnStep<IViewPlaceableRoomTestContext, char, int>>();
            roomSpawner.CallBase = true;
            roomSpawner.Object.SuccessPercent = 100;

            int maxVal = 3;
            int rooms = 3;
            SpawnList<RoomHallIndex> compare = new SpawnList<RoomHallIndex>();
            compare.Add(new RoomHallIndex(0, false), Int32.MaxValue / maxVal / rooms * 1);
            compare.Add(new RoomHallIndex(1, false), Int32.MaxValue / maxVal / rooms * 2);
            compare.Add(new RoomHallIndex(2, false), Int32.MaxValue / maxVal / rooms * 3);

            roomSpawner.Setup(p => p.SpawnRandInCandRooms(mockMap.Object, It.IsAny<SpawnList<RoomHallIndex>>(), mockSpawns.Object, 100));

            roomSpawner.Object.DistributeSpawns(mockMap.Object, mockSpawns.Object);

            roomSpawner.Verify(p => p.SpawnRandInCandRooms(mockMap.Object, It.Is<SpawnList<RoomHallIndex>>(s => s.Equals(compare)), mockSpawns.Object, 100), Times.Exactly(1));
        }


        [Test]
        public void DueSpawnStepMiddleABC()
        {
            //A<=B=>C order of rooms
            Mock<IViewPlaceableRoomTestContext> mockMap = new Mock<IViewPlaceableRoomTestContext>(MockBehavior.Strict);

            Mock<FloorPlan> mockFloor = new Mock<FloorPlan>(MockBehavior.Strict);
            mockFloor.SetupGet(p => p.RoomCount).Returns(3);
            mockFloor.Setup(p => p.GetRoom(0)).Returns(new TestFloorPlanGen('A'));
            Mock<TestFloorPlanGen> startRoom = new Mock<TestFloorPlanGen>(MockBehavior.Strict);
            startRoom.Object.Identifier = 'B';
            startRoom.SetupGet(p => p.Draw).Returns(new Rect(2, 2, 4, 4));
            mockFloor.Setup(p => p.GetRoom(1)).Returns(startRoom.Object);
            mockFloor.Setup(p => p.GetRoom(2)).Returns(new TestFloorPlanGen('C'));
            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            List<int> adjacents = new List<int>();
            adjacents.Add(1);
            mockFloor.Setup(p => p.GetAdjacentRooms(0)).Returns(adjacents);
            adjacents = new List<int>();
            adjacents.Add(0);
            adjacents.Add(2);
            mockFloor.Setup(p => p.GetAdjacentRooms(1)).Returns(adjacents);
            adjacents = new List<int>();
            adjacents.Add(1);
            mockFloor.Setup(p => p.GetAdjacentRooms(2)).Returns(adjacents);
            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            mockMap.Setup(p => p.GetLoc(0)).Returns(new Loc(3, 3));

            Mock<List<char>> mockSpawns = new Mock<List<char>>(MockBehavior.Strict);


            Mock<DueSpawnStep<IViewPlaceableRoomTestContext, char, int>> roomSpawner = new Mock<DueSpawnStep<IViewPlaceableRoomTestContext, char, int>>();
            roomSpawner.CallBase = true;
            roomSpawner.Object.SuccessPercent = 100;

            int maxVal = 2;
            int rooms = 3;
            SpawnList<RoomHallIndex> compare = new SpawnList<RoomHallIndex>();
            compare.Add(new RoomHallIndex(0, false), Int32.MaxValue / maxVal / rooms * 2);
            compare.Add(new RoomHallIndex(1, false), Int32.MaxValue / maxVal / rooms * 1);
            compare.Add(new RoomHallIndex(2, false), Int32.MaxValue / maxVal / rooms * 2);

            roomSpawner.Setup(p => p.SpawnRandInCandRooms(mockMap.Object, It.IsAny<SpawnList<RoomHallIndex>>(), mockSpawns.Object, 100));

            roomSpawner.Object.DistributeSpawns(mockMap.Object, mockSpawns.Object);

            roomSpawner.Verify(p => p.SpawnRandInCandRooms(mockMap.Object, It.Is<SpawnList<RoomHallIndex>>(s => s.Equals(compare)), mockSpawns.Object, 100), Times.Exactly(1));
        }


        [Test]
        public void DueSpawnStepLoopABCD()
        {
            //A=>B
            //v  v
            //C=>D order of rooms
            Mock<IViewPlaceableRoomTestContext> mockMap = new Mock<IViewPlaceableRoomTestContext>(MockBehavior.Strict);

            Mock<FloorPlan> mockFloor = new Mock<FloorPlan>(MockBehavior.Strict);
            mockFloor.SetupGet(p => p.RoomCount).Returns(4);
            Mock<TestFloorPlanGen> startRoom = new Mock<TestFloorPlanGen>(MockBehavior.Strict);
            startRoom.Object.Identifier = 'A';
            startRoom.SetupGet(p => p.Draw).Returns(new Rect(2, 2, 4, 4));
            mockFloor.Setup(p => p.GetRoom(0)).Returns(startRoom.Object);
            mockFloor.Setup(p => p.GetRoom(1)).Returns(new TestFloorPlanGen('B'));
            mockFloor.Setup(p => p.GetRoom(2)).Returns(new TestFloorPlanGen('C'));
            mockFloor.Setup(p => p.GetRoom(3)).Returns(new TestFloorPlanGen('D'));
            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            List<int> adjacents = new List<int>();
            adjacents.Add(1);
            adjacents.Add(2);
            mockFloor.Setup(p => p.GetAdjacentRooms(0)).Returns(adjacents);
            adjacents = new List<int>();
            adjacents.Add(0);
            adjacents.Add(3);
            mockFloor.Setup(p => p.GetAdjacentRooms(1)).Returns(adjacents);
            adjacents = new List<int>();
            adjacents.Add(0);
            adjacents.Add(3);
            mockFloor.Setup(p => p.GetAdjacentRooms(2)).Returns(adjacents);
            adjacents = new List<int>();
            adjacents.Add(2);
            adjacents.Add(1);
            mockFloor.Setup(p => p.GetAdjacentRooms(3)).Returns(adjacents);
            mockMap.SetupGet(p => p.RoomPlan).Returns(mockFloor.Object);

            mockMap.Setup(p => p.GetLoc(0)).Returns(new Loc(3, 3));

            Mock<List<char>> mockSpawns = new Mock<List<char>>(MockBehavior.Strict);


            Mock<DueSpawnStep<IViewPlaceableRoomTestContext, char, int>> roomSpawner = new Mock<DueSpawnStep<IViewPlaceableRoomTestContext, char, int>>();
            roomSpawner.CallBase = true;
            roomSpawner.Object.SuccessPercent = 100;

            int maxVal = 3;
            int rooms = 4;
            SpawnList<RoomHallIndex> compare = new SpawnList<RoomHallIndex>();
            compare.Add(new RoomHallIndex(0, false), Int32.MaxValue / maxVal / rooms * 1);
            compare.Add(new RoomHallIndex(1, false), Int32.MaxValue / maxVal / rooms * 2);
            compare.Add(new RoomHallIndex(2, false), Int32.MaxValue / maxVal / rooms * 2);
            compare.Add(new RoomHallIndex(3, false), Int32.MaxValue / maxVal / rooms * 3);

            roomSpawner.Setup(p => p.SpawnRandInCandRooms(mockMap.Object, It.IsAny<SpawnList<RoomHallIndex>>(), mockSpawns.Object, 100));

            roomSpawner.Object.DistributeSpawns(mockMap.Object, mockSpawns.Object);

            roomSpawner.Verify(p => p.SpawnRandInCandRooms(mockMap.Object, It.Is<SpawnList<RoomHallIndex>>(s => s.Equals(compare)), mockSpawns.Object, 100), Times.Exactly(1));
        }
    }


    public interface IPlaceableRoomTestContext : ITiledGenContext, IFloorPlanGenContext, IPlaceableGenContext<char>
    { }
    
    public interface IViewPlaceableRoomTestContext : ITiledGenContext, IFloorPlanGenContext, IPlaceableGenContext<char>, IViewPlaceableGenContext<int>
    { }
}
