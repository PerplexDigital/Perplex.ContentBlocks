using Athlon.Features.OurPeople;
using Athlon.Infrastructure.ModelsBuilder;
using System;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockPeople1ViewModelFactory : ContentBlockViewModelFactory<ContentBlockPeople1>
    {
        private readonly IPeopleAccessor _peopleAccessor;

        public ContentBlockPeople1ViewModelFactory(IPeopleAccessor peopleAccessor)
        {
            _peopleAccessor = peopleAccessor;
        }

        public override IContentBlockViewModel<ContentBlockPeople1> Create(ContentBlockPeople1 content, Guid id, Guid definitionId, Guid layoutId)
        {
            var people = _peopleAccessor.GetAllPersons();
            return new ContentBlockPeople1ViewModel(people, content, id, definitionId, layoutId);
        }
    }
}
