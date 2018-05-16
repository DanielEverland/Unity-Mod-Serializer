using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UMS.Converters
{
    public class GameObjectConverter : DirectConverter<GameObject>
    {
        private string KEY_NAME = "name";
        private string KEY_COMPONENTS = "components";

        public override object CreateInstance(Type type)
        {
            return new GameObject();
        }
        public override Result DoSerialize(GameObject obj, out Data data)
        {
            Result result = Result.Success;
            data = new Data(new Dictionary<string, Data>());

            data[KEY_NAME] = new Data(obj.name);

            result += SerializeComponents(obj, ref data);

            return result;
        }
        private Result SerializeComponents(GameObject obj, ref Data data)
        {
            Result result = Result.Success;

            if (!data.IsDictionary)
                return Result.Error("Type mismatch. Expected Dictionary");
            
            Data listData = new Data(new List<Data>());
            foreach (Component comp in obj.GetComponents<Component>())
            {
                result += Serializer.Serialize(comp, out Data compData);
                result += listData.Add(compData);
            }

            data[KEY_COMPONENTS] = listData;

            return result;
        }
        public override Result DoDeserialize(Data data, ref GameObject obj)
        {
            Result result = Result.Success;

            if (!data.IsDictionary)
                return Result.Error("Type mismatch. Expected dictionary");

            obj.name = data[KEY_NAME].String;

            result += DeserializeComponents(data[KEY_COMPONENTS], ref obj);

            return result;
        }
        private Result DeserializeComponents(Data data, ref GameObject obj)
        {
            if (!data.IsList)
                return Result.Error("Type mismatch. Expected List", data);

            Result result = Result.Success;

            foreach (Data referenceData in data.List)
            {
                Result referenceResult = MetaData.GetReference(referenceData, out string id);

                if (!referenceResult.Succeeded)
                    continue;

                Data componentData = ObjectHandler.GetData(id);

                if (!componentData.IsDictionary)
                    return Result.Error("Type mismatch. Expected Dictionary", componentData);
                
                result += MetaData.GetType(componentData, out Type componentType);

                Component component = GetComponent(componentType, obj);
                                
                result += Serializer.Deserialize(componentData, componentType, ref component);
            }

            return result;
        }
        private Component GetComponent(Type type, GameObject obj)
        {
            //We don't use AssignableFrom to avoid catching RectTransform
            if(type == typeof(Transform))
            {
                return obj.GetComponent<Transform>();
            }
            else if(type == typeof(RectTransform))
            {
                Component rectTrans = obj.GetComponent<RectTransform>();

                //This can happen if we're deserializing a UI object
                //that has not been assigned to a canvas yet. In that
                //case we simply convert it manually
                if(rectTrans == null)
                {
                    return obj.AddComponent<RectTransform>();
                }
                else
                {
                    return rectTrans;
                }
            }
            else
            {
                return obj.AddComponent(type);
            }
        }
    }
}
