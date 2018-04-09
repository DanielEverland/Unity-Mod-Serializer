using System;
using System.Collections;
using UMS.Reflection;

namespace UMS.Converters
{
    public sealed class ArrayConverter : Converter<Array>
    {
        public override bool RequestInheritanceSupport(Type storageType)
        {
            return false;
        }

        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            Array array = (Array)instance;
            Type elementType = storageType.GetElementType();

            Result result = Result.Success;

            serialized = Data.CreateArray(array.GetLengths());

            int[] indexes = new int[array.Rank];
            for (int d = 0; d < array.Rank; d++)
            {
                for (int i = d == 0 ? 0 : 1; i < array.GetLength(d); i++)
                {
                    indexes[d] = i;

                    object obj = array.GetValue(indexes);

                    Result itemResult = Serializer.TrySerialize(elementType, obj, out Data serializedItem);
                    result.AddMessages(itemResult);

                    if (itemResult.Failed)
                        continue;

                    serialized.SetValue(serializedItem, indexes);
                }                                
            }
            
            return result;
        }

        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            var result = Result.Success;

            // Verify that we actually have an List
            if ((result += CheckType(data, DataType.Array)).Failed)
            {
                return result;
            }

            Type elementType = storageType.GetElementType();

            var serializedList = data.AsList;
            var list = new ArrayList(serializedList.Count);
            int existingCount = list.Count;

            for (int i = 0; i < serializedList.Count; ++i)
            {
                var serializedItem = serializedList[i];
                object deserialized = null;
                if (i < existingCount) deserialized = list[i];

                var itemResult = Serializer.TryDeserialize(serializedItem, elementType, ref deserialized);
                result.AddMessages(itemResult);
                if (itemResult.Failed) continue;

                if (i < existingCount) list[i] = deserialized;
                else list.Add(deserialized);
            }

            instance = list.ToArray(elementType);
            return result;
        }

        public override object CreateInstance(Data data, Type storageType)
        {
            return MetaType.Get(Serializer.Config, storageType).CreateInstance();
        }
    }
}