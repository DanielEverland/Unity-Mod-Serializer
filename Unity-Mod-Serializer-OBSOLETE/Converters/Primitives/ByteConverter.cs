using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Converters.Primitives
{
    public sealed class ByteConverter : DirectConverter<byte>
    {
        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            serialized = new Data((byte)instance);
            return Result.Success;
        }
        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            if (!data.IsInt64)
                return Result.Fail("Expected type Int64");

            instance = Convert.ChangeType(data.AsInt64, typeof(byte));
            return Result.Success;
        }
    }
}
