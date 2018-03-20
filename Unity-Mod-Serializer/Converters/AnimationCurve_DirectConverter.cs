using System;
using System.Collections.Generic;
using UnityEngine;

namespace UMS.Converters
{
    partial class ConverterRegistrar
    {
#pragma warning disable 0649
        public static AnimationCurve_DirectConverter Register_AnimationCurve_DirectConverter;
#pragma warning restore
    }
    public class AnimationCurve_DirectConverter : DirectConverter<AnimationCurve>
    {
        protected override Result DoSerialize(AnimationCurve model, Dictionary<string, Data> serialized)
        {
            var result = Result.Success;

            result += SerializeMember(serialized, null, "keys", model.keys);
            result += SerializeMember(serialized, null, "preWrapMode", model.preWrapMode);
            result += SerializeMember(serialized, null, "postWrapMode", model.postWrapMode);

            return result;
        }

        protected override Result DoDeserialize(Dictionary<string, Data> data, ref AnimationCurve model)
        {
            var result = Result.Success;

            var t0 = model.keys;
            result += DeserializeMember(data, null, "keys", out t0);
            model.keys = t0;

            var t1 = model.preWrapMode;
            result += DeserializeMember(data, null, "preWrapMode", out t1);
            model.preWrapMode = t1;

            var t2 = model.postWrapMode;
            result += DeserializeMember(data, null, "postWrapMode", out t2);
            model.postWrapMode = t2;

            return result;
        }

        public override object CreateInstance(Data data, Type storageType)
        {
            return new AnimationCurve();
        }
    }
}