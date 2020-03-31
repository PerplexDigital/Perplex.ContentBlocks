using System;
using System.Collections.Generic;

namespace Perplex.ContentBlocks.Definitions
{
    public class SerializableContentBlockDefinition : IContentBlockDefinition
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Mirrored { get; set; }

        public string Intro { get; set; }

        public string Description { get; set; }

        public string PreviewImage { get; set; }

        public int? DataTypeId { get; set; }

        public Guid? DataTypeKey { get; set; }

        public List<Guid> CategoryIds { get; set; }

        public List<SerializableContentBlockLayout> Layouts { get; set; }

        public List<string> LimitToDocumentTypes { get; set; }

        public List<string> LimitToCultures { get; set; }

        IEnumerable<Guid> IContentBlockDefinition.CategoryIds => CategoryIds;
        IEnumerable<IContentBlockLayout> IContentBlockDefinition.Layouts => Layouts;

        IEnumerable<string> IContentBlockDefinition.LimitToDocumentTypes => LimitToDocumentTypes;
        IEnumerable<string> IContentBlockDefinition.LimitToCultures => LimitToCultures;
    }
}
