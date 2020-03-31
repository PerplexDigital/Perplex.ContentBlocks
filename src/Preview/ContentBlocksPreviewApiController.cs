using Athlon.Features.Cookiemelding;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Web.WebApi;
using static Athlon.Constants.ContentBlocks.Preview;

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

            // Doe alsof onze cookies zijn geaccepteerd zodat er geen cookiebar in beeld komt
            cookies.Add($"{CookiemeldingManager.PERPLEX_COOKIE_NAME}={(int)EnmCookieApproval.All}");

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
        /// Verandert de originele HTML die we krijgen van Umbraco's preview modus
        /// naar de HTML zoals wij hem willen hebben.
        /// </summary>
        /// <param name="originalHtml">Originele HTML van de pagina op basis van Umbraco's preview modus</param>
        /// <returns></returns>
        private string TransformOriginalHtmlToPreviewHtml(string originalHtml)
        {
            var doc = new HtmlDocument();
            var node = HtmlNode.CreateNode(originalHtml);
            doc.DocumentNode.AppendChild(node);

            doc.DocumentNode.FirstChild.AddClass("perplex-preview");

            var previewLabel = doc.DocumentNode.SelectSingleNode("//a[@id='umbracoPreviewBadge']");
            if (previewLabel != null)
            {
                previewLabel.Remove();
            }

            var body = doc.DocumentNode.SelectSingleNode("//body");
            if (body != null)
            {
                var script = GetScrollSyncScript(doc);
                body.AppendChild(script);
            }

            // .OuterHtml bevat niet doctype declaratie dus die voegen we handmatig toe
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
