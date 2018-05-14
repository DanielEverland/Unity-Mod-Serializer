using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters.Primitives
{
    public class ShortConverter : DirectConverter<short>
    {
        public override Result DoSerialize(short obj, out Data data)
        {
            data = new Data(obj);
            return Result.Success;
        }
        public override Result DoDeserialize(Data data, ref short obj)
        {
            if (!data.IsLong)
                return Result.Error("Type mismatch. Expected Long", data);

            obj = System.Convert.ToInt16(data.Long);
            return Result.Success;
        }
    }
}
