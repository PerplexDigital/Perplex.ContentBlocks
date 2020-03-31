using Perplex.ContentBlocks.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Perplex.ContentBlocks.Presets
{
    public class InMemoryContentBlocksPresetRepository : IContentBlocksPresetRepository
    {
        private readonly IDocumentTypeAliasProvider _documentTypeAliasProvider;

        public InMemoryContentBlocksPresetRepository(IDocumentTypeAliasProvider documentTypeAliasProvider)
        {
            _documentTypeAliasProvider = documentTypeAliasProvider;
        }

        private readonly IDictionary<Guid, IContentBlocksPreset> _presets = new Dictionary<Guid, IContentBlocksPreset>();

        public void Add(IContentBlocksPreset preset)
            => _presets[preset.Id] = preset;

        public void Remove(Guid id)
            => _presets.Remove(id);

        public IContentBlocksPreset GetById(Guid id)
        {
            return _presets.TryGetValue(id, out var preset) ? preset : null;
        }

        public IEnumerable<IContentBlocksPreset> GetAll()
        {
            return _presets.Values;
        }

        public IContentBlocksPreset GetPresetForPage(int pageId, string culture)
        {
            if (string.IsNullOrEmpty(culture))
            {
                return null;
            }

            string documentType = _documentTypeAliasProvider.GetDocumentTypeAlias(pageId);
            return GetPresetForPage(documentType, culture);
        }

        public IContentBlocksPreset GetPresetForPage(string documentType, string culture)
        {
            if (string.IsNullOrEmpty(documentType))
            {
                return null;
            }

            bool isEmptyOrContains(IEnumerable<string> input, string toMatch)
                => input?.Any() != true || input.Any(i => string.Equals(i, toMatch, StringComparison.InvariantCultureIgnoreCase));

            return GetAll()?.FirstOrDefault(p =>
                isEmptyOrContains(p.ApplyToCultures, culture) &&
                isEmptyOrContains(p.ApplyToDocumentTypes, documentType));
        }
    }
}
