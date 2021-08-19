﻿using System.Collections.Generic;

namespace Perplex.ContentBlocks.Utils.Cookies
{
    public interface IHttpCookiesAccessor
    {
        IDictionary<string, string> Cookies { get; }
    }
}
