using System;
using System.Collections.Generic;

namespace Perplex.ContentBlocks.Definitions
{
    public class InMemoryContentBlockDefinitionRepository : IContentBlockDefinitionRepository
    {
        public InMemoryContentBlockDefinitionRepository(IContentBlockDefinitionFilterer definitionFilterer)
        {
            _definitionFilterer = definitionFilterer;
            _definitions = new Dictionary<Guid, IContentBlockDefinition>();
        }

        private readonly IDictionary<Guid, IContentBlockDefinition> _definitions;

        private readonly IContentBlockDefinitionFilterer _definitionFilterer;

        public IContentBlockDefinition GetById(Guid id)
        {
            return _definitions.TryGetValue(id, out var definition) ? definition : null;
        }

        public IEnumerable<IContentBlockDefinition> GetAll()
        {
            return _definitions.Values;
        }

        public IEnumerable<IContentBlockDefinition> GetAllForPage(int pageId, string culture)
            => _definitionFilterer.FilterForPage(GetAll(), pageId, culture);

        public IEnumerable<IContentBlockDefinition> GetAllForPage(string documentType, string culture)
            => _definitionFilterer.FilterForPage(GetAll(), documentType, culture);

        public void Add(IContentBlockDefinition definition)
            => _definitions[definition.Id] = definition;

        public void Remove(Guid id)
            => _definitions.Remove(id);
    }
}
