using System;
using System.Collections.Generic;
using UnityEngine;

namespace UMS.Converters
{
    public sealed class Keyframe_DirectConverter : DirectConverter<Keyframe>
    {
        protected override Result DoSerialize(Keyframe model, Dictionary<string, Data> serialized)
        {
            var result = Result.Success;

            result += SerializeMember(serialized, null, "time", model.time);
            result += SerializeMember(serialized, null, "value", model.value);
            result += SerializeMember(serialized, null, "tangentMode", model.tangentMode);
            result += SerializeMember(serialized, null, "inTangent", model.inTangent);
            result += SerializeMember(serialized, null, "outTangent", model.outTangent);

            return result;
        }

        protected override Result DoDeserialize(Dictionary<string, Data> data, ref Keyframe model)
        {
            var result = Result.Success;

            var t0 = model.time;
            result += DeserializeMember(data, null, "time", out t0);
            model.time = t0;

            var t1 = model.value;
            result += DeserializeMember(data, null, "value", out t1);
            model.value = t1;

            var t2 = model.tangentMode;
            result += DeserializeMember(data, null, "tangentMode", out t2);
            model.tangentMode = t2;

            var t3 = model.inTangent;
            result += DeserializeMember(data, null, "inTangent", out t3);
            model.inTangent = t3;

            var t4 = model.outTangent;
            result += DeserializeMember(data, null, "outTangent", out t4);
            model.outTangent = t4;

            return result;
        }

        public override object CreateInstance(Data data, Type storageType)
        {
            return new Keyframe();
        }
    }
}