using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Reflection
{
    /// <summary>
    /// Helper method for using reflection. It's recommended to
    /// use this when serializing, since it includes functionality
    /// like checking for Ignore attributes, and since we never serialize
    /// during runtime of a game, we can afford the poor performance.
    /// </summary>
    public static class ReflectionHelper
    {
        private static BindingFlags _memberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
        public static Result SerializeMember(string memberName, object obj, out Data data)
        {
            Result result = Result.Success;
            
            result += SerializeMember(GetMember(memberName, obj.GetType()), obj, out data);

            return result;
        }
        public static Result SerializeMember(MemberInfo member, object obj, out Data data)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return SerializeField(member as FieldInfo, obj, out data);
                case MemberTypes.Property:
                    return SerializeProperty(member as PropertyInfo, obj, out data);                
            }

            data = Data.Null;
            return Result.Error("Cannot serialize " + member.MemberType);
        }
        /// <summary>
        /// Serializes an entire objects member into a Data instance
        /// </summary>
        public static Result SerializeObject(object obj, out Data data)
        {
            Result result = Result.Success;

            Debugging.Info(DebuggingFlags.Reflection, "Serializing " + obj + " using reflection");

            data = new Data(new Dictionary<string, Data>());            
            foreach (MemberInfo member in obj.GetType().GetMembers(_memberFlags))
            {
                if (IsIgnored(member))
                    continue;

                switch (member.MemberType)
                {
                    case MemberTypes.Field:
                        SerializeFieldIntoDictionary(member as FieldInfo, obj, data);
                        break;
                    case MemberTypes.Property:
                        SerializePropertyIntoDictionary(member as PropertyInfo, obj, data);
                        break;
                }
            }

            return result;
        }
        /// <summary>
        /// Helper method for serializing a property into an existing dictionary
        /// </summary>
        /// <param name="data">The Data object containing a dictionary, which we add the serialized Data object to</param>
        public static Result SerializePropertyIntoDictionary(PropertyInfo property, object obj, Data data)
        {
            Result result = Result.Success;
            
            result += SerializeProperty(property, obj, out Data objectData);
            if (result.Succeeded)
            {
                data[property.Name] = objectData;
            }

            return result;
        }
        /// <summary>
        /// Helper method for serializing properties
        /// </summary>
        /// <param name="data">A Data object containing the properties value</param>
        public static Result SerializeProperty(PropertyInfo property, object obj, out Data data)
        {
            Result result = Result.Success;
            data = Data.Null;
            
            if (obj == null)
                return Result.Error("Object is null " + property.DeclaringType);

            if (property == null)
                return Result.Error("Property is null " + obj);

            if (property.GetGetMethod() == null)
                return Result.Error("Getter isn't defined on " + property);

            if (property.GetSetMethod() == null)
                return Result.Error("Setter isn't defined on " + property);

            if (IsIgnored(property))
                return Result.Error("Tried to serialize a property with the Ignore attribute " + property);

            Debugging.Verbose(DebuggingFlags.Reflection, "Serializing property " + property + " on " + obj + "\n" + data);

            result += Serializer.Serialize(property.GetValue(obj), out data);

            return result;
        }
        /// <summary>
        /// Helper method for serializing a field into an existing dictionary
        /// </summary>
        /// <param name="data">The Data object containing a dictionary, which we add the serialized Data object to</param>
        public static Result SerializeFieldIntoDictionary(FieldInfo field, object obj, Data data)
        {
            Result result = Result.Success;

            result += SerializeField(field, obj, out Data objectData);
            if (result.Succeeded)
            {
                data[field.Name] = objectData;
            }

            return result;
        }
        /// <summary>
        /// Helper method for serializing fields
        /// </summary>
        /// <param name="data">A Data object containing the fields value</param>
        public static Result SerializeField(FieldInfo field, object obj, out Data data)
        {
            Result result = Result.Success;
            data = Data.Null;

            if (obj == null)
                return Result.Error("Object is null " + field.DeclaringType);

            if (field == null)
                return Result.Error("Property is null " + obj);

            if(IsIgnored(field))
                return Result.Error("Tried to serialize a field with the Ignore attribute " + field);

            Debugging.Verbose(DebuggingFlags.Reflection, "Serializing field " + field + " on " + obj + "\n" + data);

            result += Serializer.Serialize(field.GetValue(obj), out data);

            return result;
        }
        public static Result DeserializeObject(Data data, ref object deserializedObject)
        {
            if (!data.IsDictioanry)
                return Result.Error("Type mismatch. Expected dictionary", data);

            Debugging.Warning(DebuggingFlags.Reflection, "Deserializing " + data + " using reflection!");

            Result result = Result.Success;

            foreach (KeyValuePair<string, Data> keyValuePair in data.AsDictionary)
            {
                result += DeserializeMemeber(keyValuePair.Key, keyValuePair.Value, ref deserializedObject);
            }

            return result;
        }
        public static Result DeserializeMemeber(string memberName, Data data, ref object deserializedObject)
        {
            MemberInfo member = GetMember(memberName, deserializedObject.GetType());

            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return DeserializeField(member as FieldInfo, data, ref deserializedObject);
                case MemberTypes.Property:
                    return DeserializeProperty(member as PropertyInfo, data, ref deserializedObject);
                default:
                    return Result.Error("Cannot deserialize " + member.MemberType);
            }
        }
        /// <summary>
        /// Helper method for deserializing properties
        /// </summary>
        public static Result DeserializeProperty(PropertyInfo property, Data data, ref object deserializedObject)
        {
            Result result = Result.Success;
            data = Data.Null;

            if (deserializedObject == null)
                return Result.Error("Object is null " + property.DeclaringType);

            if (property == null)
                return Result.Error("Property is null " + deserializedObject);

            if (property.GetGetMethod() == null)
                result += Result.Warn("Getter isn't defined on " + property);

            if (property.GetSetMethod() == null)
                return Result.Error("Setter isn't defined on " + property);
            
            object deserializedData = null;
            result += Serializer.Deserialize(data, property.DeclaringType, ref deserializedData);

            if (deserializedData == null)
                return Result.Error("Deserialized data is null!");

            Debugging.Verbose(DebuggingFlags.Reflection, string.Format("Deserializing {0} as property {1} into {2} from {3}", deserializedData, property, deserializedObject, data));

            property.SetValue(deserializedObject, deserializedData);

            return result;
        }
        /// <summary>
        /// Helper method for deserializing a Data object into a field
        /// </summary>
        public static Result DeserializeField(FieldInfo field, Data data, ref object deserializedObject)
        {
            Result result = Result.Success;

            if (deserializedObject == null)
                return Result.Error("Object is null " + field.DeclaringType);

            if (field == null)
                return Result.Error("Property is null " + deserializedObject);
            
            object deserializedData = null;
            result += Serializer.Deserialize(data, field.DeclaringType, ref deserializedData);

            if (deserializedData == null)
                return Result.Error("Deserialized data is null!");

            Debugging.Verbose(DebuggingFlags.Reflection, string.Format("Deserializing {0} as field {1} into {2} from {3}", deserializedData, field, deserializedObject, data));

            field.SetValue(deserializedObject, deserializedData);

            return result;
        }
        /// <summary>
        /// Helper method for grabbing 
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MemberInfo GetMember(string memberName, System.Type type)
        {
            MemberInfo[] members = type.GetMember(memberName).Where(x => x.MemberType == MemberTypes.Field || x.MemberType == MemberTypes.Property).ToArray();
            if (members.Length > 1)
            {
                throw new System.InvalidOperationException("Found more than one property or field with the name " + memberName + ". This shouldn't be possible");
            }
            else if (members.Length == 0)
            {
                throw new System.NullReferenceException("Couldn't find member " + memberName + " on " + type);
            }

            return members[0];
        }
        public static bool IsIgnored(MemberInfo member)
        {
            return member.GetCustomAttributes().Any(x => x.GetType() == typeof(IgnoreAttribute));
        }
    }
}
