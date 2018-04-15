﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UMS.Converters
{
    public class Vector3IntConverter : DirectConverter<Vector3Int>
    {
        public override Result DoSerialize(Vector3Int obj, out Data data)
        {
            Result result = Result.Success;

            data = new Data(new List<Data>());

            result += Serializer.Serialize(obj.x, out Data xData);
            result += Serializer.Serialize(obj.y, out Data yData);
            result += Serializer.Serialize(obj.z, out Data zData);

            data.Add(xData);
            data.Add(yData);
            data.Add(zData);

            return result;
        }
        public override Result DoDeserialize(Data data, ref Vector3Int obj)
        {
            Result result = Result.Success;

            if (!data.IsList)
                return Result.Error("Type mismatch. Expected list", data);

            int xValue = 0, yValue = 0, zValue = 0;

            result += Serializer.Deserialize(data[0], ref xValue);
            result += Serializer.Deserialize(data[1], ref yValue);
            result += Serializer.Deserialize(data[2], ref zValue);

            obj.x = xValue;
            obj.y = yValue;
            obj.z = zValue;

            return result;
        }
    }
}