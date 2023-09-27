using Perplex.ContentBlocks.PropertyEditor.Configuration;
using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Perplex.ContentBlocks.Utils;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;

namespace Perplex.ContentBlocks.PropertyEditor;

public class ContentBlocksPropertyEditor : IDataEditor
{
    private readonly ContentBlocksModelValueDeserializer _deserializer;
    private readonly ContentBlockUtils _utils;

    private readonly IIOHelper _iOHelper;
    private readonly ILocalizedTextService _localizedTextService;
    private readonly IShortStringHelper _shortStringHelper;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly IPropertyValidationService _validationService;
    private readonly IEditorConfigurationParser _editorConfigurationParser;

    public ContentBlocksPropertyEditor(
        ContentBlocksModelValueDeserializer deserializer,
        ContentBlockUtils utils,
        IIOHelper iOHelper,
        ILocalizedTextService localizedTextService,
        IShortStringHelper shortStringHelper,
        IJsonSerializer jsonSerializer,
        IPropertyValidationService validationService,
        IEditorConfigurationParser editorConfigurationParser)
    {
        _deserializer = deserializer;
        _utils = utils;
        _iOHelper = iOHelper;
        _localizedTextService = localizedTextService;
        _shortStringHelper = shortStringHelper;
        _jsonSerializer = jsonSerializer;
        _validationService = validationService;
        _editorConfigurationParser = editorConfigurationParser;
    }

    public string Alias { get; } = Constants.PropertyEditor.Alias;
    public EditorType Type { get; } = EditorType.PropertyValue;
    public string Name { get; } = Constants.PropertyEditor.Name;

    // Icon cannot be NULL for Umbraco 8.6+,
    // it will actually crash the UI.
    public string Icon { get; } = "icon-list";

    public string Group { get; } = "Lists";

    public bool IsDeprecated { get; } = false;

    public IDictionary<string, object> DefaultConfiguration => GetConfigurationEditor().DefaultConfiguration;

    public IPropertyIndexValueFactory PropertyIndexValueFactory
        => new DefaultPropertyIndexValueFactory();

    public IConfigurationEditor GetConfigurationEditor()
        => new ContentBlocksConfigurationEditor(_iOHelper, _editorConfigurationParser);

    public IDataValueEditor GetValueEditor()
        => GetValueEditor(null);

    public IDataValueEditor GetValueEditor(object? configuration)
    {
        var validator = new ContentBlocksValidator(_deserializer, _utils, _validationService, _shortStringHelper);

        bool hideLabel = (configuration as ContentBlocksConfiguration)?.HideLabel
            ?? ContentBlocksConfiguration.DefaultConfiguration.HideLabel;

        return new ContentBlocksValueEditor(_deserializer, _utils, _localizedTextService, _shortStringHelper, _jsonSerializer)
        {
            View = Constants.PropertyEditor.ViewPath,
            Configuration = configuration,
            HideLabel = hideLabel,
            ValueType = ValueTypes.Json,
            Validators = { validator }
        };
    }
}
