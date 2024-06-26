using Perplex.ContentBlocks.PropertyEditor.Configuration;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;

namespace Perplex.ContentBlocks.PropertyEditor;

[DataEditor(Constants.PropertyEditor.Alias, ValueEditorIsReusable = false, ValueType = ValueTypes.Json)]
public class PerplexContentBlocksPropertyEditor : DataEditor
{
    private readonly IIOHelper _ioHelper;
    private readonly IConfigurationEditorJsonSerializer _configEditorSerializer;

    public PerplexContentBlocksPropertyEditor(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper, IConfigurationEditorJsonSerializer configEditorSerializer)
        : base(dataValueEditorFactory)
    {
        SupportsReadOnly = true;
        _ioHelper = ioHelper;
        _configEditorSerializer = configEditorSerializer;
    }

    protected override IDataValueEditor CreateValueEditor() =>
        DataValueEditorFactory.Create<ContentBlocksValueEditor>(Attribute!);

    protected override IConfigurationEditor CreateConfigurationEditor() =>
        new ContentBlocksConfigurationEditor(_ioHelper, _configEditorSerializer);
}
