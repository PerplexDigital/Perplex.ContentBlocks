using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;

namespace Perplex.ContentBlocks.PropertyEditor.Value;

public class ContentBlocksValueDeserializer(IJsonSerializer jsonSerializer, IContentTypeService contentTypeService)
{
    /// <summary>
    /// Deserializes the given JSON to an instance of <see cref="ContentBlocksValue"/>.
    /// </summary>
    /// <param name="json">JSON to deserialize</param>
    /// <returns></returns>
    public ContentBlocksValue? Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            var value = jsonSerializer.Deserialize<ContentBlocksValue>(json);
            if (value is null)
            {
                return null;
            }

            Refine(value);

            return value;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Refines the given <see cref="ContentBlocksValue"/>. This will remove invalid blocks (e.g. due to missing element types) and populates metadata such as content type aliases and property types for all blocks in the given model.
    /// This data is required for e.g. <see cref="IDataValueEditor.ToEditor(IProperty, string?, string?)"/> of all contained property editors to work correctly.
    /// </summary>
    /// <param name="model"></param>
    private void Refine(ContentBlocksValue model)
    {
        var elementTypes = GetElementTypes(model);

        ContentBlocksValueUtils.Modify(model, RefineBlock);

        ContentBlockValue? RefineBlock(ContentBlockValue? block)
        {
            if (block?.Content?.ContentTypeKey is not Guid contentTypeKey ||
                !elementTypes.TryGetValue(contentTypeKey, out var elementType))
            {
                // Remove this block
                return null;
            }

            PopulateMetadata(block.Content, elementType);

            return block;
        }
    }

    private static void PopulateMetadata(BlockItemData block, IContentType elementType)
    {
        block.ContentTypeAlias = elementType.Alias;

        var propertiesByAlias = elementType.CompositionPropertyTypes.ToDictionary(pt => pt.Alias);

        foreach (var blockValue in block.Values)
        {
            if (propertiesByAlias.TryGetValue(blockValue.Alias, out var propType))
            {
                blockValue.PropertyType = propType;
            }
        }
    }

    private Dictionary<Guid, IContentType> GetElementTypes(ContentBlocksValue model)
    {
        var contentTypeKeys = GetContentTypeKeys(model);

        return contentTypeService
            .GetMany(contentTypeKeys)
            .ToDictionary(ct => ct.Key);

        static HashSet<Guid> GetContentTypeKeys(ContentBlocksValue model)
        {
            var keys = new HashSet<Guid>();

            ContentBlocksValueUtils.Iterate(model, block => AddKey(block.Content, keys));

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
