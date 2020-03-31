using Athlon.Infrastructure.ModelsBuilder;
using System.Linq;
using Umbraco.Core.Models.PublishedContent;

namespace Perplex.ContentBlocks.Extensions
{
    public static class ContentBlockExtensions
    {
        /// <summary>
        /// Levert de Image op die bij het gegeven ContentBlock hoort.
        /// Indien een ContentBlock geen Image heeft levert dit dus null op.
        /// </summary>
        /// <param name="contentBlock"></param>
        /// <returns></returns>
        public static IPublishedContent GetImage(this IPublishedElement contentBlock)
        {
            switch (contentBlock)
            {
                case ContentBlockCarbanner1 carbanner:
                    return carbanner.Image;

                case ContentBlockContact1 contact:
                    return contact.Image;

                case ContentBlockEvents1 events:
                    return events?.Item1?.Image;

                case ContentBlockForms1 form:
                    return form.Image;

                case ContentBlockForms2 form:
                    return form.Image;

                case ContentBlockHeaderCampaign1 headerCampaign:
                    return headerCampaign.Image;

                case ContentBlockHeaderHomepage1 headerHomepage:
                    return headerHomepage.Image;

                case ContentBlockHeaderHomepage2 headerHomepage:
                    return headerHomepage?.Slides?.FirstOrDefault()?.Image;

                case ContentBlockHeaderNormal1 headerNormal:
                    return headerNormal.Image;

                case ContentBlockHeaderNormal2 headerNormal:
                    return headerNormal.Image;

                case ContentBlockHeaderTransparent1 headerTransparant:
                    return headerTransparant.Image;

                case ContentBlockImageIconGrid1 imageIconGrid:
                    return imageIconGrid?.Blocks?.FirstOrDefault()?.Image;

                case ContentBlockIntro1 intro:
                    return intro.Image;

                case ContentBlockIntro2 intro:
                    return intro.Image;

                case ContentBlockNewsSummary1 newsSummary:
                    return newsSummary?.NewsItems?.FirstOrDefault()?.HeaderImage;

                case ContentBlockNormal1 normal:
                    return normal.Image;

                case ContentBlockNormal2 normal:
                    return normal.Image;

                case ContentBlockPageLinks1 pagelinks:
                    return pagelinks?.Item1?.Image;

                case ContentBlockParallax parallax:
                    return parallax.Image1;

                case ContentBlockSlider1 slider:
                    return slider?.Slides?.FirstOrDefault()?.Image;

                case ContentBlockThreeColumns1 threeColumns:
                    return threeColumns.Image;

                case ContentBlockThreeColumns2 threeColumns:
                    return threeColumns.Image;

                case ContentBlockTimeline1 timeline:
                    return timeline?.Milestones?.FirstOrDefault()?.Image;

                case ContentBlockArticles1 _:
                case ContentBlockChallenges1 _:
                case ContentBlockCountries1 _:
                case ContentBlockFaq1 _:
                case ContentBlockForms3 _:
                case ContentBlockIntro3 _:
                case ContentBlockKeypoints1 _:
                case ContentBlockMap1 _:
                case ContentBlockPeople1 _:
                case ContentBlockSolutions1 _:
                case ContentBlockStrategicPartners1 _:
                case ContentBlockText1 _:
                case ContentBlockTextAndLinks1 _:
                case ContentBlockTextAndLinks2 _:
                case ContentBlockTextTwoColumns1 _:
                    return null;

                default:
                    throw new System.Exception($"GetHeaderImage() not implemented for {contentBlock.ContentType.Alias}");
            }
        }
    }
}
