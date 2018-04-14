using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UMS.Converters
{
    public class Vector4Converter : DirectConverter<Vector4>
    {
        public override Result DoSerialize(Vector4 obj, out Data data)
        {
            Result result = Result.Success;

            data = new Data(new List<Data>());

            result += Serializer.Serialize(obj.x, out Data xData);
            result += Serializer.Serialize(obj.y, out Data yData);
            result += Serializer.Serialize(obj.z, out Data zData);
            result += Serializer.Serialize(obj.w, out Data wData);

            data.Add(xData);
            data.Add(yData);
            data.Add(zData);
            data.Add(wData);

            return result;
        }
        public override Result DoDeserialize(Data data, ref Vector4 obj)
        {
            Result result = Result.Success;

            if (!data.IsList)
                return Result.Error("Type mismatch. Expected list", data);

            result += Serializer.Deserialize(data[0], ref obj.x);
            result += Serializer.Deserialize(data[1], ref obj.y);
            result += Serializer.Deserialize(data[2], ref obj.z);
            result += Serializer.Deserialize(data[3], ref obj.w);

            return result;
        }
    }
}
