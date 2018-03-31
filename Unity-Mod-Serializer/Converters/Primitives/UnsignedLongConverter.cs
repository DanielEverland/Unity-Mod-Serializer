using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Converters.Primitives
{
    public sealed class UnsignedLongConverter : DirectConverter<ulong>
    {
        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            if (Serializer.Config.Serialize64BitIntegerAsString)
            {
                serialized = new Data((string)Convert.ChangeType(instance, typeof(string)));
            }
            else
            {
                serialized = new Data((ulong)instance);
            }

            return Result.Success;
        }
        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            if (Serializer.Config.Serialize64BitIntegerAsString)
            {
                if (!data.IsString)
                    return Result.Fail("Expected type string");

                instance = Convert.ChangeType(data.AsString, typeof(ulong));
            }
            else
            {
                if (!data.IsInt64)
                    return Result.Fail("Expected type Int64");

                instance = data.AsInt64;
            }

            return Result.Success;
        }
    }
}
