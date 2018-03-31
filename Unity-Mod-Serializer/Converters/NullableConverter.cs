using System;

namespace UMS.Converters
{
    /// <summary>
    /// The reflected converter will properly serialize nullable types. However,
    /// we do it here instead as we can emit less serialization data.
    /// </summary>
    public sealed class NullableConverter : Converter
    {
        public override Type ModelType => typeof(Nullable<>);

        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            // null is automatically serialized
            return Serializer.TrySerialize(Nullable.GetUnderlyingType(storageType), instance, out serialized);
        }

        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            // null is automatically deserialized
            return Serializer.TryDeserialize(data, Nullable.GetUnderlyingType(storageType), ref instance);
        }

        public override object CreateInstance(Data data, Type storageType)
        {
            return storageType;
        }
    }
}