using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters.Primitives
{
    public class ByteConverter : DirectConverter<byte>
    {
        public override Result DoSerialize(byte obj, out Data data)
        {
            data = new Data(obj);
            return Result.Success;
        }
        public override Result DoDeserialize(Data data, ref byte obj)
        {
            if (!data.IsLong)
                return Result.Error("Type mismatch. Expected long type", data);

            obj = System.Convert.ToByte(data.Long);
            return Result.Success;
        }
    }
}
