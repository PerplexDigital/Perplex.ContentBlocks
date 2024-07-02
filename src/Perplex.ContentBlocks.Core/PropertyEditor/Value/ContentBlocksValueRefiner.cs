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

        ContentBlocksValueIterator.Iterate(model,
            block => Refine(block.Content, elementTypes),
            variant => Refine(variant.Content, elementTypes));
    }

    private static void Refine(BlockItemData? block, Dictionary<Guid, IContentType> elementTypes)
    {
        if (block is null || !elementTypes.TryGetValue(block.ContentTypeKey, out var elementType))
        {
            return;
        }

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

    private Dictionary<Guid, IContentType> GetElementTypes(ContentBlocksValue model)
    {
        var contentTypeKeys = GetContentTypeKeys(model);

        return contentTypeService
            .GetAll(contentTypeKeys)
            .ToDictionary(b => b.Key);

        static HashSet<Guid> GetContentTypeKeys(ContentBlocksValue model)
        {
            var keys = new HashSet<Guid>();

            ContentBlocksValueIterator.Iterate(model,
                block => AddKey(block.Content, keys),
                variant => AddKey(variant.Content, keys));

            return keys;

            static void AddKey(BlockItemData? block, HashSet<Guid> keys)
            {
                if (block?.ContentTypeKey is Guid key)
                {
                    keys.Add(key);
                }
            }
        }
    }
}
