using Umbraco.Cms.Core.PropertyEditors;
using static Perplex.ContentBlocks.Constants.PropertyEditor.Configuration;

namespace Perplex.ContentBlocks.PropertyEditor.Configuration;

/// <summary>
/// The structure of the editor in the backoffice
/// </summary>
[Flags]
public enum Structure
{
    /// <summary>
    /// Default value - nothing
    /// </summary>
    None = 0,

    /// <summary>
    /// Blocks only
    /// </summary>
    Blocks = 1,

    /// <summary>
    /// Header only
    /// </summary>
    Header = 2,

    /// <summary>
    /// Blocks and Header
    /// </summary>
    All = Blocks | Header,
}

/// <summary>
/// ContentBlocks configuration
/// </summary>
public class ContentBlocksConfiguration
{
    /// <summary>
    /// Added for detecting out of date configuration objects in the future.
    /// </summary>
    [ConfigurationField(VersionKey)]
    public int Version { get; set; }

    /// <summary>
    /// The structure of the editor in the backoffice
    /// </summary>
    [ConfigurationField(StructureKey)]
    public Structure Structure { get; set; }

    /// <summary>
    /// Current configuration version.
    /// </summary>
    public const int CurrentVersion = 4;

    public static readonly ContentBlocksConfiguration DefaultConfiguration = new()
    {
        Version = CurrentVersion,
        Structure = Structure.All
    };
}
