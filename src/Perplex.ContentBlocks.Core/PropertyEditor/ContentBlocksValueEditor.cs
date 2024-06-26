using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Editors;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Strings;

namespace Perplex.ContentBlocks.PropertyEditor;

public class ContentBlocksValueEditor : DataValueEditor, IDataValueReference
{
    public ContentBlocksValueEditor(
        IShortStringHelper shortStringHelper, IJsonSerializer jsonSerializer, IIOHelper ioHelper,
        DataEditorAttribute attribute, ContentBlocksValidator validator)
        : base(shortStringHelper, jsonSerializer, ioHelper, attribute)
    {
        Validators.Add(validator);
    }

    public override object? ToEditor(IProperty property, string? culture = null, string? segment = null)
    {
        // TODO: Implement

        return base.ToEditor(property, culture, segment);
    }

    public override object? FromEditor(ContentPropertyData editorValue, object? currentValue)
    {
        // TODO: Implement

        return base.FromEditor(editorValue, currentValue);
    }

    public IEnumerable<UmbracoEntityReference> GetReferences(object? value)
    {
        // TODO: Implement

        yield break;
    }
}