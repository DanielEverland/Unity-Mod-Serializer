using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UMS.Converters;

namespace UMS
{
    /// <summary>
    /// The serialization converter allows for customization of the serialization
    /// process.
    /// </summary>
    /// <remarks>
    /// You do not want to derive from this class - there is no way to actually
    /// use it within the serializer.. Instead, derive from either Converter or
    /// DirectConverter
    /// </remarks>
    public abstract class BaseConverter
    {
        private readonly BindingFlags _memberBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        /// The serializer that was owns this converter.
        /// </summary>
        public Serializer Serializer;

        /// <summary>
        /// Construct an object instance that will be passed to TryDeserialize.
        /// This should **not** deserialize the object.
        /// </summary>
        /// <param name="data">The data the object was serialized with.</param>
        /// <param name="storageType">
        /// The field/property type that is storing the instance.
        /// </param>
        /// <returns>An object instance</returns>
        public virtual object CreateInstance(Data data, Type storageType)
        {
            if (RequestCycleSupport(storageType))
            {
                throw new InvalidOperationException("Please override CreateInstance for " +
                    GetType().FullName + "; the object graph for " + storageType +
                    " can contain potentially contain cycles, so separated instance creation " +
                    "is needed");
            }

            return storageType;
        }

        /// <summary>
        /// If true, then the serializer will support cyclic references with the
        /// given converted type.
        /// </summary>
        /// <param name="storageType">
        /// The field/property type that is currently storing the object that is
        /// being serialized.
        /// </param>
        public virtual bool RequestCycleSupport(Type storageType)
        {
            if (storageType == typeof(string)) return false;

            return storageType.Resolve().IsClass || storageType.Resolve().IsInterface;
        }

        /// <summary>
        /// If true, then the serializer will include inheritance data for the
        /// given converter.
        /// </summary>
        /// <param name="storageType">
        /// The field/property type that is currently storing the object that is
        /// being serialized.
        /// </param>
        public virtual bool RequestInheritanceSupport(Type storageType)
        {
            return storageType.Resolve().IsSealed == false;
        }

        /// <summary>
        /// Serialize the actual object into the given data storage.
        /// </summary>
        /// <param name="instance">
        /// The object instance to serialize. This will never be null.
        /// </param>
        /// <param name="serialized">The serialized state.</param>
        /// <param name="storageType">
        /// The field/property type that is storing this instance.
        /// </param>
        /// <returns>If serialization was successful.</returns>
        public abstract Result TrySerialize(object instance, out Data serialized, Type storageType);

        /// <summary>
        /// Deserialize data into the object instance.
        /// </summary>
        /// <param name="input">Serialization data to deserialize from.</param>
        /// <param name="instance">
        /// The object instance to deserialize into.
        /// </param>
        /// <param name="storageType">
        /// The field/property type that is storing the instance.
        /// </param>
        /// <returns>
        /// True if serialization was successful, false otherwise.
        /// </returns>
        public abstract Result TryDeserialize(Data input, ref object instance, Type storageType);

        protected Result FailExpectedType(Data data, params DataType[] types)
        {
            return Result.Fail(GetType().Name + " expected one of " +
                string.Join(", ", types.Select(t => t.ToString()).ToArray()) +
                " but got " + data.Type + " in " + data);
        }

        protected Result CheckType(Data data, DataType type)
        {
            if (data.Type != type)
            {
                return Result.Fail(GetType().Name + " expected " + type + " but got " + data.Type + " in " + data);
            }
            return Result.Success;
        }

        protected Result CheckKey(Data data, string key, out Data subitem)
        {
            return CheckKey(data.AsDictionary, key, out subitem);
        }

        protected Result CheckKey(Dictionary<string, Data> data, string key, out Data subitem)
        {
            if (data.TryGetValue(key, out subitem) == false)
            {
                return Result.Fail(GetType().Name + " requires a <" + key + "> key in the data " + data);
            }
            return Result.Success;
        }

