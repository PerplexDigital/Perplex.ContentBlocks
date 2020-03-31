using Athlon.Infrastructure.ModelsBuilder;
using System;
using System.Collections.Generic;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockPeople1ViewModel : ContentBlockViewModel<ContentBlockPeople1>
    {
        public IEnumerable<Person> People { get; }

        public ContentBlockPeople1ViewModel(
            IEnumerable<Person> people, ContentBlockPeople1 content, Guid id, Guid definitionId, Guid layoutId)
            : base(content, id, definitionId, layoutId)
        {
            People = people;
        }
    }
}
