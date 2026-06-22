namespace Perplex.ContentBlocks.Preview;

/// <summary>
/// Default preview scroll script.
/// Scrolls exactly to the anchor generated for a Content Block.
/// Does not take into account a fixed header bar or other elements like that.
/// </summary>
public class DefaultPreviewScrollScriptProvider : IPreviewScrollScriptProvider
{
    private const string _scrollScript = @"
        if (typeof window.scrollTo === ""function"") {
            window.scrollTo({
                top: element.offsetTop,
                behavior: ""smooth""
            });
        }";

    public string ScrollScript { get; } = _scrollScript;
}
