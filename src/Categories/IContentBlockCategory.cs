using System;

namespace Perplex.ContentBlocks.Categories
{
    public interface IContentBlockCategory
    {
        Guid Id { get; }
        string Name { get; }
        string Icon { get; }
        bool IsHidden { get; }
    }
}
