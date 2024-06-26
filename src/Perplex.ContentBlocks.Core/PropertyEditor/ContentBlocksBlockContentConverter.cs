using System.Text.Json.Nodes;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Umbraco.Cms.Core.Serialization;

namespace Perplex.ContentBlocks.PropertyEditor;

public class ContentBlocksBlockContentConverter(BlockEditorConverter converter, IJsonSerializer serializer)
{
    public IPublishedElement? ConvertToElement(JsonNode? content, PropertyCacheLevel referenceCacheLevel, bool preview)
    {
        if (ConvertToBlockItemData(content) is not BlockItemData data)
        {
            return null;
        }

        return converter.ConvertToElement(data, referenceCacheLevel, preview);
    }

    public BlockItemData? ConvertToBlockItemData(JsonNode? content)
    {
        if (content is null || !serializer.TryDeserialize(content.ToString(), out BlockItemData? data))
        {
            return null;
        }

        return data;
    }
}
