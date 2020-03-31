using Athlon.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using Umbraco.Web;

namespace Perplex.ContentBlocks.Definitions
{
    /// <summary>
    /// IContentBlockDefinitionService implementatie die de definities leest uit een JSON bestand
    /// op de schijf. Deze implementatie zal de configuratie bij elke wijziging van het bestand opnieuw uitlezen.
    /// </summary>
    public class JsonContentBlockDefinitionService : IContentBlockDefinitionService
    {
        private static IDictionary<Guid, IContentBlockDefinition> _definitions;

        private const string _configFilePath = "~/App_Data/ContentBlocks/definitions.json";
        private readonly CachedJsonFileReader<List<SerializableContentBlockDefinition>> _cachedJsonFileReader;
        private readonly IContentBlockDefinitionFilterer _definitionFilterer;

        public JsonContentBlockDefinitionService(
            CachedJsonFileReader<List<SerializableContentBlockDefinition>> cachedFileReader,
            IContentBlockDefinitionFilterer definitionFilterer)
        {
            _cachedJsonFileReader = cachedFileReader;
            _cachedJsonFileReader.Watch(HostingEnvironment.MapPath(_configFilePath), UpdateDefinitions);
            _definitionFilterer = definitionFilterer;
        }

        private void UpdateDefinitions()
        {
            IEnumerable<IContentBlockDefinition> definitions = _cachedJsonFileReader.GetData();

            if (definitions != null)
            {
                _definitions = definitions.ToDictionary(d => d.Id);
            }
        }

        private IDictionary<Guid, IContentBlockDefinition> GetDefinitions()
        {
            if (_definitions == null)
            {
                UpdateDefinitions();
            }

            return _definitions;
        }

        public IContentBlockDefinition GetById(Guid id)
        {
            var definitions = GetDefinitions();

            if (definitions == null)
            {
                return null;
            }

            return definitions.TryGetValue(id, out var definition) ? definition : null;
        }

        public IEnumerable<IContentBlockDefinition> GetAll()
        {
            var definitions = GetDefinitions();

            if (definitions == null)
            {
                return null;
            }

            return definitions.Values;
        }

        public IEnumerable<IContentBlockDefinition> GetAllForPage(int pageId, string culture)
            => _definitionFilterer.FilterForPage(GetAll(), pageId, culture);

        public IEnumerable<IContentBlockDefinition> GetAllForPage(string documentType, string culture)
            => _definitionFilterer.FilterForPage(GetAll(), documentType, culture);
    }
}
