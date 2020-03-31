using Athlon.Rendering.Specialized;
using Athlon.Infrastructure.ModelsBuilder;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Rendering
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ContentBlocksRenderingComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            // Renderer
            composition.Register<IContentBlockRenderer, ContentBlockRenderer>(Lifetime.Singleton);

            // General View Model factory
            composition.Register(
                typeof(IContentBlockViewModelFactory<>),
                typeof(ContentBlockViewModelFactory<>), Lifetime.Singleton);

            // Specialized View Model factories
            composition.Register(
                typeof(IContentBlockViewModelFactory<ContentBlockSolutions1>),
                typeof(ContentBlockSolutions1ViewModelFactory), Lifetime.Scope);

            composition.Register(
                typeof(IContentBlockViewModelFactory<ContentBlockSlider1>),
                typeof(ContentBlockSlider1ViewModelFactory), Lifetime.Scope);

            composition.Register(
                typeof(IContentBlockViewModelFactory<ContentBlockPeople1>),
                typeof(ContentBlockPeople1ViewModelFactory), Lifetime.Scope);

            composition.Register(
                typeof(IContentBlockViewModelFactory<ContentBlockMap1>),
                typeof(ContentBlockMap1ViewModelFactory), Lifetime.Scope);

            composition.Register(
                typeof(IContentBlockViewModelFactory<ContentBlockArticles1>),
                typeof(ContentBlockArticles1ViewModelFactory), Lifetime.Scope);

            composition.Register(
                typeof(IContentBlockViewModelFactory<ContentBlockChallenges1>),
                typeof(ContentBlockChallenges1ViewModelFactory), Lifetime.Scope);

            composition.Register(
                typeof(IContentBlockViewModelFactory<ContentBlockNewsSummary1>),
                typeof(ContentBlockNewsSummary1ViewModelFactory), Lifetime.Scope);

            composition.Register(
                typeof(IContentBlockViewModelFactory<ContentBlockCountries1>),
                typeof(ContentBlockCountries1ViewModelFactory), Lifetime.Scope);

            composition.Register(
                typeof(IContentBlockViewModelFactory<ContentBlockFaq1>),
                typeof(ContentBlockFaq1ViewModelFactory), Lifetime.Scope);

            composition.Register(
               typeof(IContentBlockViewModelFactory<ContentBlockVideo1>),
               typeof(ContentBlockVideo1ViewModelFactory), Lifetime.Scope);

            composition.Register(
                typeof(IContentBlockViewModelFactory<ContentBlockArticleSlider1>),
                typeof(ContentBlockArticleSlider1ViewModelFactory), Lifetime.Scope);

            composition.Register(
                typeof(IContentBlockViewModelFactory<ContentBlockFeeddexRatings1>),
                typeof(ContentBlockFeeddexRatings1ViewModelFactory), Lifetime.Scope);

            composition.Register(
               typeof(IContentBlockViewModelFactory<ContentBlockHeaderLeaseDeals1>),
               typeof(ContentBlockHeaderLeaseDeals1ViewModelFactory), Lifetime.Scope);
        }
    }
}
