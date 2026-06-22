using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace Perplex.ContentBlocks.Utils;
internal static class JsonUtils
{
    /// <summary>
    /// Attempts to parse a JSON string into a <see cref="JsonNode"/>.
    /// </summary>
    /// <param name="json">The JSON string to parse. Can be null or whitespace.</param>
    /// <param name="node">When this method returns, contains the parsed <see cref="JsonNode"/> if parsing succeeded; otherwise, null.</param>
    /// <returns>True if the JSON string was successfully parsed into a <see cref="JsonNode"/>; otherwise, false.</returns>
    internal static bool TryParseJson(string? json, [NotNullWhen(true)] out JsonNode? node)
    {
        node = null;

        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        try
        {
            node = JsonNode.Parse(json);
            return node is not null;
        }
        catch
        {
            return false;
        }
    }
}
