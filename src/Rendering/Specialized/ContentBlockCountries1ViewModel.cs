using Athlon.Infrastructure.Accessors;
using Athlon.Infrastructure.ModelsBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Web;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockCountries1ViewModel : ContentBlockViewModel<ContentBlockCountries1>
    {
        public IEnumerable<Country> Countries { get; }

        public ContentBlockCountries1ViewModel(ICountriesAccessor countriesAccessor,
            ContentBlockCountries1 content, Guid id, Guid definitionId, Guid layoutId)
            : base(content, id, definitionId, layoutId)
        {
            Countries = countriesAccessor.Countries?.Children<Country>().Where(x => x.ShowInMenu);
        }
    }
}
