# Implement IPropertyIndexValueFactory for ContentBlocks

## Goal

Prevent the full JSON blob of ContentBlocks property values from being indexed by Examine/Lucene. Instead, extract and index only the relevant text content from the nested block properties (similar to how Umbraco does it for BlockList/BlockGrid).

## Current State

- `PerplexContentBlocksPropertyEditor` extends `DataEditor`
- `DataEditor.PropertyIndexValueFactory` returns `DefaultPropertyIndexValueFactory` (indexes raw JSON string)
- Our value model: `ContentBlocksValue` has a `Header` (`ContentBlockValue?`) and `Blocks` (`List<ContentBlockValue>?`)
- Each `ContentBlockValue` has a `Content` property of type `BlockItemData` (Umbraco's standard block item data with `ContentTypeKey`, `Key`, and `Values` list of `BlockPropertyValue`)

## Umbraco's Approach

Umbraco uses a class hierarchy:
1. `JsonPropertyIndexValueFactoryBase<TSerialized>` (public) — deserializes JSON, calls `Handle()`, then `HandleResume()`
2. `BlockValuePropertyIndexValueFactoryBase<TSerialized>` (internal) — implements `Handle()` to iterate data items, resolve content types, and recursively index nested property values
3. `BlockValuePropertyIndexValueFactory` (internal sealed) — concrete impl for standard block values

We **cannot** inherit from classes 2 or 3 (they're `internal`). We **can** inherit from `JsonPropertyIndexValueFactoryBase<TSerialized>`.

## Key Difference: Our Value vs Umbraco's BlockValue

Umbraco's `BlockValuePropertyIndexValueFactory` deserializes to a simple model with `ContentData` (list of `BlockItemData`) and `Expose` (list of `BlockItemVariation`).

Our value has:
- `header` — a single `ContentBlockValue` with a `Content` (`BlockItemData`) property
- `blocks` — a list of `ContentBlockValue`, each with a `Content` (`BlockItemData`) property
- Disabled blocks (`isDisabled: true`) should probably be skipped
- No `expose` concept — all content is considered exposed/published

So essentially we need to collect all `BlockItemData` from header + blocks (excluding disabled), and process them the same way Umbraco processes its `contentData`.

## Plan

### 1. Create `ContentBlocksPropertyIndexValueFactory`

**File:** `PropertyEditor/ContentBlocksPropertyIndexValueFactory.cs`

- Inherit from `JsonPropertyIndexValueFactoryBase<ContentBlocksValue>`
- Inject `PropertyEditorCollection`, `IJsonSerializer`, `IOptionsMonitor<IndexingSettings>`
- Implement `Handle()`:
  - Collect all `BlockItemData` from non-disabled header + blocks
  - For each `BlockItemData`, resolve the `IContentType` from `contentTypeDictionary` using `ContentTypeKey`
  - For each property value in the block, look up the property editor and recursively call its `PropertyIndexValueFactory.GetIndexValues()` to get the index values for nested properties
  - Prefix field names with the parent property alias + item index (e.g., `myProp.items[0].title`)
- Implement `HandleResume()`:
  - Aggregate all indexed text content into a single summary value under the property's own alias (for standard full-text search)

This is essentially a re-implementation of `BlockValuePropertyIndexValueFactoryBase.Handle()` and `HandleResume()` since those are `internal`. The logic is straightforward: ~80 lines of code, adapting the patterns from the Umbraco source.

### 2. Update `PerplexContentBlocksPropertyEditor`

**File:** `PropertyEditor/ContentBlocksPropertyEditor.cs`

- Inject `ContentBlocksPropertyIndexValueFactory` via constructor
- Override `PropertyIndexValueFactory` property to return it

### 3. Register in DI

Check if we need to register the factory. Since it's injected into the editor and Umbraco resolves editors from DI, we likely need to register `ContentBlocksPropertyIndexValueFactory` as a singleton/transient in a composer.

**File:** Likely an existing composer or a new registration in the existing DI setup.

## Considerations

- **Culture handling:** Replicate Umbraco's logic where nested properties inherit culture variance from parent when the parent varies by culture.
- **Segment handling:** Same as culture.
- **RawFieldPrefix handling:** Replicate the rename logic to ensure `__Raw_` prefix is always at the start of field names.
- **`UmbracoExamineFieldNames.RawFieldPrefix`** is in `Umbraco.Cms.Infrastructure.Examine` — need to check if this is accessible from our project (we reference `Umbraco.Cms.Api.Management` which should transitively include Infrastructure).
- **Disabled blocks:** Skip blocks where `IsDisabled == true`.
