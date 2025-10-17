namespace Perplex.ContentBlocks.PropertyEditor.Value;

/// <summary>
/// Convenience methods to operate on a <see cref="ContentBlocksValue"/>.
/// </summary>
public static class ContentBlocksValueUtils
{
    /// <summary>
    /// Iterate the header and blocks of the given <see cref="ContentBlockValue"/>.
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

    /// <summary>
    /// Iterate the header and blocks of the given <see cref="ContentBlockValue"/>.
    /// Returns the results of all callbacks as an IEnumerable[T].
    /// </summary>
    /// <param name="value">Value to iterate</param>
    /// <param name="blockFn">Callback for each block value</param>
    public static IEnumerable<T> Map<T>(ContentBlocksValue? value, Func<ContentBlockValue, T> blockFn)
    {
        if (value is null)
        {
            return [];
        }

        return [
            .. Map(value.Header, blockFn),
            .. Map(value.Blocks, blockFn)
        ];
    }

    private static T[] Map<T>(IEnumerable<ContentBlockValue?>? values, Func<ContentBlockValue, T> blockFn)
        => values?.SelectMany(value => Map(value, blockFn))?.ToArray() ?? [];

    private static IEnumerable<T> Map<T>(ContentBlockValue? value, Func<ContentBlockValue, T> blockFn)
    {
        if (value is null)
        {
            yield break;
        }

        yield return blockFn(value);
    }

    /// <summary>
    /// Modifies all blocks in the given <see cref="ContentBlockValue"/> based on the given <paramref name="modifyFn"/>.
    /// If the new value is <c>null</c> the block will be removed.
    /// </summary>
    /// <param name="value">Value to modify</param>
    /// <param name="modifyFn">Modification function for each block</param>
    public static void Modify(ContentBlocksValue value, Func<ContentBlockValue, ContentBlockValue?> modifyFn)
    {
        if (value.Header is ContentBlockValue header)
        {
            value.Header = modifyFn(header);
        }

        if (value.Blocks?.Count > 0)
        {
            var i = value.Blocks.Count;

            while (i-- > 0)
            {
                var block = value.Blocks[i];
                var newBlock = modifyFn(block);

                if (newBlock is null)
                {
                    value.Blocks.RemoveAt(i);
                    continue;
                }

                value.Blocks[i] = newBlock;
            }
        }
    }
}
