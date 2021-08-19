using System;
using System.Collections.Generic;
using System.Linq;

namespace Perplex.ContentBlocks.Definitions
{
    public class ContentBlockDefinition : IContentBlockDefinition
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PreviewImage { get; set; }
        public int? DataTypeId { get; set; }
        public Guid? DataTypeKey { get; set; }
        public IEnumerable<Guid> CategoryIds { get; set; }
        public IEnumerable<IContentBlockLayout> Layouts { get; set; }

        public virtual IEnumerable<string> LimitToDocumentTypes { get; set; }
            = Enumerable.Empty<string>();

        public virtual IEnumerable<string> LimitToCultures { get; set; }
            = Enumerable.Empty<string>();
    }
}
