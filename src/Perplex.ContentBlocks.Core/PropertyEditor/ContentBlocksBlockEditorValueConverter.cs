#if NET5_0_OR_GREATER
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using static Umbraco.Cms.Core.PropertyEditors.BlockListConfiguration;

namespace Perplex.ContentBlocks.PropertyEditor
{
    // TODO: Need Umbraco 11 for BlockPropertyValueConverterBase.

    // public class ContentBlocksBlockEditorValueConverter : BlockPropertyValueConverterBase<BlockListModel, BlockListItem, BlockListLayoutItem, BlockConfiguration>
    // {
    //     public override bool IsConverter(IPublishedPropertyType propertyType)
    //         => propertyType.EditorAlias == Constants.PropertyEditor.BlockEditor.Alias;
    // }
}
#endif
