namespace Perplex.ContentBlocks.PropertyEditor.Value;

public static class ContentBlocksValueIterator
{
    /// <summary>
    /// Iterate all <see cref="ContentBlockValue"/> instances and <see cref="ContentBlockVariantValue"/> instances of the given <see cref="ContentBlockValue"/>.
    /// </summary>
    /// <param name="value">Value to iterate</param>
    /// <param name="blockFn">Callback for each block value</param>
    /// <param name="variantFn">Callback for each variant value</param>
    public static void Iterate(ContentBlocksValue? value, Action<ContentBlockValue> blockFn, Action<ContentBlockVariantValue> variantFn)
    {
        if (value is null)
        {
            return;
        }

        Iterate(value.Header, blockFn, variantFn);
        Iterate(value.Blocks, blockFn, variantFn);
    }

    /// <summary>
    /// Iterate all <see cref="ContentBlockValue"/> instances and <see cref="ContentBlockVariantValue"/> instances of the given <see cref="ContentBlockValue"/>.
    /// Returns the results of all callbacks as an IEnumerable[T].
    /// </summary>
    /// <param name="value">Value to iterate</param>
    /// <param name="blockFn">Callback for each block value</param>
    /// <param name="variantFn">Callback for each variant value</param>
    public static IEnumerable<T> Iterate<T>(ContentBlocksValue? value, Func<ContentBlockValue, T> blockFn, Func<ContentBlockVariantValue, T> variantFn)
    {
        if (value is null)
        {
            return [];
        }

        return [
            .. Iterate(value.Header, blockFn, variantFn),
            .. Iterate(value.Blocks, blockFn, variantFn)
        ];
    }

    private static void Iterate(IEnumerable<ContentBlockValue?>? values, Action<ContentBlockValue> blockFn, Action<ContentBlockVariantValue> variantFn)
    {
        if (values is null)
        {
            return;
        }

        foreach (var value in values)
        {
            Iterate(value, blockFn, variantFn);
        }
    }

    private static void Iterate(ContentBlockValue? value, Action<ContentBlockValue> blockFn, Action<ContentBlockVariantValue> variantFn)
    {
        if (value is null)
        {
            return;
        }

        blockFn(value);

        if (value.Variants is not null)
        {
            foreach (var variant in value.Variants)
            {
                variantFn(variant);
            }
        }
    }

    private static T[] Iterate<T>(IEnumerable<ContentBlockValue?>? values, Func<ContentBlockValue, T> blockFn, Func<ContentBlockVariantValue, T> variantFn)
        => values?.SelectMany(value => Iterate(value, blockFn, variantFn))?.ToArray() ?? [];

    private static IEnumerable<T> Iterate<T>(ContentBlockValue? value, Func<ContentBlockValue, T> blockFn, Func<ContentBlockVariantValue, T> variantFn)
    {
        if (value is null)
        {
            yield break;
        }

        yield return blockFn(value);

        if (value.Variants is not null)
        {
            foreach (var variant in value.Variants)
            {
                yield return variantFn(variant);
            }
        }
    }
}
