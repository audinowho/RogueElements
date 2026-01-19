# Contributing to RogueElements

First off, **thank you** for considering contributing! I truly believe in open source and the power of community collaboration. Unlike many repositories, I actively welcome contributions of all kinds - from bug fixes to new features.

## My Promise to Contributors

- **I will respond to every PR and issue** - I guarantee feedback on all contributions
- **Bug fixes are obvious accepts** - If it fixes a bug, it's getting merged
- **New features are welcome** - I'm genuinely open to new ideas and enhancements
- **Direct line of communication** - If I'm not responding to a PR or issue, email me directly at johnvondrashek@gmail.com

## Ways to Contribute

### Bug Fixes
Found a bug in dungeon generation? Room placement acting strange? Stairs spawning in walls? Open a PR - these are obvious accepts.

### New Features
Ideas that would be great additions:
- New `GenStep` implementations (terrain types, room shapes, spawning strategies)
- Additional `RoomGen` variants for different dungeon aesthetics
- Improved corridor algorithms
- New utility classes for procedural generation

### Documentation
- Example tutorials (Ex9 and beyond!)
- Integration guides for game engines (Unity, Godot, MonoGame)
- API documentation improvements

### Tests
More test coverage is always welcome. The project uses NUnit with Moq.

## Getting Started

```bash
# Clone your fork
git clone https://github.com/YOUR_USERNAME/RogueElements.git

# Build
dotnet build RogueElements.sln

# Run tests
dotnet test RogueElements.Tests/RogueElements.Tests.csproj

# Run examples to understand the library
dotnet run --project RogueElements.Examples/RogueElements.Examples.csproj
```

## Pull Request Guidelines

1. **Target `master` branch** for most contributions
2. **Include tests** for new functionality when possible
3. **Follow existing code style** - the project uses StyleCop and CodeCracker analyzers
4. **Keep commits focused** - one logical change per commit
5. **Write clear commit messages** describing the "why"

## Creating Custom GenSteps

If you're adding a new generation step, follow this pattern:

```csharp
[Serializable]
public class MyCustomStep<T> : GenStep<T>
    where T : /* required context interfaces */
{
    public override void Apply(T map)
    {
        // Your generation logic here
        // Use map.Rand for deterministic randomness
    }
}
```

## Code of Conduct

This project follows the [Rule of St. Benedict](CODE_OF_CONDUCT.md) as its code of conduct.

## Questions?

- Open an issue
- Email: johnvondrashek@gmail.com
