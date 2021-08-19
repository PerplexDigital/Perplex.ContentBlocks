using Perplex.ContentBlocks.Categories;
using Perplex.ContentBlocks.Definitions;
using Perplex.ContentBlocks.Presets;
using System;
using System.Linq;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using static Umbraco.Cms.Core.Constants;

namespace DemoWebsite.v9
{
    public class ExampleComposer : ComponentComposer<ExampleComponent> { }

    public class ExampleComponent : IComponent
    {
        private readonly IContentBlockDefinitionRepository _definitions;
        private readonly PropertyEditorCollection _propertyEditors;
        private readonly IDataTypeService _dataTypeService;
        private readonly IContentTypeService _contentTypeService;
        private readonly IContentBlockCategoryRepository _categoryRepository;
        private readonly IContentBlocksPresetRepository _presetRepository;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IConfigurationEditorJsonSerializer _configurationEditorJsonSerializer;

        public ExampleComponent(
            IContentBlockDefinitionRepository definitions,
            PropertyEditorCollection propertyEditors,
            IDataTypeService dataTypeService,
            IContentTypeService contentTypeService,
            IContentBlockCategoryRepository categoryRepository,
            IContentBlocksPresetRepository presetRepository,
            IShortStringHelper shortStringHelper,
            IConfigurationEditorJsonSerializer configurationEditorJsonSerializer)
        {
            _definitions = definitions;
            _propertyEditors = propertyEditors;
            _dataTypeService = dataTypeService;
            _contentTypeService = contentTypeService;
            _categoryRepository = categoryRepository;
            _presetRepository = presetRepository;
            _shortStringHelper = shortStringHelper;
            _configurationEditorJsonSerializer = configurationEditorJsonSerializer;
        }

        public void Initialize()
        {
            Guid dataTypeKey = new Guid("ec17fffe-3a33-4a08-a61a-3a6b7008e20f");
            CreateExampleBlock("exampleBlock", dataTypeKey);

            var specialCategoryId = new Guid("AAC6EE6A-EA54-4E90-A33B-049E39786BF5");
            _categoryRepository.Add(new ContentBlockCategory
            {
                Id = specialCategoryId,
                Name = "Specials",
                Icon = "/Media/icons.svg#icon-cat-special"
            });

            // Block
            var block = new ContentBlockDefinition
            {
                Name = "Example Block",
                Id = new Guid("11111111-1111-1111-1111-111111111111"),
                DataTypeKey = dataTypeKey,
                // PreviewImage will usually be a path to some image,
                // for this demo we use a base64-encoded PNG of 3x2 pixels
                PreviewImage = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAMAAAACCAYAAACddGYaAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAATSURBVAiZYzzDwPCfAQqYGJAAACokAc/b6i7NAAAAAElFTkSuQmCC",
                Description = "Example Block",

                Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("22222222-2222-2222-2222-222222222222"),
                            Name = "Red",
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vitae interdum dolor, sit amet luctus odio.",
                            PreviewImage = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAMAAAACCAYAAACddGYaAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAATSURBVAiZYzzDwPCfAQqYGJAAACokAc/b6i7NAAAAAElFTkSuQmCC",
                            ViewPath = "~/Views/Partials/ExampleBlock/Red.cshtml"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("33333333-3333-3333-3333-333333333333"),
                            Name = "Green",
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vitae interdum dolor, sit amet luctus odio. Nam laoreet at odio eu faucibus. Vivamus non rhoncus erat, sit amet efficitur ipsum.",
                            PreviewImage = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAMAAAACCAYAAACddGYaAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAATSURBVAiZYyy+JPafAQqYGJAAADcdAl5UlCmyAAAAAElFTkSuQmCC",
                            ViewPath = "~/Views/Partials/ExampleBlock/Green.cshtml"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("44444444-4444-4444-4444-444444444444"),
                            Name = "Blue",
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vitae interdum dolor, sit amet luctus odio. Nam laoreet at odio eu faucibus. Vivamus non rhoncus erat, sit amet efficitur ipsum. Pellentesque tempus tellus eu posuere varius. Nulla elementum lacus lacus. Curabitur elementum faucibus velit sed mollis.",
                            PreviewImage = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAMAAAACCAYAAACddGYaAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAATSURBVAiZYzRJXfKfAQqYGJAAADOAAkAWXApqAAAAAElFTkSuQmCC",
                            ViewPath = "~/Views/Partials/ExampleBlock/Blue.cshtml",
                        },
                    },

                CategoryIds = new[]
                {
                    Perplex.ContentBlocks.Constants.Categories.Content,
                    specialCategoryId,
                }
            };

