using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Perplex.ContentBlocks.Utils.Cookies;
using System.Net.Mime;
using System.Text;
using Umbraco.Cms.Web.BackOffice.Controllers;
using static Perplex.ContentBlocks.Constants.Preview;

namespace Perplex.ContentBlocks.Preview;

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
    public async Task<IActionResult> GetPreviewForIframe(int? pageId, string culture)
    {
        string html = string.Empty;

        if (pageId != null)
        {
            html = await GetPreviewHtml(pageId.Value, culture);
        }

        return Content(html, MediaTypeNames.Text.Html, Encoding.UTF8);
    }

    private async Task<string> GetPreviewHtml(int pageId, string culture)
    {
        string host = Request.Scheme + "://" + Request.Host;
        string path = GetPreviewPath(pageId, culture);

        if (!Uri.TryCreate($"{host}/{path}", UriKind.Absolute, out Uri? previewUri))
        {
            return string.Empty;
        }

        var message = new HttpRequestMessage(HttpMethod.Get, previewUri);

        IList<string> cookies = new List<string>();
        if (_httpCookiesAccessor.Cookies.TryGetValue(UmbracoCookieName, out string? umbracoCookieValue))
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

    if (element != null) {
        " + _scrollScriptProvider.ScrollScript + @"
    }
}
</script>";

        var scriptNode = HtmlNode.CreateNode(script);
        parent.AppendChild(scriptNode);
    }

    private static string GetPreviewPath(int pageId, string culture)
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
