using System;

namespace Perplex.ContentBlocks.Presets
{
    public class ContentBlockPreset : IContentBlockPreset
    {
        public Guid Id { get; set; }
        public Guid DefinitionId { get; set; }
        public Guid LayoutId { get; set; }
        public bool IsMandatory { get; set; }
    }
}
