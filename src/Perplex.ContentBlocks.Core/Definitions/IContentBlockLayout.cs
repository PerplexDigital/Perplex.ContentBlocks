using System;

namespace Perplex.ContentBlocks.Definitions;

public interface IContentBlockLayout
{
    Guid Id { get; }
    string Name { get; }
    string Description { get; }
    string PreviewImage { get; }

    /// <summary>
    /// Full path to the View file of this ContentBlockLayout,
    /// e.g. "~/Views/Partials/ContentBlocks/ExampleBlock.cshtml"
    /// </summary>
    string ViewPath { get; }
}
