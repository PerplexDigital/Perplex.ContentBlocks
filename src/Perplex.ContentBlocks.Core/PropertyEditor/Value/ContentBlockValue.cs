﻿using System.Text.Json.Serialization;
using Umbraco.Cms.Core.Models.Blocks;

namespace Perplex.ContentBlocks.PropertyEditor.Value;

public class ContentBlockValue
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("definitionId")]
    public Guid DefinitionId { get; set; }

    [JsonPropertyName("layoutId")]
    public Guid LayoutId { get; set; }

    [JsonPropertyName("presetId")]
    public Guid? PresetId { get; set; }

    [JsonPropertyName("isDisabled")]
    public bool IsDisabled { get; set; }

    [JsonPropertyName("content")]
    public BlockItemData? Content { get; set; }
}
