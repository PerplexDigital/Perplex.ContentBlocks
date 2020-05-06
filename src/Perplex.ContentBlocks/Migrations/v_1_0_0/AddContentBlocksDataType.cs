using Perplex.ContentBlocks.Utils.DataTypes;
using Umbraco.Core.Migrations;

namespace Perplex.ContentBlocks.Migrations.v_1_0_0
{
    public class AddContentBlocksDataType : MigrationBase
    {
        private readonly IDataTypeRegistrationService _dataTypeRegistrationService;

        public AddContentBlocksDataType(
            IMigrationContext context,
            IDataTypeRegistrationService dataTypeRegistrationService)
            : base(context)
        {
            _dataTypeRegistrationService = dataTypeRegistrationService;
        }

        public override void Migrate()
        {
            _dataTypeRegistrationService.RegisterDataType(
                   "Perplex.ContentBlocks",
                   Constants.PropertyEditor.Alias);
        }
    }
}
