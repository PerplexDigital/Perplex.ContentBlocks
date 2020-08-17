using Newtonsoft.Json;
using System.Collections.Generic;

namespace Perplex.ContentBlocks.PropertyEditor.ModelValue
{
    public class ContentBlocksModelValue
    {
        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("header")]
        public ContentBlockModelValue Header { get; set; }

        [JsonProperty("blocks")]
        public List<ContentBlockModelValue> Blocks { get; set; }
    }
}
