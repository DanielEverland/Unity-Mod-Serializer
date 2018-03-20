using System;
using System.Collections.Generic;
using UnityEngine;

namespace UMS.Converters
{
    partial class ConverterRegistrar
    {
#pragma warning disable 0649
        public static Gradient_DirectConverter Register_Gradient_DirectConverter;
#pragma warning restore
    }
    public class Gradient_DirectConverter : DirectConverter<Gradient>
    {
        protected override Result DoSerialize(Gradient model, Dictionary<string, Data> serialized)
        {
            var result = Result.Success;

            result += SerializeMember(serialized, null, "alphaKeys", model.alphaKeys);
            result += SerializeMember(serialized, null, "colorKeys", model.colorKeys);

            return result;
        }

        protected override Result DoDeserialize(Dictionary<string, Data> data, ref Gradient model)
        {
            var result = Result.Success;

            var t0 = model.alphaKeys;
            result += DeserializeMember(data, null, "alphaKeys", out t0);
            model.alphaKeys = t0;

            var t1 = model.colorKeys;
            result += DeserializeMember(data, null, "colorKeys", out t1);
            model.colorKeys = t1;

            return result;
        }

        public override object CreateInstance(Data data, Type storageType)
        {
            return new Gradient();
        }
    }
}