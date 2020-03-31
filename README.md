# Perplex.ContentBlocks

## Installation

The package can be installed using NuGet:

`Install-Package Perplex.ContentBlocks`

## Usage

1. Create a collection of content blocks
2. Add the blocks on a page and edit their content
3. Render the content blocks on the front-end.

### Creating a Content Block

The steps to create a new content block are as follows:

1. Create a Document Type

2. Create a Nested Content data type

3. Register the Content Block

You can either register your own implementation of an `IContentBlockDefinitionService`,
or simply add your block to the default repository, which is empty.
For the latter case, simply inject the existing `IContentBlockDefinitionService` sometime on startup and add a content block definition:

```csharp
// Inject this
IContentBlockDefinitionRepository definitionRepo;

// Create your definition
var definition = new ContentBlockDefinition
{
    // Content Block definition here
});

// Register your content block
definitionRepo.Add(definition);
```

### Create Nested Content for Element Type

-   Create Nested Content
-   Min Items: 1
-   Max Items: 1
-   Hide Label: ✔

### Render Content Blocks

To render all content blocks, you can either use the `IContentBlocksRenderer` directly,
or call an extension method with the Content Blocks model value (of type `IContentBlocks). In both cases we run the example code in the Razor view file.

The examples assume the property alias of the Perplex.ContentBlocks property is `"contentBlocks"` which translates to a ModelsBuilder property of `ContentBlocks`.

1. Using the extension method:

`@Html.RenderContentBlocks(Model.ContentBlocks)`

2. Using the renderer:

```csharp
@{
    // Inject / put on your View Model
    IContentBlocksRenderer renderer;
}
@renderer.Render(Model.ContentBlocks)
```

## Advanced

### Content Block Presets

### Custom View Model

-   Register Specialized View Model Factory
-   Define in your view `@model IContentBlockViewModel<MyCustomViewModel>`
