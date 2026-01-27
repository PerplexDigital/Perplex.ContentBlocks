namespace Perplex.ContentBlocks.StaticAssets.PackageManifest;

using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

public class PackageManifestReaderComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<IPackageManifestReader, PackageManifestReader>();
    }
}

public class PackageManifestReader : IPackageManifestReader
{
    public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync()
    {
        var assembly = typeof(PackageManifestReader).Assembly;
        var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        var version = versionInfo.ProductVersion ?? "1.0.0";

        IEnumerable<PackageManifest> manifests =
        [
            new PackageManifest()
            {
                Name = "Perplex.ContentBlocks",
                AllowPublicAccess = false,
                AllowTelemetry = false,
                Version = version,
                Extensions =
                [
                    new
                    {
                        type = "backofficeEntryPoint",
                        alias = "Perplex.ContentBlocks EntryPoint",
                        name = "Perplex.ContentBlocks EntryPoint",
                        js = $"/App_Plugins/Perplex.ContentBlocks/perplex.content-blocks.js?v={version}"
                    }
                ]
            }
        ];

        return Task.FromResult(manifests);
    }
}
