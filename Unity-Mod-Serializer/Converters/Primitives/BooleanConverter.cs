using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters.Primitives
{
    public class BooleanConverter : DirectConverter<bool>
    {
        public override Result DoSerialize(bool obj, out Data data)
        {
            data = new Data(obj);

            return Result.Success;
        }
        public override Result DoDeserialize(Data data, ref bool obj)
        {
            if (!data.IsBool)
                return Result.Error("Type mismatch. Expected type bool", data);

            obj = data.Bool;
            return Result.Success;
        }        
    }
}
