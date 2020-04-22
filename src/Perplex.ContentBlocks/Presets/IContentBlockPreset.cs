using System;

namespace Perplex.ContentBlocks.Presets
{
    public interface IContentBlockPreset
    {
        Guid Id { get; }
        Guid DefinitionId { get; }
        Guid LayoutId { get; }

        /// <summary>
        /// When set to true, the block specified by this preset cannot be hidden or removed.
        /// </summary>
        bool IsMandatory { get; }
    }
}
