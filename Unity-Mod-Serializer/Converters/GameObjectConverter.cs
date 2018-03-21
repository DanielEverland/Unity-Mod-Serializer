using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        private const string NAME_KEY = "name";
        
        public override Result TryDeserialize(Data data, ref object instance, Type storageType)
        {
            GameObject obj = (GameObject)instance;

            obj.name = data[NAME_KEY].AsString;

            return Result.Success;
        }

        public override Result TrySerialize(object instance, out Data serialized, Type storageType)
        {
            Dictionary<string, Data> _data = new Dictionary<string, Data>();
            GameObject obj = (GameObject)instance;

            _data.Add(NAME_KEY, new Data(obj.name));

            serialized = new Data(_data);

            return Result.Success;
        }
    }
}
