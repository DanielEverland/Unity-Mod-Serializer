using System;
using UMS.Reflection;

namespace UMS.Converters
{
    /// <summary>
    /// This allows you to forward serialization of an object to one of its
    /// members. For example,
    ///
    /// [Forward("Values")]
    /// struct Wrapper {
    ///     public int[] Values;
    /// }
    ///
    /// Then `Wrapper` will be serialized into a JSON array of integers. It will
    /// be as if `Wrapper` doesn't exist.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public sealed class ForwardAttributeAttribute : Attribute
    {
        /// <summary>
        /// The name of the member we should serialize as.
        /// </summary>
        public string MemberName;

        /// <summary>
        /// Forward object serialization to an instance member. See class
        /// comment.
        /// </summary>
        /// <param name="memberName">
        /// The name of the member that we should serialize this object as.
        /// </param>
        public ForwardAttributeAttribute(string memberName)
        {
            MemberName = memberName;
        }
    }
    [Ignore]
    public sealed class ForwardConverter : BaseConverter
    {
        private string _memberName;

        public ForwardConverter(ForwardAttributeAttribute attribute)
        {
            _memberName = attribute.MemberName;
        }

        private Result GetProperty(object instance, out MetaProperty property)
        {
            var properties = MetaType.Get(Serializer.Config, instance.GetType()).Properties;
            for (int i = 0; i < properties.Length; ++i)
            {
                if (properties[i].MemberName == _memberName)
                {
                    property = properties[i];
                    return Result.Success;
                }
            }

            property = default(MetaProperty);
            return Result.Fail("No property named \"" + _memberName + "\" on " + instance.GetType().CSharpName());
        }

        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            serialized = Data.Null;
            var result = Result.Success;

            MetaProperty property;
            if ((result += GetProperty(instance, out property)).Failed) return result;

            var actualInstance = property.Read(instance);
            return Serializer.TrySerialize(property.StorageType, actualInstance, out serialized);
        }

        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            var result = Result.Success;

            MetaProperty property;
            if ((result += GetProperty(instance, out property)).Failed) return result;

            object actualInstance = null;
            if ((result += Serializer.TryDeserialize(data, property.StorageType, ref actualInstance)).Failed)
                return result;

            property.Write(instance, actualInstance);
            return result;
        }

        public override object CreateInstance(Data data, Type storageType)
        {
            return MetaType.Get(Serializer.Config, storageType).CreateInstance();
        }
    }
}