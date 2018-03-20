using System;
using System.Collections.Generic;
using UnityEngine;

namespace UMS.Converters
{
    partial class ConverterRegistrar
    {
        public static RectOffset_DirectConverter Register_RectOffset_DirectConverter;
    }
    public class RectOffset_DirectConverter : DirectConverter<RectOffset>
    {
        protected override Result DoSerialize(RectOffset model, Dictionary<string, Data> serialized)
        {
            var result = Result.Success;

            result += SerializeMember(serialized, null, "bottom", model.bottom);
            result += SerializeMember(serialized, null, "left", model.left);
            result += SerializeMember(serialized, null, "right", model.right);
            result += SerializeMember(serialized, null, "top", model.top);

            return result;
        }

        protected override Result DoDeserialize(Dictionary<string, Data> data, ref RectOffset model)
        {
            var result = Result.Success;

            var t0 = model.bottom;
            result += DeserializeMember(data, null, "bottom", out t0);
            model.bottom = t0;

            var t2 = model.left;
            result += DeserializeMember(data, null, "left", out t2);
            model.left = t2;

            var t3 = model.right;
            result += DeserializeMember(data, null, "right", out t3);
            model.right = t3;

            var t4 = model.top;
            result += DeserializeMember(data, null, "top", out t4);
            model.top = t4;

            return result;
        }

        public override object CreateInstance(Data data, Type storageType)
        {
            return new RectOffset();
        }
    }
}