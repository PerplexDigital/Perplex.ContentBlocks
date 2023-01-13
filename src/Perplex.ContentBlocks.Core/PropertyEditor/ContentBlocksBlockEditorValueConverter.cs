#if NET7_0_OR_GREATER
#nullable enable
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPoco.fastJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Umbraco.Cms.Infrastructure.Serialization;

namespace Perplex.ContentBlocks.PropertyEditor
{
    public class ContentBlocksBlockEditorValueConverter : BlockPropertyValueConverterBase<BlockListModel, BlockListItem, ContentBlocksBlockLayoutItem, ContentBlocksBlockConfiguration>
    {
        public ContentBlocksBlockEditorValueConverter(BlockEditorConverter blockBlockEditorConverter) : base(blockBlockEditorConverter)
        {
        }

        public override bool IsConverter(IPublishedPropertyType propertyType)
            => propertyType.EditorAlias == Constants.PropertyEditor.BlockEditor.Alias;

        /// <inheritdoc />
        public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview)
            => source?.ToString();

        /// <inheritdoc />
        public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview)
        {
            //// Get configuration
            //BlockListConfiguration? configuration = propertyType.DataType.ConfigurationAs<BlockListConfiguration>();
            //if (configuration is null)
            //{
            //    return null;
            //}

            if (inter?.ToString() is not string json)
            {
                return null;
            }

            BlockListModel CreateEmptyModel() => BlockListModel.Empty;

            BlockListModel CreateModel(IList<BlockListItem> items) => new BlockListModel(items);

            var blockConfigurations = new[]
            {
                new ContentBlocksBlockConfiguration
                {
                    ContentElementTypeKey = new Guid("462eb822-9fb0-45f1-826c-0f1693887e6c"),
                }
            };

            var blockEditorValue = JToken.Parse(json)["blocks"]?.ToString();
            if (blockEditorValue is null)
            {
                return CreateEmptyModel();
            }

            BlockListModel blockModel = UnwrapBlockModel(referenceCacheLevel, blockEditorValue, preview, blockConfigurations, CreateEmptyModel, CreateModel);

            return blockModel;
        }

        protected override BlockEditorDataConverter CreateBlockEditorDataConverter() => new ContentBlocksBlockEditorDataConverter();

        protected override BlockItemActivator<BlockListItem> CreateBlockItemActivator() => new BlockListItemActivator(BlockEditorConverter);

        private class BlockListItemActivator : BlockItemActivator<BlockListItem>
        {
            public BlockListItemActivator(BlockEditorConverter blockConverter) : base(blockConverter)
            {
            }

            protected override Type GenericItemType => typeof(BlockListItem<,>);
        }
    }

    public class ContentBlocksBlockEditorDataConverter : BlockEditorDataConverter
    {
        public ContentBlocksBlockEditorDataConverter() : base(Constants.PropertyEditor.BlockEditor.Alias)
        {
        }

        protected override IEnumerable<ContentAndSettingsReference>? GetBlockReferences(JToken jsonLayout)
        {
            IEnumerable<ContentBlocksBlockLayoutItem>? blockListLayout = jsonLayout.ToObject<IEnumerable<ContentBlocksBlockLayoutItem>>();
            return blockListLayout?.Select(x => new ContentAndSettingsReference(x.ContentUdi, x.SettingsUdi)).ToList();
        }
    }

    public class ContentBlocksBlockConfiguration : IBlockConfiguration
    {
        public Guid ContentElementTypeKey { get; set; }
        public Guid? SettingsElementTypeKey { get; set; }
    }

    public class ContentBlocksBlockLayoutItem : IBlockLayoutItem
    {
        [JsonProperty("contentUdi", Required = Required.Always)]
        [JsonConverter(typeof(UdiJsonConverter))]
        public Udi? ContentUdi { get; set; }

        [JsonProperty("settingsUdi", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(UdiJsonConverter))]
        public Udi? SettingsUdi { get; set; }

        [JsonProperty("layoutId", Required = Required.Always)]
        public Guid LayoutId { get; set; }

        [JsonProperty("isDisabled", Required = Required.Always)]
        public bool IsDisabled { get; set; }
    }
}

#endif
