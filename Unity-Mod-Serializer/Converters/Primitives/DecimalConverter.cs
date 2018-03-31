using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Converters.Primitives
{
    public sealed class DecimalConverter : DirectConverter<decimal>
    {
        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            serialized = new Data((double)Convert.ChangeType(instance, typeof(double)));
            return Result.Success;
        }
        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            if (!data.IsDouble)
                return Result.Fail("Expected type double");

            instance = Convert.ChangeType(data.AsDouble, typeof(double));
            return Result.Success;
        }
    }
}
