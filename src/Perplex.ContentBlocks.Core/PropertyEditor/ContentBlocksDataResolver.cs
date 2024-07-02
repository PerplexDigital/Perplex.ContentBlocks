using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Perplex.ContentBlocks.PropertyEditor;

public class ContentBlocksDataResolver(IContentTypeService contentTypeService)
{
    public Dictionary<Guid, BlockItemData> Resolve(ContentBlocksModelValue model)
    {
        var elementTypes = GetElementTypes(model);
        var data = new Dictionary<Guid, BlockItemData>();

        foreach (var item in Resolve(model.Header, elementTypes))
        {
            data[item.id] = item.data;
        }

        if (model.Blocks is not null)
        {
            foreach (var item in model.Blocks.SelectMany(block => Resolve(block, elementTypes)))
            {
                data[item.id] = item.data;
            }
        }

        return data;
    }

    private static IEnumerable<(Guid id, BlockItemData data)> Resolve(ContentBlockModelValue? block, Dictionary<Guid, IContentType> elementTypes)
    {
        if (block is null)
        {
            yield break;
        }

        if (Resolve(block.Content, elementTypes) is BlockItemData data)
        {
            yield return (block.Id, data);
        }

        foreach (var variant in block.Variants ?? [])
        {
            if (Resolve(variant.Content, elementTypes) is BlockItemData variantData)
            {
                yield return (variant.Id, variantData);
            }
        }
    }

    private static BlockItemData? Resolve(BlockItemData? block, Dictionary<Guid, IContentType> elementTypes)
    {
        if (block is null ||
            !elementTypes.TryGetValue(block.ContentTypeKey, out var contentType))
        {
            return null;
        }

        return EnsureProperties(block, contentType);
    }

    private Dictionary<Guid, IContentType> GetElementTypes(ContentBlocksModelValue model)
    {
        var contentTypeKeys = GetContentTypeKeys(model);
        return contentTypeService.GetAll(contentTypeKeys).ToDictionary(b => b.Key);
    }

    private static BlockItemData EnsureProperties(BlockItemData block, IContentType elementType)
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

        return block;
    }

    private static Guid[] GetContentTypeKeys(ContentBlocksModelValue model)
    {
        var keys = new List<Guid>();

        if (model.Header?.Content?.ContentTypeKey is Guid headerKey)
        {
            keys.Add(headerKey);
        }

        foreach (var block in model.Blocks ?? [])
        {
            if (block.Content?.ContentTypeKey is Guid blockKey &&
                !keys.Contains(blockKey))
            {
                keys.Add(blockKey);
            }
        }

        return [.. keys];
    }
}
