namespace Perplex.ContentBlocks.Presets;

public interface IContentBlocksPresetRepository
{
    void Add(IContentBlocksPreset preset);

    void Remove(Guid id);

    IEnumerable<IContentBlocksPreset> GetAll();

    IContentBlocksPreset? GetPresetForPage(string documentType, string? culture);
}
