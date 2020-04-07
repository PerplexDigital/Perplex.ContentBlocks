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
                Description = "Content page header",

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
                DataTypeId = 1065,
                PreviewImage = $"{previewFolder}/Normal-1/Preview.png",
                Description = "Two columns with image and text",

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

                        new ContentBlockLayout
                        {
                            Id = new Guid("3aaa12d6-6f49-44be-b5ca-21fd7173d273"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{previewFolder}/Normal-1/Layout-1.png",
                            ViewPath = "~/Views/Partials/ContentBlocks/ExampleBlock/ExampleBlock_Layout-1.cshtml"
                        },

                        new ContentBlockLayout
                        {
                            Id = new Guid("4aaa12d6-6f49-44be-b5ca-21fd7173d273"),
                            Name = "Layout 3",
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
