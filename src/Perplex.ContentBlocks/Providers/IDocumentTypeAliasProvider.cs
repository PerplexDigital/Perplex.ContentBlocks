using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Perplex.ContentBlocks.Providers
{
    public interface IDocumentTypeAliasProvider
    {
        string GetDocumentTypeAlias(int pageId);
    }
}