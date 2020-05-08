using System;

namespace Perplex.ContentBlocks.PropertyEditor.Configuration
{
    [Flags]
    public enum Structure
    {
        None = 0,

        Blocks = 1,
        Header = 2,

        All = Blocks | Header,
    }

    public class ContentBlocksConfiguration
    {
        /// <summary>
        /// Added for detecting out of date configuration objects in the future.
        /// </summary>
        public int Version { get; set; }

        public Structure Structure { get; set; }
        public bool DisablePreview { get; set; }
        public bool HideLabel { get; set; }
    }
}
