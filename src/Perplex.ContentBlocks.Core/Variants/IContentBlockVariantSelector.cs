﻿using Perplex.ContentBlocks.PropertyEditor.ModelValue;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Perplex.ContentBlocks.Variants;

public interface IContentBlockVariantSelector
{
    /// <summary>
    /// Select a variant to render for the given block.
    /// To render the default block content; return null.
    /// </summary>
    /// <param name="block">The block to select a variant for</param>
    /// <param name="content">The content containing the block</param>
    /// <param name="preview">Indicates if this block is rendered in preview mode</param>
    /// <returns>A variant to render for this block; or null if the default content should be rendered.</returns>
    ContentBlockVariantModelValue? SelectVariant(ContentBlockModelValue block, IPublishedElement content, bool preview);
}
