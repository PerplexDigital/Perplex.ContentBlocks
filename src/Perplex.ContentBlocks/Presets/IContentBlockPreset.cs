using System;
using System.Collections.Generic;

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

        /// <summary>
        /// The initial values of the generated Content Block per alias
        /// </summary>
        IDictionary<string, object> Values { get; }
    }
}
