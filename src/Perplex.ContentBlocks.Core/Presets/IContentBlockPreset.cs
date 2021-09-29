using System;
using System.Collections.Generic;

namespace Perplex.ContentBlocks.Presets
{
    public interface IContentBlockPreset
    {
        /// <summary>
        /// Unique id of this preset.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Definition to use for this preset.
        /// </summary>
        Guid DefinitionId { get; }

        /// <summary>
        /// Layout to use for this preset.
        /// </summary>
        Guid LayoutId { get; }

        /// <summary>
        /// When set to true, the block specified by this preset cannot be hidden or removed.
        /// </summary>
        bool IsMandatory { get; }

        /// <summary>
        /// The initial values of the generated Content Block per property alias of the IPublishedElement.
        /// </summary>
        IDictionary<string, object> Values { get; }

        /// <summary>
        /// The variants of this preset.
        /// </summary>
        IEnumerable<IContentBlockVariantPreset> Variants { get; set; }
    }
}
