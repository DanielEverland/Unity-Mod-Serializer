using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters.Primitives
{
    public class FloatConverter : DirectConverter<float>
    {
        public override Result DoSerialize(float obj, out Data data)
        {
            data = new Data(obj);
            return Result.Success;
        }
        public override Result DoDeserialize(Data data, ref float obj)
        {
            if (!data.IsDouble)
                return Result.Error("Type mismatch. Expected Double", data);

            obj = System.Convert.ToSingle(data.AsDouble);
            return Result.Success;
        }
    }
}
