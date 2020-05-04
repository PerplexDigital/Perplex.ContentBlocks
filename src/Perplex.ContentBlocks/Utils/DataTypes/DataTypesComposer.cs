using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Perplex.ContentBlocks.Utils.DataTypes
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Install)]
    public class DataTypesComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IDataTypeRegistrationService, DataTypeRegistrationService>(Lifetime.Singleton);
        }
    }
}
