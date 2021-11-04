using System;
using System.Collections.Generic;

namespace Perplex.ContentBlocks.Categories
{
    public interface IContentBlockCategoryRepository
    {
        void Add(IContentBlockCategory category);

        void Remove(Guid id);

        IContentBlockCategory GetById(Guid id);

        IEnumerable<IContentBlockCategory> GetAll(bool includeHidden);
    }
}
