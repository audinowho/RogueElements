# RogueElements

[![NuGet](https://img.shields.io/nuget/v/RogueElements.svg)](https://www.nuget.org/packages/RogueElements/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET Standard 2.0](https://img.shields.io/badge/.NET%20Standard-2.0-blue.svg)](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)

**Procedural roguelike map generation library for C#.** Generate dungeons with rooms, corridors, items, enemies, and terrain using a flexible pipeline architecture.

<p align="center"><img src="https://i.imgur.com/0Ir5F6I.gif" alt="RogueElements Debug View"></p>

## Features

- **Pipeline Architecture** - Chain generation steps like shader passes
- **Game-Agnostic** - Integrate with any game engine (Unity, MonoGame, Godot, etc.)
- **Two Layout Modes** - Freeform rooms or grid-based dungeons
- **Extensible** - Create custom room shapes, paths, and spawning logic
- **Deterministic** - Seed-based generation for reproducible maps
- **Well-Tested** - Comprehensive unit test coverage

## Quick Start

```bash
dotnet add package RogueElements
```

```csharp
// 1. Create your map context implementing IGenContext interfaces
public class MyMapContext : ITiledGenContext, IFloorPlanGenContext
{
    // ... implement required members
}

// 2. Build a generation pipeline
var mapGen = new MapGen<MyMapContext>();

// Initialize tiles
mapGen.GenSteps.Add(-1, new InitTilesStep<MyMapContext>(50, 50));

// Create room layout
mapGen.GenSteps.Add(0, new InitFloorPlanStep<MyMapContext>());
mapGen.GenSteps.Add(1, new FloorPathBranch<MyMapContext>());
mapGen.GenSteps.Add(2, new ConnectRoomStep<MyMapContext>());

// Render to tiles
mapGen.GenSteps.Add(3, new DrawFloorToTileStep<MyMapContext>());

// 3. Generate!
MyMapContext map = mapGen.GenMap(seed: 12345);
```

## How It Works

RogueElements uses a **pipeline pattern** where `GenStep` operations progressively build up map state:

<p align="center"><img src="https://i.imgur.com/CgNN8mS.png" alt="Pipeline Diagram"></p>

### Core Classes

| Class | Purpose |
|-------|---------|
| `MapGen<T>` | Orchestrates the generation pipeline |
| `GenStep<T>` | Base class for generation operations |
| `IGenContext` | Interface for map state containers |

### Generation Steps

| Step | Description |
|------|-------------|
| `InitFloorPlanStep` | Initialize room list |
| `FloorPathBranch` | Create branching room paths |
| `ConnectRoomStep` | Add extra room connections |
| `DrawFloorToTileStep` | Render rooms to tiles |
| `FloorStairsStep` | Place entrance/exit stairs |
| `PerlinWaterStep` | Generate water terrain |
| `RandomSpawnStep` | Distribute items and enemies |

## Layout Modes

### Freeform (FloorPlan)
Rooms placed freely with flexible positioning. Best for organic, cave-like dungeons.

### Grid-Based (GridPlan)
Rooms aligned to a grid with cardinal connections. Best for structured, traditional dungeons.

```
Grid Layout Example:
┌───┐   ┌───┐   ┌───┐
│ A │───│ B │───│ C │
└───┘   └─┬─┘   └───┘
          │
        ┌─┴─┐
        │ D │
        └───┘
```

## Examples

The `RogueElements.Examples` project contains 8 progressive tutorials:

```bash
dotnet run --project RogueElements.Examples
```

| Example | Concept |
|---------|---------|
| Ex1 | Basic tiles |
| Ex2 | Freeform rooms |
| Ex3 | Grid layouts |
| Ex4 | Stairs placement |
| Ex5 | Water/terrain |
| Ex6 | Item spawning |
| Ex7 | Special rooms |
| Ex8 | Full integration |

See [RogueElements.Examples/README.md](RogueElements.Examples/README.md) for detailed walkthroughs.

## Documentation

Each folder contains its own README with detailed documentation:

- [**RogueElements/**](RogueElements/README.md) - Core library architecture
- [**MapGen/**](RogueElements/MapGen/README.md) - Pipeline and GenStep system
- [**FloorPlan/**](RogueElements/MapGen/FloorPlan/README.md) - Freeform room generation
- [**Grid/**](RogueElements/MapGen/Grid/README.md) - Grid-based layouts
- [**Spawning/**](RogueElements/MapGen/Spawning/README.md) - Entity placement
- [**Rand/**](RogueElements/Rand/README.md) - RNG and weighted selection

## Integration Examples

RogueElements integrates with popular game libraries:

- **[RogueSharp](https://bitbucket.org/FaronBracy/roguesharp)** - See Ex8_Integration for IMapCreationStrategy pattern
- **Unity** - Implement context interfaces in MonoBehaviour
- **MonoGame** - Direct integration with tile-based rendering

## Credits

- [**Brogue**](https://sites.google.com/site/broguegame/) - Inspiration for step-based dungeon generation
- [**Spike Chunsoft Mystery Dungeon**](http://www.spike-chunsoft.co.jp/) - Reference for grid-based floor layouts
- [**RogueSharp**](https://bitbucket.org/FaronBracy/roguesharp) - C# roguelike library integration example

## License

MIT License - see [LICENSE](LICENSE) for details.

---

![Repobeats](https://repobeats.axiom.co/api/embed/your-hash-here.svg "Repobeats analytics image")
