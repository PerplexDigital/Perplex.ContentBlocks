using Perplex.ContentBlocks.PropertyEditor.Configuration;
using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Perplex.ContentBlocks.Utils;
using System.Collections.Generic;

#if NET5_0
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
using Umbraco.Web;
#endif

namespace Perplex.ContentBlocks.PropertyEditor
{
    public class ContentBlocksPropertyEditor : IDataEditor
    {
        private readonly ContentBlocksModelValueDeserializer _deserializer;
        private readonly ContentBlockUtils _utils;
        private readonly IRuntimeState _runtimeState;
        private readonly IHttpContextAccessor _httpContextAccessor;

#if NET5_0
        private readonly IIOHelper _iOHelper;
        private readonly ILocalizedTextService _localizedTextService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IJsonSerializer _jsonSerializer;

        public ContentBlocksPropertyEditor(
            ContentBlocksModelValueDeserializer deserializer,
            ContentBlockUtils utils,
            IRuntimeState runtimeState,
            IIOHelper iOHelper,
            ILocalizedTextService localizedTextService,
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer,
            IHttpContextAccessor httpContextAccessor)
        {
            _deserializer = deserializer;
            _utils = utils;
            _runtimeState = runtimeState;
            _iOHelper = iOHelper;
            _localizedTextService = localizedTextService;
            _shortStringHelper = shortStringHelper;
            _jsonSerializer = jsonSerializer;
            _httpContextAccessor = httpContextAccessor;
        }
#elif NET472
        public ContentBlocksPropertyEditor(
            ContentBlocksModelValueDeserializer deserializer,
            ContentBlockUtils utils,
            IRuntimeState runtimeState,
            IHttpContextAccessor httpContextAccessor)
        {
            _deserializer = deserializer;
            _utils = utils;
            _runtimeState = runtimeState;
            _httpContextAccessor = httpContextAccessor;
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
#if NET5_0
            => new ContentBlocksConfigurationEditor(_iOHelper);
#elif NET472
            => new ContentBlocksConfigurationEditor();
#endif

        public IDataValueEditor GetValueEditor()
            => GetValueEditor(null);

        public IDataValueEditor GetValueEditor(object configuration)
        {
#if NET5_0
            var validator = new ContentBlocksValidator(_deserializer, _utils, _runtimeState, _httpContextAccessor);

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
            var validator = new ContentBlocksValidator(_deserializer, _utils, _runtimeState, _httpContextAccessor);

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
