using System;
using System.Collections;
using UMS.Reflection;

#if !UNITY_EDITOR && UNITY_WSA
// For System.Reflection.TypeExtensions
using System.Reflection;
#endif

namespace UMS.Converters
{
    public class ReflectedConverter : Converter<ICollection>
    {
        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            serialized = Data.CreateDictionary();
            var result = Result.Success;

            MetaType metaType = MetaType.Get(Serializer.Config, instance.GetType());
            metaType.EmitAotData(/*throwException:*/ false);

            for (int i = 0; i < metaType.Properties.Length; ++i)
            {
                MetaProperty property = metaType.Properties[i];
                if (property.CanRead == false) continue;

                Data serializedData;

                var itemResult = Serializer.TrySerialize(property.StorageType, property.OverrideConverterType,
                                                         property.Read(instance), out serializedData);
                result.AddMessages(itemResult);
                if (itemResult.Failed)
                {
                    continue;
                }

                serialized.AsDictionary[property.JsonName] = serializedData;
            }

            return result;
        }

        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            var result = Result.Success;

            // Verify that we actually have an Object
            if ((result += CheckType(data, DataType.Object)).Failed)
            {
                return result;
            }

            MetaType metaType = MetaType.Get(Serializer.Config, storageType);
            metaType.EmitAotData(/*throwException:*/ false);

            for (int i = 0; i < metaType.Properties.Length; ++i)
            {
                MetaProperty property = metaType.Properties[i];
                if (property.CanWrite == false) continue;

                Data propertyData;
                if (data.AsDictionary.TryGetValue(property.JsonName, out propertyData))
                {
                    object deserializedValue = null;

                    // We have to read in the existing value, since we need to
                    // support partial deserialization. However, this is bad for
                    // perf.
                    // TODO: Find a way to avoid this call when we are not doing
                    //       a partial deserialization Maybe through a new
                    //       property, ie, Serializer.IsPartialSerialization,
                    //       which just gets set when starting a new
                    //       serialization? We cannot pipe the information
                    //       through CreateInstance unfortunately.
                    if (property.CanRead)
                    {
                        deserializedValue = property.Read(instance);
                    }

                    var itemResult = Serializer.TryDeserialize(propertyData, property.StorageType,
                                                               property.OverrideConverterType, ref deserializedValue);
                    result.AddMessages(itemResult);
                    if (itemResult.Failed) continue;

                    property.Write(instance, deserializedValue);
                }
            }

            return result;
        }

        public override object CreateInstance(Data data, Type storageType)
        {
            MetaType metaType = MetaType.Get(Serializer.Config, storageType);
            return metaType.CreateInstance();
        }
    }
}