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

            PopulateMetadata(value);

            return value;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Populates metadata such as content type aliases and property types for all blocks in the given model.
    /// This data is required for e.g. <see cref="IDataValueEditor.ToEditor(IProperty, string?, string?)"/> of all contained property editors to work correctly.
    /// </summary>
    /// <param name="model"></param>
    private void PopulateMetadata(ContentBlocksValue model)
    {
        var elementTypes = GetElementTypes(model);
        ContentBlocksValueIterator.Iterate(model, block => PopulateMetadata(block.Content, elementTypes));
    }

    private static void PopulateMetadata(BlockItemData? block, Dictionary<Guid, IContentType> elementTypes)
    {
        if (block is null || !elementTypes.TryGetValue(block.ContentTypeKey, out var elementType))
        {
            return;
        }

        block.ContentTypeAlias = elementType.Alias;

        var propertiesByAlias = elementType.PropertyTypes.ToDictionary(pt => pt.Alias);

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

            ContentBlocksValueIterator.Iterate(model, block => AddKey(block.Content, keys));

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
