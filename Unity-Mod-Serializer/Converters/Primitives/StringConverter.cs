using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters.Primitives
{
    public class StringConverter : DirectConverter<string>
    {
        public override object CreateInstance(Type type)
        {
            return string.Empty;
        }
        public override Result DoSerialize(string obj, out Data data)
        {
            data = new Data(obj);
            return Result.Success;
        }
        public override Result DoDeserialize(Data data, ref string obj)
        {
            if (!data.IsString)
                return Result.Error("Type mismatch. Expected String", data);

            obj = data.String;
            return Result.Success;
        }
    }
}
