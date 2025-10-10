using Microsoft.Extensions.DependencyInjection;
using Perplex.ContentBlocks.Utils;
using System.Reflection;
using System.Text.Json.Nodes;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;

namespace Perplex.ContentBlocks.PropertyEditor.Value;

public class ContentBlocksContentNotificationHandler
(
    IJsonSerializer jsonSerializer,
    IServiceProvider serviceProvider
) : ComplexPropertyEditorContentNotificationHandler
{
    private static readonly MethodInfo? _blockListFormatPropertyValue =
        typeof(BlockListPropertyNotificationHandler).GetMethod("FormatPropertyValue", BindingFlags.NonPublic | BindingFlags.Instance);

    protected override string EditorAlias { get; } = Constants.PropertyEditor.Alias;

    protected override string FormatPropertyValue(string rawJson, bool onlyMissingKeys)
    {
        if (onlyMissingKeys || _blockListFormatPropertyValue is null)
        {
            // ContentBlocks never has missing keys
            return rawJson;
        }

        if (!JsonUtils.TryParseJson(rawJson, out var node) ||
            node is not JsonObject jsonObject)
        {
            return rawJson;
        }

        var blockListHandler = ActivatorUtilities.CreateInstance<BlockListPropertyNotificationHandler>(serviceProvider);

        if (node["header"] is JsonObject header)
        {
            UpdateBlock(header, blockListHandler);
        }

        if (node["blocks"] is JsonArray blocks)
        {
            foreach (var block in blocks.OfType<JsonObject>())
            {
                UpdateBlock(block, blockListHandler);
            }
        }

        return jsonSerializer.Serialize(jsonObject);
    }

    private void UpdateBlock(JsonObject block, BlockListPropertyNotificationHandler blockListHandler)
    {
        block["id"] = Guid.NewGuid();

        if (block["content"] is JsonObject blockData &&
            UpdateBlockEditor(blockData, blockListHandler) is JsonObject updatedBlockData)
        {
            block["content"] = updatedBlockData.DeepClone();
        }
    }

    private JsonObject? UpdateBlockEditor(JsonObject obj, BlockListPropertyNotificationHandler blockListHandler)
    {
        if (_blockListFormatPropertyValue is null)
        {
            return null;
        }

        // We will use Umbraco's BlockListPropertyNotificationHandler to handle the updated keys
        // of the block editor data but we use a different data format in ContentBlocks than what they expect
        // hence we will transform our data to the desired Umbraco format (contentData, settingsData) here.
        var blockEditorFormat = new JsonObject
        {
            ["contentData"] = new JsonArray { obj.DeepClone() },
            ["settingsData"] = new JsonArray(),
        };

        var rawJson = jsonSerializer.Serialize(blockEditorFormat);
        var onlyMissingKeys = false;

        var updatedJson = _blockListFormatPropertyValue.Invoke(blockListHandler, [rawJson, onlyMissingKeys]) as string;

        if (!JsonUtils.TryParseJson(updatedJson, out var updatedNode) ||
            updatedNode is not JsonObject updatedObj ||
            updatedObj["contentData"] is not JsonArray contentData ||
            contentData.Count == 0 ||
            contentData[0] is not JsonObject updatedContent)
        {
            return null;
        }

        return updatedContent;
    }
}
