namespace Perplex.ContentBlocks.Definitions;

public class InMemoryContentBlockDefinitionRepository(IContentBlockDefinitionFilterer definitionFilterer) : IContentBlockDefinitionRepository
{
    private readonly Dictionary<Guid, IContentBlockDefinition> _definitions = [];

    public IContentBlockDefinition? GetById(Guid id)
        => _definitions.TryGetValue(id, out var definition) ? definition : null;

    public IEnumerable<IContentBlockDefinition> GetAll()
        => _definitions.Values;

    public IEnumerable<IContentBlockDefinition> GetAllForPage(string documentType, string? culture)
        => definitionFilterer.FilterForPage(GetAll(), documentType, culture);

    public void Add(IContentBlockDefinition definition)
        => _definitions[definition.Id] = definition;

    public void Remove(Guid id)
        => _definitions.Remove(id);
}
