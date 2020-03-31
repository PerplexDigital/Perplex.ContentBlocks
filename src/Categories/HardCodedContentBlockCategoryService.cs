using System;
using System.Collections.Generic;
using System.Linq;
using categories = Athlon.Constants.ContentBlocks.Categories;
using static Athlon.Constants.ContentBlocks.Umbraco;

namespace Perplex.ContentBlocks.Categories
{
    public class HardCodedContentBlockCategoryService : IContentBlockCategoryService
    {
        private readonly IDictionary<Guid, IContentBlockCategory> _categories = new IContentBlockCategory[]
        {
            new ContentBlockCategory
            {
                Id = categories.Headers,
                Name = "Headers",
                Icon = $"{ContentBlocksAssetsFolder}/icons.svg#icon-cat-header"
            },

            new ContentBlockCategory
            {
                Id = categories.Content,
                Name = "Content",
                Icon = $"{ContentBlocksAssetsFolder}/icons.svg#icon-cat-content"
            },

            new ContentBlockCategory
            {
                Id = categories.Specials,
                Name = "Specials",
                Icon = $"{ContentBlocksAssetsFolder}/icons.svg#icon-cat-special"
            },

            new ContentBlockCategory
            {
                Id = categories.Conversion,
                Name = "Conversion",
                Icon = $"{ContentBlocksAssetsFolder}/icons.svg#icon-cat-conversion"
            },

            new ContentBlockCategory
            {
                Id = categories.Forms,
                Name = "Forms",
                Icon = $"{ContentBlocksAssetsFolder}/icons.svg#icon-cat-form"
            },

            new ContentBlockCategory
            {
                Id = categories.System,
                Name = "System",
                Icon = $"{ContentBlocksAssetsFolder}/icons.svg#icon-cat-lock",
                IsHidden = true
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
    }
}
