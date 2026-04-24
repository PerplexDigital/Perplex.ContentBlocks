using Microsoft.Extensions.Options;
using Perplex.ContentBlocks.PropertyEditor.Value;
using System.Text;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Extensions;

namespace Perplex.ContentBlocks.PropertyEditor;

/// <summary>
/// Custom <see cref="IPropertyIndexValueFactory"/> for ContentBlocks that extracts
/// and indexes individual nested property values instead of the raw JSON blob.
/// </summary>
public sealed class ContentBlocksPropertyIndexValueFactory
(
    PropertyEditorCollection propertyEditorCollection,
    IJsonSerializer jsonSerializer,
    IOptionsMonitor<IndexingSettings> indexingSettings
) : JsonPropertyIndexValueFactoryBase<ContentBlocksValue>(jsonSerializer, indexingSettings)
{
    protected override IEnumerable<IndexValue> Handle
    (
        ContentBlocksValue deserializedPropertyValue,
        IProperty property,
        string? culture,
        string? segment,
        bool published,
        IEnumerable<string> availableCultures,
        IDictionary<Guid, IContentType> contentTypeDictionary
    )
    {
        var result = new List<IndexValue>();
        var index = 0;

        foreach (var block in GetAllBlocks(deserializedPropertyValue))
        {
            var content = block.Content;
            if (content is null)
            {
                continue;
            }

            if (!contentTypeDictionary.TryGetValue(content.ContentTypeKey, out IContentType? contentType))
            {
                continue;
            }

            var propertyTypeDictionary = contentType
                .CompositionPropertyTypes
                .Select(propertyType =>
                {
                    if (culture is not null)
                    {
                        propertyType.Variations |= ContentVariation.Culture;
                    }

                    if (segment is not null)
                    {
                        propertyType.Variations |= ContentVariation.Segment;
                    }

                    return propertyType;
                })
                .ToDictionary(x => x.Alias);

            result.AddRange(GetNestedResults(
                $"{property.Alias}.items[{index}]",
                culture,
                segment,
                published,
                propertyTypeDictionary,
                content,
                availableCultures,
                contentTypeDictionary));

            index++;
        }

        return RenameKeysToEnsureRawSegmentsIsAPrefix(result);
    }

    protected override IEnumerable<IndexValue> HandleResume
    (
        List<IndexValue> indexedContent,
        IProperty property,
        string? culture,
        string? segment,
        bool published
    )
    {
        var indexedCultures = indexedContent
            .DistinctBy(v => v.Culture)
            .Select(v => v.Culture)
            .WhereNotNull()
            .ToArray();

        var cultures = indexedCultures.Length > 0
            ? indexedCultures
            : new string?[] { culture };

        return cultures.Select(c => new IndexValue
        {
            Culture = c,
            FieldName = property.Alias,
            Values = [GetResumeFromAllContent(indexedContent, c)]
        });
    }

    /// <summary>
    /// Collects all non-disabled blocks (header + blocks).
    /// </summary>
    private static IEnumerable<ContentBlockValue> GetAllBlocks(ContentBlocksValue value)
    {
        if (value.Header is { IsDisabled: false })
        {
            yield return value.Header;
        }

        if (value.Blocks is not null)
        {
            foreach (var block in value.Blocks)
            {
                if (!block.IsDisabled)
                {
                    yield return block;
                }
            }
        }
    }

    private IEnumerable<IndexValue> GetNestedResults
    (
        string keyPrefix,
        string? culture,
        string? segment,
        bool published,
        Dictionary<string, IPropertyType> propertyTypeDictionary,
        BlockItemData blockItemData,
        IEnumerable<string> availableCultures,
        IDictionary<Guid, IContentType> contentTypeDictionary
    )
    {
        foreach (var propertyValue in blockItemData.Values)
        {
            if (!propertyTypeDictionary.TryGetValue(propertyValue.Alias, out IPropertyType? propertyType))
            {
                continue;
            }

            IDataEditor? editor = propertyEditorCollection[propertyType.PropertyEditorAlias];
            if (editor is null)
            {
                continue;
            }

            var propertyCulture = propertyValue.Culture ?? culture;

            if (!propertyType.VariesByCulture() && propertyCulture is not null)
            {
                continue;
            }

            var subProperty = new Property(propertyType);
            IEnumerable<IndexValue> indexValues = null!;

            if (propertyType.VariesByCulture() && propertyCulture is null)
            {
                foreach (var availableCulture in availableCultures)
                {
                    subProperty.SetValue(propertyValue.Value, availableCulture, segment);
                    if (published)
                    {
                        subProperty.PublishValues(availableCulture, segment ?? "*");
                    }

                    indexValues = editor.PropertyIndexValueFactory.GetIndexValues(
                        subProperty, availableCulture, segment, published, availableCultures, contentTypeDictionary);
                }
            }
            else
            {
                subProperty.SetValue(propertyValue.Value, propertyCulture, segment);
                if (published)
                {
                    subProperty.PublishValues(propertyCulture ?? "*", segment ?? "*");
                }

                indexValues = editor.PropertyIndexValueFactory.GetIndexValues(
                    subProperty, propertyCulture, segment, published, availableCultures, contentTypeDictionary);
            }

            var rawDataCultures = blockItemData.Values
                .Select(v => v.Culture)
                .Distinct()
                .WhereNotNull()
                .ToArray();

            foreach (IndexValue indexValue in indexValues)
            {
                indexValue.FieldName = $"{keyPrefix}.{indexValue.FieldName}";

                if (indexValue.Culture is null && rawDataCultures.Length > 0)
                {
                    foreach (var rawDataCulture in rawDataCultures)
                    {
                        yield return new IndexValue
                        {
                            Culture = rawDataCulture,
                            FieldName = indexValue.FieldName,
                            Values = indexValue.Values
                        };
                    }
                }
                else
                {
                    indexValue.Culture = rawDataCultures.Length > 0 ? indexValue.Culture : null;
                    yield return indexValue;
                }
            }
        }
    }

    private static List<IndexValue> RenameKeysToEnsureRawSegmentsIsAPrefix(List<IndexValue> indexContent)
    {
        foreach (IndexValue indexValue in indexContent)
        {
            if (indexValue.FieldName.Length > 1
                && indexValue.FieldName[1..].Contains(UmbracoExamineFieldNames.RawFieldPrefix))
            {
                indexValue.FieldName = UmbracoExamineFieldNames.RawFieldPrefix +
                                       indexValue.FieldName.Replace(UmbracoExamineFieldNames.RawFieldPrefix, string.Empty);
            }
        }

        return indexContent;
    }

    private static string GetResumeFromAllContent(List<IndexValue> indexedContent, string? culture)
    {
        var stringBuilder = new StringBuilder();
        foreach (IndexValue indexValue in indexedContent.Where(v => v.Culture == culture || v.Culture is null))
        {
            if (indexValue.FieldName.Contains(UmbracoExamineFieldNames.RawFieldPrefix))
            {
                continue;
            }

            foreach (var value in indexValue.Values)
            {
                if (value is not null)
                {
                    stringBuilder.AppendLine(value.ToString());
                }
            }
        }

        return stringBuilder.ToString();
    }
}
