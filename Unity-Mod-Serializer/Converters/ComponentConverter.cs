using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using UMS.MemberBlockers;
using UnityEngine;

namespace UMS.Converters
{
    public class ComponentConverter : Converter<Component>
    {
        public static Component CurrentComponent { get; set; }
        
        public override object CreateInstance(Data data, Type storageType)
        {
            return CurrentComponent;
        }

        public override Result TryDeserialize(Data input, ref object instance, Type storageType)
        {
            if (!input.IsDictionary)
                return Result.Fail("Input data is not dictionary");
            
            DeserializationHelper.TryDeserializeDictionary(input.AsDictionary, ref instance, storageType);
            
            return Result.Success;
        }
        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            Dictionary<string, Data> dictionary = new Dictionary<string, Data>();
            Component component = (Component)instance;

            foreach (MemberInfo member in instance.GetType().GetMembers(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (MemberBlockerAttribute.IsBlocked(member))
                    continue;

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
