using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UMS.Converters
{
    public class QuaternionConverter : DirectConverter<Quaternion>
    {
        public override Result DoSerialize(Quaternion obj, out Data data)
        {
            Result result = Result.Success;

            data = new Data(new List<Data>());
            for (int i = 0; i < 4; i++)
            {
                result += Serializer.Serialize(obj[i], out Data itemData);
                data.Add(itemData);
            }

            return result;
        }
        public override Result DoDeserialize(Data data, ref Quaternion obj)
        {
            Result result = Result.Success;

            if (!data.IsList)
                return Result.Error("Type mismatch. Expected list", data);

            if (data.AsList.Count != 4)
                return Result.Error("Missing items. Expected a list of 4 floats", data);

            for (int i = 0; i < 4; i++)
            {
                float value = 0;
                result += Serializer.Deserialize(data[i], ref value);

                obj[i] = value;
            }

            return result;
        }
    }
}
