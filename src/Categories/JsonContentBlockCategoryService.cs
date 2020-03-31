using Athlon.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;

namespace Perplex.ContentBlocks.Categories
{
    /// <summary>
    /// IContentBlockDefinitionService implementatie die de definities leest uit een JSON bestand
    /// op de schijf. Deze implementatie zal de configuratie bij elke wijziging van het bestand opnieuw uitlezen.
    /// </summary>
    public class JsonContentBlockCategoryService : IContentBlockCategoryService
    {
        private static IDictionary<Guid, IContentBlockCategory> _categories;

        private const string _configFilePath = "~/App_Data/ContentBlocks/categories.json";
        private readonly CachedJsonFileReader<List<SerializableContentBlockCategory>> _cachedJsonFileReader;

        public JsonContentBlockCategoryService(CachedJsonFileReader<List<SerializableContentBlockCategory>> cachedFileReader)
        {
            string json = JsonConvert.SerializeObject(new HardCodedContentBlockCategoryService().GetAll(true), Formatting.Indented);

            _cachedJsonFileReader = cachedFileReader;
            _cachedJsonFileReader.Watch(HostingEnvironment.MapPath(_configFilePath), UpdateCategories);
        }

        private void UpdateCategories()
        {
            IEnumerable<IContentBlockCategory> categories = _cachedJsonFileReader.GetData();
            if (categories != null)
            {
                _categories = categories.ToDictionary(d => d.Id);
            }
        }

        private IDictionary<Guid, IContentBlockCategory> GetCategories()
        {
            if (_categories == null)
            {
                UpdateCategories();
            }

            return _categories;
        }

        public IContentBlockCategory GetById(Guid id)
        {
            var categories = GetCategories();
            if (categories?.Any() != true)
            {
                return null;
            }

            return categories.TryGetValue(id, out var definition) ? definition : null;
        }

        public IEnumerable<IContentBlockCategory> GetAll(bool includeHidden)
        {
            var categories = GetCategories()?.Values ?? Enumerable.Empty<IContentBlockCategory>();

            return includeHidden
                ? categories
                : categories.Where(category => !category.IsHidden);
        }
    }
}
