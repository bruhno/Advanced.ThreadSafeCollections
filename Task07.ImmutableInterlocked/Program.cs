using System.Collections.Immutable;

var dict = ImmutableDictionary<int, string>.Empty;

ImmutableInterlocked.TryAdd(ref dict, 1, "One");