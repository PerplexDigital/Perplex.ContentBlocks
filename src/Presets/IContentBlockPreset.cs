using System;

namespace Perplex.ContentBlocks.Presets
{
    public interface IContentBlockPreset
    {
        Guid Id { get; }        
        Guid DefinitionId { get; }
        Guid LayoutId { get; }
        bool IsMandatory { get; }
    }
}
