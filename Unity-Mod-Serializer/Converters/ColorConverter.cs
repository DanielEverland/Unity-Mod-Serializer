using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UMS.Converters
{
    public class ColorConverter : DirectConverter<Color>
    {
        public override Result DoSerialize(Color obj, out Data data)
        {
            List<Data> pixelData = new List<Data>()
            {
                new Data(((Color32)obj).r),
                new Data(((Color32)obj).g),
                new Data(((Color32)obj).b),
                new Data(((Color32)obj).a),
            };

            data = new Data(pixelData);

            return Result.Success;
        }
        public override Result DoDeserialize(Data data, ref Color obj)
        {
            if (!data.IsList)
                return Result.Error("Type mismatch. Expected List");

            if (data.List.Count != 4)
                return Result.Exception(new System.IndexOutOfRangeException());

            obj = new Color32()
            {
                r = data[0].Byte,
                g = data[1].Byte,
                b = data[2].Byte,
                a = data[3].Byte,
            };

            return Result.Success;
        }
    }
}
