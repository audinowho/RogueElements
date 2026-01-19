# RogueElements

C# library for procedural roguelike map generation using a pipeline architecture.

## Claude Code Rules

- **Do not commit without explicit user consent** - Always ask before running `git commit`
- **Do not push without explicit user consent** - Always ask before running `git push`

## Quick Start

```bash
# Build
dotnet build RogueElements.sln

# Test
dotnet test RogueElements.Tests/RogueElements.Tests.csproj

# Run examples
dotnet run --project RogueElements.Examples/RogueElements.Examples.csproj
```

## Architecture

Pipeline pattern: `MapGen<T>` orchestrates `GenStep<T>` passes that modify `IGenContext`.

```
MapGen.GenMap(seed)
  → GenStep.Apply(context)  // repeated for each step
  → GenStep.Apply(context)
  → ...
  → returns IGenContext
```

### Core Abstractions

| Class/Interface | Purpose |
|-----------------|---------|
| `MapGen<T>` | Orchestrator - holds priority-ordered GenSteps, calls `GenMap(seed)` |
| `GenStep<T>` | Base class for generation passes - implement `Apply(T map)` |
| `IGenContext` | Base interface for map state - provides `Rand`, `InitSeed()`, `FinishGen()` |
| `Priority` | Ordering mechanism for GenSteps (lower = earlier) |
| `PriorityList<T>` | Container holding GenSteps by Priority |

### Key Context Interfaces

Implement these to enable specific GenStep types:

| Interface | Enables |
|-----------|---------|
| `ITiledGenContext` | Tile-based operations (get/set tiles, wall detection) |
| `IFloorPlanGenContext` | Freeform room placement via `FloorPlan` |
| `IRoomGridGenContext` | Grid-based room layouts via `GridPlan` |
| `IPlaceableGenContext<T>` | Spawning entities (items, stairs, mobs) |

## Key Entry Points

- `RogueElements/MapGen/MapGen.cs` - Main orchestrator
- `RogueElements/MapGen/GenStep.cs` - Base step class
- `RogueElements.Examples/Program.cs` - Interactive examples runner

## Directory Guide

| Directory | Purpose |
|-----------|---------|
| `RogueElements/` | Core library |
| `RogueElements/MapGen/` | Generation pipeline (GenStep, MapGen, contexts) |
| `RogueElements/MapGen/FloorPlan/` | Freeform room-based generation |
| `RogueElements/MapGen/Grid/` | Grid-based room layouts |
| `RogueElements/MapGen/Rooms/` | Room shape generators (RoomGenSquare, RoomGenCave, etc.) |
| `RogueElements/MapGen/Spawning/` | Entity placement (items, stairs, mobs) |
| `RogueElements/MapGen/Tiles/` | Tile manipulation and water generation |
| `RogueElements/Rand/` | RNG utilities (RandRange, SpawnList, noise) |
| `RogueElements/Priority/` | Priority queue for step ordering |
| `RogueElements.Examples/` | 8 progressive examples (Ex1-Ex8) |
| `RogueElements.Tests/` | NUnit tests with Moq |

## Examples Progression

| Example | Concept |
|---------|---------|
| Ex1_Tiles | Static tiles, `InitTilesStep` |
| Ex2_Rooms | Freeform rooms via `FloorPlan` |
| Ex3_Grid | Grid-based layouts via `GridPlan` |
| Ex4_Stairs | Stair placement |
| Ex5_Terrain | Water/terrain via Perlin noise |
| Ex6_Items | Item spawning |
| Ex7_Special | Special room placement |
| Ex8_Integration | Full pipeline combining all concepts |

## Patterns & Conventions

- **Naming**: PascalCase for all public members, `Step` suffix for GenStep subclasses
- **Generics**: GenSteps constrain `T` to required context interfaces
- **Serialization**: All GenSteps are `[Serializable]` for save/load support
- **Testing**: NUnit + Moq, test files mirror source structure
- **Style**: StyleCop + CodeCracker analyzers enforced

## Creating Custom Steps

1. Inherit from `GenStep<T>` where T implements needed interfaces
2. Override `Apply(T map)` with generation logic
3. Add to MapGen via `layout.GenSteps.Add(priority, step)`

```csharp
public class MyStep : GenStep<ITiledGenContext>
{
    public override void Apply(ITiledGenContext map)
    {
        // modify map.Tiles, use map.Rand for randomness
    }
}
```

## Creating Custom Contexts

1. Inherit from `IGenContext` (minimum)
2. Add interfaces as needed (ITiledGenContext, IFloorPlanGenContext, etc.)
3. See `RogueElements.Examples/Common/BaseMapGenContext.cs` for reference

## Deep Dive Documentation

For detailed architecture and code flow documentation, see `docs/claude/`:

| Document | Purpose |
|----------|---------|
| [architecture.md](docs/claude/architecture.md) | Interface hierarchy, GenStep categories, data flow diagrams |
| [flows.md](docs/claude/flows.md) | Traced code paths for key operations |
| [patterns.md](docs/claude/patterns.md) | Step-by-step recipes for common modifications |

## Debug Support

```csharp
GenContextDebug.OnInit += handler;   // Map initialization
GenContextDebug.OnStep += handler;   // Each step execution
GenContextDebug.OnStepIn += handler; // Step entry
GenContextDebug.OnStepOut += handler; // Step exit
```
