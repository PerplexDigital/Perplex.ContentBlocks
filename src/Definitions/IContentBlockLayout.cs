using System;

namespace Perplex.ContentBlocks.Definitions
{
    public interface IContentBlockLayout
    {
        Guid Id { get; }
        string Name { get; }
        string Description { get; }
        string PreviewImage { get; }
        string ViewName { get; }
    }
}
