using System;
using System.Collections.Generic;

namespace Perplex.ContentBlocks.Definitions
{
    public interface IContentBlockDefinitionService
    {
        /// <summary>
        /// Levert een specifiek contentblock op
        /// </summary>
        /// <param name="id">Het id van het contentblock</param>
        /// <returns></returns>
        IContentBlockDefinition GetById(Guid id);

        /// <summary>
        /// Levert alle contentblokken op die er zijn
        /// </summary>
        /// <returns></returns>
        IEnumerable<IContentBlockDefinition> GetAll();

        /// <summary>
        /// Levert alle contentblokken op die beschikbaar zijn voor de gegeven pagina
        /// </summary>        
        /// <returns></returns>
        IEnumerable<IContentBlockDefinition> GetAllForPage(int pageId, string culture);

        /// <summary>
        /// Levert alle contentblokken op die beschikbaar zijn voor de gegeven pagina
        /// </summary>        
        /// <returns></returns>
        IEnumerable<IContentBlockDefinition> GetAllForPage(string documentType, string culture);
    }
}
