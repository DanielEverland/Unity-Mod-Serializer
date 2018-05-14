using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters.Primitives
{
    public class IntegerConverter : DirectConverter<int>
    {
        public override Result DoSerialize(int obj, out Data data)
        {
            data = new Data(obj);
            return Result.Success;
        }
        public override Result DoDeserialize(Data data, ref int obj)
        {
            if (!data.IsInt)
                return Result.Error("Type mismatch. Expected Int", data);

            obj = System.Convert.ToInt32(data.Int);
            return Result.Success;
        }
    }
}
