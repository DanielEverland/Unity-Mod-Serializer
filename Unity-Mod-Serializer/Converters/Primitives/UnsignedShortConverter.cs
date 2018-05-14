using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters.Primitives
{
    public class UnsignedShortConverter : DirectConverter<ushort>
    {
        public override Result DoSerialize(ushort obj, out Data data)
        {
            data = new Data(obj);
            return Result.Success;
        }
        public override Result DoDeserialize(Data data, ref ushort obj)
        {
            if (!data.IsLong)
                return Result.Error("Type mismatch. Expected Long", data);

            obj = System.Convert.ToUInt16(data.Long);
            return Result.Success;
        }
    }
}
