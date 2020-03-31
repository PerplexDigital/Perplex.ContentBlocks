# Perplex.ContentBlocks

## Installation

The package can be installed using NuGet:

`Install-Package Perplex.ContentBlocks`

## Configuration

In order to use this package, you will need to configure at least 1 Content Block.

After that is done, you can add the data type `Perplex.ContentBlocks` as a property to any document type where you want to use this content editor.

In short, the steps to configure a Content Block are:

1. Create a document type

    - Add any properties you need for the Content Block
    - Tick "Is an element type" in Permissions

2. Create a data type based on Nested Content

    - Select the document type created in step 1
    - Set min. items and max. items to 1
    - Hide the label

3. Describe the Content Block using an implementation of the `IContentBlockDefinition` interface.

    - Documentation of what every property means can be found [here](#content-block-definition).

4. Add the definition created in step 3 to an `IContentBlockRepository`

    - Either use the built-in repository:

        ```csharp
        // Inject
        IContentBlockDefinitionRepository definitions;

        // Your definition
        var definition = new ContentBlockDefinition { /* ... */ };

        // Add to the repository
        definitions.Add(definition);
        ```

    - Or register your own implementation in a composer and expose it there:
        ```csharp
        composition
            .RegisterUnique<IContentBlockDefinitionRepository, MyDefinitionRepository>();
        ```
        - Make sure your composer runs after the `ContentBlockDefinitionComposer`.

### <a name="content-block-definition"></a>Content Block Definition

### Content Block Categories

Content Blocks are organized in categories. The categories are retrieved from a registered `IContentBlockCategoryRepository`. By default, this package contains two categories: "Headers" and "Content". You can manipulate these categories by either

-   Inject the `IContentBlockCategoryRepository` and call `Add()` / or `Remove()` to add / remove entries.
-   Register a custom implementation of the `IContentBlockCategoryRepository`:
    ```csharp
        composition
            .RegisterUnique<IContentBlockCategoryRepository, MyCategoryRepository>();
    ```
    -   Make sure your composer runs after the `ContentBlockCategoriesComposer`.

## Rendering Content Blocks

To render all Content Blocks from the page containing the blocks, you can either use the `IContentBlocksRenderer` directly, or call an extension method with the Content Blocks model value (of type `IContentBlocks). In both cases we run the example code in the Razor view file.

The examples assume the property alias of the Perplex.ContentBlocks property is `"contentBlocks"` which translates to a ModelsBuilder property of `ContentBlocks`.

1. Using the extension method:

```csharp
@using Perplex.ContentBlocks.Rendering;
@Html.RenderContentBlocks(Model.ContentBlocks)
```

2. Using the renderer:

```csharp
@{
    // Inject / put on your View Model
    IContentBlocksRenderer renderer;
}
@renderer.Render(Model.ContentBlocks)
```

The renderer will then render every Content Block by using their configured View and pass in a generic `IContentBlockViewModel<TContent>` where `TContent` is the ModelsBuilder type of the content.

### Rendering a Content Block

Because a Content Block is simply an `IPublishedElement` and generates ModelsBuilder models, you can either use `.Value()` or strongly typed ModelsBuilder properties to render its properties.

A view file of a Content Block looks like this:

```csharp
@using Perplex.ContentBlocks.Rendering;
@model IContentBlockViewModel<ContentBlockHeader>
<h1>@Model.Content.Title</h1>
<img src="@Model.Content.Image.Url" />
```

The `Model.Content` property is be the `IPublishedElement` of the Content Block content.

## Advanced

### Content Block Presets

TODO: Explanation

### Custom View Model

-   Register a custom view model factory that produces your custom model
    -   This custom model should implement `IContentBlockViewModel`.
-   This custom model will now be passed to your view:

```
-- MyCustomContentBlock.cshtml
@model MyCustomContentBlockViewModel
<h1>@Model.MyCustomProperty</h1>
```
