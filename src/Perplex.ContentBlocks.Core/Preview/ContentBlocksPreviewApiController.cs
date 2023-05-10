using HtmlAgilityPack;
using Perplex.ContentBlocks.Utils.Cookies;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using static Perplex.ContentBlocks.Constants.Preview;

#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
#elif NETFRAMEWORK
using System.Net.Http.Headers;
using System.Web.Http;
using Umbraco.Web.WebApi;
#endif

namespace Perplex.ContentBlocks.Preview
{
    public class ContentBlocksPreviewApiController : UmbracoAuthorizedApiController
    {
        private static readonly HttpClient _httpClient;
        private readonly IPreviewScrollScriptProvider _scrollScriptProvider;
        private readonly IHttpCookiesAccessor _httpCookiesAccessor;

        static ContentBlocksPreviewApiController()
        {
            var handler = new HttpClientHandler
            {
                UseCookies = false,
                // Do not validate any certificates, which fails on self signed certificates and causes the preview to fail.
                ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true
            };

            _httpClient = new HttpClient(handler);
        }

        public ContentBlocksPreviewApiController(IPreviewScrollScriptProvider scrollScriptProvider, IHttpCookiesAccessor httpCookiesAccessor)
        {
            _scrollScriptProvider = scrollScriptProvider;
            _httpCookiesAccessor = httpCookiesAccessor;
        }

        [HttpGet]
#if NET6_0_OR_GREATER
        public async Task<IActionResult> GetPreviewForIframe(int? pageId, string culture)
#elif NETFRAMEWORK
        public async Task<HttpResponseMessage> GetPreviewForIframe(int? pageId, string culture)
#endif
        {
            string html = string.Empty;

            if (pageId != null)
            {
                html = await GetPreviewHtml(pageId.Value, culture);
            }

#if NET6_0_OR_GREATER
            return Content(html, MediaTypeNames.Text.Html, Encoding.UTF8);
#elif NETFRAMEWORK
            var response = new HttpResponseMessage
            {
                Content = new StringContent(html)
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
#endif
        }

        private async Task<string> GetPreviewHtml(int pageId, string culture)
        {
#if NET6_0_OR_GREATER
            string host = Request.Scheme + "://" + Request.Host;
#elif NETFRAMEWORK
            string host = Request.RequestUri.GetLeftPart(UriPartial.Authority);
#endif
            string path = GetPreviewPath(pageId, culture);

            if (!Uri.TryCreate($"{host}/{path}", UriKind.Absolute, out Uri previewUri))
            {
                return string.Empty;
            }

            var message = new HttpRequestMessage(HttpMethod.Get, previewUri);

            IList<string> cookies = new List<string>();
            if (_httpCookiesAccessor.Cookies.TryGetValue(UmbracoCookieName, out string umbracoCookieValue))
            {
                cookies.Add($"{UmbracoCookieName}={umbracoCookieValue}");
            }

            // Preview cookie to enable preview mode
            cookies.Add($"{UmbracoPreviewCookieName}={UmbracoPreviewCookieValue}");

            string cookieHeader = string.Join("; ", cookies);
            message.Headers.Add("Cookie", cookieHeader);

            var result = await _httpClient.SendAsync(message);
            string html = await result.Content.ReadAsStringAsync();

            if (!result.IsSuccessStatusCode)
            {
                // Do not modify the HTML when it was not successful
                return html;
            }
            else
            {
                return TransformOriginalHtmlToPreviewHtml(html);
            }
        }

        /// <summary>
        /// Transforms the original HTML we receive from Umbraco's preview mode to the format we want.
        /// </summary>
        /// <param name="originalHtml">Original HTML of this page based on Umbraco's preview mode</param>
        /// <returns></returns>
        private string TransformOriginalHtmlToPreviewHtml(string originalHtml)
        {
            if (string.IsNullOrEmpty(originalHtml))
            {
                return originalHtml;
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(originalHtml);

            if (doc.DocumentNode.SelectSingleNode("//*[@id='umbracoPreviewBadge']") is HtmlNode previewLabel)
            {
                // Remove Umbraco Preview Badge
                previewLabel.Remove();
            }

            if (doc.DocumentNode.SelectSingleNode("//body") is HtmlNode body)
            {
                AppendScrollScript(body);
            }

            // .OuterHtml does not contain a DOCTYPE declaration so we add this manually
            return "<!DOCTYPE html>" + doc.DocumentNode.OuterHtml;
        }

        private void AppendScrollScript(HtmlNode parent)
        {
            string script = @"
                <script>
                window.addEventListener('message', receiveMessage, false);

                function receiveMessage(event) {
                    var element = document.getElementById(event.data.blockId);
                   " + _scrollScriptProvider.ScrollScript + @"
                }

                // Jump.js v1.0.2 - https://github.com/callmecavs/jump.js
                // Used as a (temporary?) replacement for window.scrollTo.
                // Chrome v81+ stopped respecting { behavior: ""smooth"" } and snaps instantly instead.
                // When it has been fixed this library can be removed.
                !function(){""use strict"";var o=function(o,n,t,i){return(o/=i/2)<1?t/2*o*o+n:-t/2*(--o*(o-2)-1)+n},n=""function""==typeof Symbol&&""symbol""==typeof Symbol.iterator?function(o){return typeof o}:function(o){return o&&""function""==typeof Symbol&&o.constructor===Symbol&&o!==Symbol.prototype?""symbol"":typeof o},t=function(){var t=void 0,i=void 0,e=void 0,r=void 0,u=void 0,c=void 0,d=void 0,a=void 0,f=void 0,s=void 0,l=void 0,v=void 0;function b(){return window.scrollY||window.pageYOffset}function w(o){return o.getBoundingClientRect().top+i}function y(o){f||(f=o),l=u(s=o-f,i,d,a),window.scrollTo(0,l),s<a?window.requestAnimationFrame(y):function(){window.scrollTo(0,i+d),t&&c&&(t.setAttribute(""tabindex"","" - 1""),t.focus());""function""==typeof v&&v();f=!1}()}return function(f){var s=arguments.length>1&&void 0!==arguments[1]?arguments[1]:{};switch(a=s.duration||1e3,r=s.offset||0,v=s.callback,u=s.easing||o,c=s.a11y||!1,i=b(),void 0===f?""undefined"":n(f)){case""number"":t=void 0,c=!1,e=i+f;break;case""object"":e=w(t=f);break;case""string"":t=document.querySelector(f),e=w(t)}switch(d=e-i+r,n(s.duration)){case""number"":a=s.duration;break;case""function"":a=s.duration(d)}window.requestAnimationFrame(y)}}();window.jump=t}();
                </script>
            ";

            var scriptNode = HtmlNode.CreateNode(script);
            parent.AppendChild(scriptNode);
        }

        private string GetPreviewPath(int pageId, string culture)
        {
            string path = UmbracoPreviewPath + "?id=" + pageId;
            if (!string.IsNullOrEmpty(culture))
            {
                // If a content type is not set to "allow varying by culture"
                // the culture will be null.
                path += "&culture=" + culture;
            }

            return path;
        }
    }
}
