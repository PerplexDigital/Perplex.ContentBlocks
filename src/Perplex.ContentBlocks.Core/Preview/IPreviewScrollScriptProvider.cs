namespace Perplex.ContentBlocks.Preview;

public interface IPreviewScrollScriptProvider
{
    /// <summary>
    /// JavaScript function to execute when scrolling to a specific Content Block.
    /// The JavaScript provided will be executed within a function scope that
    /// has already set an "element" variable to the DOM element of the scroll anchor.
    /// </summary>
    string ScrollScript { get; }
}
