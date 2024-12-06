using Perplex.ContentBlocks.PropertyEditor.Configuration;
using Perplex.ContentBlocks.PropertyEditor.Value;
using Perplex.ContentBlocks.Rendering;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Perplex.ContentBlocks.PropertyEditor;

public class ContentBlocksValueConverter(IServiceProvider serviceProvider, ContentBlocksValueDeserializer deserializer, BlockEditorConverter converter)
    : PropertyValueConverterBase
{
    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType)
        => PropertyCacheLevel.Elements;

    public override bool IsConverter(IPublishedPropertyType propertyType)
        => propertyType.EditorAlias == Constants.PropertyEditor.Alias;

    public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview)
    {
        if (deserializer.Deserialize(inter?.ToString()) is not ContentBlocksValue value)
        {
            return Rendering.ContentBlocks.Empty;
        }

        var config = propertyType.DataType.ConfigurationAs<ContentBlocksConfiguration>() ?? ContentBlocksConfiguration.DefaultConfiguration;

        var header = config.Structure.HasFlag(Structure.Header)
            ? CreateViewModel(value.Header)
            : null;

        var blocks = config.Structure.HasFlag(Structure.Blocks)
            ? value.Blocks?.Select(CreateViewModel).OfType<IContentBlockViewModel>().ToArray() ?? []
            : [];

        return new Rendering.ContentBlocks
        {
            Header = header,
            Blocks = blocks
        };

        IContentBlockViewModel? CreateViewModel(ContentBlockValue? block)
        {
            if (block?.Content is null || converter.ConvertToElement(owner, block.Content, referenceCacheLevel, preview) is not IPublishedElement content)
            {
                return null;
            }

            var contentType = content.GetType();
            var genericViewModelFactoryType = typeof(IContentBlockViewModelFactory<>).MakeGenericType([contentType]);

            if (serviceProvider.GetService(genericViewModelFactoryType) is not IContentBlockViewModelFactory viewModelFactory)
            {
                return null;
            }

            return viewModelFactory.Create(content, block.Id, block.DefinitionId, block.LayoutId);
        }
    }

    public override bool? IsValue(object? value, PropertyValueLevel level)
    {
        if (level != PropertyValueLevel.Object)
        {
            // We only want to check at the Object level to prevent duplicate parsing logic
            return null;
        }

        if (value is not IContentBlocks model)
        {
            // Value must be invalid
            return false;
        }

        // Valid with at least 1 block
        return model.Header is not null || model.Blocks.Any();
    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        => typeof(IContentBlocks);
}
