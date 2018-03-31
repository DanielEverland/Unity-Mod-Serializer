using System;

namespace UMS.Converters.Primitives
{
    public sealed class BoolConverter : DirectConverter<bool>
    {
        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            serialized = new Data((bool)instance);
            return Result.Success;
        }
        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            if (!data.IsBool)
                return Result.Fail("Expected type of bool!");

            instance = data.AsBool;
            return Result.Success;
        }
    }
}
