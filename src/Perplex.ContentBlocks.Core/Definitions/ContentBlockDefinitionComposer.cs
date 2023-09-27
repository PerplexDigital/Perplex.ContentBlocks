using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;

namespace Perplex.ContentBlocks.Definitions;

public class ContentBlockDefinitionComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddUnique<IContentBlockDefinitionRepository, InMemoryContentBlockDefinitionRepository>();
        builder.Services.AddSingleton<IContentBlockDefinitionFilterer, ContentBlockDefinitionFilterer>();
    }
}
