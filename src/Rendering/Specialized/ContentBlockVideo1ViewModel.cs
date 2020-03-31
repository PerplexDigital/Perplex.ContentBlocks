using Athlon.Features.Cookiemelding;
using Athlon.Infrastructure.Accessors;
using Athlon.Infrastructure.ModelsBuilder;
using System;
using System.Web;

namespace Perplex.ContentBlocks.Rendering.Specialized
{
    public class ContentBlockVideo1ViewModel : ContentBlockViewModel<ContentBlockVideo1>
    {
        public CookiemeldingManager CookiemeldingManager { get; }
        public string LabelPlayVideo { get; }

        public IHtmlString VideoIframe {
            get {
                if (string.IsNullOrEmpty(this.Content.VideoId))
                    return null;


                var isVimeo = int.TryParse(this.Content.VideoId, out int n);

                HtmlString video;
                if (isVimeo)
                {
                    video = new HtmlString("<iframe class=\"g-content-video-1__base__frame js-contentVideoFrame\" src=\"https://player.vimeo.com/video/" + this.Content.VideoId + "?api=1\" frameborder=\"0\" webkitallowfullscreen mozallowfullscreen allowfullscreen allow=\"autoplay\"></iframe>");
                }
                else
                {
                    video = new HtmlString("<iframe class=\"g-content-video-1__base__frame js-contentVideoFrame\" src=\"https://www.youtube.com/embed/" + Content.VideoId + "\" frameborder=\"0\" allow=\"accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe>");
               
                }
               
                return CookiemeldingManager.InsertIframePlaceholdersIfNecessary(video);
            }
        }

        public ContentBlockVideo1ViewModel(
            IGlobalTextAccessor globalTextAccessor,
            ICookiemeldingAccessor cookiemeldingAccessor, 
            ContentBlockVideo1 content, Guid id, Guid definitionId, Guid layoutId)
            : base(content, id, definitionId, layoutId)
        {

            CookiemeldingManager = new CookiemeldingManager(HttpContext.Current, cookiemeldingAccessor);
            LabelPlayVideo = globalTextAccessor.GlobalText?.LabelPlayVideo;
        }
    }
}
