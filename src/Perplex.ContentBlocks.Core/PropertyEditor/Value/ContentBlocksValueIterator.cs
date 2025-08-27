namespace Perplex.ContentBlocks.PropertyEditor.Value;

/// <summary>
/// Helper to iterate over the header and all blocks of a <see cref="ContentBlocksValue"/>.
/// </summary>
public static class ContentBlocksValueIterator
{
    /// <summary>
    /// Iterate all <see cref="ContentBlockValue"/> instances of the given <see cref="ContentBlockValue"/>.
    /// </summary>
    /// <param name="value">Value to iterate</param>
    /// <param name="blockFn">Callback for each block value</param>
    public static void Iterate(ContentBlocksValue? value, Action<ContentBlockValue> blockFn)
    {
        if (value is null)
        {
            return;
        }

        Iterate(value.Header, blockFn);
        Iterate(value.Blocks, blockFn);
    }

    /// <summary>
    /// Iterate all <see cref="ContentBlockValue"/> instances of the given <see cref="ContentBlockValue"/>.
    /// Returns the results of all callbacks as an IEnumerable[T].
    /// </summary>
    /// <param name="value">Value to iterate</param>
    /// <param name="blockFn">Callback for each block value</param>
    public static IEnumerable<T> Iterate<T>(ContentBlocksValue? value, Func<ContentBlockValue, T> blockFn)
    {
        if (value is null)
        {
            return [];
        }

        return [
            .. Iterate(value.Header, blockFn),
            .. Iterate(value.Blocks, blockFn)
        ];
    }

    private static void Iterate(IEnumerable<ContentBlockValue?>? values, Action<ContentBlockValue> blockFn)
    {
        if (values is null)
        {
            return;
        }

        foreach (var value in values)
        {
            Iterate(value, blockFn);
        }
    }

    private static void Iterate(ContentBlockValue? value, Action<ContentBlockValue> blockFn)
    {
        if (value is null)
        {
            return;
        }

        blockFn(value);
    }

    private static T[] Iterate<T>(IEnumerable<ContentBlockValue?>? values, Func<ContentBlockValue, T> blockFn)
        => values?.SelectMany(value => Iterate(value, blockFn))?.ToArray() ?? [];

    private static IEnumerable<T> Iterate<T>(ContentBlockValue? value, Func<ContentBlockValue, T> blockFn)
    {
        if (value is null)
        {
            yield break;
        }

        yield return blockFn(value);
    }
}
