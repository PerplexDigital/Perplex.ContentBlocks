using Athlon.Infrastructure.Configuration;
using Athlon.Infrastructure.ModelsBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Web;
using static Athlon.Constants.ContentBlocks.Umbraco;
using categories = Athlon.Constants.ContentBlocks.Categories;

namespace Perplex.ContentBlocks.Definitions
{
    public class HardCodedContentBlockDefinitionService : IContentBlockDefinitionService
    {
        private readonly bool _isProduction;

        public HardCodedContentBlockDefinitionService(IContentBlockDefinitionFilterer definitionFilterer, IConfigurationManager configurationManager)
        {
            _definitionFilterer = definitionFilterer;
            _isProduction = configurationManager.GetAppSettingByKey("Athlon.Environment").Equals("Production", StringComparison.OrdinalIgnoreCase);
            _definitions = GetDefinitions();
        }

        private IDictionary<Guid, IContentBlockDefinition> GetDefinitions()
        {
            return new IContentBlockDefinition[]
            {
                new ContentBlockDefinition
                {
                    Name = "Image Icon Grid",

                    Id = new Guid("E954BC93-D1C3-41BC-95BB-A7847041BDD3"),
                    DataTypeId = 1837,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Image-Icon-Grid-1/Preview.png",
                    Intro = "Use this block to summarize short text paragraphs with a small image or icon.",
                    Description = "<strong>Text with image or icon.</strong> Use this block to summarize short text paragraphs with a small image or icon in a well arranged view, for example for benefits or USP’s. You can show the items in 3 or 4 columns in two different lay-outs.",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("50EFEA7B-3981-4417-9D71-D294CFFB0814"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Image-Icon-Grid-1/Layout-1.png",
                            ViewName = "Image-Icon-Grid-1/Layout-1"
                        },

                        new ContentBlockLayout
                        {
                            Id = new Guid("3A6073BF-0FCE-403B-A095-1AE5458A8AC7"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Image-Icon-Grid-1/Layout-2.png",
                            ViewName = "Image-Icon-Grid-1/Layout-2"
                        },

                        new ContentBlockLayout
                        {
                            Id = new Guid("6723AD26-3583-4D0A-81CF-A116DA0E5D2C"),
                            Name = "Layout 3",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Image-Icon-Grid-1/Layout-3.png",
                            ViewName = "Image-Icon-Grid-1/Layout-3"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("7407ACF3-9D03-4BCB-B410-BEA554904D95"),
                            Name = "Layout 4",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Image-Icon-Grid-1/Layout-4.png",
                            ViewName = "Image-Icon-Grid-1/Layout-4"
                        }
                    },

                    CategoryIds = new[]
                    {
                        categories.Content
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Intro with image",
                    Id = new Guid("410156e4-5441-4b81-9161-52092b30d601"),
                    DataTypeId = 1179,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Intro-1/Preview.png",
                    Intro = "This content block is most suitable to start a page with. The titles are slightly larger than average which gives...",
                    Description = "This content block is most suitable to start a page with. The titles are slightly larger than average which gives the content in this block more attention, perfect to use directly after the header. It is also possible to make a reference to a (detail) page with the optional button. <br/><br/>There are 6 lay-outs to choose from, with variations in the image sizes and position. ",
                    Mirrored = true,

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("289c2e5e-7fe8-492f-86a8-2fa3edec3af3"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Intro-1/Layout-1.png",
                            ViewName = "Intro-1/Layout-1"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("ced48dc8-d178-48b9-b603-23b54b467eb1"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Intro-1/Layout-2.png",
                            ViewName = "Intro-1/Layout-2"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("a81e2d08-6b77-4b84-aae1-7c697d14dc18"),
                            Name = "Layout 3",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Intro-1/Layout-3.png",
                            ViewName = "Intro-1/Layout-3"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("06f899ff-988b-4265-b7ff-13f3984e0c78"),
                            Name = "Layout 4",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Intro-1/Layout-4.png",
                            ViewName = "Intro-1/Layout-4"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("85c411c6-ce2a-4e49-802e-be31b7c9f0ad"),
                            Name = "Layout 5",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Intro-1/Layout-5.png",
                            ViewName = "Intro-1/Layout-5"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("e88e1f40-6990-4eef-9d18-d61d05a7a4ca"),
                            Name = "Layout 6",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Intro-1/Layout-6.png",
                            ViewName = "Intro-1/Layout-6"
                        }
                    },

                    CategoryIds = new[]
                    {
                        categories.Content
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Intro with headliner",
                    Id = new Guid("9d698b77-26c4-4021-a81e-04b549bdc4a2"),
                    DataTypeId = 1275,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Intro-3/Preview.png",
                    Intro = "This content block is most suitable to start a page with. The titles are slightly larger than average which gives...",
                    Description = "This content block is most suitable to start a page with. The titles are slightly larger than average which gives the content in this block more attention, perfect to use directly after the header. It is also possible to make a reference to a (detail) page with the optional button. <br/><br/> This intro consists of text and a headliner in 2 different lay-outs to choose from.",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("72a01e18-b832-4a03-a9bb-bd71a7bba9d5"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Intro-3/Layout-1.png",
                            ViewName = "Intro-3/Layout-1"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("549116a7-2a60-42b9-8fd0-556519cd6417"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Intro-3/Layout-2.png",
                            ViewName = "Intro-3/Layout-2"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Content
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Keypoints",
                    Id = new Guid("33047a06-7ff6-4a40-9c82-3767bcb9694c"),
                    DataTypeId = 1276,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Keypoints-1/Preview.png",
                    Intro = "Text with enumeration. Use this block to summarize text in a well arranged view, for example for benefits or USP’s. You can add...",
                    Description = "<strong>Text with enumeration.</strong> Use this block to summarize text in a well arranged view, for example for benefits or USP’s. You can add a maximum of 12 and a minimum of 3 key points here, accompanied by a short introduction text in 3 different lay-outs.",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("03e993af-4cc8-412a-b150-0b56a730480b"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Keypoints-1/Layout-1.png",
                            ViewName = "Keypoints-1/Layout-1"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("85e60067-7f11-49b5-ad6f-6582151c5083"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Keypoints-1/Layout-2.png",
                            ViewName = "Keypoints-1/Layout-2"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("9ecebeea-0531-4a4d-8379-db46d1bc3c9f"),
                            Name = "Layout 3",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Keypoints-1/Layout-3.png",
                            ViewName = "Keypoints-1/Layout-3"
                        }
                    },

                    CategoryIds = new[]
                    {
                        categories.Content
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Normal",
                    Id = new Guid("12902bee-6c27-4f0f-99de-f7182df7d91f"),
                    DataTypeId = 1277,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Normal-1/Preview.png",
                    Intro = "Two columns with image and text. A paragraph of text accompanied by an image in 8 different lay-outs to choose from, with variations in the...",
                    Description = "<strong>Two columns with image and text.</strong> A paragraph of text accompanied by an image in 8 different lay-outs to choose from, with variations in the image sizes, position and background colour. It is also possible to make a reference to a (detail) page with the optional button. <br/><br/>To make your page visual attractive, combine with the other content blocks. ",
                    Mirrored = true,

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("2aaa12d6-6f49-44be-b5ca-21fd7173d273"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Normal-1/Layout-1.png",
                            ViewName = "Normal-1/Layout-1"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("cfb4fc04-3444-402c-9ac3-f9a1bf3e4253"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Normal-1/Layout-2.png",
                            ViewName = "Normal-1/Layout-2"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("0e61890c-3898-4b6d-b8cf-e5021592f84b"),
                            Name = "Layout 3",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Normal-1/Layout-3.png",
                            ViewName = "Normal-1/Layout-3"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("3a8b5a44-1564-4120-a391-b6fe889075f7"),
                            Name = "Layout 4",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Normal-1/Layout-4.png",
                            ViewName = "Normal-1/Layout-4"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("d71acc05-108d-41e2-b834-9b9f57f81784"),
                            Name = "Layout 5",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Normal-1/Layout-5.png",
                            ViewName = "Normal-1/Layout-5"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("60b604cb-ce8b-4498-bbcd-c5ff3029b4b5"),
                            Name = "Layout 6",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Normal-1/Layout-6.png",
                            ViewName = "Normal-1/Layout-6"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("7949658b-a39b-4eb1-98f1-44f727a29fac"),
                            Name = "Layout 7",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Normal-1/Layout-7.png",
                            ViewName = "Normal-1/Layout-7"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("1cf4d3c5-fe00-4873-9955-95ace9d20f80"),
                            Name = "Layout 8",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Normal-1/Layout-8.png",
                            ViewName = "Normal-1/Layout-8"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("2B2AAE5D-6A70-47A6-863F-AAE848EF2113"),
                            Name = "Layout 9",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Normal-1/Layout-9.png",
                            ViewName = "Normal-1/Layout-9"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("93E98D7D-0C3F-4D2C-8CD8-C210537A954E"),
                            Name = "Layout 10",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Normal-1/Layout-10.png",
                            ViewName = "Normal-1/Layout-10"
                        }
                    },

                    CategoryIds = new[]
                    {
                        categories.Content
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Parallax",
                    Id = new Guid("9aaae15a-7c05-482d-a251-7a0b8e221157"),
                    DataTypeId = 1281,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Parallax-1/Preview.png",
                    Intro = "Text with 3 images and a headliner. This content block can be used to decorate / enrich your page with some imagery, accompanied...",
                    Description = "<strong>Text with 3 images and a headliner.</strong> This content block can be used to decorate / enrich your page with some imagery, accompanied by a short text. The images move through a subtle animation automatically. You can also add a headliner below the images. <br/><br/>Use this block to give more attention to a subject.",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("b77a4c19-6656-4e7c-b2de-9fb9e027f1ab"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Parallax-1/Layout-1.png",
                            ViewName = "Parallax-1/Layout-1"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("039925c0-b250-4e45-8fbc-c78a319baed0"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Parallax-1/Layout-2.png",
                            ViewName = "Parallax-1/Layout-2"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Content
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Pagelinks & Text",
                    Id = new Guid("F311CFA3-25D2-4A75-A20B-885AD0EFE334"),
                    DataTypeKey = new Guid("ad875a8d-e461-4739-be77-794f19f305d7"),
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Pagelinks-Text-1/Preview.png",
                    Intro = "Crosslinks and text. This content block can be used for crosslinks to other pages within the website. Use this block for...",
                    Description = "<strong>Crosslinks and text.</strong> This content block can be used for crosslinks to other pages within the website. Use this block for example to showcase reviews and cases related to the content on this page accompanied by a text block with a header. A crosslink always has an image. This content block is available in 2 background colours, white and blue. ",
                    Mirrored = true,

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("A6D9F9BB-016F-4E2F-B4A4-8BCE8912B243"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Pagelinks-Text-1/Layout-1.png",
                            ViewName = "Pagelinks-Text-1/Layout-1"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("C8ED8721-9679-4A07-8D98-8600D56F04DD"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Pagelinks-Text-1/Layout-2.png",
                            ViewName = "Pagelinks-Text-1/Layout-1"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("8880F5DF-9CBF-40A3-80ED-09DB959C22BD"),
                            Name = "Layout 3",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Pagelinks-Text-1/Layout-3.png",
                            ViewName = "Pagelinks-Text-1/Layout-3"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("29BA2D1F-AD8B-4E2E-9A52-F9CB66D6B629"),
                            Name = "Layout 4",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Pagelinks-Text-1/Layout-4.png",
                            ViewName = "Pagelinks-Text-1/Layout-4"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Content
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Text",
                    Id = new Guid("703F7417-0DEB-432F-BE90-5D7AF8AB27F6"),
                    DataTypeId = 1802,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Text-1/Preview.png",
                    Intro = "Use this content block to show text only in 1 column.",
                    Description = "<strong>Text in 1 column.</strong> Use this content block to show text only in 1 column. This block works best in between content blocks with images for a nice and varied overview. Separate your text in an introduction and body text for the most optimal result. ",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("2FA1CCC1-09A1-4946-80F4-2CF8A008A6D2"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Text-1/Layout-1.png",
                            ViewName = "Text-1/Layout-1"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("4B280B4F-A539-4C7C-A9CD-6970FAF3DA6D"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Text-1/Layout-2.png",
                            ViewName = "Text-1/Layout-2"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Content
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Text Two Columns",
                    Id = new Guid("87E87693-DE62-4EA6-8D2F-0EE698AE7077"),
                    DataTypeId = 1804,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Text-Two-Columns-1/Preview.png",
                    Intro = "Use this content block to show text only in 2 columns.",
                    Description = "<strong>Text in two columns.</strong> Use this content block to show text only in 2 columns. This block works best in between content blocks with images for a nice and varied overview. ",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("CD032410-855C-4CAC-8A43-D2209648B0D3"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Text-Two-Columns-1/Layout-1.png",
                            ViewName = "Text-Two-Columns-1/Layout-1"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("1E759C1D-01EF-473A-86F5-DEBFA8955D4B"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Text-Two-Columns-1/Layout-2.png",
                            ViewName = "Text-Two-Columns-1/Layout-2"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Content
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Three Columns",
                    Id = new Guid("a6fad8ad-9c1a-4887-8840-316716021798"),
                    DataTypeId = 1279,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Three-Columns-1/Preview.png",
                    Intro = "Three columns with image and text. Two paragraphs of text accompanied by an image in 4 different lay-outs to choose from. It is also...",
                    Description = "<strong>Three columns with image and text.</strong> Two paragraphs of text accompanied by an image in 4 different lay-outs to choose from. It is also possible to make a reference to (detail) pages with the optional buttons.<br/><br/>To make your page visual attractive, combine with the other content blocks. ",
                    Mirrored = true,

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("575cb673-bc6a-491d-ae8a-5cb6af4bd24b"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Three-Columns-1/Layout-1.png",
                            ViewName = "Three-Columns-1/Layout-1"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("73115a7d-2264-4669-82d5-586c455db3b1"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Three-Columns-1/Layout-2.png",
                            ViewName = "Three-Columns-1/Layout-2"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("dd3bbd34-1fd7-42a8-a64e-2325e212db09"),
                            Name = "Layout 3",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Three-Columns-1/Layout-3.png",
                            ViewName = "Three-Columns-1/Layout-3"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("1b7ec10b-e2bb-4dac-976f-5e0feb4de043"),
                            Name = "Layout 4",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Three-Columns-1/Layout-4.png",
                            ViewName = "Three-Columns-1/Layout-4"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Content
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Video",
                    Id = new Guid("0416325D-89C1-4068-AC4E-04170DB14677"),
                    DataTypeId = 2145,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Video-1/Preview.png",
                    Intro = "Use this content block to present YouTube or Vimeo video’s within your page, combined with text.",
                    Description = "<strong>Video with text.</strong> Use this content block to present YouTube or Vimeo video’s within your page, combined with text. Choose one of the three available lay-outs. Layout #3 is also suitable for video without text (when the text fields are left blank). <br/><br/>Please note that users have to accept their cookies in order to watch a video. Therefore, uploading an image (used as a placeholder) is mandatory in these blocks.",
                    Mirrored = true,

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("7CAA179B-AEB8-42D8-A91F-C6501097E3A1"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Video-1/Layout-1.png",
                            ViewName = "Video-1/Layout-1"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("90234CFB-978E-4781-AEB4-0F3A54B5F7C9"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Video-1/Layout-2.png",
                            ViewName = "Video-1/Layout-2"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("35F29D1F-0B64-480C-91CE-45984E428CDB"),
                            Name = "Layout 3",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Video-1/Layout-3.png",
                            ViewName = "Video-1/Layout-3"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Content
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Contact",
                    Id = new Guid("5b4b043e-070e-46e2-a0d3-284a99f299f4"),
                    DataTypeId = 1283,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Contact-1/Preview.png",
                    Intro = "Contact banner. Use this banner to create a visual reference to a contact form.",
                    Description = "<strong>Contact banner.</strong> Use this banner to create a visual reference to a contact form.",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("afce9513-9999-4914-a8fd-c61672357aca"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Contact-1/Layout-1.png",
                            ViewName = "Contact-1/Layout-1"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("62f6a4ad-b623-4bb3-ab25-8103ab9ebecb"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Contact-1/Layout-2.png",
                            ViewName = "Contact-1/Layout-2"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Conversion
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Form with image",
                    Id = new Guid("117cac2a-78f6-423c-a51a-4f6ee85a8d2b"),
                    DataTypeId = 1285,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Forms-1/Preview.png",
                    Intro = "Within this content block you can select one of the available (contact) forms accompanied by an image in 6 different lay-outs, varying in...",
                    Description = "Within this content block you can select one of the available (contact) forms accompanied by an image in 6 different lay-outs, varying in image position and background colour. Adjustments on the form itself can be made within the ‘Forms section’. ",
                    Mirrored = true,

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("d5113469-061c-4bba-8510-9fe731c2f75a"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Forms-1/Layout-1.png",
                            ViewName = "Forms-1/Layout-1"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("46393a8d-7b04-4fa8-8142-85a229ae2e08"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Forms-1/Layout-2.png",
                            ViewName = "Forms-1/Layout-2"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("c5e131ca-a3b2-4a16-bbdb-0a7733ebc70e"),
                            Name = "Layout 3",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Forms-1/Layout-3.png",
                            ViewName = "Forms-1/Layout-3"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("7d6b0d27-5eee-46a6-9fb3-7d55a571e062"),
                            Name = "Layout 4",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Forms-1/Layout-4.png",
                            ViewName = "Forms-1/Layout-4"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("ef21534a-7766-4188-8814-7c50096b094c"),
                            Name = "Layout 5",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Forms-1/Layout-5.png",
                            ViewName = "Forms-1/Layout-5"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("64ae47ee-fd3f-4cc1-99ef-6491f878243b"),
                            Name = "Layout 6",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Forms-1/Layout-6.png",
                            ViewName = "Forms-1/Layout-6"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Forms
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Form with text",
                    Id = new Guid("44e8a78a-df5a-4c90-824a-6b4a901c27d2"),
                    DataTypeId = 1288,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Forms-3/Preview.png",
                    Intro = "Within this content block you can select one of the available (contact) forms accompanied by text (instruction or clarification) in... ",
                    Description = "Within this content block you can select one of the available (contact) forms accompanied by text (instruction or clarification) in 3 different background colours. Adjustments on the form itself can be made within the ‘Forms section’. ",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("4b008c07-62db-4e7c-bdeb-60a3a5a2b3e8"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Forms-3/Layout-1.png",
                            ViewName = "Forms-3/Layout-1"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("416755f3-6c85-4c57-a9d8-48e456948f41"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Forms-3/Layout-2.png",
                            ViewName = "Forms-3/Layout-2"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("bf21f612-23de-4125-ab7f-944d5478894b"),
                            Name = "Layout 3",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Forms-3/Layout-3.png",
                            ViewName = "Forms-3/Layout-3"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Forms
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Campaign",
                    Id = new Guid("c8230467-f777-460c-bac1-8766026cf5a3"),
                    DataTypeId = 1302,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Campaign-1/Preview.png",
                    Intro = "Campaign header. Use the additional ‘menu bar’ to highlight (sub) pages of the campaign. You can add an image or...",
                    Description = "<strong>Campaign header.</strong> Use the additional ‘menu bar’ to highlight (sub) pages of the campaign. You can add an image or video within each slide of this header. Use this header on a campaign page with clear headers and recognizable content. ",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("23825c8e-2699-4bcf-bc35-2d0b8996f4bb"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Campaign-1/Layout-1.png",
                            ViewName = "Header-Campaign-1/Layout-1"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Headers
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Homepage #1",
                    Id = new Guid("a5763bd8-52fa-477a-b305-cd4ada0c8c80"),
                    DataTypeId = 1213,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Homepage-1/Preview.png",
                    Intro = "Three blocks. Highlight the three most important subjects directly. You can add an image or video within this header...",
                    Description = "<strong>Three blocks.</strong> Highlight the three most important subjects directly. You can add an image or video within this header.  Use this header on the homepage with clear titles and recognizable content.",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("27d0b28f-ffc3-4ba5-8c47-b8a6846ab07b"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Homepage-1/Layout-1.png",
                            ViewName = "Header-Homepage-1/Layout-1"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Headers
                    },

                    LimitToDocumentTypes = new[] { Homepage.ModelTypeAlias }
                },

                new ContentBlockDefinition
                {
                    Name = "Homepage #2",
                    Id = new Guid("18b7a686-3f8a-4e8d-be41-4201eff46703"),
                    DataTypeId = 1306,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Homepage-2/Preview.png",
                    Intro = "Slider. Highlight the most important content of the website in a slider. You can add an image or video within each slide...",
                    Description = "<strong>Slider.</strong> Highlight the most important content of the website in a slider. You can add an image or video within each slide of this header. Use this header on the homepage with clear headers and recognizable content. Please note that only the first slide is visible directly. ",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("f66bc524-56d3-412f-b5e1-d5d0d3fb8967"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Homepage-2/Layout-1.png",
                            ViewName = "Header-Homepage-2/Layout-1"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Headers
                    },

                    LimitToDocumentTypes = new[] { Homepage.ModelTypeAlias }
                },

                new ContentBlockDefinition
                {
                    Name = "Normal",
                    Id = new Guid("7fe32235-3240-466b-92c8-3e6e626692aa"),
                    DataTypeId = 1289,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Normal-1/Preview.png",
                    Intro = "Content page header. Use this header to clarify the purpose (main message) of a content page by using clear headers and recognizable content...",
                    Description = "<strong>Content page header.</strong> Use this header to clarify the purpose (main message) of a content page by using clear headers and recognizable content. You can add an image or video within this header. This content block is available in 4 lay-outs, with variations in the image size, position and background colour. <br/><br/> To make your page visual attractive, combine with an intro block in which the image position differs or with intro block #3 (text only).  ",
                    Mirrored = true,

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("9e1246fe-56b8-4133-b384-dacdc2cb9964"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Normal-1/Layout-1.png",
                            ViewName = "Header-Normal-1/Layout-1"
                        },

                        new ContentBlockLayout
                        {
                            Id = new Guid("b1347d07-5c58-4242-a242-28da7face81b"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Normal-1/Layout-2.png",
                            ViewName = "Header-Normal-1/Layout-2"
                        },

                        new ContentBlockLayout
                        {
                            Id = new Guid("a2052a40-c166-423a-8a30-bb07e7920734"),
                            Name = "Layout 3",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Normal-1/Layout-3.png",
                            ViewName = "Header-Normal-1/Layout-3"
                        },

                        new ContentBlockLayout
                        {
                            Id = new Guid("a4cffd1d-8cca-4715-a245-e0e171b8391e"),
                            Name = "Layout 4",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Normal-1/Layout-4.png",
                            ViewName = "Header-Normal-1/Layout-4"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("93E98D7D-0C3F-4D2C-8CD8-C210537A954E"),
                            Name = "Layout 5",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Normal-1/Layout-5.png",
                            ViewName = "Header-Normal-1/Layout-5"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("240C0B1F-F856-4625-A1CB-AC778EE4BF50"),
                            Name = "Layout 6",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Normal-1/Layout-6.png",
                            ViewName = "Header-Normal-1/Layout-6"
                        }
                    },

                    CategoryIds = new[]
                    {
                        categories.Headers
                    }
                },


                new ContentBlockDefinition
                {
                    Name = "Transparent",
                    Id = new Guid("e862800c-0277-4cac-a042-41466a4609f8"),
                    DataTypeId = 1291,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Transparent-1/Preview.png",
                    Intro = "Header. You can only add a transparent image within this header to achieve the effect as previewed. Use this header to clarify the...",
                    Description = "<strong>Header.</strong> You can only add a transparent image within this header to achieve the effect as previewed. Use this header to clarify the purpose of this content page with clear headers and recognizable content. ",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("4b7455c8-ddcf-4d66-bbb8-909ebc116755"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Transparent-1/Layout-1.png",
                            ViewName = "Header-Transparent-1/Layout-1"
                        },

                        new ContentBlockLayout
                        {
                            Id = new Guid("cc404503-c7c8-4cff-b45f-cf289f7c452b"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Transparent-1/Layout-2.png",
                            ViewName = "Header-Transparent-1/Layout-2"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Headers
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Car banner",
                    Id = new Guid("9cb8a7c2-7166-482e-bb98-73c8f5979513"),
                    DataTypeId = 1293,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Carbanner-1/Preview.png",
                    Intro = "Banner with transparent image. This banner can be used to separate sections in a page for example on longer pages, or to enrich the page...",
                    Description = "<strong>Banner with transparent image.</strong> This banner can be used to separate sections in a page for example on longer pages, or to enrich the page with additional branding. The banner is available in 2 background colours, yellow and blue.",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("2231a449-8ea8-4abe-8264-e52cb770ea73"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Carbanner-1/Layout-1.png",
                            ViewName = "Carbanner-1/Layout-1"
                        },

                        new ContentBlockLayout
                        {
                            Id = new Guid("b3a955d8-6263-43bd-b9d3-2773c7f81020"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Carbanner-1/Layout-2.png",
                            ViewName = "Carbanner-1/Layout-2"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Specials
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Countries",
                    Id = new Guid("0344592E-98D4-40F8-9FC3-07772111ED48"),
                    DataTypeId = 1700,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Countries-1/Preview.png",
                    Intro = "Links to Athlon countries. This content block consists of links to the different Athlon country websites. For each country, a flag and...",
                    Description = "<strong>Links to Athlon countries.</strong> This content block consists of links to the different Athlon country websites. For each country, a flag and website link is shown automatically. You only have to add a title here.",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("342CD733-47DD-469E-9BF1-FF67198B72C8"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Countries-1/Layout-1.png",
                            ViewName = "Countries-1/Layout-1"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Specials
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Events",
                    Id = new Guid("43dbab17-c581-462e-9226-cbc293c76ddc"),
                    DataTypeId = 1295,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Events-1/Preview.png",
                    Intro = "Upcoming events. A content block to highlight three upcoming events manually. Add a link to a content page to show more information...",
                    Description = "<strong>Upcoming events.</strong> A content block to highlight three upcoming events manually. Add a link to a content page to show more information about your event.",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("c3fbe8fb-d3eb-4700-99f9-7f121e6ccfd9"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Events-1/Layout-1.png",
                            ViewName = "Events-1/Layout-1"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Specials
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "FAQ",
                    Id = new Guid("AB769BC5-552D-4AD2-9B40-E8015C4FCDC5"),
                    DataTypeId = 1724,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/FAQ-1/Preview.png",
                    Intro = "Frequently Asked Questions. Use this content block to show FAQ questions related to the content on this page. Select an FAQ category...",
                    Description = "<strong>Frequently Asked Questions.</strong> Use this content block to show FAQ questions related to the content on this page. Select an FAQ category or separate FAQ questions to highlight within this content block. You can show the questions and answers in one or two columns. ",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("F058CF78-1053-41BC-8C18-EC220EA6114E"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/FAQ-1/Layout-1.png",
                            ViewName = "FAQ-1/Layout-1"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("C384C13B-0D9A-41C0-ADB0-DFDB57F04556"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/FAQ-1/Layout-2.png",
                            ViewName = "FAQ-1/Layout-2"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Specials
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Map",
                    Id = new Guid("b1c38a87-4533-489b-9fce-f5d502b05249"),
                    DataTypeId = 1296,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Map-1/Preview.png",
                    Intro = "Locations map. Use this content block to show Athlon partnerships and locations globally in a visual way. An up to date image will be...",
                    Description = "<strong>Locations map.</strong> Use this content block to show Athlon partnerships and locations globally in a visual way. An up to date image will be shown automatically. You only have to add a title and text here.",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("126c7bbb-8fb1-4ffc-be18-6ed98f1c0d50"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Map-1/Layout-1.png",
                            ViewName = "Map-1/Layout-1"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Specials
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "News Summary",
                    Id = new Guid("e5da0d3c-2c2c-4d4c-8cec-cb118e216b8a"),
                    DataTypeId = 1297,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/News-Summary-1/Preview.png",
                    Intro = "Latest news. With this content block you can automatically show the last 3 news items within the website. Select 3 other news...",
                    Description = "<strong>Latest news.</strong> With this content block you can automatically show the last 3 news items within the website. Select 3 other news items manually (related to the content on your page) to overrule this. Choose one of the three available lay-outs.",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("dead56ad-eef4-4a62-b777-358b8909b90a"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/News-Summary-1/Layout-1.png",
                            ViewName = "News-Summary-1/Layout-1"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("3c84ea4b-b86c-4673-bad4-75b5d645a198"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/News-Summary-1/Layout-2.png",
                            ViewName = "News-Summary-1/Layout-2"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("258d7d3b-8491-419e-b7cc-0eae0ee58607"),
                            Name = "Layout 3",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/News-Summary-1/Layout-3.png",
                            ViewName = "News-Summary-1/Layout-3"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Specials
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Page Links",
                    Id = new Guid("c12cde89-9bdd-4025-a1b2-b04c3b5d0e17"),
                    DataTypeId = 1298,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Page-Links-1/Preview.png",
                    Intro = "Links. This content block can be used to highlight three (related) pages with a reference to them. Each link has an image and title...",
                    Description = "<strong>Links.</strong> This content block can be used to highlight three (related) pages with a reference to them. Each link has an image and title. Choose one of the 3 available lay-outs. ",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("687dc827-fb89-45c7-9ef3-e59ba8879330"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Page-Links-1/Layout-1.png",
                            ViewName = "Page-Links-1/Layout-1"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("8cbe6d6e-8305-445d-9c13-d26c4a9703e1"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Page-Links-1/Layout-2.png",
                            ViewName = "Page-Links-1/Layout-2"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("0041ee15-f647-442e-97e7-9e646614bfd2"),
                            Name = "Layout 3",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Page-Links-1/Layout-3.png",
                            ViewName = "Page-Links-1/Layout-3"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Specials
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "People",
                    Id = new Guid("C3CBBA92-3B4A-41F3-AE2C-7632F75D65E4"),
                    DataTypeId = 1459,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/People-1/Preview.png",
                    Intro = "Our people overview. Within this content block, the content of the ‘Our people’ section (images, names and functions) is shown...",
                    Description = "<strong>Our people overview.</strong> Within this content block, the content of the ‘Our people’ section (images, names and functions) is shown automatically. You only have to add a title here.<br/><br/>Go to the ‘Our people’ page within your website to update this information. ",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("86CE7FC4-C777-434D-8E68-8A1687548023"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/People-1/Layout-1.png",
                            ViewName = "People-1/Layout-1"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Specials
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Slider",
                    Id = new Guid("fbded0ed-f156-4474-a57a-dbf5ba1a7896"),
                    DataTypeId = 1294,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Slider-1/Preview.png",
                    Intro = "Image slider. Use this slider to show images accompanied by text. The slider can be used for a story or action plan for example...",
                    Description = "<strong>Image slider.</strong> Use this slider to show images accompanied by text. The slider can be used for a story or action plan for example. Add a title and text to each image to clarify. Choose one of the 2 available lay-outs.",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("25b652eb-491d-44f6-8325-5b98476325ab"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Slider-1/Layout-1.png",
                            ViewName = "Slider-1/Layout-1"
                        },

                        new ContentBlockLayout
                        {
                            Id = new Guid("be90e099-5b79-47ed-a7b6-47ea445c6601"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Slider-1/Layout-2.png",
                            ViewName = "Slider-1/Layout-2"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Specials
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Strategic Partners",
                    Id = new Guid("0C0279E9-1457-4E65-B3B5-31A25A222734"),
                    DataTypeId = 1704,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Strategic-Partners-1/Preview.png",
                    Intro = "Partner logo’s. This content block can be used to show strategic partners with logo’s and country flags in a well-organized view...",
                    Description = "<strong>Partner logo’s.</strong> This content block can be used to show strategic partners with logo’s and country flags in a well-organized view. Partner information needs to be added manually. ",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("EACBE20D-C81C-4AAA-90B4-12F318031D8E"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Strategic-Partners-1/Layout-1.png",
                            ViewName = "Strategic-Partners-1/Layout-1"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Specials
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Timeline",
                    Id = new Guid("3711d5e4-2087-43e3-8144-954bd5ffd1b9"),
                    DataTypeId = 1299,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Timeline-1/Preview.png",
                    Intro = "Timeline. For example the history of Athlon can be shown in a visual timeline. Add milestones with icons and images to enrich...",
                    Description = "<strong>Timeline.</strong> For example the history of Athlon can be shown in a visual timeline. Add milestones with icons and images to enrich the timeline. ",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("69ad4209-0005-4bc8-afe9-4ce10567ee8c"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Timeline-1/Layout-1.png",
                            ViewName = "Timeline-1/Layout-1"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Specials
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Articles",
                    Id = new Guid("3714C47C-5B5F-43A1-B083-1CCFAE0EBF04"),
                    DataTypeId = 1501,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Articles-1/Preview.png",
                    Intro = "Waiting for text.. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna...",
                    Description = "Waiting for description..",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("34D5C8D3-1A1A-47A2-B293-0510F025F599"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Articles-1/Layout-1.png",
                            ViewName = "Articles-1/Layout-1"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.System
                    },

                    LimitToDocumentTypes = new []
                    {
                        Infrastructure.ModelsBuilder.Articles.ModelTypeAlias
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Challenges",
                    Id = new Guid("09C4DB7C-BB50-4C0B-B212-41EEB10DDD97"),
                    DataTypeId = 1502,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Challenges-1/Preview.png",
                    Intro = "Waiting for text.. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna...",
                    Description = "Waiting for description..",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("731220CB-D324-493C-A373-CC6FCF124A3D"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Challenges-1/Layout-1.png",
                            ViewName = "Challenges-1/Layout-1"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.System
                    },

                    LimitToDocumentTypes = new []
                    {
                        Infrastructure.ModelsBuilder.Challenges.ModelTypeAlias
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Solutions",
                    Id = new Guid("f83be540-c820-44f9-b875-e3b8bdefd24d"),
                    DataTypeId = 1327,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Solutions-1/Preview.png",
                    Intro = "Waiting for text.. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna...",
                    Description = "Waiting for description..",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("735a3f40-686b-4fc6-90c5-ba6f2879eb87"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Solutions-1/Layout-1.png",
                            ViewName = "Solutions-1/Layout-1"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.System
                    },

                    LimitToDocumentTypes = new []
                    {
                        OurSolutions.ModelTypeAlias
                    }
                },
                new ContentBlockDefinition
                {
                    Name = "Article Slider",
                    Id = new Guid("1BFE38DA-B867-416C-9BC0-DD6454722F3B"),
                    DataTypeId = 3313,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Article-Slider-1/Preview.png",
                    Intro = "Waiting for text.. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna...",
                    Description = "Waiting for description..",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("09645461-C63A-4422-B3AC-A003B3303195"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Article-Slider-1/Layout-1.png",
                            ViewName = "Article-Slider-1/Layout-1"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Specials
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Quality Mark",
                    Id = new Guid("48846338-2702-42EB-983E-C7854738C150"),
                    DataTypeId = 3315,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Quality-Mark-1/Preview.png",
                    Intro = "Waiting for text.. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna...",
                    Description = "Waiting for description..",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("ECA4834F-2BB9-4DBA-B281-DE61D54AA385"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Quality-Mark-1/Layout-1.png",
                            ViewName = "Quality-Mark-1/Layout-1"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Specials
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Quick Links",
                    Id = new Guid("A184C18D-E6C1-445D-8136-CBAFCF7DA4D9"),
                    DataTypeId = 3319,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Quick-Links-1/Preview.png",
                    Intro = "Waiting for text.. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna...",
                    Description = "Waiting for description..",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("A184C18D-E6C1-445D-8136-CBAFCF7DA4D9"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Quick-Links-1/Layout-1.png",
                            ViewName = "Quick-Links-1/Layout-1"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Specials
                    },
                },

                new ContentBlockDefinition
                {
                    Name = "Feeddex Ratings",
                    Id = new Guid("F8CD439C-1146-4090-9B2A-270B6F3C8086"),
                    DataTypeId = 3322,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Feeddex-Ratings-1/Preview.png",
                    Intro = "Waiting for text.. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna...",
                    Description = "Waiting for description..",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("EBE334BE-945C-4A5F-A2B4-E6D17B0EA03B"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Feeddex-Ratings-1/Layout-1.png",
                            ViewName = "Feeddex-Ratings-1/Layout-1"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Specials
                    },

                    LimitToCultures = new[]
                    {
                        "nl-NL"
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Lease Deals",
                    Id = new Guid(ContentBlocksLeaseDealsGuid),
                    DataTypeId = 3338,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Leasedeals-1/Preview.png",
                    Intro = "Waiting for text.. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna...",
                    Description = "Waiting for description..",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("DB2DE9AF-712E-42EA-9CCE-0F2F51FE4A8A"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Leasedeals-1/Layout-1.png",
                            ViewName = "Leasedeals-1/Layout-1"
                        },
                        new ContentBlockLayout
                        {
                            Id = new Guid("038C76AD-31EB-4406-B330-936C53B6A8FE"),
                            Name = "Layout 2",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Leasedeals-1/Layout-2.png",
                            ViewName = "Leasedeals-1/Layout-2"
                        }
                    },

                    CategoryIds = new[]
                    {
                        categories.Specials
                    },
                },

                new ContentBlockDefinition
                {
                    Name = "Lease Deals",
                    Id = new Guid("94661142-30B2-4F3F-9087-CF2403E19ED8"),
                    DataTypeId = 3326,
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Leasedeals-1/Preview.png",
                    Intro = "Waiting for text.. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna...",
                    Description = "Waiting for description...",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("FA3E8BC3-C4EE-4B04-924F-0CF6B8E6AC18"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Leasedeals-1/Layout-1.png",
                            ViewName = "Header-Leasedeals-1/Layout-1"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Headers
                    }
                },

                new ContentBlockDefinition
                {
                    Name = "Rating and testimonial",
                    Id = new Guid("61E41267-AACC-4FC3-9630-2C36BC579C03"),
                    DataTypeKey = new Guid("3a1fd735-b622-4a65-be3c-260a65be89d3"),
                    PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Leasedeals-1/Preview.png",
                    Intro = "Waiting for text.. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna...",
                    Description = "Waiting for description...",

                    Layouts = new IContentBlockLayout[]
                    {
                        new ContentBlockLayout
                        {
                            Id = new Guid("E06C49A1-4BC4-40DF-81AA-05801A2B217E"),
                            Name = "Layout 1",
                            Description = "",
                            PreviewImage = $"{ContentBlocksPreviewFolder}/Header-Leasedeals-1/Layout-1.png",
                            ViewName = "Rating-Testimonial-1/Layout-1"
                        },
                    },

                    CategoryIds = new[]
                    {
                        categories.Specials
                    }
                },
            }.ToDictionary(d => d.Id);
        }

        private readonly IDictionary<Guid, IContentBlockDefinition> _definitions;

        private readonly IContentBlockDefinitionFilterer _definitionFilterer;

        public IContentBlockDefinition GetById(Guid id)
        {
            return _definitions.TryGetValue(id, out var definition) ? definition : null;
        }

        public IEnumerable<IContentBlockDefinition> GetAll()
        {
            return _definitions.Values;
        }

        public IEnumerable<IContentBlockDefinition> GetAllForPage(int pageId, string culture)
            => _definitionFilterer.FilterForPage(GetAll(), pageId, culture);

        public IEnumerable<IContentBlockDefinition> GetAllForPage(string documentType, string culture)
            => _definitionFilterer.FilterForPage(GetAll(), documentType, culture);
    }
}
