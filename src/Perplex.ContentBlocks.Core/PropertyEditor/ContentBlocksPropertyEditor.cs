using Perplex.ContentBlocks.PropertyEditor.Configuration;
using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Perplex.ContentBlocks.Utils;
using System;
using System.Collections.Generic;

#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
#elif NET472
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web;
#endif

namespace Perplex.ContentBlocks.PropertyEditor
{
    public class ContentBlocksPropertyEditor : IDataEditor
    {
        private readonly ContentBlocksModelValueDeserializer _deserializer;
        private readonly ContentBlockUtils _utils;

#if NET5_0_OR_GREATER
        private readonly IIOHelper _iOHelper;
        private readonly ILocalizedTextService _localizedTextService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IPropertyValidationService _validationService;

        public ContentBlocksPropertyEditor(
            ContentBlocksModelValueDeserializer deserializer,
            ContentBlockUtils utils,
            IIOHelper iOHelper,
            ILocalizedTextService localizedTextService,
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer,
            IPropertyValidationService validationService)
        {
            _deserializer = deserializer;
            _utils = utils;
            _iOHelper = iOHelper;
            _localizedTextService = localizedTextService;
            _shortStringHelper = shortStringHelper;
            _jsonSerializer = jsonSerializer;
            _validationService = validationService;
        }
#elif NET472
        private readonly Lazy<PropertyEditorCollection> _propertyEditorCollection;
        private readonly IDataTypeService _dataTypeService;
        private readonly ILocalizedTextService _textService;

        public ContentBlocksPropertyEditor(
            ContentBlocksModelValueDeserializer deserializer,
            ContentBlockUtils utils,
            Lazy<PropertyEditorCollection> propertyEditorCollection,
            IDataTypeService dataTypeService,
            ILocalizedTextService textService)
        {
            _deserializer = deserializer;
            _utils = utils;
            _propertyEditorCollection = propertyEditorCollection;
            _dataTypeService = dataTypeService;
            _textService = textService;
        }
#endif

        public string Alias { get; } = Constants.PropertyEditor.Alias;
        public EditorType Type { get; } = EditorType.PropertyValue;
        public string Name { get; } = Constants.PropertyEditor.Name;

        // Icon cannot be NULL for Umbraco 8.6+,
        // it will actually crash the UI.
        public string Icon { get; } = "icon-list";

        public string Group { get; } = "Lists";

        public bool IsDeprecated { get; } = false;
        public IDictionary<string, object> DefaultConfiguration { get; }

        public IPropertyIndexValueFactory PropertyIndexValueFactory
            => new DefaultPropertyIndexValueFactory();

        public IConfigurationEditor GetConfigurationEditor()
#if NET5_0_OR_GREATER
            => new ContentBlocksConfigurationEditor(_iOHelper);
#elif NET472
            => new ContentBlocksConfigurationEditor();
#endif

        public IDataValueEditor GetValueEditor()
            => GetValueEditor(null);

        public IDataValueEditor GetValueEditor(object configuration)
        {
#if NET5_0_OR_GREATER
            var validator = new ContentBlocksValidator(_deserializer, _utils, _validationService, _shortStringHelper);

            bool hideLabel = (configuration as ContentBlocksConfiguration)?.HideLabel
                ?? ContentBlocksConfigurationEditor._defaultConfiguration.HideLabel;

            return new ContentBlocksValueEditor(_deserializer, _utils, _localizedTextService, _shortStringHelper, _jsonSerializer)
            {
                View = Constants.PropertyEditor.ViewPath,
                Configuration = configuration,
                HideLabel = hideLabel,
                ValueType = ValueTypes.Json,
                Validators = { validator }
            };
#elif NET472
            var validator = new ContentBlocksValidator(_deserializer, _utils, _propertyEditorCollection.Value, _dataTypeService, _textService);

            bool hideLabel = (configuration as ContentBlocksConfiguration)?.HideLabel
                ?? ContentBlocksConfigurationEditor._defaultConfiguration.HideLabel;

            return new ContentBlocksValueEditor(Constants.PropertyEditor.ViewPath, _deserializer, _utils, validator)
            {
                Configuration = configuration,
                HideLabel = hideLabel,
                ValueType = ValueTypes.Json,
            };
#endif
        }
    }
}
