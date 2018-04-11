using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UMS.Converters
{
    public class ShaderConverter : DirectConverter<Shader>
    {
        private const string KEY_NAME = "name";

        public override object CreateInstance(Data data, Type storageType)
        {
            if (!data.IsDictionary)
                throw new ArgumentException("Expected dictionary");

            Result result = DeserializeMember(data.AsDictionary, null, KEY_NAME, out string name);

            if (result.Succeeded)
            {
                return Shader.Find(name);
            }
            else
            {
                throw new ArgumentException("Failed deserializing shader name - " + result.FormattedMessages);
            }
        }
        protected override Result DoSerialize(Shader model, Dictionary<string, Data> serialized)
        {
            serialized.Add(KEY_NAME, new Data(model.name));

            return Result.Success;
        }
        protected override Result DoDeserialize(Dictionary<string, Data> data, ref Shader model)
        {
            if (!data.ContainsKey(KEY_NAME))
                return Result.Fail("Missing " + KEY_NAME);

            Data nameData = data[KEY_NAME];

            if (!nameData.IsString)
                return Result.Fail("Expected type string");

            model = Shader.Find(nameData.AsString);

            return Result.Success;
        }        
    }
}
