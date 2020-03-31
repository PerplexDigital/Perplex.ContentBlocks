using Athlon.Infrastructure.Accessors;
using Athlon.Infrastructure.ModelsBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockMap1ViewModel : ContentBlockViewModel<ContentBlockMap1>
    {
        public Dictionary<string, string> CountryNames { get; private set; }

        public ContentBlockMap1ViewModel(IGlobaalBeheerAccessor globaalBeheerAccessor, IBeheerAccessor beheerAccessor, ContentBlockMap1 content, Guid id, Guid definitionId, Guid layoutId)
            : base(content, id, definitionId, layoutId)
        {
            CountryNames = new Dictionary<string, string>();
            var localCountries = beheerAccessor.Beheer.FirstChild<Countries>().Children<Country>();
            var globalCountries = globaalBeheerAccessor.GlobaalBeheer.FirstChild<GlobalCountries>()?.Children<GlobalCountry>();
            foreach(var globalCountry in globalCountries)
            {
                var localCountry = localCountries.FirstOrDefault(x => x.GlobalCountry?.Id == globalCountry.Id);
                if(localCountry != null)
                {
                    CountryNames.Add(globalCountry.Key.ToString(), localCountry?.Name);
                }
                else
                {
                    CountryNames.Add(globalCountry.Key.ToString(), globalCountry.Name.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries).First());
                }
            }
        }
    }
}