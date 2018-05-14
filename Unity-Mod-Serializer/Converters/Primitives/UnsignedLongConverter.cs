using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters.Primitives
{
    public class UnsignedLongConverter : DirectConverter<ulong>
    {
        public override Result DoSerialize(ulong obj, out Data data)
        {
            data = new Data(obj);
            return Result.Success;
        }
        public override Result DoDeserialize(Data data, ref ulong obj)
        {
            if (!data.IsULong)
                return Result.Error("Type mismatch. Expected ULong", data);

            obj = data.ULong;
            return Result.Success;
        }
    }
}
