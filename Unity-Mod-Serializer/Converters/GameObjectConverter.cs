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

        public override object CreateInstance(Type type)
        {
            return new GameObject();
        }
        public override Result DoSerialize(GameObject obj, out Data data)
        {
            Result result = Result.Success;
            data = new Data(new Dictionary<string, Data>());

            data[KEY_NAME] = new Data(obj.name);

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
