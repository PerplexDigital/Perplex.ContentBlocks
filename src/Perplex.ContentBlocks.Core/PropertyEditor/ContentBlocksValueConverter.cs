using Perplex.ContentBlocks.DeliveryApi;
using Perplex.ContentBlocks.PropertyEditor.Configuration;
using Perplex.ContentBlocks.PropertyEditor.Value;
using Perplex.ContentBlocks.Rendering;
using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.DeliveryApi;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Umbraco.Extensions;

namespace Perplex.ContentBlocks.PropertyEditor;

public class ContentBlocksValueConverter
(
    IServiceProvider serviceProvider,
    ContentBlocksValueDeserializer deserializer,
    BlockEditorConverter converter,
    IApiElementBuilder apiElementBuilder
) : PropertyValueConverterBase, IDeliveryApiPropertyValueConverter
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

        if (value is IContentBlocks model)
        {
            // Valid with at least 1 block
            return model.Header is not null || model.Blocks.Any();
        }

        if (value is string inter)
        {
            // Umbraco incorrectly passes in the Inter value when PropertyValueLevel is set to Object in some cases:
            // https://github.com/umbraco/Umbraco-CMS/issues/20338
            // We will attempt to deserialize it here in that case, but Umbraco should fix this in the future
            // so this case will never be hit anymore.
            // This method (IsValue) is called on every render and we do not want to deserialize every time
            // but rather look at the cached Object value.
            if (deserializer.Deserialize(inter) is ContentBlocksValue modelValue)
            {
                // Valid with at least 1 block
                return modelValue.Header is not null || modelValue.Blocks?.Count > 0;
            }
        }

        // If we end up here it is invalid.
        return false;
    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        => typeof(IContentBlocks);

    public PropertyCacheLevel GetDeliveryApiPropertyCacheLevel(IPublishedPropertyType propertyType)
        => GetPropertyCacheLevel(propertyType);

    public Type GetDeliveryApiPropertyValueType(IPublishedPropertyType propertyType)
        => typeof(IApiContentBlocks);

    public object? ConvertIntermediateToDeliveryApiObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview, bool expanding)
    {
        var modelValue = ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);
        if (modelValue is not IContentBlocks contentBlocks)
        {
            return null;
        }

        return new ApiContentBlocks
        {
            Header = Map(contentBlocks.Header),
            Blocks = [.. contentBlocks.Blocks.Select(Map).OfType<IApiContentBlockViewModel>()],
        };

        ApiContentBlockViewModel? Map(IContentBlockViewModel? vm)
        {
            if (vm is null)
            {
                return null;
            }

            return new ApiContentBlockViewModel
            {
                Id = vm.Id,
                DefinitionId = vm.DefinitionId,
                LayoutId = vm.LayoutId,
                Content = apiElementBuilder.Build(vm.Content),
            };
        }
    }
}
