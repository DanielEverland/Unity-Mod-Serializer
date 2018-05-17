using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UMS.Converters
{
    public class ShaderConverter : DirectConverter<Shader>
    {
        public override Result DoSerialize(Shader obj, out Data data)
        {
            data = new Data(obj.name);

            return Result.Success;
        }
        public override Result DoDeserialize(Data data, ref Shader obj)
        {
            if (!data.IsString)
                return Result.Error("Type mismatch. Expected string");

            obj = Shader.Find(data.String);

            return Result.Success;
        }        
    }
}
