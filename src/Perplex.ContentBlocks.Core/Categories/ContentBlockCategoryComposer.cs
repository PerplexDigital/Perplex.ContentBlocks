﻿using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;

namespace Perplex.ContentBlocks.Categories;

public class ContentBlockCategoriesComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddUnique<IContentBlockCategoryRepository, InMemoryContentBlockCategoryRepository>();
    }
}
