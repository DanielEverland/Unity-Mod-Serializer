using System;
using System.Collections.Generic;
using UnityEngine;

namespace UMS.Converters
{
    partial class ConverterRegistrar
    {
#pragma warning disable 0649
        public static Rect_DirectConverter Register_Rect_DirectConverter;
#pragma warning restore
    }
    public class Rect_DirectConverter : DirectConverter<Rect>
    {
        protected override Result DoSerialize(Rect model, Dictionary<string, Data> serialized)
        {
            var result = Result.Success;

            result += SerializeMember(serialized, null, "xMin", model.xMin);
            result += SerializeMember(serialized, null, "yMin", model.yMin);
            result += SerializeMember(serialized, null, "xMax", model.xMax);
            result += SerializeMember(serialized, null, "yMax", model.yMax);

            return result;
        }

        protected override Result DoDeserialize(Dictionary<string, Data> data, ref Rect model)
        {
            var result = Result.Success;

            var t0 = model.xMin;
            result += DeserializeMember(data, null, "xMin", out t0);
            model.xMin = t0;

            var t1 = model.yMin;
            result += DeserializeMember(data, null, "yMin", out t1);
            model.yMin = t1;

            var t2 = model.xMax;
            result += DeserializeMember(data, null, "xMax", out t2);
            model.xMax = t2;

            var t3 = model.yMax;
            result += DeserializeMember(data, null, "yMax", out t3);
            model.yMax = t3;

            return result;
        }

        public override object CreateInstance(Data data, Type storageType)
        {
            return new Rect();
        }
    }
}