using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Perplex.ContentBlocks.PropertyEditor.Value;

public class ContentBlocksValueRefiner(IContentTypeService contentTypeService)
{
    /// <summary>
    /// Refines the data of a <see cref="ContentBlocksValue"/> by ensuring that all block items have the correct property values.
    /// </summary>
    /// <param name="model">Model to refine</param>
    public void Refine(ContentBlocksValue model)
    {
        var elementTypes = GetElementTypes(model);

        Refine(model.Header, elementTypes);

        foreach (var block in model.Blocks ?? [])
        {
            Refine(block, elementTypes);
        }
    }

    private static void Refine(ContentBlockValue? block, Dictionary<Guid, IContentType> elementTypes)
    {
        if (block is null)
        {
            return;
        }

        Refine(block.Content, elementTypes);

        foreach (var variant in block.Variants ?? [])
        {
            Refine(variant.Content, elementTypes);
        }
    }

    private static void Refine(BlockItemData? block, Dictionary<Guid, IContentType> elementTypes)
    {
        if (block is null ||
            !elementTypes.TryGetValue(block.ContentTypeKey, out var contentType))
        {
            return;
        }

        Refine(block, contentType);
    }

    private Dictionary<Guid, IContentType> GetElementTypes(ContentBlocksValue model)
    {
        var contentTypeKeys = GetContentTypeKeys(model);
        return contentTypeService.GetAll(contentTypeKeys).ToDictionary(b => b.Key);
    }

    private static void Refine(BlockItemData block, IContentType elementType)
    {
        block.ContentTypeAlias = elementType.Alias;

        foreach (IPropertyType propType in elementType.CompositionPropertyTypes)
        {
            if (block.PropertyValues.ContainsKey(propType.Alias))
            {
                continue;
            }

            if (block.RawPropertyValues.TryGetValue(propType.Alias, out var rawValue))
            {
                // Raw value exists, use it
                block.PropertyValues[propType.Alias] = new BlockItemData.BlockPropertyValue(rawValue, propType);
                continue;
            }

            // No value exists, ensure we add a NULL value for both.
            block.PropertyValues[propType.Alias] = new BlockItemData.BlockPropertyValue(null, propType);
            block.RawPropertyValues[propType.Alias] = null;
        }
    }

    private static HashSet<Guid> GetContentTypeKeys(ContentBlocksValue model)
    {
        var keys = new HashSet<Guid>();

        AddKeys(model.Header, keys);

        foreach (var block in model.Blocks ?? [])
        {
            AddKeys(block, keys);
        }

        return keys;

        static void AddKeys(ContentBlockValue? block, HashSet<Guid> keys)
        {
            if (block is null)
            {
                return;
            }

            if (block?.Content?.ContentTypeKey is Guid key)
            {
                keys.Add(key);
            }

            foreach (var variant in block?.Variants ?? [])
            {
                if (variant.Content?.ContentTypeKey is Guid variantKey)
                {
                    keys.Add(variantKey);
                }
            }
        }
    }
}
