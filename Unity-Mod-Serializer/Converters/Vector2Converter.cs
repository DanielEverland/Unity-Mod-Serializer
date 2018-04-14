using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UMS.Converters
{
    public class Vector2Converter : DirectConverter<Vector2>
    {
        public override Result DoSerialize(Vector2 obj, out Data data)
        {
            Result result = Result.Success;

            data = new Data(new List<Data>());

            result += Serializer.Serialize(obj.x, out Data xData);
            result += Serializer.Serialize(obj.y, out Data yData);

            data.Add(xData);
            data.Add(yData);

            return result;
        }
        public override Result DoDeserialize(Data data, ref Vector2 obj)
        {
            Result result = Result.Success;

            if (!data.IsList)
                return Result.Error("Type mismatch. Expected list", data);

            result += Serializer.Deserialize(data[0], ref obj.x);
            result += Serializer.Deserialize(data[1], ref obj.y);

            return result;
        }
    }
}
