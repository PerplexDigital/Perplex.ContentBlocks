using System;
using System.Collections.Generic;
using System.Linq;
using static Perplex.ContentBlocks.Constants.PropertyEditor;
using categories = Perplex.ContentBlocks.Constants.Categories;

namespace Perplex.ContentBlocks.Categories
{
    public class InMemoryContentBlockCategoryRepository : IContentBlockCategoryRepository
    {
        private readonly IDictionary<Guid, IContentBlockCategory> _categories = new IContentBlockCategory[]
        {
            new ContentBlockCategory
            {
                Id = categories.Headers,
                Name = "Headers",
                Icon = $"{AssetsFolder}/icons.svg#icon-cat-header",
                IsEnabledForHeaders = true,
                IsDisabledForBlocks = true,
            },

            new ContentBlockCategory
            {
                Id = categories.Content,
                Name = "Content",
                Icon = $"{AssetsFolder}/icons.svg#icon-cat-content",
            },
        }.ToDictionary(d => d.Id);

        public IContentBlockCategory GetById(Guid id)
        {
            return _categories.TryGetValue(id, out var definition) ? definition : null;
        }

        public IEnumerable<IContentBlockCategory> GetAll(bool includeHidden)
        {
            var categories = _categories.Values;
            if (includeHidden)
            {
                return categories;
            }
            else
            {
                return categories.Where(category => !category.IsHidden);
            }
        }

        public void Add(IContentBlockCategory category)
            => _categories[category.Id] = category;

        public void Remove(Guid id)
            => _categories.Remove(id);
    }
}
