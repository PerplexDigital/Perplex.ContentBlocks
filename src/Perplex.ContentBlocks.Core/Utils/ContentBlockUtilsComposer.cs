﻿#if NET5_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
#elif NET472
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif

namespace Perplex.ContentBlocks.Utils
{
#if NET5_0_OR_GREATER
    public class ContentBlockUtilsComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<ContentBlockUtils>();
        }
    }
#elif NET472
    [RuntimeLevel(MinLevel = RuntimeLevel.Boot)]
    public class ContentBlockUtilsComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<ContentBlockUtils>(Lifetime.Singleton);
        }
    }
#endif    
}