            // Header
            var header = new ContentBlockDefinition
            {
                Name = "Example Header",
                Id = new Guid("55555555-5555-5555-5555-555555555555"),
                DataTypeKey = dataTypeKey,
                PreviewImage = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAMAAAACCAYAAACddGYaAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAATSURBVAiZY/zz0v8/AxQwMSABAEvFAzckGfK1AAAAAElFTkSuQmCC",
                Description = "Example Block",

                Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("66666666-6666-6666-6666-666666666666"),
                            Name = "Yellow",
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vitae interdum dolor, sit amet luctus odio.",
                            PreviewImage = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAMAAAACCAYAAACddGYaAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAATSURBVAiZY/zz0v8/AxQwMSABAEvFAzckGfK1AAAAAElFTkSuQmCC",
                            ViewPath = "~/Views/Partials/ExampleHeader/Yellow.cshtml"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("77777777-7777-7777-7777-777777777777"),
                            Name = "Magenta",
                            Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vitae interdum dolor, sit amet luctus odio. Nam laoreet at odio eu faucibus. Vivamus non rhoncus erat, sit amet efficitur ipsum.",
                            PreviewImage = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAMAAAACCAYAAACddGYaAAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAOxAAADsQBlSsOGwAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAATSURBVAiZY/zP8P8/AxQwMSABAEYIAwEl5g6iAAAAAElFTkSuQmCC",
                            ViewPath = "~/Views/Partials/ExampleHeader/Magenta.cshtml"
                        },
                    },

                CategoryIds = new[]
                {
                    Perplex.ContentBlocks.Constants.Categories.Headers,
                }
            };

            _definitions.Add(block);
            _definitions.Add(header);

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

            _presetRepository.Add(new ContentBlocksPreset
            {
                Id = new Guid("72d1b24f-ed6d-4c27-ad21-8fec48b6060c"),
                Name = "Test",
                Blocks = new[]
                {
                    new ContentBlockPreset
                    {
                        Id = new Guid("198bec2a-3404-409a-80e5-cb002aa5858e"),
                        DefinitionId = new Guid("11111111-1111-1111-1111-111111111111"),
                        LayoutId = new Guid("33333333-3333-3333-3333-333333333333"),
                        Values =
                        {
                            ["title"] = "Preset Title value",
                        },
                    },
                }
            });
        }

        private void CreateExampleBlock(string contentTypeAlias, Guid dataTypeKey)
        {
            CreateExampleBlockElementType(contentTypeAlias);
            CreateExampleBlockDataType(contentTypeAlias, dataTypeKey);
        }

        private void CreateExampleBlockElementType(string contentTypeAlias)
        {
            if (_contentTypeService.Get(contentTypeAlias) != null)
            {
                // Already created
                return;
            }

            IContentType contentType = new ContentType(_shortStringHelper, -1)
            {
                Alias = contentTypeAlias,
                IsElement = true,
                Name = "Example Block",
                PropertyGroups = new PropertyGroupCollection(new[]
                {
                    new PropertyGroup(new PropertyTypeCollection(true, new[]
                    {
                        new PropertyType(_shortStringHelper, PropertyEditors.Aliases.TextBox, ValueStorageType.Ntext)
                        {
                            PropertyEditorAlias = PropertyEditors.Aliases.TextBox,
                            Name = "Title",
                            Alias = "title",
                        },
                        new PropertyType(_shortStringHelper, PropertyEditors.Aliases.TextBox, ValueStorageType.Ntext)
                        {
                            PropertyEditorAlias = PropertyEditors.Aliases.TinyMce,
                            Name = "Text",
                            Alias = "text",
                        },
                    }))
                    {
                        Name = "Content",
                    }
                })
            };

            _contentTypeService.Save(contentType);
        }

        private void CreateExampleBlockDataType(string contentTypeAlias, Guid dataTypeKey)
        {
            if (_dataTypeService.GetDataType(dataTypeKey) != null)
            {
                // Already there
                return;
            }

            if (!(_propertyEditors.TryGet("Umbraco.NestedContent", out var editor) && editor is NestedContentPropertyEditor nestedContentEditor))
            {
                throw new InvalidOperationException("Nested Content property editor not found!");
            }

            var dataType = new DataType(nestedContentEditor, _configurationEditorJsonSerializer, -1)
            {
                Name = "Perplex.ContentBlocks - ExampleBlock",
                Key = dataTypeKey,
                Configuration = new NestedContentConfiguration
                {
                    ConfirmDeletes = false,
                    HideLabel = true,
                    MinItems = 1,
                    MaxItems = 1,
                    ShowIcons = false,
                    ContentTypes = new[]
                    {
                        new NestedContentConfiguration.ContentType
                        {
                            Alias = contentTypeAlias,
                            TabAlias = "Content",
                            Template = "{{title}}"
                        }
                    }
                }
            };

            _dataTypeService.Save(dataType);
        }

        public void Terminate()
        {
        }
    }
}
