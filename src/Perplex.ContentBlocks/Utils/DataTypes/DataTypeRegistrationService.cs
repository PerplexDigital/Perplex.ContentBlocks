using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Perplex.ContentBlocks.Utils.DataTypes
{
    public class DataTypeRegistrationService : IDataTypeRegistrationService
    {
        private readonly PropertyEditorCollection _propertyEditorCollection;
        private readonly IDataTypeService _dataTypeService;

        public DataTypeRegistrationService(
            IDataTypeService dataTypeService,
            PropertyEditorCollection propertyEditorCollection)
        {
            _dataTypeService = dataTypeService;
            _propertyEditorCollection = propertyEditorCollection;
        }

        public void RegisterDataType(string dataTypeName, string propertyEditorAlias, params string[] folders)
        {
            if (!_propertyEditorCollection.TryGet(propertyEditorAlias, out var editor))
            {
                throw new System.Exception($"No Property Editor with alias '{propertyEditorAlias}' was found.");
            }

            int parentId = -1;
            if (folders.Length > 0)
            {
                EntityContainer container;

                for (int i = 0; i < folders.Length; i++)
                {
                    string folder = folders[i];
                    container = _dataTypeService.GetContainers(folder, i + 1)?.FirstOrDefault();
                    if (container == null)
                    {
                        var attempt = _dataTypeService.CreateContainer(parentId, folder);
                        if (attempt.Result.Entity is EntityContainer newContainer)
                        {
                            _dataTypeService.SaveContainer(newContainer);
                            parentId = newContainer.Id;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        parentId = container.Id;
                    }
                }
            }

            var dataType = new DataType(editor, parentId) { Name = dataTypeName };

            _dataTypeService.Save(dataType);
        }
    }
}
