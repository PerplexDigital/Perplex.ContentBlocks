using Athlon.Infrastructure.Accessors;
using Athlon.Infrastructure.ModelsBuilder;
using System;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockCountries1ViewModelFactory : ContentBlockViewModelFactory<ContentBlockCountries1>
    {
        private readonly ICountriesAccessor _countriesAccessor;

        public ContentBlockCountries1ViewModelFactory(ICountriesAccessor countriesAccessor)
        {
            _countriesAccessor = countriesAccessor;
        }

        public override IContentBlockViewModel<ContentBlockCountries1> Create(ContentBlockCountries1 content, Guid id, Guid definitionId, Guid layoutId)
            => new ContentBlockCountries1ViewModel(_countriesAccessor, content, id, definitionId, layoutId);
    }
}
