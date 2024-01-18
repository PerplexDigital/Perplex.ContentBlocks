using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;

namespace Perplex.ContentBlocks.PropertyEditor.ModelValue;

public class ContentBlocksModelValueCopyingHandler : INotificationHandler<ContentCopyingNotification>
{
    private readonly ILogger<ContentBlocksModelValueCopyingHandler> _logger;

    public ContentBlocksModelValueCopyingHandler(ILogger<ContentBlocksModelValueCopyingHandler> logger)
    {
        _logger = logger;
    }

    public void Handle(ContentCopyingNotification notification)
        => UpdateContentBlocksKeys(notification.Copy);

    private void UpdateContentBlocksKeys(IContent entity)
    {
        try
        {
            var properties = entity.Properties.Where(p => p.PropertyType.PropertyEditorAlias == Constants.PropertyEditor.Alias);
            foreach (var prop in properties)
            {
                // Update all property values -- i.e. all variants
                foreach (var propValue in prop.Values)
                {
                    if (propValue is null) continue;

                    string? culture = propValue.Culture;
                    string? segment = propValue.Segment;

                    string? value = propValue.EditedValue as string;
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    var contentBlocks = JsonConvert.DeserializeObject<JObject>(value);

                    var header = contentBlocks?.Value<JObject>("header");
                    if (header is not null)
                    {
                        UpdateContentBlockKeys(header);
                    }

                    if (contentBlocks?.Value<JArray>("blocks") is JArray blocks)
                    {
                        foreach (JObject block in blocks)
                        {
                            UpdateContentBlockKeys(block);
                        }
                    }

                    prop.SetValue(JsonConvert.SerializeObject(contentBlocks), culture: culture, segment: segment);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update ContentBlock keys and IDs while copying a node.");
        }
    }

    private void UpdateContentBlockKeys(JObject block)
    {
        block["id"] = Guid.NewGuid();

        if (block.Value<JArray>("content") is JArray nestedContentItems)
        {
            foreach (var nestedContentItem in nestedContentItems)
            {
                UpdateNestedContentKey(nestedContentItem as JObject);
            }
        }

        var variants = block.Value<JArray>("variants");
        if (variants != null)
        {
            foreach (JObject variant in variants)
            {
                UpdateContentBlockKeys(variant);
            }
        }
    }

    private void UpdateNestedContentKey(JObject? nestedContent)
    {
        if (nestedContent is null || nestedContent["key"] == null)
        {
            return;
        }

        nestedContent["key"] = Guid.NewGuid();

        // Also update any nested Nested Content items inside this nestedContent
        foreach (var property in nestedContent.Properties())
        {
            // NestedContent stores nested NestedContent as strings rather than arrays for some reason,
            // i.e. if one of the properties inside this object is also
            // a NestedContent its value will be a string that looks like this:
            // "[{ \"key\": \" ... \" }]" instead of a real array like [{ "key": "..." }]
            // This means we have to actually parse each value to check if it's a JArray or not.
            if (
                property.Value is JValue value &&
                value.Type == JTokenType.String &&
                value.ToString() is string rawValue &&
                rawValue.TrimStart().StartsWith("[")
            )
            {
                try
                {
                    var array = JArray.Parse(value.ToString());

                    if (IsNestedContentValue(array))
                    {
                        foreach (var child in array.Children<JObject>())
                        {
                            UpdateNestedContentKey(child);
                        }

                        nestedContent[property.Name] = JsonConvert.SerializeObject(array);
                    }
                }
                catch (JsonReaderException)
                {
                    // Not actually an Array -- just ignore
                }
            }
        }
    }

    private bool IsNestedContentValue(JArray value)
    {
        return value.First is JObject obj && obj["key"] != null && obj["ncContentTypeAlias"] != null;
    }
}
