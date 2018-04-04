using System;
using UMS.Reflection;

#if !UNITY_EDITOR && UNITY_WSA
// For System.Reflection.TypeExtensions
using System.Reflection;
#endif

namespace UMS.Converters
{
    public sealed class TypeConverter : DirectConverter<Type>
    {
        public override bool RequestInheritanceSupport(Type type)
        {
            return false;
        }

        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            var type = (Type)instance;
            serialized = new Data(type.FullName);
            return Result.Success;
        }

        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            if (data.IsString == false)
            {
                return Result.Fail("Type converter requires a string");
            }

            instance = TypeCache.GetType(data.AsString);
            if (instance == null)
            {
                return Result.Fail("Unable to find type " + data.AsString);
            }
            return Result.Success;
        }

        public override object CreateInstance(Data data, Type storageType)
        {
            return storageType;
        }
    }
}