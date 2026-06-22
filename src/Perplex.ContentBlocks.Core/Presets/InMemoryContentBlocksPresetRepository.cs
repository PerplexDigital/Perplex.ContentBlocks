namespace Perplex.ContentBlocks.Presets;

public class InMemoryContentBlocksPresetRepository : IContentBlocksPresetRepository
{
    private readonly Dictionary<Guid, IContentBlocksPreset> _presets = [];

    public void Add(IContentBlocksPreset preset)
        => _presets[preset.Id] = preset;

    public void Remove(Guid id)
        => _presets.Remove(id);

    public IContentBlocksPreset? GetById(Guid id)
        => _presets.TryGetValue(id, out var preset) ? preset : null;

    public IEnumerable<IContentBlocksPreset> GetAll()
        => _presets.Values;

    public IContentBlocksPreset? GetPresetForPage(string documentType, string? culture)
    {
        if (string.IsNullOrEmpty(documentType))
        {
            return null;
        }

        return GetAll()?.FirstOrDefault(p =>
            IsEmptyOrContains(p.ApplyToCultures, culture) &&
            IsEmptyOrContains(p.ApplyToDocumentTypes, documentType));

        static bool IsEmptyOrContains(IEnumerable<string> input, string? toMatch)
            => input?.Any() != true || input.Any(i => string.Equals(i, toMatch, StringComparison.InvariantCultureIgnoreCase));
    }
}
