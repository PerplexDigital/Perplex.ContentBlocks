#if NET7_0_OR_GREATER
#nullable enable
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Umbraco.Cms.Infrastructure.Serialization;
using static Umbraco.Cms.Core.PropertyEditors.BlockListConfiguration;

namespace Perplex.ContentBlocks.PropertyEditor
{
    public class ContentBlocksBlockEditorValueConverter : BlockPropertyValueConverterBase<BlockListModel, BlockListItem, ContentBlocksBlockLayoutItem, ContentBlocksBlockConfiguration>
    {
        public ContentBlocksBlockEditorValueConverter(BlockEditorConverter blockBlockEditorConverter) : base(blockBlockEditorConverter)
        {
        }

        public override bool IsConverter(IPublishedPropertyType propertyType)
            => propertyType.EditorAlias == Constants.PropertyEditor.BlockEditor.Alias;

        protected override BlockEditorDataConverter CreateBlockEditorDataConverter() => new ContentBlocksBlockEditorDataConverter();

        protected override BlockItemActivator<BlockListItem> CreateBlockItemActivator()
        {
            throw new System.NotImplementedException();
        }
    }

    public class ContentBlocksBlockEditorDataConverter : BlockEditorDataConverter
    {
        public ContentBlocksBlockEditorDataConverter() : base(Constants.PropertyEditor.BlockEditor.Alias)
        {
        }

        protected override IEnumerable<ContentAndSettingsReference> GetBlockReferences(JToken jsonLayout)
        {
            throw new System.NotImplementedException();
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
