using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ProtoBuf.Meta;
using FastMember;

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
            
            public List<MemberValue> Serialize(object obj)
            {
                List<MemberValue> toReturn = new List<MemberValue>();

                foreach (Member member in _serializableMembers)
                {
                    object value = _accessor[obj, member.Name];

                    if (value == null)
                        continue;

                    toReturn.Add(new MemberValue(member.Hashcode, value.GetType(), Serializer.Serialize(value)));
                }

                return toReturn;
            }
            public void Deserialize(object obj, List<MemberValue> data)
            {
                foreach (MemberValue value in data)
                {
                    if (!_nameLookup.ContainsKey(value.MemberID))
                    {
                        Debug.LogWarning($"Couldn't find lookup for {value.MemberID} - {obj.GetType()}");
                        return;
                    }
                    
                    _accessor[obj, _nameLookup[value.MemberID]] = Serializer.Deserialize(value.Data, AssemblyManager.GetType(value.TypeName));
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
        public static List<MemberValue> Serialize(object obj)
        {
#if DEBUG
            if (obj == null)
                throw new NullReferenceException("Object is null");
#endif

            return GetSerializationGraph(obj.GetType()).Serialize(obj);
        }
        public static void Deserialize(object obj, List<MemberValue> data)
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
            if (MemberBlockerAttribute.IsBlocked(member))
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
    }
}
