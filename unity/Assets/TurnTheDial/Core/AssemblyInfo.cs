using System.Runtime.CompilerServices;

// Test hooks on Fight (FightTestHooks.cs) are internal: only the Core tests can set board state directly.
[assembly: InternalsVisibleTo("TurnTheDial.Core.Tests")]