        protected Result SerializeMembers(Dictionary<string, Data> data, object instance, IEnumerable<string> memberNames)
        {
            Result result = Result.Success;

            Type instanceType = instance.GetType();

            foreach (string memberName in memberNames)
            {
                MemberInfo[] members = instanceType.GetMember(memberName, _memberBindingFlags);

                if (members.Length == 0)
                    return Result.Fail("Cannot find member " + memberName + " on " + instanceType);

                foreach (MemberInfo member in members)
                {
                    switch (member.MemberType)
                    {
                        case MemberTypes.Field:
                            result += SerializeField(data, instance, member as FieldInfo);
                            break;
                        case MemberTypes.Property:
                            result += SerializeProperty(data, instance, member as PropertyInfo);
                            break;
                    }
                }
            }

            return result;
        }

        private Result SerializeField(Dictionary<string, Data> data, object instance, FieldInfo field)
        {
            object fieldValue = field.GetValue(instance);
            
            Result result = Serializer.TrySerialize(field.FieldType, null, fieldValue, out Data memberData);
            data.Set(field.Name, memberData);

            return result;
        }

        private Result SerializeProperty(Dictionary<string, Data> data, object instance, PropertyInfo property)
        {
            if (property.GetMethod == null)
            {
#if DEBUG
                return Result.Warn("No get method for " + property + " on " + instance.GetType() + " - skipping!");
#else
                return Result.Success;
#endif
            }

            object fieldValue = property.GetValue(instance);

            Result result = Serializer.TrySerialize(property.PropertyType, null, fieldValue, out Data memberData);
            data.Set(property.Name, memberData);

            return result;
        }
        
        protected Result SerializeMember<T>(Dictionary<string, Data> data, Type overrideConverterType, string name, T value)
        {
            Data memberData;
            var result = Serializer.TrySerialize(typeof(T), overrideConverterType, value, out memberData);
            if (result.Succeeded) data[name] = memberData;
            return result;
        }

        protected Result DeserializeMembers(Dictionary<string, Data> data, object instance, IEnumerable<string> memberNames)
        {
            Result result = Result.Success;

            Type instanceType = instance.GetType();

            foreach (string memberName in memberNames)
            {
                if (!data.ContainsKey(memberName))
                {
                    return Result.Fail("Data doesn't contain " + memberName);
                }

                Data memberData = data[memberName];

                MemberInfo[] members = instanceType.GetMember(memberName, _memberBindingFlags);

                if (members.Length == 0)
                    return Result.Fail("Cannot find member " + memberName + " on " + instanceType);

                foreach (MemberInfo member in members)
                {
                    switch (member.MemberType)
                    {
                        case MemberTypes.Field:
                            result += DeserializeField(memberData, instance, member as FieldInfo);
                            break;
                        case MemberTypes.Property:
                            result += DeserializeProperty(memberData, instance, member as PropertyInfo);
                            break;
                    }
                }
            }

            return result;
        }

        private Result DeserializeField(Data data, object instance, FieldInfo field)
        {
            object deserialized = null;
            Result result = Serializer.TryDeserialize(data, field.FieldType, null, ref deserialized);
            
            if(deserialized != null)
            {
                field.SetValue(instance, deserialized);
            }

            return result;
        }

        private Result DeserializeProperty(Data data, object instance, PropertyInfo property)
        {
            if (property.SetMethod == null)
            {
#if DEBUG
                return Result.Warn("No set method for " + property + " on " + instance.GetType() + " - skipping!");
#else
                return Result.Success;
#endif
            }

            object deserialized = null;
            Result result = Serializer.TryDeserialize(data, property.PropertyType, null, ref deserialized);

            if (deserialized != null)
            {
                property.SetValue(instance, deserialized);
            }

            return result;
        }

        protected Result DeserializeMember<T>(Dictionary<string, Data> data, Type overrideConverterType, string name, out T value)
        {
            Data memberData;
            if (data.TryGetValue(name, out memberData) == false)
            {
                value = default(T);
                return Result.Fail("Unable to find member \"" + name + "\"");
            }

            object storage = null;
            var result = Serializer.TryDeserialize(memberData, typeof(T), overrideConverterType, ref storage);
            value = (T)storage;
            return result;
        }
    }
}