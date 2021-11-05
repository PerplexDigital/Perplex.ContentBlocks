using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Perplex.ContentBlocks.PropertyEditor.ModelValue
{
    public class ContentBlockVariantModelValue
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        /// <summary>
        /// JSON NestedContent
        /// </summary>
        [JsonProperty("content")]
        public JArray Content { get; set; }
    }
}
