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
    public int Version { get; set; }

    /// <summary>
    /// The structure of the editor in the backoffice
    /// </summary>
    public Structure Structure { get; set; }

    /// <summary>
    /// Indicates if the preview component should be hidden
    /// </summary>
    public bool DisablePreview { get; set; }

    /// <summary>
    /// Indicates if the label of the editor should be hidden
    /// </summary>
    public bool HideLabel { get; set; }

    /// <summary>
    /// Indicates if the property group container should be hidden
    /// </summary>
    public bool HidePropertyGroupContainer { get; set; }

    /// <summary>
    /// Indicates if it is allowed to add blocks without first setting a header
    /// </summary>
    public bool AllowBlocksWithoutHeader { get; set; }

    /// <summary>
    /// Current configuration version.
    /// </summary>
    public const int VERSION = 3;

    public static readonly ContentBlocksConfiguration DefaultConfiguration = new()
    {
        Version = VERSION,

        HideLabel = true,
        Structure = Structure.Blocks | Structure.Header,
        DisablePreview = false,

        // It is quite likely this will default to "false" in the future
        // considering hiding the property group container is messing with
        // the default Umbraco UI and also causes some flickering upon page load
        // when the group is being hidden after our editor is initialized.
        HidePropertyGroupContainer = true,

        AllowBlocksWithoutHeader = false,
    };
}
