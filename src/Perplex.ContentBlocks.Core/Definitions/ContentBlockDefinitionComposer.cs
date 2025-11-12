using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Perplex.ContentBlocks.Definitions;

public class ContentBlockDefinitionComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<IContentBlockDefinitionRepository, InMemoryContentBlockDefinitionRepository>();
        builder.Services.AddSingleton<IContentBlockDefinitionFilterer, ContentBlockDefinitionFilterer>();
    }
}
