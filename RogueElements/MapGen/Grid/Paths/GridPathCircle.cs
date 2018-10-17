using System;

namespace RogueElements
{
    [Serializable]
    public class GridPathCircle<T> : GridPathStartStepGeneric<T>
        where T : class, IRoomGridGenContext
    {

        public RandRange CircleRoomRatio;

        public RandRange Paths;
        
        public GridPathCircle()
            : base()
        {

        }

        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            if (floorPlan.GridWidth < 3 || floorPlan.GridHeight < 3)
                throw new InvalidOperationException("Not enough room to create path.");

            int maxRooms = 2 * floorPlan.GridWidth + 2 * floorPlan.GridHeight - 4;
            int roomOpen = maxRooms * CircleRoomRatio.Pick(rand) / 100;
            int paths = Paths.Pick(rand);
            if (roomOpen < 1 && paths < 1)
                roomOpen = 1;

            for (int x = 1; x < floorPlan.GridWidth; x++)
            {
                SafeAddHall(new Loc(x - 1, 0), new Loc(x, 0), floorPlan, GenericHalls.Pick(rand), GetDefaultGen());
                SafeAddHall(new Loc(x - 1, floorPlan.GridHeight - 1), new Loc(x, floorPlan.GridHeight - 1), floorPlan, GenericHalls.Pick(rand), GetDefaultGen());
                if (x == 1)
                {
                    RollOpenRoom(rand, floorPlan, new Loc(x - 1, 0), ref roomOpen, ref maxRooms);
                    RollOpenRoom(rand, floorPlan, new Loc(x - 1, floorPlan.GridHeight - 1), ref roomOpen, ref maxRooms);
                }
                RollOpenRoom(rand, floorPlan, new Loc(x, 0), ref roomOpen, ref maxRooms);
                RollOpenRoom(rand, floorPlan, new Loc(x, floorPlan.GridHeight - 1), ref roomOpen, ref maxRooms);
            }

            for (int y = 1; y < floorPlan.GridHeight; y++)
            {
                SafeAddHall(new Loc(0, y - 1), new Loc(0, y), floorPlan, GenericHalls.Pick(rand), GetDefaultGen());
                SafeAddHall(new Loc(floorPlan.GridWidth - 1, y - 1), new Loc(floorPlan.GridWidth - 1, y), floorPlan, GenericHalls.Pick(rand), GetDefaultGen());

                if (y < floorPlan.GridHeight - 1)
                {
                    RollOpenRoom(rand, floorPlan, new Loc(0, y), ref roomOpen, ref maxRooms);
                    RollOpenRoom(rand, floorPlan, new Loc(floorPlan.GridWidth - 1, y), ref roomOpen, ref maxRooms);
                }
            }

            //create inner paths
            for (int pathsMade = 0; pathsMade < paths; pathsMade++)
            {
                Dir4 startDir = (Dir4)(rand.Next() % 4);
                int x = rand.Next() % (floorPlan.GridWidth - 2) + 1;
                int y = rand.Next() % (floorPlan.GridHeight - 2) + 1;
                switch (startDir)
                {
                    case Dir4.Down:
                        y = 0;
                        break;
                    case Dir4.Left:
                        x = floorPlan.GridWidth - 1;
                        break;
                    case Dir4.Up:
                        y = floorPlan.GridHeight - 1;
                        break;
                    case Dir4.Right:
                        x = 0;
                        break;
                }

                Loc wanderer = new Loc(x, y);
                Dir4 prevDir = Dir4.None; // direction of movement
                int pathLength = (startDir.ToAxis() == Axis4.Vert) ? floorPlan.GridHeight - 2 : floorPlan.GridWidth - 2;
                for (int currentLength = 0; currentLength < pathLength; currentLength++)
                {
                    Loc sample = new Loc();
                    Dir4 nextDir = (Dir4)rand.Next(0, 4);
                    bool canMoveHere = false;
                    while (!canMoveHere)
                    {
                        nextDir = (Dir4)(((int)nextDir + 1) % 4);
                        if (nextDir == prevDir)
                            continue;
                        sample = wanderer + nextDir.GetLoc();
                        if (sample.X > 0 && sample.X < floorPlan.GridWidth-1 && sample.Y > 0 && sample.Y < floorPlan.GridHeight-1)
                            canMoveHere = true;
                    }
                    prevDir = nextDir.Reverse();

                    floorPlan.SetConnectingHall(wanderer, sample, GenericHalls.Pick(rand));
                    wanderer = sample;


                    if (!floorPlan.IsRoomOpen(wanderer.X, wanderer.Y))
                    {
                        if (currentLength == pathLength-1)//determine if the room should be default
                        {
                            floorPlan.SetRoomGen(wanderer.X, wanderer.Y, GenericRooms.Pick(rand));
                        }
                        else
                            floorPlan.SetRoomGen(wanderer.X, wanderer.Y, GetDefaultGen());
                    }
                }
            }
        }

        private void RollOpenRoom(IRandom rand, GridPlan floorPlan, Loc loc, ref int roomOpen, ref int maxRooms)
        {
            if (RollRatio(rand, ref roomOpen, ref maxRooms))
                floorPlan.SetRoomGen(loc.X, loc.Y, GenericRooms.Pick(rand));
        }
    }
}
