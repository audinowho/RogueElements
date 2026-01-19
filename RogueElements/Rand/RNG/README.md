# RNG

[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg)](../../../../LICENSE)

Core seedable random number generator implementation for deterministic roguelike generation.

## Overview

The `RNG` folder contains the foundational random number generation used throughout RogueElements. The key requirement is **determinism** - given the same seed, the generator must produce identical sequences across platforms and runs.

## Why Deterministic Seeds Matter for Roguelikes

1. **Seed Sharing** - Players can share seeds to experience the same dungeon
2. **Debugging** - Reproduce bugs exactly by saving the seed
3. **Replay Systems** - Record only player inputs, regenerate map from seed
4. **Testing** - Unit tests produce consistent results
5. **Fair Competition** - Daily/weekly challenge runs with identical maps

## Core Classes

### IRandom Interface

The abstraction used throughout the library:

```csharp
public interface IRandom
{
    ulong FirstSeed { get; }        // Original seed for reproduction

    ulong NextUInt64();             // Raw 64-bit value
    int Next();                     // 0 to int.MaxValue
    int Next(int maxValue);         // 0 to maxValue (exclusive)
    int Next(int minValue, int maxValue);  // minValue to maxValue (exclusive)
    double NextDouble();            // 0.0 to 1.0
}
```

### ReRandom

The default RNG implementation using xoshiro256** algorithm:

```csharp
// Create with explicit seed
var rand = new ReRandom(12345UL);

// Create with time-based seed
var rand = new ReRandom();

// Access original seed for reproduction
ulong seed = rand.FirstSeed;

// Generate values
int roll = rand.Next(1, 7);           // 1-6 (d6)
int index = rand.Next(items.Count);   // Random array index
double percent = rand.NextDouble();   // 0.0-1.0
```

**Algorithm Properties:**

- **xoshiro256**** - Modern, fast PRNG with excellent statistical properties
- **256-bit state** - Large enough for any parallel application
- **Sub-nanosecond speed** - Minimal overhead
- **Passes all known tests** - Including BigCrush statistical test suite

### SplitMix64

Used internally to initialize xoshiro256** state from a single seed:

```csharp
// Internal usage in ReRandom constructor
SplitMix64 sm = new SplitMix64(seed);
s[0] = sm.Next();
s[1] = sm.Next();
s[2] = sm.Next();
s[3] = sm.Next();
```

## Usage in Map Generation

Every `IGenContext` provides access to `Rand`:

```csharp
public class MyStep : GenStep<ITiledGenContext>
{
    public override void Apply(ITiledGenContext map)
    {
        // Use map.Rand for all randomness
        int roomWidth = map.Rand.Next(4, 10);
        int roomHeight = map.Rand.Next(4, 10);

        // Never create new Random() instances!
        // This breaks determinism.
    }
}
```

### Initialization Flow

```csharp
// 1. Call GenMap with a seed
MapGenContext context = layout.GenMap(seed);

// 2. Context initializes its RNG
public void InitSeed(ulong seed)
{
    this.Rand = new ReRandom(seed);
}

// 3. All GenSteps use context.Rand
// 4. Same seed = identical map every time
```

## Global Access

For convenience, `MathUtils` provides a global RNG:

```csharp
// Get global RNG instance
IRandom rand = MathUtils.Rand;

// Reseed both RNG and Noise
MathUtils.ReSeedRand(seed);

// Generate initial seed for new maps
ulong newSeed = MathUtils.Rand.NextUInt64();
MapGenContext context = layout.GenMap(newSeed);
```

## Platform Considerations

**Warning about NextDouble():**

```csharp
/// <remarks>
/// Floating point operations, including doubles, are non-deterministic.
/// They will vary by compiler, architecture, etc.
/// Understand the risks before using.
/// </remarks>
public virtual double NextDouble()
```

For strict cross-platform determinism, prefer integer operations:

```csharp
// Instead of: if (rand.NextDouble() < 0.3)
// Use:        if (rand.Next(100) < 30)
```

## Reproducibility Pattern

Save and restore seeds for debugging:

```csharp
// Save seed before generation
ulong seed = MathUtils.Rand.NextUInt64();
Console.WriteLine($"Generating with seed: {seed}");

// Generate map
var context = layout.GenMap(seed);

// Later, reproduce exact same map
var sameContext = layout.GenMap(seed);  // Identical!
```

## See Also

- [Noise/](../Noise/) - Position-based noise (different use case than sequential RNG)
- [IRandPicker](../) - Higher-level random selection utilities
- [MapGen/](../../MapGen/) - How RNG integrates with the generation pipeline

---

![Repobeats analytics](https://repobeats.axiom.co/api/embed/3c5a3b7f5e0c1d8a9b7c5e3a1f9d8b7c6e5a4d3c2b1a0.svg "Repobeats analytics image")
