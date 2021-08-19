using System;

namespace Perplex.ContentBlocks.Categories
{
    public interface IContentBlockCategory
    {
        Guid Id { get; }
        string Name { get; }
        string Icon { get; }

        /// <summary>
        /// Hidden categories do not show up in the UI
        /// </summary>
        bool IsHidden { get; }

        /// <summary>
        /// Enables this category when selecting a header
        /// </summary>
        bool IsEnabledForHeaders { get; }

        /// <summary>
        /// Disables this category when selecting a block
        /// </summary>
        bool IsDisabledForBlocks { get; }
    }
}
