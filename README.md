# RogueElements #


RogueElements is a C# library that allows the user to randomly generate maps for use in roguelikes.  Generation is implemented in a series of interchangeable steps, similar to shader passes.  These steps all share a base class, which the user can inherit to make their own steps.  Additionally, RogueElements contains a collection of functions designed to make working with 4-directional and 8-directional tile maps more convenient.

RogueElements does NOT provide a base engine for the gameplay of an actual roguelike; that's for the game developers themselves to decide on.  A map generation context specified by the developer is all that is needed to integrate the library with their game.  It will inherit all interfaces that the developer is interested in to allow the correct steps to apply to it.



# Overview #


The library revolves around 3 major classes:
`MapGen`, `GenStep`, and `IGenContext`.

`IGenContext` is an interface that represents the map you wish to generate.  Implement it with your own user-defined `MapGenContext` class so that it can be passed into RogueElements's other classes.  Other interfaces in RogueElements inherit from `IGenContext`, and specify more features that RogueElements will be allowed to interact with.  For example, implementing `ITiledGenContext` indicates that your `MapGenContext` class has tiles that can be get and set.

`GenStep` is a class with a single Apply function, which will perform an operation on any IGenContext passed in (specified by its class parameter).  Many `GenStep`s have constraints on what kind of `IGenContext` they will accept.  For example, `PerlinWaterStep` will randomly generate user-specified water terrain on the map using Perlin Noise, but it only allows classes implementing `ITiledGenContext` as its parameter.

`MapGen` is the class that generates the map.  Add `GenStep`s to the GenSteps list, then call the method `GenMap()` to output a `MapGenContext`.


The flow of map generation resembles a shader pipeline:

<p align="center"><img src="https://i.imgur.com/CgNN8mS.png"></p>

An example map generation pipeline.  The `GenStep`s can be swapped in and out.

* `InitFloorPlanStep<MapGenContext>`: Initializes a list of rooms (A `FloorPlan`).
* `FloorPathBranch<MapGenContext>`: Creates the shape of the path of rooms in the grid as a minimum spanning tree.
* `ConnectRoomStep<MapGenContext>`: Randomly connects adjacent rooms in the `FloorPlan`.
* `DrawFloorToTileStep<MapGenContext>`: Draws the list of freehand rooms onto the actual map tiles.
* `FloorStairsStep<MapGenContext, StairsUp, StairsDown>`: For adding an up and down stairs to your map.  You must provide the StairsUp and StairsDown classes.
* `PerlinWaterStep<MapGenContext>`: For generating water patterns on your map using Perlin Noise.
* `RandomSpawnStep<MapGenContext, Item>`: For distributing items across the floor in a random pattern.  You must provide the Item class.
* `RandomSpawnStep<MapGenContext, Mob>`: For distributing items across the floor in a random pattern.  You must provide the Mob class.


RogueElements.Examples contains examples of how to set up a `MapGen`.  Each example builds on the previous one.



# Credits #

- [Brogue](https://sites.google.com/site/broguegame/): A major inspiration in itemizing steps to generate dungeon maps.
- [Spike Chunsoft Mystery Dungeon Series](http://www.spike-chunsoft.co.jp/) - Several floor layouts used as a reference for grid-based floor steps.
- [RogueSharp](https://bitbucket.org/FaronBracy/roguesharp) - A C# library dedicated to creating a full roguelike, used as an example for integrating RogueElements.

