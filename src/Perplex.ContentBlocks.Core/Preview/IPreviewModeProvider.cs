namespace Perplex.ContentBlocks.Preview;

public interface IPreviewModeProvider
{
    /// <summary>
    /// Indicates if ContentBlocks are being rendered in the back office preview window.
    /// </summary>
    bool IsPreviewMode { get; }
}
