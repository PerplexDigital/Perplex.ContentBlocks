using Perplex.ContentBlocks.PropertyEditor.Configuration;
using Perplex.ContentBlocks.PropertyEditor.Value;
using Perplex.ContentBlocks.Rendering;
using Perplex.ContentBlocks.Variants;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Perplex.ContentBlocks.PropertyEditor;

public class ContentBlocksValueConverter(IServiceProvider serviceProvider, ContentBlocksValueDeserializer deserializer,
    IContentBlockVariantSelector variantSelector, BlockEditorConverter converter)
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

        var interValue = new ContentBlocksInterValue
        {
            Header = SelectBlock(value.Header, owner),
            Blocks = value.Blocks?.Select(block => SelectBlock(block, owner)).OfType<ContentBlockInterValue>().ToArray() ?? [],
        };

        var config = propertyType.DataType.ConfigurationAs<ContentBlocksConfiguration>() ?? ContentBlocksConfiguration.DefaultConfiguration;

        var header = config.Structure.HasFlag(Structure.Header)
            ? CreateViewModel(interValue.Header)
            : null;

        var blocks = config.Structure.HasFlag(Structure.Blocks)
            ? interValue.Blocks.Select(CreateViewModel).OfType<IContentBlockViewModel>().ToArray()
            : [];

        return new Rendering.ContentBlocks
        {
            Header = header,
            Blocks = blocks
        };

        ContentBlockInterValue? SelectBlock(ContentBlockValue? original, IPublishedElement owner)
        {
            if (original is null || original.IsDisabled)
            {
                return null;
            }

            // Start with default content
            var block = new ContentBlockInterValue
            {
                Id = original.Id,
                DefinitionId = original.DefinitionId,
                LayoutId = original.LayoutId,
                Content = original.Content,
            };

            if (variantSelector.SelectVariant(original, owner, preview) is ContentBlockVariantValue variant)
            {
                // Use variant instead, note we always use the definition + layout specified by the block
                block.Id = variant.Id;
                block.Content = variant.Content;
            };

            return block;
        }

        IContentBlockViewModel? CreateViewModel(ContentBlockInterValue? block)
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
