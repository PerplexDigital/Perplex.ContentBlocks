using System;
using System.Collections.Generic;

namespace Perplex.ContentBlocks.Categories
{
    public interface IContentBlockCategoryService
    {
        IContentBlockCategory GetById(Guid id);

        IEnumerable<IContentBlockCategory> GetAll(bool includeHidden);
    }
}
