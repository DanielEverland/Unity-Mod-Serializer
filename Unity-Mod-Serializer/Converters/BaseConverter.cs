using System.Collections.Generic;
using System.Reflection;

namespace UMS.Converters
{
    /// <summary>
    /// Converters converts object from memory into a serializable data format.
    /// </summary>
    public abstract class BaseConverter<T> : IBaseConverter
    {
        private static readonly BindingFlags _memberBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        /// The type this converter supports
        /// </summary>
        public System.Type ModelType { get { return typeof(T); } }
        
        public abstract Result DoSerialize(T obj, out Data data);
        public abstract Result DoDeserialize(Data data, ref T obj);

        public virtual object CreateInstance(System.Type type)
        {
            try
            {
                return System.Activator.CreateInstance(type);
            }
            catch (System.Exception)
            {
                throw new System.NotImplementedException("Please implement CreateInstance on " + GetType().Name);
            };
        }
        public Result Serialize(object obj, out Data data)
        {
            return DoSerialize((T)obj, out data);
        }
        public Result Deserialize(Data data, ref object obj)
        {
            System.Type objType = obj.GetType();

            if (!typeof(T).IsAssignableFrom(objType))
                throw new System.ArgumentException(string.Format("Type mismatch. Cannot deserialize ({0}) using Converter type ({1})", objType, typeof(T)));

            T outObject = (T)obj;
            Result result = DoDeserialize(data, ref outObject);

            obj = outObject;
            return result;
        }
        protected Result SerializeMembers(Data data, object instance, IEnumerable<string> memberNames)
        {
            if (!data.IsDictionary)
                return Result.Error("Type mismatch. Expected Dictionary", data);

            Result result = Result.Success;

            System.Type instanceType = instance.GetType();

            foreach (string memberName in memberNames)
            {
                MemberInfo[] members = instanceType.GetMember(memberName, _memberBindingFlags);

                if (members.Length == 0)
                    return Result.Error("Cannot find member " + memberName + " on " + instanceType);

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

        private Result SerializeField(Data data, object instance, FieldInfo field)
        {
            object fieldValue = field.GetValue(instance);

            Result result = Serializer.Serialize(fieldValue, out Data memberData);
            data.Dictionary.Set(field.Name, memberData);

            return result;
        }

        private Result SerializeProperty(Data data, object instance, PropertyInfo property)
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

            Result result = Serializer.Serialize(fieldValue, out Data memberData);
            data.Dictionary.Set(property.Name, memberData);

            return result;
        }
        protected Result DeserializeMembers(Data data, object instance, IEnumerable<string> memberNames)
        {
            Result result = Result.Success;

            System.Type instanceType = instance.GetType();

            foreach (string memberName in memberNames)
            {
                if (!data.Dictionary.ContainsKey(memberName))
                {
                    return Result.Error("Data doesn't contain " + memberName);
                }

                Data memberData = data[memberName];

                MemberInfo[] members = instanceType.GetMember(memberName, _memberBindingFlags);

                if (members.Length == 0)
                    return Result.Error("Cannot find member " + memberName + " on " + instanceType);

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
            Result result = Serializer.Deserialize(data, field.FieldType, ref deserialized);

            if (deserialized != null)
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
            Result result = Serializer.Deserialize(data, property.PropertyType, ref deserialized);

            if (deserialized != null)
            {
                property.SetValue(instance, deserialized);
            }

            return result;
        }
    }
}
