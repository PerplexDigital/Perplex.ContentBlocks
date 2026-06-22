namespace Perplex.ContentBlocks.Definitions;

public interface IContentBlockDefinitionRepository
{
    void Add(IContentBlockDefinition definition);

    void Remove(Guid id);

    IContentBlockDefinition? GetById(Guid id);

    IEnumerable<IContentBlockDefinition> GetAll();

    IEnumerable<IContentBlockDefinition> GetAllForPage(string documentType, string? culture);
}
