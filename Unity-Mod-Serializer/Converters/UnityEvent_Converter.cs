using System;
using UnityEngine;
using UnityEngine.Events;

namespace UMS.Converters
{
    // The standard reflection converter has started causing Unity to crash
    // when processing UnityEvent. We can send the serialization through
    // JsonUtility which appears to work correctly instead.
    //
    // We have to support legacy serialization formats so importing works as
    // expected.

    // Heyoo it's ya-boi Daniel here. Not sure if this still applies, but I'm
    // switching to a reflection scheme in order to register converters. For
    // the time being I'm not going to disable this converter since we're
    // using a much newer version of Unity. If this starts causing issues,
    // add an [Ignore] attribute to the class
    public class UnityEvent_Converter : Converter<UnityEvent>
    {
        public override bool RequestCycleSupport(Type storageType)
        {
            return false;
        }

        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            Type objectType = (Type)instance;

            Result result = Result.Success;
            instance = JsonUtility.FromJson(JsonPrinter.CompressedJson(data), objectType);
            return result;
        }

        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            Result result = Result.Success;
            serialized = JsonParser.Parse(JsonUtility.ToJson(instance));
            return result;
        }
    }
}