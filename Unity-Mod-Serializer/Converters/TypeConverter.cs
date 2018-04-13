using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters
{
    public class TypeConverter : BaseConverter<Type>
    {
        public override Result DoSerialize(Type obj, out Data data)
        {
            data = new Data(obj.AssemblyQualifiedName);
            return Result.Success;
        }
        public override Result DoDeserialize(Data data, ref Type obj)
        {
            if (!data.IsString)
                return Result.Error("Type mismatch. Expected string", data);

            obj = Type.GetType(data.AsString);
            return Result.Success;
        }
    }
}
