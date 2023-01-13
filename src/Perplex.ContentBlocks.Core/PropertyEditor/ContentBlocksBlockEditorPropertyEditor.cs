#if NET5_0_OR_GREATER
using Umbraco.Cms.Core.PropertyEditors;
#elif NETFRAMEWORK
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
#endif

namespace Perplex.ContentBlocks.PropertyEditor
{
    [DataEditor(
        Constants.PropertyEditor.BlockEditor.Alias,
        Constants.PropertyEditor.BlockEditor.Name,
        Constants.PropertyEditor.BlockEditor.ViewPath,
        ValueType = ValueTypes.Json,
        HideLabel = true)]
    public class ContentBlocksBlockEditorPropertyEditor : DataEditor
    {
#if NET5_0_OR_GREATER

        public ContentBlocksBlockEditorPropertyEditor(IDataValueEditorFactory dataValueEditorFactory, EditorType type = EditorType.PropertyValue) : base(dataValueEditorFactory, type)
        {
        }

#endif

#if NETFRAMEWORK
        public ContentBlocksBlockEditorPropertyEditor(ILogger logger, EditorType type = EditorType.PropertyValue) : base(logger, type)
        {
        }
#endif
    }
}
