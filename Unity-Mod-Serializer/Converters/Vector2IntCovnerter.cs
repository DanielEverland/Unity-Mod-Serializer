using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UMS.Converters
{
    public class Vector2IntCovnerter : DirectConverter<Vector2Int>
    {
        public override Result DoSerialize(Vector2Int obj, out Data data)
        {
            Result result = Result.Success;

            data = new Data(new List<Data>());

            result += Serializer.Serialize(obj.x, out Data xData);
            result += Serializer.Serialize(obj.y, out Data yData);

            data.Add(xData);
            data.Add(yData);

            return result;
        }
        public override Result DoDeserialize(Data data, ref Vector2Int obj)
        {
            Result result = Result.Success;

            if (!data.IsList)
                return Result.Error("Type mismatch. Expected list", data);

            int xValue = 0, yValue = 0;

            result += Serializer.Deserialize(data[0], ref xValue);
            result += Serializer.Deserialize(data[1], ref yValue);

            obj.x = xValue;
            obj.y = yValue;

            return result;
        }
    }
}
