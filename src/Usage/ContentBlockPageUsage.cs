using System;

namespace Perplex.ContentBlocks.Usage
{
    public class ContentBlockPageUsage : IContentBlockPageUsage
    {
        public Guid ContentBlockDefinitionId { get; set; }
        public Guid PageId { get; set; }
        public int WebsiteId { get; set; }
        public string Culture { get; set; }
        public int UsageAmount { get; set; }
    }
}
