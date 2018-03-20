using System;
using System.Collections.Generic;
using UnityEngine;

namespace UMS.Converters
{
    partial class ConverterRegistrar
    {
#pragma warning disable 0649
        public static LayerMask_DirectConverter Register_LayerMask_DirectConverter;
#pragma warning restore
    }
    public class LayerMask_DirectConverter : DirectConverter<LayerMask>
    {
        protected override Result DoSerialize(LayerMask model, Dictionary<string, Data> serialized)
        {
            var result = Result.Success;

            result += SerializeMember(serialized, null, "value", model.value);

            return result;
        }

        protected override Result DoDeserialize(Dictionary<string, Data> data, ref LayerMask model)
        {
            var result = Result.Success;

            var t0 = model.value;
            result += DeserializeMember(data, null, "value", out t0);
            model.value = t0;

            return result;
        }

        public override object CreateInstance(Data data, Type storageType)
        {
            return new LayerMask();
        }
    }
}