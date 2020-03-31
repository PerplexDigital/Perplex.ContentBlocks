using System;
using System.Collections.Generic;

namespace Perplex.ContentBlocks.Usage
{
    public interface IContentBlockUsage
    {
        Guid ContentBlockDefinitionId { get; }
        IEnumerable<IContentBlockPageUsage> PageUses { get; }
        int TotalUses { get; }
    }
}
