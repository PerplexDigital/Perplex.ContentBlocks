# Release Notes

Summary of changes in each release. For a full changelog see [the commit history](https://github.com/PerplexDigital/Perplex.ContentBlocks/commits/master).

## v1.6.0 - <sub><sup>2020-09-10</sup></sub>

-   Editor UI can now be localized (#22)
    -   Using Umbraco's built-in `<localize>` directive and `lang/` XML files.
    -   Looks at language of logged in Umbraco user.
    -   Thanks to @AndersBrohus for the PR and Danish translations
    -   Thanks to @stefankip for the Dutch translations
-   Fix crash in back-office when editor contains block definitions that do not exist in the repository.

## v1.5.3 - <sub><sup>2020-08-28</sup></sub>

-   Fix disappearing content in the backoffice (#38)
    -   This issue occurred in multilingual setups and was introduced in 1.5.2.
    -   Content is still present in the database but did not show up in the backoffice editor.

## v1.5.2 - <sub><sup>2020-08-24</sup></sub>

-   Fix an issue where RTEs inside blocks are "killed" when reordering ContentBlocks (#28)
-   Fix macro issues when macro is inserted in an RTE in a ContentBlock (#31)
    -   Macro property value was not transformed correctly and therefore not rendered correctly in the frontend.

## v1.5.1 - <sub><sup>2020-08-05</sup></sub>

-   Block picker: fixes mouse hover causing blocks to jump in some situations due to scrollbar appearing

## v1.5.0 - <sub><sup>2020-07-24</sup></sub>

-   Copy/paste blocks: do not abort entirely when header cannot be pasted (#20)
-   Added option to allow adding blocks even when header is not set yet (#24)
-   Fixed crash in preview mode for HTML with multiple root nodes
-   Some small layout fixes

## v1.4.2 - <sub><sup>2020-06-29</sup></sub>

-   Fixes incorrect scale of preview window contents when loading the iframe the first time on new pages.
    -   Page content inside the iframe would not be scaled as usual and would display much bigger than it should. Refreshing the preview fixed the issue, but now it will be displayed properly immediately.

## v1.4.1 - <sub><sup>2020-06-10</sup></sub>

-   NuGet package is split into two:
    -   `Perplex.ContentBlocks.Core`: Assembly only
    -   `Perplex.ContentBlocks`: Full package, except it only contains `App_Plugins` itself and depends on `Perplex.ContentBlocks.Core` for the assembly.
-   Minor bugfix: handling of dataTypeId in JavaScript aligned with C#.
    -   JavaScript would skip a zero as it's falsy whereas in C# we check for `null`. Logic in JS has been adjusted to behave the same as in C# now.

## v1.4.0 - <sub><sup>2020-05-27</sup></sub>

-   Layout descriptions are now shown in the layout picker
    -   Thanks to @jveer for the issue to get this started
-   Close all other block settings when opening settings on another block
-   Added `<content-blocks-icon>` component to help render icons
-   Fixes preview not working in Edge Legacy

## v1.3.0 - <sub><sup>2020-05-19</sup></sub>

-   Added option "Hide property group container".
    -   This is the default behavior of our editor but could never be turned off. This hides the containing property group (= "tab" in v7) of this editor. However, when adding other property editors to the same property group this no longer looks good. Turning this option off will render our editor inside the group like other editors.
-   Editor initialization is more smooth.
    -   We will not wait with all rendering anymore until data is loaded, some elements can always be rendered.
    -   Turning off "Hide property group container" will contribute a lot too as hiding the group during intialization is very noticeable on page load (it will appear then hide when this editor is loaded).
-   Cleaned up NuGet dependencies. Only `UmbracoCms.Web` remains.
-   Restored transition effect on closing/opening settings button.
-   Removes date from "Last update:", only showing time now.

## v1.2.1 - <sub><sup>2020-05-15</sup></sub>

-   Show server validation messages properly if property is culture variant.
-   Fixes "jumpy" layout sliders by not showing them until fully initialized.

## v1.2.0 - <sub><sup>2020-05-08</sup></sub>

-   Initial public release
