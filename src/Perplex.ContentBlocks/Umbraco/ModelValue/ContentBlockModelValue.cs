using Newtonsoft.Json.Linq;
using System;

namespace Perplex.ContentBlocks.Umbraco.ModelValue
{
    public class ContentBlockModelValue
    {
        public Guid Id { get; set; }

        public Guid DefinitionId { get; set; }

        public Guid LayoutId { get; set; }

        /// <summary>
        /// Indien dit blok uit een preset komt zal dit een waarde hebben
        /// en wijzen naar de betreffende IContentBlockPreset
        /// </summary>
        public Guid? PresetId { get; set; }

        public bool IsDisabled { get; set; }

        /// <summary>
        /// JSON NestedContent
        /// </summary>
        public JArray Content { get; set; }
    }
}
