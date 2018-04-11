using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Converters.Primitives
{
    public sealed class IntConverter : DirectConverter<int>
    {
        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            serialized = new Data((int)instance);
            return Result.Success;
        }
        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            if (!data.IsInt64)
                return Result.Fail("Expected type Int64");

            instance = Convert.ChangeType(data.AsInt64, typeof(int));
            return Result.Success;
        }
    }
}
