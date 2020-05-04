using System;

namespace Perplex.ContentBlocks.Umbraco.Configuration
{
    [Flags]
    public enum EditorLayout
    {
        None = 0,

        Blocks = 1,
        Header = 2,

        All = Blocks | Header,
    }

    public class ContentBlocksConfiguration
    {
        public EditorLayout Layout { get; set; }
        public bool DisablePreview { get; set; }
    }
}
