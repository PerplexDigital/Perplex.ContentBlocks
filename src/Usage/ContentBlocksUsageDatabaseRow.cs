using System;

namespace Perplex.ContentBlocks.Usage
{
    public class ContentBlocksUsageDatabaseRow
    {
        public Guid PageId { get; set; }
        public int WebsiteId { get; set; }
        public string Culture { get; set; }
        public Guid ContentBlockDefinitionId { get; set; }
        public int Amount { get; set; }
    }
}
