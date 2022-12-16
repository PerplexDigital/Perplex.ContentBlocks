#if NET5_0_OR_GREATER

using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Extensions;

namespace Perplex.ContentBlocks.PropertyEditor
{
    /// <summary>
    /// Manifest Filter to ensure the main ContentBlocks JavaScript file that creates the AngularJS module "perplexContentBlocks" always runs first.
    /// In addition it ensures all package.manifest files are loaded.
    /// Starting with Umbraco v11 the load order of package.manifest files has changed and the file <see cref="MANIFEST_FILE"/> is no longer loaded first like it
    /// was in v8, v9 and v10.
    /// This filter is applied to v9 and v10 as well to pre-empt Umbraco backporting the v11 change to v9 and v10, breaking them too.
    /// </summary>
    public class ContentBlocksManifestFilter : IManifestFilter
    {
        private const string MANIFEST_FILE = "perplex.content-blocks.requires.js";
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly IManifestParser _manifestParser;

        public ContentBlocksManifestFilter(IWebHostEnvironment webHostEnv, IManifestParser manifestParser)
        {
            _webHostEnv = webHostEnv;
            _manifestParser = manifestParser;
        }

        public void Filter(List<PackageManifest> manifests)
        {
            EnsureManifestFilesAreLoaded(manifests);
            FixManifestLoadOrder(manifests);
        }

        /// <summary>
        /// Ensure our manifest files from App_Plugins\Perplex.ContentBlocks\ are loaded.
        /// When Umbraco fixes https://github.com/umbraco/Umbraco-CMS/issues/13565 this method will no longer be needed but
        /// at the same time shouldn't be harmful to keep around as it only does something when no Perplex.ContentBlocks manifests are found.
        /// </summary>
        /// <param name="manifests">Currently loaded manifests</param>
        private void EnsureManifestFilesAreLoaded(List<PackageManifest> manifests)
        {
            if (manifests is not null && !manifests.Any(IsContentBlocksManifest))
            {
                // Perplex.ContentBlocks manifest files are not loaded due to v11 bug, so if there not a single "Perplex.ContentBlocks"
                // entry in the current manifests we add them all.
                // We will manually read them from disk here ourselves.
                var contentBlocksPath = Path.Combine(_webHostEnv.ContentRootPath, "App_Plugins", "Perplex.ContentBlocks");
                if (Directory.Exists(contentBlocksPath))
                {
                    var contentBlocksManifests = Directory.GetFiles(contentBlocksPath, "package.manifest", SearchOption.AllDirectories);
                    foreach (var manifestPath in contentBlocksManifests)
                    {
                        if (!File.Exists(manifestPath)) continue;

                        var packageManifestJson = File.ReadAllText(manifestPath);
                        if (string.IsNullOrWhiteSpace(packageManifestJson)) continue;

                        var packageManifest = _manifestParser.ParseManifest(packageManifestJson);

                        // Umbraco throws when PackageName is null so ensure it is not.
                        packageManifest.PackageName ??= "";

                        manifests.Add(packageManifest);
                    }
                }
            }

            static bool IsContentBlocksManifest(PackageManifest manifest)
                => manifest?.Scripts?.Any(script => script?.Contains("/App_Plugins/Perplex.ContentBlocks/", StringComparison.OrdinalIgnoreCase) == true) == true;
        }

        private static void FixManifestLoadOrder(List<PackageManifest> manifests)
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
