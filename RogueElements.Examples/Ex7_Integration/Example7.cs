using System;
using System.Collections.Generic;
using System.Text;
//using RogueSharp;
//using RogueSharp.MapCreation;

namespace RogueElements.Examples.Ex7_Integration
{
    public static class Example7
    {
        public static void Run()
        {
            string title = "7: WIP";
            //ExampleCreationStrategy<Map> exampleCreation = new ExampleCreationStrategy<Map>();


            ////Initialize a 6x4 grid of 10x10 cells.
            //InitGridPlanStep<MapGenContext> startGen = new InitGridPlanStep<MapGenContext>();
            //startGen.CellX = 6;
            //startGen.CellY = 4;

            //startGen.CellWidth = 9;
            //startGen.CellHeight = 9;
            //exampleCreation.Layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(-4, startGen));



            ////Create a path that is composed of a ring around the edge
            //GridPathBranch<MapGenContext> path = new GridPathBranch<MapGenContext>();
            //path.RoomRatio = new RandRange(70);
            //path.BranchRatio = new RandRange(0, 50);

            //SpawnList<RoomGen<MapGenContext>> genericRooms = new SpawnList<RoomGen<MapGenContext>>();
            ////cross
            //genericRooms.Add(new RoomGenSquare<MapGenContext>(new RandRange(4, 8), new RandRange(4, 8)));
            ////round
            //genericRooms.Add(new RoomGenRound<MapGenContext>(new RandRange(5, 9), new RandRange(5, 9)));
            //path.GenericRooms = genericRooms;

            //SpawnList<PermissiveRoomGen<MapGenContext>> genericHalls = new SpawnList<PermissiveRoomGen<MapGenContext>>();
            //genericHalls.Add(new RoomGenAngledHall<MapGenContext>(50));
            //path.GenericHalls = genericHalls;

            //exampleCreation.Layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(-4, path));



            ////Output the rooms into a FloorPlan
            //exampleCreation.Layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(-2, new DrawGridToFloorStep<MapGenContext>()));




            ////Draw the rooms of the FloorPlan onto the tiled map, with 1 TILE padded on each side
            //exampleCreation.Layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(0, new DrawFloorToTileStep<MapGenContext>(1)));



            ////Add the stairs up and down
            //exampleCreation.Layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(2, new FloorStairsStep<MapGenContext, StairsUp, StairsDown>(new StairsUp(), new StairsDown())));


            ////Generate water (specified by user as Terrain 2) with a frequency of 30%, using Perlin Noise in an order of 3.
            //int terrain = 2;
            //PerlinWaterStep<MapGenContext> waterPostProc = new PerlinWaterStep<MapGenContext>(40, 3, new Tile(terrain));
            //exampleCreation.Layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(3, waterPostProc));

            ////Remove walls where diagonals of water exist and replace with water
            //exampleCreation.Layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(4, new DropDiagonalBlockStep<MapGenContext>(new Tile(terrain))));
            ////Remove water stuck in the walls
            //exampleCreation.Layout.GenSteps.Add(new GenPriority<GenStep<MapGenContext>>(4, new EraseIsolatedStep<MapGenContext>(new Tile(terrain))));





            ////Run the generator and print
            //exampleCreation.Seed = MathUtils.Rand.NextUInt64();
            //Map map = Map.Create(exampleCreation);
            //Print(map, title);
        }
    }
}
