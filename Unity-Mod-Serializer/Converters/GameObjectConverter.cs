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

            obj.name = data[KEY_NAME].AsString;

            return result;
        }
    }
}
