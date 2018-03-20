using System;
using System.Collections.Generic;
using UnityEngine;

namespace UMS.Converters
{
    partial class ConverterRegistrar
    {
#pragma warning disable 0649
        public static GUIStyleState_DirectConverter Register_GUIStyleState_DirectConverter;
#pragma warning restore
    }
    public class GUIStyleState_DirectConverter : DirectConverter<GUIStyleState>
    {
        protected override Result DoSerialize(GUIStyleState model, Dictionary<string, Data> serialized)
        {
            var result = Result.Success;

            result += SerializeMember(serialized, null, "background", model.background);
            result += SerializeMember(serialized, null, "textColor", model.textColor);

            return result;
        }

        protected override Result DoDeserialize(Dictionary<string, Data> data, ref GUIStyleState model)
        {
            var result = Result.Success;

            var t0 = model.background;
            result += DeserializeMember(data, null, "background", out t0);
            model.background = t0;

            var t2 = model.textColor;
            result += DeserializeMember(data, null, "textColor", out t2);
            model.textColor = t2;

            return result;
        }

        public override object CreateInstance(Data data, Type storageType)
        {
            return new GUIStyleState();
        }
    }
}