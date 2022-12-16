# Release Notes

Summary of changes in each release. For a full changelog see [the commit history](https://github.com/PerplexDigital/Perplex.ContentBlocks/commits/master).

## v2.1.7 - <sub><sup>2022-12-16</sup></sub>

- Added workaround for an Umbraco 11 issue detailed [on GitHub](https://github.com/umbraco/Umbraco-CMS/issues/13565).
  - In some cases the package.manifest files of Perplex.ContentBlocks are not loaded in an Umbraco 11 installation.
  - This workaround should ensure they are always loaded.
  - This issue only occurs in Umbraco v11 but the workaround has been applied to v9 and v10 as well in case Umbraco decides to backport this bug to those versions.

## v2.1.6 - <sub><sup>2022-11-28</sup></sub>

- Fixed AngularJS console error in Umbraco 11.
  - Umbraco changed the load order of package.manifest files which caused this `[$injector:nomod]` error for the `perplexContentBlocks` AngularJS module.
  - This error only occurs in Umbraco v11 but the fix has been applied to v9 and v10 as well in case Umbraco decides to backport [their change](https://github.com/umbraco/Umbraco-CMS/commit/897cf4ca195927e7bf9932f740fd25461b68a226) to those versions.

## v2.1.5 - <sub><sup>2022-09-23</sup></sub>

- Fixed visual bug in the Block and Layout picker overlays ([#71](https://github.com/PerplexDigital/Perplex.ContentBlocks/issues/71))
  - This issue occured only in Umbraco 10.2.0 and later.
  - Thanks to [@royberris](https://github.com/royberris) for raising the issue.

## v2.1.4 - <sub><sup>2022-05-18</sup></sub>

- Fixed an issue that required editors to have access to the Settings section to be able to use ContentBlocks ([#59](https://github.com/PerplexDigital/Perplex.ContentBlocks/issues/59))
  - This issue occured only in 2.1.2 and 2.1.3
  - Thanks to [@torerikk](https://github.com/torerikk) for raising the issue.

## v2.1.3 - <sub><sup>2022-05-17</sup></sub>

- Fixed layout issue with [Umbraco PR 11901](https://github.com/umbraco/Umbraco-CMS/pull/11901) when using multiple RTEs in a 1 Content Block
  - That PR adds a `top: 50px` which for some reason pushes the RTE toolbar of the 1st RTE 50px down when multiple RTEs are used within a Content Block inside an Umbraco tab. The toolbar then also blocks the user from focusing the RTE body.
  - Our `p-block__main` has `overflow: hidden` which caused this minor UI bug but this does not seem to do anything useful so we removed it to fix this bugged interaction with that PR's change.

## v2.1.2 - <sub><sup>2022-02-18</sup></sub>

- Variants: fixed front-end rendering of macros in an RTE of a block variant.
- Fixed layout slider rendering when placing ContentBlocks directly on a secondary tab and not in a group.
  - Layout sliders were not rendered correctly in that case and the layout name was not visible.
  - This could only occur in Umbraco 8.17+ and 9.
- Property type scaffold cache is now refreshed when a datatype is saved in the backoffice
  - In earlier versions a page refresh was required since we lazily cache property type scaffolds after first retrieval in Angular.

## v2.1.1 - <sub><sup>2021-12-10</sup></sub>

- Fixed error on `dotnet publish` in .NET 6.0
  - Related to duplicate include of App_Plugins\Perplex.ContentBlocks files.
    The files are included from $(ProjectDir) as well as from the NuGet package and .NET 6 does not allow these duplicates.

## v2.1.0 - <sub><sup>2021-11-25</sup></sub>

- Improved complex validation support

  - We now implement `ComplexEditorValidator` directly to achieve this
  - Umbraco dependency was increased to 8.7.0 to make this possible

- Added support to customize some parts of the ContentBlocks editor UI

  - The body of the content block editor can be replaced with a custom AngularJS component
  - Buttons can be added to the top bar of each content block, next to the settings button
  - An example of an addon that replaces the body and adds a button is [available here](src/DemoWebsite.v9/App_Plugins/MyContentBlocksAddon)

- Added support for block variants
  - It is now possible to create block variants
    - The core package does not provide any UI to visualize or manage these variants
    - If you want to use it you will have to write some custom code and provide custom components to render the UI
  - The variant to render in the front-end will be selected in C# using the new service `IContentBlockVariantSelector`
    - By default this package will never render a variant and will simply render the default block
    - To customize this behavior, provide a custom implementation
  - The data model is completely backwards compatible and existing data will not be touched. If you do not use variants you will not notice any difference compared to the previous version.
  - Full disclosure: the trigger to create this functionality is our package [uMarketingSuite.ContentBlocks](https://www.nuget.org/packages/uMarketingSuite.ContentBlocks/) that adds some uMarketingSuite features to ContentBlocks as an addon.
    - Just to make it clear; there is no direct connection/dependency between Perplex.ContentBlocks and uMarketingSuite and this will not happen in the future. This package now provides very generic support for variants and any uMarketingSuite related code is isolated within the separate uMarketingSuite.ContentBlocks package.

## v2.0.0 - <sub><sup>2021-11-05</sup></sub>

- Added .NET 5 + Umbraco 9 support
  - The package can now be installed in both v8 and v9 websites
  - The API and features are the same as in v1.9.0.

## v1.9.0 - <sub><sup>2021-06-30</sup></sub>

- Added support for [Media Tracking](https://umbraco.com/blog/umbraco-86-release/#media) introduced in Umbraco 8.6.0
  - The minimum required Umbraco version is raised to 8.6.0
  - Thanks to [@patrickdemooij9](https://github.com/patrickdemooij9) for providing a PR.

## v1.8.0 - <sub><sup>2021-05-27</sup></sub>

- Presets can now contain initial values for properties.
  - Example code to set initial values:
  ```csharp
  new ContentBlockPreset
  {
      Id = ...,
      DefinitionId = ...,
      LayoutId = ...,
      Values =
      {
          // Set initial values for properties "title" and "text"
          // of the Content Block
          ["title"] = "Default title here",
          ["text"] = "<p>Lorem ipsum ...</p>"
      },
  },
  ```
  - This is a breaking change to `Perplex.ContentBlocks.Presets.IContentBlockPreset` since we add a property to it (`IDictionary<string, object> Values`) but this will only actually break if you use a custom implementation of this interface. If you simply use the built-in `Perplex.ContentBlocks.Presets.ContentBlockPreset` existing code will not be affected.

## v1.7.0 - <sub><sup>2021-02-18</sup></sub>

- Support for complex validation introduced in Umbraco 8.7
- Added a search bar to filter blocks in the picker UI
- Added convenience extensions methods to `HtmlHelper` to render a subset of ContentBlocks rather than everything.
  - `@Html.RenderContentBlock(IContentBlockViewModel)` to render one specific block
  - `@Html.RenderContentBlocks(IEnumerable<IContentBlockViewModel>)` to render some specific blocks
  - One use case is to render the header separately from the blocks in a different part of the page.
- Some styling improvements

## v1.6.3 - <sub><sup>2020-12-30</sup></sub>

- Fixed Content Block `id` + Nested Content `key` properties not getting new values when a content node is copied (#45).

  - These properties are supposed to be unique. NestedContent uses the `key` property as a cache key so having multiple Content Blocks on different pages share the same `key` can lead to issues. These properties will now be recursively updated to new unique keys when a node is copied.
  - Note this was only an issue when copying an entire content node. Copying blocks using the ContentBlocks UI already updated these properties correctly.

  - Thanks to @glombek for reporting this issue and for providing code.

- Fixed preset blocks not initializing any property value until expanded
  - This meant if preset blocks were not opened manually by the user in Umbraco they would not appear in the model value of your content, as if the blocks are not on the page at all. In this case the blocks would be empty anyway since the user has not provided any content but this was not intended so has been fixed.

## v1.6.2 - <sub><sup>2020-10-30</sup></sub>

- Handles a couple of rare exceptions

## v1.6.1 - <sub><sup>2020-10-26</sup></sub>

- Fixed data disappearing in the back office in some cases since 8.7
  - This occurred with some property editors, e.g. when using Nested Content inside a block.
- Removed new border around Nested Content items inside block in 8.7+

## v1.6.0 - <sub><sup>2020-09-10</sup></sub>

- Editor UI can now be localized (#22)
  - Using Umbraco's built-in `<localize>` directive and `lang/` XML files.
  - Looks at language of logged in Umbraco user.
  - Thanks to @AndersBrohus for the PR and Danish translations
  - Thanks to @stefankip for the Dutch translations
- Fix crash in back-office when editor contains block definitions that do not exist in the repository.

## v1.5.3 - <sub><sup>2020-08-28</sup></sub>

- Fix disappearing content in the backoffice (#38)
  - This issue occurred in multilingual setups and was introduced in 1.5.2.
  - Content is still present in the database but did not show up in the backoffice editor.

## v1.5.2 - <sub><sup>2020-08-24</sup></sub>

- Fix an issue where RTEs inside blocks are "killed" when reordering ContentBlocks (#28)
- Fix macro issues when macro is inserted in an RTE in a ContentBlock (#31)
  - Macro property value was not transformed correctly and therefore not rendered correctly in the frontend.

## v1.5.1 - <sub><sup>2020-08-05</sup></sub>

- Block picker: fixes mouse hover causing blocks to jump in some situations due to scrollbar appearing

## v1.5.0 - <sub><sup>2020-07-24</sup></sub>

- Copy/paste blocks: do not abort entirely when header cannot be pasted (#20)
- Added option to allow adding blocks even when header is not set yet (#24)
- Fixed crash in preview mode for HTML with multiple root nodes
- Some small layout fixes

## v1.4.2 - <sub><sup>2020-06-29</sup></sub>

- Fixes incorrect scale of preview window contents when loading the iframe the first time on new pages.
  - Page content inside the iframe would not be scaled as usual and would display much bigger than it should. Refreshing the preview fixed the issue, but now it will be displayed properly immediately.

## v1.4.1 - <sub><sup>2020-06-10</sup></sub>

- NuGet package is split into two:
  - `Perplex.ContentBlocks.Core`: Assembly only
  - `Perplex.ContentBlocks`: Full package, except it only contains `App_Plugins` itself and depends on `Perplex.ContentBlocks.Core` for the assembly.
- Minor bugfix: handling of dataTypeId in JavaScript aligned with C#.
  - JavaScript would skip a zero as it's falsy whereas in C# we check for `null`. Logic in JS has been adjusted to behave the same as in C# now.

## v1.4.0 - <sub><sup>2020-05-27</sup></sub>

- Layout descriptions are now shown in the layout picker
  - Thanks to @jveer for the issue to get this started
- Close all other block settings when opening settings on another block
- Added `<content-blocks-icon>` component to help render icons
- Fixes preview not working in Edge Legacy

## v1.3.0 - <sub><sup>2020-05-19</sup></sub>

- Added option "Hide property group container".
  - This is the default behavior of our editor but could never be turned off. This hides the containing property group (= "tab" in v7) of this editor. However, when adding other property editors to the same property group this no longer looks good. Turning this option off will render our editor inside the group like other editors.
- Editor initialization is more smooth.
  - We will not wait with all rendering anymore until data is loaded, some elements can always be rendered.
  - Turning off "Hide property group container" will contribute a lot too as hiding the group during intialization is very noticeable on page load (it will appear then hide when this editor is loaded).
- Cleaned up NuGet dependencies. Only `UmbracoCms.Web` remains.
- Restored transition effect on closing/opening settings button.
- Removes date from "Last update:", only showing time now.

## v1.2.1 - <sub><sup>2020-05-15</sup></sub>

- Show server validation messages properly if property is culture variant.
- Fixes "jumpy" layout sliders by not showing them until fully initialized.

## v1.2.0 - <sub><sup>2020-05-08</sup></sub>

- Initial public release
