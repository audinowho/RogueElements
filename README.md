# RogueElements #


RogueElements is a C# library that allows the user to randomly generate maps for use in roguelikes.  It breaks the task of generation down into steps, similar to passes on a shader, which can be swapped in and out.  These steps can also be defined by the user, inheriting from a base class.  Additionally, RogueElements contains a collection of functions designed to make working with 4-directional and 8-directional tile maps more convenient.

RogueElements does NOT provide a base engine for the gameplay of an actual roguelike; that's for the game developers themselves to decide on!  A map generation context specified by the developer is all that is needed to integrate the library with their game.  It will inherit all interfaces that the developer is interested in to allow the correct steps to apply to it.

Examples can be found in the project RogueElements.Examples.



# Usage Overview #


Before writing generation code, create a new class `GenContext` that implements from `IGenContext`

To generate a map:
1. Instantiate a `MapGen<GenContext>`.
2. Add several `GenPriority<GenStep<MapGenContext>>` to its GenStep list.
3. Call `MapGen.GenMap()` with the seed parameter of your choice.


A common example of a list of GenSteps:
1. `InitGridPlanStep`: Initializes a grid of rooms (A `GridPlan`).
2. A child of `GridPathStartStep`: Creates the shape of the path of rooms in the grid.
3. `DrawGridToFloorStep`: Turns the grid into a list of freehand rooms on a plane (A `FloorPlan`).
4. `DrawFloorToTileStep`: Draws the list of freehand rooms onto the actual map tiles.
5. `StairsStep`: For adding an up and down stairs to your map.
6. `PerlinWaterStep`: For generating water patterns on your map using Perlin Noise.
7. A child of `BaseSpawnStep`: For distributing items, enemies, traps, etc. across the floor in specified patterns.


# Credits #

- [Brogue](https://sites.google.com/site/broguegame/): A major inspiration in itemizing steps to generate dungeon maps.
- [Spike Chunsoft Mystery Dungeon Series](http://www.spike-chunsoft.co.jp/) - Several floor layouts used as a reference for grid-based floor steps.
- [RogueSharp](https://bitbucket.org/FaronBracy/roguesharp) - A C# library dedicated to creating a full roguelike, used as an example for integrating RogueElements.