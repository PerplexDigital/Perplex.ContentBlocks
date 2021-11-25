using Perplex.ContentBlocks.Definitions;
using Perplex.ContentBlocks.PropertyEditor.Configuration;
using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Perplex.ContentBlocks.Rendering;
using Perplex.ContentBlocks.Variants;
using System;
using System.Collections.Generic;
using System.Linq;

#if NET5_0
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
#elif NET472
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors.ValueConverters;
#endif

namespace Perplex.ContentBlocks.PropertyEditor
{
    public class ContentBlocksValueConverter : PropertyValueConverterBase
    {
        private readonly NestedContentSingleValueConverter _nestedContentSingleValueConverter;
        private readonly ContentBlocksModelValueDeserializer _deserializer;
        private readonly IContentBlockVariantSelector _variantSelector;
#if NET5_0
        private readonly IServiceProvider _serviceProvider;
#endif

        public ContentBlocksValueConverter(
            NestedContentSingleValueConverter nestedContentSingleValueConverter,
            ContentBlocksModelValueDeserializer deserializer,
            IContentBlockVariantSelector variantSelector
#if NET5_0
            , IServiceProvider serviceProvider
#endif
        )
        {
            _nestedContentSingleValueConverter = nestedContentSingleValueConverter;
            _deserializer = deserializer;
            _variantSelector = variantSelector;
#if NET5_0
            _serviceProvider = serviceProvider;
#endif
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

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            ContentBlocksModelValue modelValue = _deserializer.Deserialize(inter?.ToString());
            if (modelValue == null)
            {
                return Rendering.ContentBlocks.Empty;
            }

            var interValue = new ContentBlocksInterValue
            {
                Header = selectBlock(modelValue.Header),
                Blocks = modelValue.Blocks?.Select(selectBlock).ToList() ?? new List<ContentBlockInterValue>(),
            };

            var config = propertyType.DataType.ConfigurationAs<ContentBlocksConfiguration>();

            var header = config.Structure.HasFlag(Structure.Header)
                ? createViewModel(interValue.Header)
                : null;

            var blocks = config.Structure.HasFlag(Structure.Blocks)
                ? interValue.Blocks.Select(createViewModel).Where(vm => vm != null).ToList()
                : Enumerable.Empty<IContentBlockViewModel>();

            return new Rendering.ContentBlocks
            {
                Header = header,
                Blocks = blocks
            };

            ContentBlockInterValue selectBlock(ContentBlockModelValue original)
            {
                if (original == null || original.IsDisabled)
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

            IContentBlockViewModel createViewModel(ContentBlockInterValue block)
            {
                if (block == null)
                {
                    return null;
                }

#if NET5_0
                IContentBlockDefinitionRepository definitionRepository = _serviceProvider.GetService<IContentBlockDefinitionRepository>();
#elif NET472
                IContentBlockDefinitionRepository definitionRepository = Current.Factory.GetInstance<IContentBlockDefinitionRepository>();
#endif
                if (definitionRepository == null)
                {
                    return null;
                }

                IContentBlockDefinition definition = definitionRepository.GetById(block.DefinitionId);
                if (definition == null || definition.Layouts == null || definition.Layouts.Any() == false)
                {
                    return null;
                }

                IContentBlockLayout layout = definition.Layouts.FirstOrDefault(l => l.Id == block.LayoutId);
                if (layout == null)
                {
                    return null;
                }

                IPublishedElement content = _nestedContentSingleValueConverter.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, block?.Content?.ToString(), preview) as IPublishedElement;
                if (content == null)
                {
                    return null;
                }

                var contentType = content.GetType();
                var genericViewModelFactoryType = typeof(IContentBlockViewModelFactory<>).MakeGenericType(new[] { contentType });

#if NET5_0
                var viewModelFactory = _serviceProvider.GetService(genericViewModelFactoryType) as IContentBlockViewModelFactory;
#elif NET472
                var viewModelFactory = Current.Factory.GetInstance(genericViewModelFactoryType) as IContentBlockViewModelFactory;
#endif

                if (viewModelFactory == null)
                {
                    return null;
                }

                return viewModelFactory.Create(content, block.Id, block.DefinitionId, block.LayoutId);
            }
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
            => typeof(IContentBlocks);
    }
}
