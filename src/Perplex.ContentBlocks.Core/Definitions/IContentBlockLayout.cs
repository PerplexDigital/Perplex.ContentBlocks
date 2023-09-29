using Perplex.ContentBlocks.Rendering;

namespace Perplex.ContentBlocks.Definitions;

public interface IContentBlockLayout
{
    Guid Id { get; }
    string Name { get; }
    string? Description { get; }
    string? PreviewImage { get; }

    /// <summary>
    /// Full path to the View file of this ContentBlockLayout,
    /// e.g. "~/Views/Partials/ContentBlocks/ExampleBlock.cshtml".
    /// When using <see cref="IContentBlockDefinition{T}"/> that uses a view component
    /// this property is optional since your defined view component will be responsible for rendering.
    /// The ViewPath will be called with model <see cref="IContentBlockViewModel{TContent}"/>.
    /// </summary>
    string? ViewPath { get; }
}
