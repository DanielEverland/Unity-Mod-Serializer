using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UMS.ConverterHelpers;

namespace UMS.Converters
{
    public sealed class GameObjectConverter : DirectConverter<GameObject>
    {
        public override object CreateInstance(Data input, Type storageType)
        {
            return new GameObject();
        }

        private const string COMPONENT_KEY = "components";
        private const string CHILDREN_KEY = "children";

        protected override Result DoDeserialize(Dictionary<string, Data> dictionary, ref GameObject gameObject)
        {
            List<Data> componentList = dictionary[COMPONENT_KEY].AsList;
            
            Result objResult = UnityEngineObjectHelper.TryDeserialize(dictionary, gameObject);
            if (!objResult.Succeeded)
                return objResult;

            foreach (Data dataIndex in componentList)
            {
                //Id of the component
                string id = MetaData.GetID(dataIndex);

                //Actual data for the component. Grabbed from the manifest using ID
                Data componentData = ObjectContainer.GetData(id);

                //Assign the current component, since ComponentConverter can't create a new instance without a GameObject to add it to.
                ComponentConverter.CurrentComponent = GetComponent(componentData, gameObject);

                //Deserialize the data into the component
                object outObj = Mods.DeserializeData(componentData, ComponentConverter.CurrentComponent.GetType());

                //Save it into the object container
                ObjectContainer.SetObject(id, outObj);

                //Remove the component to avoid tampering elsewhere.
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
        protected override Result DoSerialize(GameObject gameObject, Dictionary<string, Data> serialized)
        {
            Result objResult = UnityEngineObjectHelper.TrySerialize(serialized, gameObject);
            if (!objResult.Succeeded)
                return objResult;

            serialized.Add(CHILDREN_KEY, new Data(GetChildren(gameObject)));
            serialized.Add(COMPONENT_KEY, new Data(GetComponents(gameObject)));

            return Result.Success;
        }
        private List<Data> GetChildren(GameObject obj)
        {
            List<Data> children = new List<Data>();
            foreach (Transform child in obj.transform)
            {
                Serializer.TrySerialize(child.gameObject, out Data data);
                children.Add(data);
            }

            return children;
        }

        private List<Data> GetComponents(GameObject obj)
        {
            List<Data> components = new List<Data>();
            foreach (Component comp in obj.GetComponents<Component>())
            {
                Serializer.TrySerialize(comp, out Data data);
                components.Add(data);
            }

            return components;
        }
    }
}
