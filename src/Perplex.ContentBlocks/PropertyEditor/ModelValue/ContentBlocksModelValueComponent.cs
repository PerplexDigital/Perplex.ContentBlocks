using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Services.Implement;

namespace Perplex.ContentBlocks.PropertyEditor.ModelValue
{
    public class ContentBlocksModelValueComponent : IComponent
    {
        private readonly ILogger _logger;

        public ContentBlocksModelValueComponent(ILogger logger)
        {
            _logger = logger;
        }

        public void Initialize()
        {
            ContentService.Copying += (_, e) => UpdateContentBlocksKeys(e.Copy);
        }

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
                        string culture = propValue.Culture;
                        string segment = propValue.Segment;

                        string value = propValue.EditedValue as string;
                        if (string.IsNullOrEmpty(value))
                        {
                            continue;
                        }

                        var contentBlocks = JsonConvert.DeserializeObject<JObject>(value);

                        var header = contentBlocks.Value<JObject>("header");
                        if (header != null)
                        {
                            UpdateContentBlockKeys(header);
                        }

                        foreach (JObject block in contentBlocks.Value<JArray>("blocks"))
                        {
                            UpdateContentBlockKeys(block);
                        }

                        prop.SetValue(JsonConvert.SerializeObject(contentBlocks), culture: culture, segment: segment);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(typeof(ContentBlocksModelValueComponent), ex, "Failed to update ContentBlock keys and IDs while copying a node.");
            }
        }

        private void UpdateContentBlockKeys(JObject block)
        {
            block["id"] = Guid.NewGuid();

            var nestedContentItems = block.Value<JArray>("content");

            foreach (var nestedContentItem in nestedContentItems)
            {
                UpdateNestedContentKey(nestedContentItem as JObject);
            }
        }

        private void UpdateNestedContentKey(JObject nestedContent)
        {
            if (nestedContent == null || nestedContent["key"] == null)
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

        public void Terminate()
        {
        }
    }
}
