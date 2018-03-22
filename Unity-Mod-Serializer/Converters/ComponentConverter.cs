using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using UMS.Reflection;
using UnityEngine;

namespace UMS.Converters
{
    public partial class ConverterRegistrar
    {
        public static ComponentConverter Register_ComponentConverter;
    }
    public class ComponentConverter : Converter
    {
        public static Component CurrentComponent { get; set; }

        public override bool CanProcess(Type type)
        {
            return typeof(Component).IsAssignableFrom(type);
        }

        public override object CreateInstance(Data data, Type storageType)
        {
            return CurrentComponent;
        }

        public override Result TryDeserialize(Data input, ref object instance, Type storageType)
        {
            if (!input.IsDictionary)
                return Result.Fail("Input data is not dictionary");

            Dictionary<string, Data> dictionary = input.AsDictionary;

            foreach (KeyValuePair<string, Data> keyvaluePair in dictionary)
            {
                string memberName = keyvaluePair.Key;
                Data memberValue = keyvaluePair.Value;

                if(!TryDeserializeMember(memberName, memberValue, storageType, instance))
                {
                    Result.Warn("Couldn't deserialize member " + memberName + " with data value " + memberValue);
                }
            }

            return Result.Success;
        }
        private bool TryDeserializeMember(string memberName, Data memberValue, Type containerType, object containerInstance)
        {
            if (TryDeserializeAsField(memberName, memberValue, containerType, containerInstance))
                return true;

            if (TryDeserializeAsProperty(memberName, memberValue, containerType, containerInstance))
                return true;

            return false;
        }
        private bool TryDeserializeAsField(string memberName, Data memberValue, Type containerType, object containerInstance)
        {
            FieldInfo field = containerType.GetField(memberName);

            if (field == null)
                return false;

            object deserialized = null;
            Serializer.TryDeserialize(memberValue, field.FieldType, ref deserialized);

            if (deserialized == null)
                return false;

            field.SetValue(containerInstance, deserialized);
            return true;
        }
        private bool TryDeserializeAsProperty(string memberName, Data memberValue, Type containerType, object containerInstance)
        {
            PropertyInfo property = containerType.GetProperty(memberName);

            if (property == null)
                return false;

            if (property.SetMethod == null)
                return false;

            object deserialized = null;
            Serializer.TryDeserialize(memberValue, property.PropertyType, ref deserialized);

            if (deserialized == null)
                return false;

            property.SetValue(containerInstance, deserialized);
            return true;
        }

        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            Dictionary<string, Data> dictionary = new Dictionary<string, Data>();
            Component component = (Component)instance;

            foreach (MemberInfo member in instance.GetType().GetMembers(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public))
            {
                Data memberData = null;

                switch (member.MemberType)
                {
                    case MemberTypes.Field:
                        PollField(member as FieldInfo, instance, ref memberData);
                        break;
                    case MemberTypes.Property:
                        PollProperty(member as PropertyInfo, instance, ref memberData);
                        break;
                }

                if(memberData != null)
                {
                    dictionary.Add(member.Name, memberData);
                }
            }

            serialized = new Data(dictionary);

            return Result.Success;
        }
        private void PollField(FieldInfo field, object instance, ref Data data)
        {
            Serializer.TrySerialize(field.FieldType, field.GetValue(instance), out data);
        }
        private void PollProperty(PropertyInfo property, object instance, ref Data data)
        {
            if(property.CanRead && property.CanWrite)
            {
                Serializer.TrySerialize(property.PropertyType, property.GetValue(instance, null), out data);
            }
        }
    }
}
