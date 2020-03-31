namespace Perplex.ContentBlocks.Utils.DataTypes
{
    public interface IDataTypeRegistrationService
    {
        /// <summary>
        /// Registers a data type in Umbraco based on the specified property editor alias.
        /// </summary>
        /// <param name="dataTypeName"></param>
        /// <param name="propertyEditorAlias"></param>
        /// <param name="folders">Folder path for the new data type. If left empty, data type will be created at the root level</param>
        void RegisterDataType(string dataTypeName, string propertyEditorAlias, params string[] folders);
    }
}
