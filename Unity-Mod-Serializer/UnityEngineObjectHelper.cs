using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UMS
{
    /// <summary>
    /// Helper class for serializing UnityEngine.Object data
    /// </summary>
    public class UnityEngineObjectHelper
    {
        //We intentionally leave out hideflags just to save on data size
        private const string NAME_KEY = "name";

        public static Result Serialize(Data data, Object obj)
        {
            if (!data.IsDictionary)
                return Result.Error("Type mismatch. Expected Dictionary");

            if (obj == null)
                return Result.Error("Object is null!");

            data.Dictionary.Set(NAME_KEY, new Data(obj.name));

            return Result.Success;
        }
        public static Result Deserialize(Data data, Object obj)
        {
            if (!data.IsDictionary)
                return Result.Error("Type mismatch. Expected Dictionary");

            if (obj == null)
                return Result.Error("Object is null!");

            if (!data.Dictionary.ContainsKey(NAME_KEY))
                return Result.Error("Data did not contain key " + NAME_KEY);

            obj.name = data.Dictionary[NAME_KEY].String;

            return Result.Success;
        }
    }
}
