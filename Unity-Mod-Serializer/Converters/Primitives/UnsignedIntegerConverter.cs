using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters.Primitives
{
    public class UnsignedIntegerConverter : DirectConverter<uint>
    {
        public override Result DoSerialize(uint obj, out Data data)
        {
            data = new Data(obj);
            return Result.Success;
        }
        public override Result DoDeserialize(Data data, ref uint obj)
        {
            if (!data.IsUInt)
                return Result.Error("Type mismatch. Expected UInt", data);

            obj = System.Convert.ToUInt32(data.UInt);
            return Result.Success;
        }
    }
}
