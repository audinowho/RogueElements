# Priority Tests

## Overview

Tests for the `Priority` class, which provides multi-level priority ordering for the generation pipeline. Priorities can have multiple numeric levels (e.g., `1.2.3`) enabling fine-grained ordering of generation steps. This is essential for ensuring steps execute in the correct sequence.

## Test Classes

| Class | Description |
|-------|-------------|
| `PriorityTest.cs` | All priority comparison, equality, and indexing tests |

## Key Test Patterns

### Parameterized Comparison Tests
Tests use `[TestCase]` for exhaustive operator testing:
```csharp
[Test]
[TestCase(0, false, false, 1, true)]  // 0 < 1
[TestCase(0, true, false, 1, false)]  // 0 > 1 false
[TestCase(1, true, true, 1, true)]    // 1 >= 1
public void TestOp1(int lhs1, bool gt, bool eq, int rhs1, bool res)
{
    Priority lhs = new Priority(lhs1);
    Priority rhs = new Priority(rhs1);

    if (eq)
    {
        if (gt)
            Assert.That(lhs >= rhs, Is.EqualTo(res));
        else
            Assert.That(lhs <= rhs, Is.EqualTo(res));
    }
    // ...
}
```

### Multi-Level Priority Comparisons
Tests verify correct ordering of hierarchical priorities:
```csharp
[Test]
public void Test0Lt0p0p1()
{
    // Priority(0) < Priority(0, 0, 1)
    Assert.IsTrue(new Priority(0) < new Priority(0, 0, 1));
}

[Test]
public void Test1p0p1Gt1p0p0p1()
{
    // Priority(1, 0, 1) > Priority(1, 0, 0, 1)
    Assert.IsTrue(new Priority(1, 0, 1) > new Priority(1, 0, 0, 1));
}
```

## Behaviors Tested

### Comparison Operators
| Operator | Test Cases |
|----------|------------|
| `<` | Single level, multi-level, edge cases |
| `>` | Single level, multi-level, edge cases |
| `<=` | Includes equality cases |
| `>=` | Includes equality cases |
| `==` | Same values, different lengths, null/invalid |
| `!=` | Inverse of equality |

### Special Cases
- **Negative sub-priorities**: `Priority(0, -1) < Priority(0, 0)`
- **Invalid priorities**: `Priority.Invalid == Priority.Invalid`
- **Null construction**: `Priority(null) == Priority(new int[])` (both empty/invalid)
- **Trailing zeros**: `Priority(0) == Priority(0, 0)` (normalized)

### Indexing and Length
```csharp
[Test]
[TestCase(0, 0)]
[TestCase(1, 1)]
[TestCase(6, 13)]
public void TestIdx(int idx, int val)
{
    Priority priority = new Priority(0, 1, 2, 3, 5, 8, 13);
    Assert.AreEqual(priority[idx], val);
}

[Test]
public void TestLength()
{
    Priority priority = new Priority(0, 1, 2, 3, 5, 8, 13);
    Assert.AreEqual(priority.Length, 7);
}
```

## Example Tests

```csharp
// Multi-level ordering: 0.1 < 1.0
[Test]
public void Test0p1Lt1()
{
    Assert.IsTrue(new Priority(0, 1) < new Priority(1));
}

// Equality with normalization
[Test]
public void Test0Eq0p0()
{
    Assert.IsTrue(new Priority(0) == new Priority(0, 0));
}

// Invalid priority comparison
[Test]
public void TestInvalidCp0(bool gt, bool eq)
{
    Priority lhs = Priority.Invalid;
    Priority rhs = new Priority(0);

    // Invalid priorities never compare true to valid ones
    Assert.IsFalse(lhs < rhs);
    Assert.IsFalse(lhs > rhs);
}
```

## Running Tests

```bash
dotnet test --filter "FullyQualifiedName~PriorityTest"
```

## Adding Tests

1. Use `[TestCase]` for parameterized tests covering multiple scenarios
2. Test both directions of comparisons (A < B implies B > A)
3. Include edge cases: negative values, empty/invalid, different lengths
4. Test boundary conditions for indexing operations
