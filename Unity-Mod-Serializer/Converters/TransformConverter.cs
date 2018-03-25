using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UMS.Converters
{
    public partial class ConverterRegistrar
    {
        public static TransformConverter Register_TransformConverter;
    }
    public class TransformConverter : ComponentConverter
    {
        private const string PARENT_KEY = "parent";

        public override bool CanProcess(Type type)
        {
            //We purposely don't use IsAssignableFrom since we don't want to grab RectTransforms
            return type == typeof(Transform);
        }
        public override Result TryDeserialize(Data input, ref object instance, Type storageType)
        {
            if (input.IsDictionary)
            {
                Dictionary<string, Data> dictionary = input.AsDictionary;

                if (dictionary.ContainsKey(PARENT_KEY))
                {
                    Data parentData = dictionary[PARENT_KEY];
                    
                    if (!parentData.IsNull)
                    {
                        dictionary.Remove(PARENT_KEY);

                        AssignParent(parentData);
                    }
                }
            }

            return base.TryDeserialize(input, ref instance, storageType);
        }
        public Result AssignParent(Data parentData)
        {
            string id = MetaData.GetID(parentData);
            object obj = ObjectContainer.GetObjectFromID(id);

            Transform parent = obj as Transform;

            //Something's gone wrong in the conversion
            if (parent == null)
            {
                if (obj == null)
                {
                    return Result.Fail("Parent object is null");
                }
                else if (!(obj is Transform))
                {
                    return Result.Fail("Parent object is not of type UnityEngine.Transform");
                }
                else
                {
                    throw new Exception("Unexpected error");
                }
            }

            GameObject gameObject = CurrentComponent.gameObject;

            gameObject.transform.SetParent(parent);

            return Result.Success;
        }
    }    
}
