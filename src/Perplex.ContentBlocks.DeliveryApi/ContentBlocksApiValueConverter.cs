using Perplex.ContentBlocks.PropertyEditor;
using Perplex.ContentBlocks.Rendering;
using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.DeliveryApi;

namespace Perplex.ContentBlocks.DeliveryApi;

public class ContentBlocksApiValueConverter : IDeliveryApiPropertyValueConverter
{
    private readonly ContentBlocksValueConverter _contentBlocksValueConverter;
    private readonly IApiElementBuilder _apiElementBuilder;

    public ContentBlocksApiValueConverter(
        ContentBlocksValueConverter contentBlocksValueConverter,
        IApiElementBuilder apiElementBuilder)
    {
        _contentBlocksValueConverter = contentBlocksValueConverter;
        _apiElementBuilder = apiElementBuilder;
    }

    public bool IsConverter(IPublishedPropertyType propertyType)
        => propertyType.EditorAlias == Constants.PropertyEditor.Alias;

    public PropertyCacheLevel GetDeliveryApiPropertyCacheLevel(IPublishedPropertyType propertyType)
        => GetPropertyCacheLevel(propertyType);

    public Type GetDeliveryApiPropertyValueType(IPublishedPropertyType propertyType)
        => typeof(IApiContentBlocks);

    public object? ConvertIntermediateToDeliveryApiObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview, bool expanding)
    {
        var modelValue = _contentBlocksValueConverter.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);
        if (modelValue is not IContentBlocks contentBlocks)
        {
            return null;
        }

        return new ApiContentBlocks
        {
            Header = Map(contentBlocks.Header),
            Blocks = contentBlocks.Blocks.Select(Map).OfType<IApiContentBlockViewModel>().ToArray(),
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
                Content = _apiElementBuilder.Build(vm.Content),
            };
        }
    }

    #region Forwarded to ContentBlocksValueConverter
    public object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview) => _contentBlocksValueConverter.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);
    public object? ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview) => _contentBlocksValueConverter.ConvertIntermediateToXPath(owner, propertyType, referenceCacheLevel, inter, preview);
    public object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview) => _contentBlocksValueConverter.ConvertSourceToIntermediate(owner, propertyType, source, preview);
    public PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) => _contentBlocksValueConverter.GetPropertyCacheLevel(propertyType);
    public Type GetPropertyValueType(IPublishedPropertyType propertyType) => _contentBlocksValueConverter.GetPropertyValueType(propertyType);
    public bool? IsValue(object? value, PropertyValueLevel level) => _contentBlocksValueConverter.IsValue(value, level);
    #endregion
}
