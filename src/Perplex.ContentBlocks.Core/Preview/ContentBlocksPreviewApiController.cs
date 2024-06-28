using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Perplex.ContentBlocks.Api;
using System.Net.Mime;
using System.Text;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Preview;
using Umbraco.Cms.Core.Security;

namespace Perplex.ContentBlocks.Preview;

public class ContentBlocksPreviewApiController(
    IPreviewScrollScriptProvider scrollScriptProvider, IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
    IPreviewTokenGenerator previewTokenGenerator) : ContentBlocksApiControllerBase
{
    private static readonly HttpClient _httpClient;

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

    [HttpGet("preview")]
    public async Task<IActionResult> GetPreviewForIframe(Guid? pageId, string? culture)
    {
        string html = string.Empty;

        if (pageId != null &&
            backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser is IUser user &&
            await previewTokenGenerator.GenerateTokenAsync(user.Key) is var attempt &&
            attempt.Result is string previewToken)
        {
            html = await GetPreviewHtml(pageId.Value, culture, previewToken);
        }

        return Content(html, MediaTypeNames.Text.Html, Encoding.UTF8);
    }

    private async Task<string> GetPreviewHtml(Guid pageId, string? culture, string previewToken)
    {
        string host = Request.Scheme + "://" + Request.Host;
        string path = GetPreviewPath(pageId, culture);

        if (!Uri.TryCreate($"{host}/{path}", UriKind.Absolute, out Uri? previewUri))
        {
            return string.Empty;
        }

        var message = new HttpRequestMessage(HttpMethod.Get, previewUri);

        string[] cookies = [
            // Preview cookie to enable preview mode
            $"{Umbraco.Cms.Core.Constants.Web.PreviewCookieName}={previewToken}"
        ];

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

        HtmlDocument doc = new();
        doc.LoadHtml(originalHtml);

        if (doc.DocumentNode.SelectSingleNode("//body") is HtmlNode body)
        {
            AppendScrollScript(body);
        }

        return doc.DocumentNode.OuterHtml;
    }

    private void AppendScrollScript(HtmlNode parent)
    {
        string script = @"
<script>
window.addEventListener('message', receiveMessage, false);

function receiveMessage(event) {
    var element = document.getElementById(event.data.blockId);

    if (element != null) {
        " + scrollScriptProvider.ScrollScript + @"
    }
}
</script>";

        var scriptNode = HtmlNode.CreateNode(script);
        parent.AppendChild(scriptNode);
    }

    private static string GetPreviewPath(Guid pageId, string? culture)
    {
        string path = $"{pageId}";
        if (!string.IsNullOrEmpty(culture))
        {
            // If a content type is not set to "allow varying by culture"
            // the culture will be null.
            path += "?culture=" + culture;
        }

        return path;
    }
}
