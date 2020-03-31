using System;
using System.Collections.Generic;
using System.Linq;

namespace Perplex.ContentBlocks.Usage
{
    public class ContentBlockUsage : IContentBlockUsage
    {
        public Guid ContentBlockDefinitionId { get; set; }

        public IEnumerable<IContentBlockPageUsage> PageUses { get; set; }
            = new List<IContentBlockPageUsage>();

        public int TotalUses => PageUses.Sum(pu => pu.UsageAmount);
    }
}
