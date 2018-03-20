using System;
using System.Collections.Generic;
using UnityEngine;

namespace UMS.Converters
{
    partial class ConverterRegistrar
    {
        public static Bounds_DirectConverter Register_Bounds_DirectConverter;
    }
    public class Bounds_DirectConverter : DirectConverter<Bounds>
    {
        protected override Result DoSerialize(Bounds model, Dictionary<string, Data> serialized)
        {
            var result = Result.Success;

            result += SerializeMember(serialized, null, "center", model.center);
            result += SerializeMember(serialized, null, "size", model.size);

            return result;
        }

        protected override Result DoDeserialize(Dictionary<string, Data> data, ref Bounds model)
        {
            var result = Result.Success;

            var t0 = model.center;
            result += DeserializeMember(data, null, "center", out t0);
            model.center = t0;

            var t1 = model.size;
            result += DeserializeMember(data, null, "size", out t1);
            model.size = t1;

            return result;
        }

        public override object CreateInstance(Data data, Type storageType)
        {
            return new Bounds();
        }
    }
}