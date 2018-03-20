using System;
using UnityEngine;
using UnityEngine.Events;

namespace UMS.Converters
{
    partial class ConverterRegistrar
    {
        // Disable the converter for the time being. Unity's JsonUtility API
        // cannot be called from within a C# ISerializationCallbackReceiver
        // callback.

        // public static Internal.Converters.UnityEvent_Converter
        // Register_UnityEvent_Converter;
    }
    // The standard reflection converter has started causing Unity to crash
    // when processing UnityEvent. We can send the serialization through
    // JsonUtility which appears to work correctly instead.
    //
    // We have to support legacy serialization formats so importing works as
    // expected.
    public class UnityEvent_Converter : Converter
    {
        public override bool CanProcess(Type type)
        {
            return typeof(UnityEvent).Resolve().IsAssignableFrom(type.Resolve()) && type.IsGenericType() == false;
        }

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