namespace Cello.Apple;

internal readonly record struct IORegObject(ReadOnlyMemory<char> Name, ReadOnlyMemory<char> Id);
