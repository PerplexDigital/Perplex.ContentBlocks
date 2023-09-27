using System.Diagnostics;
using Umbraco.Cms.Core.Manifest;

namespace Perplex.ContentBlocks.StaticAssets;

internal class ManifestFilter : IManifestFilter
{
    void IManifestFilter.Filter(List<PackageManifest> manifests)
    {
        var assembly = typeof(PackageManifest).Assembly;
        var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

        manifests.Add(new PackageManifest
        {
            PackageName = "Perplex.ContentBlocks",
            Version = versionInfo.ProductVersion ?? "1.0.0",
            AllowPackageTelemetry = false,
            BundleOptions = BundleOptions.Default,
            Scripts = new[]
            {
                // Note: .requires.js should be loaded first
                "/App_Plugins/Perplex.ContentBlocks/perplex.content-blocks.requires.js",

                "/App_Plugins/Perplex.ContentBlocks/perplex.content-blocks.api.js",
                "/App_Plugins/Perplex.ContentBlocks/perplex.content-blocks.controller.js",
                "/App_Plugins/Perplex.ContentBlocks/components/perplex.content-block.component.js",
                "/App_Plugins/Perplex.ContentBlocks/components/perplex.content-blocks.add-block.js",
                "/App_Plugins/Perplex.ContentBlocks/components/perplex.content-blocks.custom-component.js",
                "/App_Plugins/Perplex.ContentBlocks/components/perplex.content-blocks.custom-components.js",
                "/App_Plugins/Perplex.ContentBlocks/components/perplex.content-blocks.icon.js",
                "/App_Plugins/Perplex.ContentBlocks/components/perplex.content-blocks.nested-content-patch.js",
                "/App_Plugins/Perplex.ContentBlocks/configuration/perplex.content-blocks.configuration.structure.js",
                "/App_Plugins/Perplex.ContentBlocks/lib/angular-slick.js",
                "/App_Plugins/Perplex.ContentBlocks/lib/slick.min.js",
                "/App_Plugins/Perplex.ContentBlocks/utils/perplex.content-blocks.copy-paste-service.js",
                "/App_Plugins/Perplex.ContentBlocks/utils/perplex.content-blocks.utils.js",
                "/App_Plugins/Perplex.ContentBlocks/utils/portal/perplex.content-blocks-portal.js",
                "/App_Plugins/Perplex.ContentBlocks/utils/property/perplex.content-blocks.property-scaffold-cache.js",
                "/App_Plugins/Perplex.ContentBlocks/utils/property/perplex.content-blocks.property.js",
                "/App_Plugins/Perplex.ContentBlocks/utils/tab-focus/perplex.content-blocks.tab-focus-once.directive.js",
                "/App_Plugins/Perplex.ContentBlocks/utils/tab-focus/perplex.content-blocks.tab-focus.service.js",
            },

            Stylesheets = new[]
            {
                "/App_Plugins/Perplex.ContentBlocks/perplex.content-blocks.css",
            },
        });
    }
}
