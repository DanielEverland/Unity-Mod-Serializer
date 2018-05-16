using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UMS.Converters.Primitives
{
    public class ArrayConverter : BaseConverter<Array>
    {
        public override Result DoSerialize(Array array, out Data data)
        {
            Result result = Result.Success;
            data = new Data(new List<Data>());

            for (int i = 0; i < array.Length; i++)
            {
                object obj = array.GetValue(i);

                result += Serializer.Serialize(obj, out Data objData);

                data.Add(objData);
            }

            data.SetMetaData(new TypeMetaData(array.GetType().GetElementType()));

            return result;
        }
        public override Result DoDeserialize(Data data, ref Array array)
        {
            if (!data.IsList)
                return Result.Error("Type mismatch. Expected List");

            Result result = Result.Success;

            //Create the array
            Type arrayType = data.GetMetaData<TypeMetaData>().Type;
            array = Array.CreateInstance(arrayType, data.List.Count);

            for (int i = 0; i < data.List.Count; i++)
            {
                Data objData = data[i];
                object deserializedObject = null;

                TypeMetaData typeMetaData = objData.GetMetaData<TypeMetaData>();

                if (typeMetaData != null)
                {
                    result += Serializer.Deserialize(objData, typeMetaData.Type, ref deserializedObject);
                }
                else
                {
                    result += Serializer.Deserialize(objData, ref deserializedObject);
                }

                array.SetValue(deserializedObject, i);
            }

            return result;
        }        
    }
}
