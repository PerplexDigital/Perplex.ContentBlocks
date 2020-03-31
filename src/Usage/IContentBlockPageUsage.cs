using System;

namespace Perplex.ContentBlocks.Usage
{
    public interface IContentBlockPageUsage
    {
        Guid ContentBlockDefinitionId { get; }
        Guid PageId { get; }
        int WebsiteId { get; }
        string Culture { get; }
        int UsageAmount { get; }
    }
}
