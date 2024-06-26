using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Perplex.ContentBlocks.PropertyEditor;

public class ContentBlocksPropertyEditorComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<ContentBlocksValidator>();
        builder.Services.AddSingleton<ContentBlocksBlockContentConverter>();
    }
}
