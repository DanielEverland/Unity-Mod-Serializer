using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UMS.ConverterHelpers;

namespace UMS.Converters
{
    partial class ConverterRegistrar
    {
        public static GameObjectConverter Register_GameObjectConverter;
    }
    public class GameObjectConverter : Converter
    {
        public override bool CanProcess(Type type)
        {
            return type == typeof(GameObject);
        }

        public override object CreateInstance(Data input, Type storageType)
        {
            return new GameObject();
        }

        private const string COMPONENT_KEY = "components";
        
        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            if (!data.IsDictionary)
                return Result.Fail("Input data is not dictionary");

            Dictionary<string, Data> dictionary = data.AsDictionary;
            List<Data> componentList = dictionary[COMPONENT_KEY].AsList;

            GameObject gameObject = (GameObject)instance;
            
            foreach (Data componentData in componentList)
            {
                ComponentConverter.CurrentComponent = GetComponent(componentData, gameObject);

                object outObj = null;
                Serializer.TryDeserialize(componentData, typeof(Component), ref outObj);

                ComponentConverter.CurrentComponent = null;
            }

            return Result.Success;
        }
        private Component GetComponent(Data data, GameObject obj)
        {
            Type type = MetaData.GetMetaDataType(data);

            if (!typeof(Component).IsAssignableFrom(type))
                throw new InvalidCastException();

            if(type == typeof(RectTransform))
            {
                return obj.GetComponent<RectTransform>();
            }
            else if(type == typeof(Transform))
            {
                return obj.GetComponent<Transform>();
            }
            else
            {
                return obj.AddComponent(type);
            }
        }
        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            serialized = null;
            Dictionary<string, Data> _data = new Dictionary<string, Data>();
            GameObject obj = (GameObject)instance;

            Result objResult = UnityEngineObjectHelper.TrySerialize(_data, obj);
            if (!objResult.Succeeded)
                return objResult;

            List<Data> components = new List<Data>();
            foreach (Component comp in obj.GetComponents<Component>())
            {
                Serializer.TrySerialize(comp, out Data data);
                components.Add(data);
            }

            _data.Add(COMPONENT_KEY, new Data(components));
            
            serialized = new Data(_data);

            return Result.Success;
        }
    }
}
