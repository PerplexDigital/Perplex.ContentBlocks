using System;

namespace Perplex.ContentBlocks.Definitions
{
    public class ContentBlockLayout : IContentBlockLayout
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PreviewImage { get; set; }
        public string ViewPath { get; set; }
    }
}
