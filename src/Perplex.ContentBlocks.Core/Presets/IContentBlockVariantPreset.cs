using System;
using System.Collections.Generic;

namespace Perplex.ContentBlocks.Presets
{
    public interface IContentBlockVariantPreset
    {
        /// <summary>
        /// Unique id of this variant preset
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The variant alias 
        /// </summary>
        string Alias { get; }

        /// <summary>
        /// The initial values of the generated Content Block variant per property alias of the IPublishedElement
        /// </summary>
        IDictionary<string, object> Values { get; }
    }
}
