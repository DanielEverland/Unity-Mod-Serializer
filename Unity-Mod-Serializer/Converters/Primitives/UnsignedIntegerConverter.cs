using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters.Primitives
{
    public class UnsignedIntegerConverter : DirectConverter<uint>
    {
        public override Result Serialize(uint obj, out Data data)
        {
            data = new Data(obj);
            return Result.Success;
        }
        public override Result Deserialize(Data data, ref uint obj)
        {
            if (!data.IsLong)
                return Result.Error("Type mismatch. Expected Long", data);

            obj = System.Convert.ToUInt32(data.AsLong);
            return Result.Success;
        }
    }
}
