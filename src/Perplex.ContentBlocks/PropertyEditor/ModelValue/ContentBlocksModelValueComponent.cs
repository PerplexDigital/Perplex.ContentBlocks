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
            ContentService.Copying += ContentService_Copying;
        }

        private void ContentService_Copying(Umbraco.Core.Services.IContentService sender, Umbraco.Core.Events.CopyEventArgs<Umbraco.Core.Models.IContent> e)
        {
            UpdateContentBlocksKeys(e.Copy);
        }

        private void UpdateContentBlocksKeys(IContent entity)
        {
            try
            {
                var properties = entity.Properties.Where(p => p.PropertyType.PropertyEditorAlias == "Perplex.ContentBlocks");
                foreach (var prop in properties)
                {
                    var cbJson = (string)prop.GetValue();

                    if (!string.IsNullOrEmpty(cbJson))
                    {
                        var contentBlocks = JsonConvert.DeserializeObject<JObject>(cbJson);

                        var header = contentBlocks.Value<JObject>("header");
                        if (header != null)
                        {
                            UpdateContentBlockKeys(header);
                        }

                        foreach (JObject block in contentBlocks.Value<JArray>("blocks"))
                        {
                            UpdateContentBlockKeys(block);
                        }

                        prop.SetValue(JsonConvert.SerializeObject(contentBlocks));
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
            block.Property("id").Value = Guid.NewGuid().ToString();

            var nestedContentItems = block.Value<JArray>("content");

            foreach (var nestedContentItem in nestedContentItems)
            {
                UpdateNestedContentKey(nestedContentItem as JObject);
            }
        }

        private void UpdateNestedContentKey(JObject nestedContent)
        {
            if (nestedContent == null || string.IsNullOrEmpty(nestedContent.Value<string>("key")))
            {
                return;
            }

            nestedContent.Property("key").Value = Guid.NewGuid().ToString();

            // Also update any nested Nested Content items inside this nestedContent
            foreach (var property in nestedContent.Properties())
            {
                var value = property.Value as JArray;

                if (value == null || !IsNestedContentValue(value))
                {
                    continue;
                }

                foreach (var child in value.Children<JObject>())
                {
                    UpdateNestedContentKey(child);
                }
            }
        }

        private bool IsNestedContentValue(JArray value)
        {
            return value.Count > 0 && value.First().Value<string>("key") != null && value.First().Value<string>("ncContentTypeAlias") != null;
        }

        public void Terminate()
        {
        }
    }
}
