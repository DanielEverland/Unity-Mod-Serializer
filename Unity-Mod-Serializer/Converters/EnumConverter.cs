using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters
{
    public class EnumConverter : BaseConverter<Enum>
    {
        public override Result DoSerialize(Enum obj, out Data data)
        {
            Type type = obj.GetType();
            long value = Convert.ToInt64(obj);

            string toSerialize = string.Format("{0}:{1}", value, type.AssemblyQualifiedName);
            data = new Data(toSerialize);

            return Result.Success;
        }
        public override Result DoDeserialize(Data data, ref Enum obj)
        {
            if (!data.IsString)
                return Result.Error("Type mismatch. Expected string");

            string serializedValue = data.AsString;
            string typeName = serializedValue.Split(':')[1];
            long value = long.Parse(serializedValue.Split(':')[0]);

            Type type = Type.GetType(typeName);

            obj = (Enum)Enum.ToObject(type, value);
            return Result.Success;
        }
    }
}
