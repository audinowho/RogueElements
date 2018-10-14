# RogueElements #


RogueElements is a C# library that allows the user to randomly generate maps for use in roguelikes.  It is designed to be flexible, allowing users to fiddle with common generation parameters, taking out or inserting steps into the generation algorithm as they please.  Users can define rooms, and mix-and-match them with path algorithms found out-of-the-box, or inherit the base classes to define their own.  Additionally, RogueElements contains a large collection of simple functions designed to make working with 4-directional and 8-directional tile maps more convenient.

It does NOT provide a base engine for the gameplay of an actual roguelike; that's for the game developers themselves to decide on!  However, the intention behind RogueElements is to make it easy to adopt, playing as nice as possible with any existing code that the developer may have written.  Developers should not need to alter any existing classes or create new ones, with the exception of a map generation context that exposes only the code that they want to the generator.

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
