# Perplex.ContentBlocks

Blocked based content editor for Umbraco. Blocks use Umbraco document types and therefore will generate ModelsBuilder models. Rendering a Content Block their properties will therefore be

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

    - See "Content Block Definition" for the documentation of all properties of `IContentBlockDefinition`.

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
        composition.RegisterUnique<IContentBlockDefinitionRepository, MyDefinitionRepository>();
        ```
        - Make sure your composer runs after the `ContentBlockDefinitionComposer`.

### Content Block Definition

The definition of a Content Block consists of the following properties:

-   Id
    -   Unique identifier of this definition. You have to create a Guid yourself and set it.
-   Name
    -   Name of the Content Block.
-   Description
    -   Description of this Content Block.
-   PreviewImage
    -   Image that shows in the UI as a preview of this block. Relative path from the root of your site to an image.
-   DataTypeId / DataTypeKey
    -   The Id (int) or Key (Guid) of the data type that was created for this Content Block.
        -   Either the DataTypeId OR the DataTypeKey has to be set
-   CategoryIds
    -   List of ids of the categories this Content Block should appear in. This references the id of a `IContentBlockCategory`. See "Content Block Categories" for more details on categories.
-   Layouts
    -   List of all layouts of this Content Block. See `Content Block Layout` below.
-   LimitToDocumentTypes
    -   List of document type aliases. When configured, the Content Block will only be available on pages of these document types.
-   LimitToCultures
    -   List of cultures (e.g. "en-US"). When configured, the Content Block will only be available on pages of these cultures.

### Content Block Layout

Each Content Block has at least 1 layout. This refers to the view that will be rendered. It is possible to define multiple layouts. The user will be able to switch layouts from Umbraco.
A layout is described using an implementation of `IContentBlockLayout`, which has the following properties:

-   Id
    -   Unique identifier of this definition. You have to create a Guid yourself and set it.
-   Name
    -   Name of this layout.
-   Description
    -   Description of this layout. This property is _not displayed_ in the UI (yet).
-   PreviewImage
    -   Preview image to use in the layout picker UI. Should a full path from the root of your site to the image, e.g. "/assets/contentblocks/exampleBlock/layout1.png"
-   ViewPath
    -   Path to the View file of this ContentBlockLayout, e.g. "~/Views/Partials/ContentBlocks/ExampleBlock/ExampleBlock_Layout1.cshtml"

### Content Block Categories

Content Blocks are organized in categories. The categories are retrieved from a registered `IContentBlockCategoryRepository`. By default, this package contains two categories: "Headers" and "Content". Their ids are available as constants in `Perplex.ContentBlocks.Constants.Categories`. You can manipulate these categories by either:

-   Inject the `IContentBlockCategoryRepository` and call `Add()` / or `Remove()` to add / remove entries.

OR

-   Register a custom implementation of the `IContentBlockCategoryRepository`:
    ```
    composition.RegisterUnique<IContentBlockCategoryRepository, MyCategoryRepository>();
    ```
    -   Make sure your composer runs after the `ContentBlockCategoriesComposer`.

## Rendering Content Blocks

To render all Content Blocks from the page containing the blocks, you can either use the `IContentBlocksRenderer` directly, or call an extension method with the Content Blocks model value (of type `IContentBlocks).

The examples assume the property alias of the Perplex.ContentBlocks property is `"contentBlocks"` which translates to a ModelsBuilder property of `ContentBlocks`. In both cases we run the example code in the Razor view file of the document type that contains the Content Blocks (e.g. `Homepage.cshtml`):

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

### Rendering a Content Block

The renderer will render each Content Block by using their configured View and pass in a generic `IContentBlockViewModel<TContent>` where `TContent` is the ModelsBuilder type of the content.

Because this `TContent` is a ModelsBuilder model, you can either use strongly typed properties or the `.Value()` method to render its properties.

An example Content Block view file for a Content Block with document type alias `"exampleBlock"` and ModelsBuilder model `ExampleBlock` will look something like this:

```csharp
@using Perplex.ContentBlocks.Rendering;
@model IContentBlockViewModel<ExampleBlock>
<h1>@Model.Content.Title</h1>
<img src="@Model.Content.Image.Url" />
```

The `Model.Content` property is the `IPublishedElement` of the Content Block content and rendering them is the same as rendering any Umbraco content in the front-end.

## Advanced

### Custom View Model

Sometimes you need a more complex view model than just the `IContentBlockViewModel<TContent>`. In this case, you can register a custom view model factory that will generate your custom view model for you.

For example, if you have the ContentBlock `ExampleBlock` and instead of the default `IContentBlockViewModel<TContent>` you want some custom view model `ExampleBlockViewModel`, this is what you do:

1.  Create your View Model with some additional properties.
    -   This custom view model should implement `IContentBlockViewModel`
    -   The example below inherits from the built-in class `ContentBlockViewModel<TContent>`, this is the easiest way

```csharp
public class ExampleBlockViewModel : ContentBlockViewModel<ExampleBlock>
{
    // We want to add the current Environment (Development / Production)
    // to our view model.
    public IEnvironment Environment { get; }

    // We inject our IEnvironmentProvider to obtain the environment
    public ExampleBlockViewModel(ExampleBlock content, Guid id, Guid definitionId, Guid layoutId,
        IEnvironmentProvider environmentProvider)
        : base(content, id, definitionId, layoutId)
    {
        Environment = environmentProvider.GetEnvironment();
    }
}
```

2. Create a View Model factory that is used to create this view model:

```csharp
public class ExampleBlockViewModelFactory : ContentBlockViewModelFactory<ExampleBlock>
{
    private readonly IEnvironmentProvider _environmentProvider;

    // Inject the required dependencies into the factory
    public ExampleBlockViewModelFactory(IEnvironmentProvider environmentProvider)
    {
        _environmentProvider = environmentProvider;
    }

    public override IContentBlockViewModel<ExampleBlock> Create(
        ExampleBlock content, Guid id, Guid definitionId, Guid layoutId)
    {
        // Pass dependencies to ExampleBlockViewModel
        return new ExampleBlockViewModel(content, id, definitionId, layoutId, _environmentProvider);
    }
}
```

3. Register your view model factory:

```csharp
composition.Register(
    typeof(IContentBlockViewModelFactory<ExampleBlock>),
    typeof(ExampleBlockViewModelFactory),
    Lifetime.Scope);
```

4. Use the view model in your view

```
-- ExampleBlock.cshtml
@model ExampleBlockViewModel

@if(Model.Environment == "Development") {
    @RenderDebugInfo()
}

@* Other properties as usual *@
<h1>@Model.Content.Title</h1>
```

### Content Block Presets

TODO
