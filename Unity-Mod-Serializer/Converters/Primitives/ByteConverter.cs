using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters.Primitives
{
    public class ByteConverter : DirectConverter<byte>
    {
        public override Result Serialize(byte obj, out Data data)
        {
            data = new Data(obj);
            return Result.Success;
        }
        public override Result Deserialize(Data data, ref byte obj)
        {
            if (!data.IsLong)
                return Result.Error("Type mismatch. Expected long type", data);

            obj = (byte)System.Convert.ChangeType(data.AsLong, typeof(byte));
            return Result.Success;
        }
    }
}
