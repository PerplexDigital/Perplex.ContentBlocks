﻿using Perplex.ContentBlocks.PropertyEditor.Configuration;
using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Perplex.ContentBlocks.Rendering;
using Perplex.ContentBlocks.Variants;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Perplex.ContentBlocks.PropertyEditor;

public class ContentBlocksValueConverter : PropertyValueConverterBase
{
    private readonly NestedContentSingleValueConverter _nestedContentSingleValueConverter;
    private readonly ContentBlocksModelValueDeserializer _deserializer;
    private readonly IContentBlockVariantSelector _variantSelector;
    private readonly IServiceProvider _serviceProvider;

    public ContentBlocksValueConverter(
        NestedContentSingleValueConverter nestedContentSingleValueConverter,
        ContentBlocksModelValueDeserializer deserializer,
        IContentBlockVariantSelector variantSelector,
        IServiceProvider serviceProvider
    )
    {
        _nestedContentSingleValueConverter = nestedContentSingleValueConverter;
        _deserializer = deserializer;
        _variantSelector = variantSelector;
        _serviceProvider = serviceProvider;
    }

    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType)
    {
        // We might be able to set this to .Elements. This ensures the cache will be refreshed
        // even after publishing any other content, which ensures no issues arise when the block
        // contains editors that reference other content (e.g. a ContentPicker).
        // However, this requires proper testing first with a wide range of editors.
        // Until that time, .Snapshot is the safest option: per request caching.
        return PropertyCacheLevel.Snapshot;
    }

    public override bool IsConverter(IPublishedPropertyType propertyType)
        => propertyType.EditorAlias == Constants.PropertyEditor.Alias;

    public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview)
    {
        ContentBlocksModelValue? modelValue = _deserializer.Deserialize(inter?.ToString());
        if (modelValue is null)
        {
            return Rendering.ContentBlocks.Empty;
        }

        var interValue = new ContentBlocksInterValue
        {
            Header = SelectBlock(modelValue.Header),
            Blocks = modelValue.Blocks?.Select(SelectBlock).OfType<ContentBlockInterValue>().ToArray() ?? Array.Empty<ContentBlockInterValue>(),
        };

        var config = propertyType.DataType.ConfigurationAs<ContentBlocksConfiguration>() ?? ContentBlocksConfiguration.DefaultConfiguration;

        var header = config.Structure.HasFlag(Structure.Header)
            ? CreateViewModel(interValue.Header)
            : null;

        var blocks = config.Structure.HasFlag(Structure.Blocks)
            ? interValue.Blocks.Select(CreateViewModel).OfType<IContentBlockViewModel>().ToArray()
            : Array.Empty<IContentBlockViewModel>();

        return new Rendering.ContentBlocks
        {
            Header = header,
            Blocks = blocks
        };

        ContentBlockInterValue? SelectBlock(ContentBlockModelValue? original)
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

            if (_variantSelector.SelectVariant(original, owner, preview) is ContentBlockVariantModelValue variant)
            {
                // Use variant instead, note we always use the definition + layout specified by the block
                block.Id = variant.Id;
                block.Content = variant.Content;
            };

            return block;
        }

        IContentBlockViewModel? CreateViewModel(ContentBlockInterValue? block)
        {
            if (block is null)
            {
                return null;
            }

            if (ParseElement(block.Content?.ToString()) is not IPublishedElement content)
            {
                return null;
            }

            var contentType = content.GetType();
            var genericViewModelFactoryType = typeof(IContentBlockViewModelFactory<>).MakeGenericType(new[] { contentType });

            if (_serviceProvider.GetService(genericViewModelFactoryType) is not IContentBlockViewModelFactory viewModelFactory)
            {
                return null;
            }

            return viewModelFactory.Create(content, block.Id, block.DefinitionId, block.LayoutId);
        }

        IPublishedElement? ParseElement(string? blockContent)
            => _nestedContentSingleValueConverter.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, blockContent, preview) as IPublishedElement;
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
