namespace Cello.Apple;

internal static class IORegHelper
{
    internal static void TrySet(IORegProperty property, ReadOnlySpan<char> name, ref short? value, bool withReparseToNegative = true)
    {
        if (property.Name.Span.SequenceEqual(name))
        {
            if (!short.TryParse(property.Value.Span, out short valueM))
            {
                if (withReparseToNegative && ushort.TryParse(property.Value.Span, out ushort valueUM))
                {
                    value = (short)valueUM;
                    return;
                }
                throw new InvalidDataException($"Invalid {name}");
            }
            value = valueM;
        }
    }

    internal static void TrySet(IORegProperty property, ReadOnlySpan<char> name, ref int? value, bool withReparseToNegative = true)
    {
        if (property.Name.Span.SequenceEqual(name))
        {
            if (!int.TryParse(property.Value.Span, out int valueM))
            {
                if (withReparseToNegative && uint.TryParse(property.Value.Span, out uint valueUM))
                {
                    value = (int)valueUM;
                    return;
                }
                throw new InvalidDataException($"Invalid {name}");
            }
            value = valueM;
        }
    }

    internal static void TrySet(IORegProperty property, ReadOnlySpan<char> name, ref long? value, bool withReparseToNegative = true)
    {
        if (property.Name.Span.SequenceEqual(name))
        {
            if (!long.TryParse(property.Value.Span, out long valueM))
            {
                if (withReparseToNegative && ulong.TryParse(property.Value.Span, out ulong valueUM))
                {
                    value = (long)valueUM;
                    return;
                }
                throw new InvalidDataException($"Invalid {name}");
            }
            value = valueM;
        }
    }

    internal static void TrySet(IORegProperty property, ReadOnlySpan<char> name, ref ulong? value)
    {
        if (property.Name.Span.SequenceEqual(name))
        {
            if (!ulong.TryParse(property.Value.Span, out ulong valueM))
            {
                throw new InvalidDataException($"Invalid {name}");
            }
            value = valueM;
        }
    }

    internal static void TrySet(IORegProperty property, ReadOnlySpan<char> name, ref bool? value)
    {
        if (property.Name.Span.SequenceEqual(name))
        {
            switch (property.Value.Span)
            {
                case "Yes":
                    value = true;
                    return;
                case "No":
                    value = false;
                    return;
            }

            throw new InvalidDataException($"Invalid {name}");
        }
    }
}
