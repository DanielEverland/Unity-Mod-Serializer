using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UMS.ConverterHelpers
{
    public static class UnityEngineObjectHelper
    {
        private const string NAME_KEY = "name";

        public static Result TrySerialize(Dictionary<string, Data> serialized, Object obj)
        {
            if (obj == null)
                return Result.Fail("Object is null!");

            serialized.Add(NAME_KEY, new Data(obj.name));

            return Result.Success;
        }
    }
}
