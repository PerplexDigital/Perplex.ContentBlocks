using Athlon.Providers;
using Athlon.Infrastructure.ModelsBuilder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Perplex.ContentBlocks.Presets
{
    public class HardCodedContentBlocksPresetService : IContentBlocksPresetService
    {
        private readonly IDocumentTypeAliasProvider _documentTypeAliasProvider;

        public HardCodedContentBlocksPresetService(IDocumentTypeAliasProvider documentTypeAliasProvider)
        {
            _documentTypeAliasProvider = documentTypeAliasProvider;
        }

        private readonly IDictionary<Guid, IContentBlocksPreset> _presets = new IContentBlocksPreset[]
        {
            new ContentBlocksPreset
            {
                Id = new Guid("23461f4d-4ac0-4e8a-b7ef-7ecaa11941ac"),
                Name = "Our Solutions",
                ApplyToDocumentTypes = new[] { OurSolutions.ModelTypeAlias },
                Blocks = new[]
                {
                    new ContentBlockPreset
                    {
                        Id = new Guid("f956fa16-3ac8-425e-b064-031e6d0436ac"),
                        IsMandatory = true,

                        // TODO: DefinitionIds / LayoutIds in Constants.ContentBlocks zetten
                        // Solutions #1
                        DefinitionId = new Guid("f83be540-c820-44f9-b875-e3b8bdefd24d"),
                        // Layout #1
                        LayoutId = new Guid("735a3f40-686b-4fc6-90c5-ba6f2879eb87")
                    }
                }
            },
            new ContentBlocksPreset
            {
                Id = new Guid("526E44A4-5CC8-4BEC-B22B-73366C7250D4"),
                Name = "Articles",
                ApplyToDocumentTypes = new[] { Infrastructure.ModelsBuilder.Articles.ModelTypeAlias },
                Blocks = new[]
                {
                    new ContentBlockPreset
                    {
                        Id = new Guid("FE01E552-FBB7-4D8C-9576-39BC98761564"),
                        IsMandatory = true,

                        // Articles #1
                        DefinitionId = new Guid("3714C47C-5B5F-43A1-B083-1CCFAE0EBF04"),
                        // Layout #1
                        LayoutId = new Guid("34D5C8D3-1A1A-47A2-B293-0510F025F599")
                    }
                }
            },
            new ContentBlocksPreset
            {
                Id = new Guid("E48FE94A-D8F6-423B-904D-C7E9295CD6F9"),
                Name = "Challenges",
                ApplyToDocumentTypes = new[] { Infrastructure.ModelsBuilder.Challenges.ModelTypeAlias },
                Blocks = new[]
                {
                    new ContentBlockPreset
                    {
                        Id = new Guid("47A8D400-2146-453B-8373-D9F7511DD64D"),
                        IsMandatory = true,

                        // Challenges #1
                        DefinitionId = new Guid("09C4DB7C-BB50-4C0B-B212-41EEB10DDD97"),
                        // Layout #1
                        LayoutId = new Guid("731220CB-D324-493C-A373-CC6FCF124A3D")
                    }
                }
            },
        }.ToDictionary(d => d.Id);

        public IContentBlocksPreset GetById(Guid id)
        {
            return _presets.TryGetValue(id, out var preset) ? preset : null;
        }

        public IEnumerable<IContentBlocksPreset> GetAll()
        {
            return _presets.Values;
        }

        public IContentBlocksPreset GetPresetForPage(int pageId, string culture)
        {
            if (string.IsNullOrEmpty(culture))
            {
                return null;
            }

            string documentType = _documentTypeAliasProvider.GetDocumentTypeAlias(pageId);
            return GetPresetForPage(documentType, culture);
        }

        public IContentBlocksPreset GetPresetForPage(string documentType, string culture)
        {
            if (string.IsNullOrEmpty(documentType))
            {
                return null;
            }

            bool isEmptyOrContains(IEnumerable<string> input, string toMatch)
                => input?.Any() != true || input.Any(i => string.Equals(i, toMatch, StringComparison.InvariantCultureIgnoreCase));

            return GetAll()?.FirstOrDefault(p =>
                isEmptyOrContains(p.ApplyToCultures, culture) &&
                isEmptyOrContains(p.ApplyToDocumentTypes, documentType));
        }
    }
}
