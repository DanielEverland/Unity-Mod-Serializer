using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProtoBuf.Meta;
using FastMember;
using UMS.Wrappers;

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

        /// <summary>
        /// Serialization graphs can quickly complete serialization for any given type
        /// </summary>
        private static Dictionary<Type, SerializationGraph> _serializationGraphs = new Dictionary<Type, SerializationGraph>();

        /// <summary>
        /// We only serialize the hashcode of member names, so we need to convert those during deserialization
        /// </summary>
        private static Dictionary<int, string> _nameLookup = new Dictionary<int, string>();

        private class SerializationGraph
        {
            public SerializationGraph(Type type)
            {
                _accessor = TypeAccessor.Create(type);

                foreach (MemberInfo member in type.GetMembers(_memberFlags))
                {
                    if (!ShouldSerialize(member))
                        continue;
                                        
                    if(member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field)
                    {
                        _serializableMembers.Add(Member.Create(member.Name));
                    }
                }
            }

            /// <summary>
            /// Contains all the members we want to serialize in a given type
            /// </summary>
            private List<Member> _serializableMembers = new List<Member>();

            /// <summary>
            /// FastMember type accessor. Makes reading fields and properties insanely fast
            /// </summary>
            private TypeAccessor _accessor;
            
            public Dictionary<int, object> Serialize(object obj)
            {
                Dictionary<int, object> toReturn = new Dictionary<int, object>();

                foreach (Member member in _serializableMembers)
                {
                    toReturn.Add(member.Hashcode, WrapperManager.Process(_accessor[obj, member.Name]));
                }

                return toReturn;
            }
            [ProtoBuf.ProtoContract]
            private class BooleanWrapper
            {
                [ProtoBuf.ProtoMember(1)]
                public bool _value;
            }
            public void Deserialize(object obj, Dictionary<int, object> data)
            {
                foreach (KeyValuePair<int, object> pair in data)
                {
#if DEBUG
                    if (!_nameLookup.ContainsKey(pair.Key))
                        Debug.LogWarning("Couldn't find lookup for " + pair.Key + " - " + obj.GetType());
#endif

                    _accessor[obj, _nameLookup[pair.Key]] = pair.Value;
                }
            }

            private struct Member
            {
                public string Name;
                public int Hashcode;

                public static Member Create(string name)
                {
                    Member toReturn = new Member()
                    {
                        Name = name,
                        Hashcode = name.GetHashCode(),
                    };

                    if (!_nameLookup.ContainsKey(toReturn.Hashcode))
                    {
                        _nameLookup.Add(toReturn.Hashcode, toReturn.Name);
                    }

                    return toReturn;
                }
            }
        }

        private static SerializationGraph GetSerializationGraph(Type type)
        {
            if (!_serializationGraphs.ContainsKey(type))
            {
                _serializationGraphs.Add(type, new SerializationGraph(type));
            }

            return _serializationGraphs[type];
        }
        public static Dictionary<int, object> Serialize(object obj)
        {
#if DEBUG
            if (obj == null)
                throw new NullReferenceException("Object is null");
#endif

            return GetSerializationGraph(obj.GetType()).Serialize(obj);
        }
        public static void Deserialize(object obj, Dictionary<int, object> data)
        {
#if DEBUG
            if (obj == null)
                throw new NullReferenceException("Object is null");

            if (data == null)
                throw new NullReferenceException("Data is null");
#endif

            GetSerializationGraph(obj.GetType()).Deserialize(obj, data);
        }

        public static bool ShouldSerialize(MemberInfo member)
        {
            if (IsIgnored(member) || MemberBlockerAttribute.IsBlocked(member))
                return false;

            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ShouldSerializeField(member as FieldInfo);
                case MemberTypes.Property:
                    return ShouldSerializeProperty(member as PropertyInfo);
            }

            return true;
        }
        public static bool ShouldSerializeProperty(PropertyInfo property)
        {
            MethodInfo getMethod = property.GetGetMethod();

            if (getMethod == null)
                return false;

            if (property.SetMethod == null)
                return false;

            return getMethod.IsPublic;
        }
        public static bool ShouldSerializeField(FieldInfo field)
        {
            if (!field.IsPublic)
            {
                return field.GetCustomAttributes().Any(x => x.GetType() == typeof(SerializeField));
            }

            return true;
        }
        public static bool IsIgnored(MemberInfo member)
        {
            return member.GetCustomAttribute(typeof(IgnoreAttribute)) != null;
        }

        #region Deprecated Shit
        public static void CreateMetaType(MetaType meta, System.Type type)
        {
            foreach (MemberInfo member in type.GetMembers(_memberFlags))
            {
                if (!ShouldSerialize(member))
                    continue;
                
                if(member.MemberType == (MemberTypes.Property | MemberTypes.Field))
                {
                    Debug.Log("Adding " + member);
                    meta.Add(member.Name);
                }
            }
        }
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

            data = new Data();
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
                if (!ShouldSerialize(member))
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

            if (ShouldSerializeProperty(property))
            {
                result += SerializeProperty(property, obj, out Data objectData);
                if (result.Succeeded)
                {
                    data[property.Name] = objectData;
                }
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
            data = new Data();
            
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

            object propertyValue = property.GetValue(obj);

            if(propertyValue != null)
            {
                result += Serializer.Serialize(propertyValue, out data);

                Debugging.Verbose(DebuggingFlags.Reflection, string.Format("Serialized property {0} as {1} from {2}", property, data, obj.GetType().Name));
            }            

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
            data = new Data();

            if (obj == null)
                return Result.Error("Object is null " + field.DeclaringType);

            if (field == null)
                return Result.Error("Property is null " + obj);

            if(IsIgnored(field))
                return Result.Error("Tried to serialize a field with the Ignore attribute " + field);

            object fieldValue = field.GetValue(obj);

            if(fieldValue != null)
            {
                result += Serializer.Serialize(fieldValue, out data);

                Debugging.Verbose(DebuggingFlags.Reflection, string.Format("Serialized field {0} as {1} from {2}", field, data, obj.GetType().Name));
            }            

            return result;
        }
        public static Result DeserializeObject(Data data, ref object deserializedObject)
        {
            Result result = Result.Success;

            if(!data.IsDictionary)
                return Result.Error("Type mismatch. Expected dictionary", data);

            System.Type type = data.GetMetaData<TypeMetaData>().Type;
            result += DeserializeObject(data, type, ref deserializedObject);

            return result;
        }
        public static Result DeserializeObject(Data data, System.Type type, ref object deserializedObject)
        {
            if (type == null)
                return Result.Error("Type is null!");

            if (!data.IsDictionary)
                return Result.Error("Type mismatch. Expected dictionary", data);

            Debugging.Warning(DebuggingFlags.Reflection, "Deserializing " + data + " using reflection!");

            Result result = Result.Success;

            foreach (KeyValuePair<string, Data> keyValuePair in data.Dictionary)
            {
                if (keyValuePair.Value.IsNull)
                    continue;

                MemberInfo member = GetMember(keyValuePair.Key, type);

                if (member == null)
                    continue;

                Debugging.Verbose(DebuggingFlags.Reflection, "Deserializing member " + keyValuePair.Key + " - " + keyValuePair.Value);

                result += DeserializeMember(member, keyValuePair.Value, type, ref deserializedObject);
            }

            return result;
        }
        public static Result DeserializeMember(MemberInfo member, Data data, System.Type type, ref object deserializedObject)
        {
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

            if (data.IsNull)
                return Result.Error("Trying to deserialize null data", data);

            if (property == null)
                return Result.Error("Property is null " + deserializedObject);

            if (deserializedObject == null)
                return Result.Error(string.Format("Object is null {0} ({1})", property, property.DeclaringType));
            
            if (property.GetGetMethod() == null)
                result += Result.Warn("Getter isn't defined on " + property);

            if (property.GetSetMethod() == null)
                return Result.Error("Setter isn't defined on " + property);
                        
            object deserializedData = null;
            result += Serializer.Deserialize(data, property.PropertyType, ref deserializedData);

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

            if (data.IsNull)
                return Result.Error("Trying to deserialize null data", data);

            if (field == null)
                return Result.Error("Property is null " + deserializedObject);

            if (deserializedObject == null)
                return Result.Error(string.Format("Object is null {0} ({1})", field, field.DeclaringType));
            
            object deserializedData = null;
            result += Serializer.Deserialize(data, field.FieldType, ref deserializedData);

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
            if (type == null)
                throw new System.NullReferenceException("Type is null!");

            MemberInfo[] members = type.GetMember(memberName, _memberFlags).Where(x => x.MemberType == MemberTypes.Field || x.MemberType == MemberTypes.Property).ToArray();
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
        
        
        
        #endregion
    }
}
