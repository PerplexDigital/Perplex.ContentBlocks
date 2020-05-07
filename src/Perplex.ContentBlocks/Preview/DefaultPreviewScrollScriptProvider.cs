namespace Perplex.ContentBlocks.Preview
{
    /// <summary>
    /// Default preview scroll script.
    /// Scrolls exactly to the anchor generated for a Content Block.
    /// Does not take into account a fixed header bar or other elements like that.
    /// </summary>
    public class DefaultPreviewScrollScriptProvider : IPreviewScrollScriptProvider
    {
        public string ScrollScript { get; } = @"
        if (element != null && typeof window.jump === ""function"") {
            window.jump(element, { duration: 500 });
        }";
    }
}
