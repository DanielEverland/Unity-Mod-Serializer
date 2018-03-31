using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Converters.Primitives
{
    public sealed class LongConverter : DirectConverter<long>
    {
        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            if (Serializer.Config.Serialize64BitIntegerAsString)
            {
                serialized = new Data((string)Convert.ChangeType(instance, typeof(string)));
            }
            else
            {
                serialized = new Data((long)instance);
            }
            
            return Result.Success;
        }
        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            if (data.IsInt64)
            {
                instance = data.AsInt64;
            }
            else if(data.IsString)
            {
                if (Serializer.Config.Serialize64BitIntegerAsString)
                {
                    instance = Convert.ChangeType(data.AsString, typeof(long));
                }
                else
                {
                    return Result.Fail("Value is serialized as string, but the config doesn't allow conversion from string");
                }
            }
            
            return Result.Success;
        }
    }
}
