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
            if (!data.IsSByte)
                return Result.Error("Type mismatch. Expected SByte", data);

            obj = (sbyte)System.Convert.ChangeType(data.SByte, typeof(sbyte));
            return Result.Success;
        }
    }
}
