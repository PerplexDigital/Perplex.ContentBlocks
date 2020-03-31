using System;
using System.Collections.Generic;

namespace Perplex.ContentBlocks.Presets
{
    public interface IContentBlocksPresetService
    {
        IEnumerable<IContentBlocksPreset> GetAll();
        IContentBlocksPreset GetPresetForPage(int pageId, string culture);
        IContentBlocksPreset GetPresetForPage(string documentType, string culture);
    }
}
