using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Converters.Primitives
{
    public sealed class StringConverter : DirectConverter<string>
    {
        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            serialized = new Data((string)instance);
            return Result.Success;
        }
        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            if (!data.IsString)
                return Result.Fail("Expected type string");

            instance = data.AsString;
            return Result.Success;
        }
    }
}
