#if NET5_0_OR_GREATER

using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Extensions;

namespace Perplex.ContentBlocks.PropertyEditor
{
    /// <summary>
    /// Manifest Filter to ensure the main ContentBlocks JavaScript file that creates the AngularJS module "perplexContentBlocks" always runs first.
    /// Starting with Umbraco v11 the load order of package.manifest files has changed and the file <see cref="MANIFEST_FILE"/> is no longer loaded first like it
    /// was in v8, v9 and v10.
    /// This filter is applied to v9 and v10 as well to pre-empt Umbraco backporting the v11 change to v9 and v10, breaking them too.
    /// </summary>
    public class ContentBlocksManifestFilter : IManifestFilter
    {
        private const string MANIFEST_FILE = "perplex.content-blocks.requires.js";

        public void Filter(List<PackageManifest> manifests)
        {
            var requiresManifest = manifests.FirstOrDefault(m => m.Scripts.Any(script => script.InvariantEndsWith(MANIFEST_FILE)));

            if (requiresManifest is not null)
            {
                // Ensure the requires file is loaded first
                manifests.Remove(requiresManifest);
                manifests.Insert(0, requiresManifest);
            }
        }
    }

    public class ContentBlocksManifestFilterComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
            => builder.ManifestFilters().Append<ContentBlocksManifestFilter>();
    }
}

#endif
