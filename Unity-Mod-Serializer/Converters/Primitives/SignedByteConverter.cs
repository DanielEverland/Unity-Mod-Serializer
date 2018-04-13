using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters.Primitives
{
    public class SignedByteConverter : DirectConverter<sbyte>
    {
        public override Result DoSerialize(sbyte obj, out Data data)
        {
            data = new Data(obj);
            return Result.Success;
        }
        public override Result DoDeserialize(Data data, ref sbyte obj)
        {
            if (!data.IsLong)
                return Result.Error("Type mismatch. Expected Long", data);

            obj = (sbyte)System.Convert.ChangeType(data.AsLong, typeof(sbyte));
            return Result.Success;
        }
    }
}
