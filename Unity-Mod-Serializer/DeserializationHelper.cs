using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UMS
{
    /// <summary>
    /// Includes some helper methods to make generic deserialization easier
    /// </summary>
    public static class DeserializationHelper
    {
        /// <summary>
        /// Deserializes a dictionary into an object using reflection
        /// </summary>
        public static Result TryDeserializeDictionary(Dictionary<string, Data> dictionary, ref object instance, Type storageType)
        {
            foreach (KeyValuePair<string, Data> keyvaluePair in dictionary)
            {
                string memberName = keyvaluePair.Key;
                Data memberValue = keyvaluePair.Value;

                Result result = TryDeserializeMember(memberName, memberValue, storageType, instance);

                if (result.Failed)
                    return result;
            }

            return Result.Success;
        }
        private static Result TryDeserializeMember(string memberName, Data memberValue, Type containerType, object containerInstance)
        {
            MemberInfo[] members = containerType.GetMember(memberName);

            foreach (MemberInfo info in members)
            {
                Result result = default(Result);

                switch (info.MemberType)
                {
                    case MemberTypes.Field:
                        result = TryDeserializeAsField(info as FieldInfo, memberValue, containerType, containerInstance);
                        break;
                    case MemberTypes.Property:
                        result = TryDeserializeAsProperty(info as PropertyInfo, memberValue, containerType, containerInstance);
                        break;
                }

                if (result.Failed)
                    return result;
            }

            return Result.Success;
        }
        private static Result TryDeserializeAsField(FieldInfo field, Data memberValue, Type containerType, object containerInstance)
        {
            if (field == null)
                return Result.Fail("Field is null");

            object deserialized = null;
            Mods.Serializer.TryDeserialize(memberValue, field.FieldType, ref deserialized);

            if (deserialized == null)
                return Result.Warn("Deserialized object is null");

            field.SetValue(containerInstance, deserialized);
            return Result.Success;
        }
        private static Result TryDeserializeAsProperty(PropertyInfo property, Data memberValue, Type containerType, object containerInstance)
        {
            if (property == null)
                return Result.Fail("Property is null");

            if (property.SetMethod == null)
                return Result.Warn("Property " + property + " doesn't have a setter");

            object deserialized = null;
            Mods.Serializer.TryDeserialize(memberValue, property.PropertyType, ref deserialized);

            if (deserialized == null)
                return Result.Warn("Deserialized object is null");

            property.SetValue(containerInstance, deserialized);
            return Result.Success;
        }
    }
}
