using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Web.WebApi;
using static Perplex.ContentBlocks.Constants.Preview;

namespace Perplex.ContentBlocks.Preview
{
    public class ContentBlocksPreviewApiController : UmbracoAuthorizedApiController
    {
        private static readonly HttpClientHandler _httpClientHandler = new HttpClientHandler { UseCookies = false };
        private static readonly HttpClient _httpClient = new HttpClient(_httpClientHandler);

        [HttpGet]
        public async Task<HttpResponseMessage> GetPreviewForIframe(int? pageId, string culture)
        {
            string html = string.Empty;

            if (pageId != null)
            {
                html = await GetPreviewHtml(pageId.Value, culture);
            }

            var response = new HttpResponseMessage
            {
                Content = new StringContent(html)
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        private async Task<string> GetPreviewHtml(int pageId, string culture)
        {
            string host = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            string path = GetPreviewPath(pageId, culture);

            if (!Uri.TryCreate($"{host}/{path}", UriKind.Absolute, out Uri previewUri))
            {
                return string.Empty;
            }

            var message = new HttpRequestMessage(HttpMethod.Get, previewUri);

            var umbracoCookie = Request.Headers.GetCookies()?.FirstOrDefault()?.Cookies?.FirstOrDefault(cc => cc.Name == UmbracoCookieName);
            IList<string> cookies = new List<string>();

            if (umbracoCookie != null)
            {
                cookies.Add($"{umbracoCookie.Name}={umbracoCookie.Value}");
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

            var doc = new HtmlDocument();
            var node = HtmlNode.CreateNode(originalHtml);
            doc.DocumentNode.AppendChild(node);

            doc.DocumentNode.FirstChild.AddClass("perplex-preview");

            var previewLabel = doc.DocumentNode.SelectSingleNode("//*[@id='umbracoPreviewBadge']");
            if (previewLabel != null)
            {
                // Remove Umbraco Preview Badge
                previewLabel.Remove();
            }

            var body = doc.DocumentNode.SelectSingleNode("//body");
            if (body != null)
            {
                var script = GetScrollSyncScript(doc);
                body.AppendChild(script);
            }

            // .OuterHtml does not contain a DOCTYPE declaration so we add this manually
            return "<!DOCTYPE html>" + doc.DocumentNode.OuterHtml;
        }

        private HtmlNode GetScrollSyncScript(HtmlDocument doc)
        {
            string script = @"
                window.addEventListener('message', receiveMessage, false);

                function receiveMessage(event) {
                    var id = event.data.blockId;
                    if (id != null) {
                        Perplex.Util.scrollToElement($(""a[id="" + id + ""]""));
                    }
                }";

            return new HtmlNode(HtmlNodeType.Element, doc, 0)
            {
                Name = "script",
                InnerHtml = script
            };
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
