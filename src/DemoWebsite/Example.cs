using Perplex.ContentBlocks.Definitions;
using System;
using System.Linq;
using Umbraco.Core.Composing;

namespace DemoWebsite
{
    public class ExampleComposer : ComponentComposer<ExampleComponent>
    {
    }

    public class ExampleComponent : IComponent
    {
        private readonly IContentBlockDefinitionRepository _definitions;

        public ExampleComponent(IContentBlockDefinitionRepository definitions)
        {
            _definitions = definitions;
        }

        public void Initialize()
        {
            string previewDir = "/previews/ExampleBlock/";

            _definitions.Add(new ContentBlockDefinition
            {
                Name = "Example Block",
                Id = new Guid("11111111-1111-1111-1111-111111111111"),
                DataTypeKey = new Guid("6c4999f8-1134-4d38-b8ac-3250f28398e7"),
                PreviewImage = previewDir + "block.png",
                Description = "Example Block",

                Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("22222222-2222-2222-2222-222222222222"),
                            Name = "Red",
                            Description = "",
                            PreviewImage = previewDir + "red.png",
                            ViewPath = "~/Views/Partials/ExampleBlock/Red.cshtml"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("33333333-3333-3333-3333-333333333333"),
                            Name = "Green",
                            Description = "",
                            PreviewImage = previewDir + "green.png",
                            ViewPath = "~/Views/Partials/ExampleBlock/Green.cshtml"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("44444444-4444-4444-4444-444444444444"),
                            Name = "Blue",
                            Description = "",
                            PreviewImage = previewDir + "blue.png",
                            ViewPath = "~/Views/Partials/ExampleBlock/Blue.cshtml",
                        },
                    },

                CategoryIds = new[]
                {
                    Perplex.ContentBlocks.Constants.Categories.Content,
                }
            });

            _definitions.Add(new ContentBlockDefinition
            {
                Name = "Example Header",
                Id = new Guid("55555555-5555-5555-5555-555555555555"),
                DataTypeKey = new Guid("6c4999f8-1134-4d38-b8ac-3250f28398e7"),
                PreviewImage = previewDir + "block.png",
                Description = "Example Block",

                Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("66666666-6666-6666-6666-666666666666"),
                            Name = "Yellow",
                            Description = "",
                            PreviewImage = previewDir + "yellow.png",
                            ViewPath = "~/Views/Partials/ExampleHeader/Yellow.cshtml"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("77777777-7777-7777-7777-777777777777"),
                            Name = "Magenta",
                            Description = "",
                            PreviewImage = previewDir + "magenta.png",
                            ViewPath = "~/Views/Partials/ExampleHeader/Magenta.cshtml"
                        },
                    },

                CategoryIds = new[]
                {
                    Perplex.ContentBlocks.Constants.Categories.Headers,
                }
            });

            var all = _definitions.GetAll().ToList();

            for (int i = 0; i < all.Count; i++)
            {
                var def = all[i];

                for (int j = 0; j < 8; j++)
                {
                    var newDef = new ContentBlockDefinition
                    {
                        CategoryIds = def.CategoryIds.ToList(),
                        DataTypeId = def.DataTypeId,
                        DataTypeKey = def.DataTypeKey,
                        Description = def.Description,
                        Id = new Guid($"{i}{j}{def.Id.ToString().Substring(2)}"),
                        Layouts = def.Layouts.Select(l => new ContentBlockLayout
                        {
                            Description = l.Description,
                            Id = new Guid($"{i}{j}{l.Id.ToString().Substring(2)}"),
                            Name = l.Name,
                            PreviewImage = l.PreviewImage,
                            ViewPath = l.ViewPath,
                        }).ToList(),
                        Name = def.Name + $" - copy #{j + 1}",
                        PreviewImage = def.PreviewImage,
                    };

                    _definitions.Add(newDef);
                }
            }
        }

        public void Terminate()
        {
        }
    }
}
