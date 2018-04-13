using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters.Primitives
{
    public class BooleanConverter : DirectConverter<bool>
    {
        public override Result Serialize(bool obj, out Data data)
        {
            data = new Data(obj);

            return Result.Success;
        }
        public override Result Deserialize(Data data, ref bool obj)
        {
            if (!data.IsBool)
                return Result.Error("Type mismatch. Expected type bool", data);

            obj = data.AsBool;

            return Result.Success;
        }        
    }
}
