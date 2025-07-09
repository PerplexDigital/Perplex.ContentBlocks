using System.Diagnostics;

namespace Perplex.ContentBlocks;

public static partial class Constants
{
    public static partial class PropertyEditor
    {
        /// <summary>
        /// The property editor alias of Perplex.ContentBlocks.
        /// </summary>
        public const string Alias = "Perplex.ContentBlocks";

        /// <summary>
        /// The property editor name of Perplex.ContentBlocks.
        /// </summary>
        public const string Name = "Perplex.ContentBlocks";

        /// <summary>
        /// The property editor UI alias of Perplex.ContentBlocks.
        /// </summary>
        public const string UiAlias = "Perplex.ContentBlocks";

        /// <summary>
        /// The view path to the main ContentBlocks HTML, including a cache buster when the product version can be read from the DLL.
        /// </summary>
        public static readonly string ViewPath = "/App_Plugins/Perplex.ContentBlocks/perplex.content-blocks.html" + (GetProductVersion() is string version ? "?v=" + version : "");

        public const string AssetsFolder = "/App_Plugins/Perplex.ContentBlocks/assets";

        private static string? GetProductVersion()
        {
            try
            {
                if (typeof(PropertyEditor).Assembly.Location is string assemblyLocation &&
                    !string.IsNullOrWhiteSpace(assemblyLocation) &&
                    FileVersionInfo.GetVersionInfo(assemblyLocation).ProductVersion is string productVersion &&
                    !string.IsNullOrWhiteSpace(productVersion))
                {
                    return productVersion;
                }
            }
            catch
            {
                // Ignore
            }

            return null;
        }
    }
}
