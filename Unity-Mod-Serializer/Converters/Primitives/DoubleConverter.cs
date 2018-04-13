using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters.Primitives
{
    public class DoubleConverter : DirectConverter<double>
    {
        public override Result Serialize(double obj, out Data data)
        {
            data = new Data(obj);
            return Result.Success;
        }
        public override Result Deserialize(Data data, ref double obj)
        {
            if (!data.IsDouble)
                return Result.Error("Type mismatch. Expected Double", data);

            obj = data.AsDouble;
            return Result.Success;
        }
    }
}
