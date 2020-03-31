using Perplex.ContentBlocks.Definitions;
using System;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace DemoWebsite
{
    public class ExampleComponent : IComponent
    {
        private readonly IContentBlockDefinitionRepository _repo;

        public ExampleComponent(IContentBlockDefinitionRepository repo)
        {
            _repo = repo;
        }

        public void Initialize()
        {
            string previewFolder = "/App_Plugins/Perplex.ContentBlocks/previews";

            _repo.Add(new ContentBlockDefinition
            {
                Name = "Example Header",
                Id = new Guid("7fe32235-3240-466b-92c8-3e6e626692aa"),
                DataTypeId = 1065,
                PreviewImage = $"{previewFolder}/Header-Normal-1/Preview.png",
                Intro = "Example Header Intro",
                Description = "<strong>Content page header.</strong> Use this header to clarify the purpose (main message) of a content page by using clear headers and recognizable content. You can add an image or video within this header. This content block is available in 4 lay-outs, with variations in the image size, position and background colour. <br/><br/> To make your page visual attractive, combine with an intro block in which the image position differs or with intro block #3 (text only).  ",

                Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("9e1246fe-56b8-4133-b384-dacdc2cb9964"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{previewFolder}/Header-Normal-1/Layout-1.png",
                            ViewPath = "~/Views/Partials/ContentBlocks/ExampleHeader/ExampleHeader_Layout-1.cshtml"
                        },
                    },

                CategoryIds = new[]
                {
                    Perplex.ContentBlocks.Constants.Categories.Headers
                }
            });

            _repo.Add(new ContentBlockDefinition
            {
                Name = "Example Block",
                Id = new Guid("12902bee-6c27-4f0f-99de-f7182df7d91f"),
                DataTypeId = 1277,
                PreviewImage = $"{previewFolder}/Normal-1/Preview.png",
                Intro = "Example Block Intro",
                Description = "<strong>Two columns with image and text.</strong> A paragraph of text accompanied by an image in 8 different lay-outs to choose from, with variations in the image sizes, position and background colour. It is also possible to make a reference to a (detail) page with the optional button. <br/><br/>To make your page visual attractive, combine with the other content blocks. ",

                Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("2aaa12d6-6f49-44be-b5ca-21fd7173d273"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{previewFolder}/Normal-1/Layout-1.png",
                            ViewPath = "~/Views/Partials/ContentBlocks/ExampleBlock/ExampleBlock_Layout-1.cshtml"
                        },
                    },

                CategoryIds = new[]
                {
                    Perplex.ContentBlocks.Constants.Categories.Content
                }
            });
        }

        public void Terminate()
        {
        }
    }

    public class ExampleComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Append<ExampleComponent>();
        }
    }
}
