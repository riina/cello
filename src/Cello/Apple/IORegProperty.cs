namespace Cello.Apple;

internal readonly record struct IORegProperty(ReadOnlyMemory<char> Name, ReadOnlyMemory<char> Value);
