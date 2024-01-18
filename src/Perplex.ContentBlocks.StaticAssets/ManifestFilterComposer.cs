using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Perplex.ContentBlocks.StaticAssets;

public class ManifestFilterComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
        => builder.ManifestFilters().Append<ManifestFilter>();
}
