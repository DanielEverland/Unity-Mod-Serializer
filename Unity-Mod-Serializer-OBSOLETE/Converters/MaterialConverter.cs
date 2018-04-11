using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.ConverterHelpers;
using UnityEngine;

namespace UMS.Converters
{
    public class MaterialConverter : DirectConverter<Material>
    {
        private const string KEY_SHADER = "shader";

        private static readonly List<string> _members = new List<string>()
        {
            KEY_SHADER,
            "globalIlluminationFlags",
            "shaderKeywords",
            "renderQueue",
            "mainTextureScale",
            "mainTextureOffset",
            "mainTexture",
            "color",
            "enableInstancing",
            "doubleSidedGI",
        };

        public override object CreateInstance(Data data, Type storageType)
        {
            if (!data.IsDictionary)
                throw new ArgumentException("Expected dictionary");

            Result result = DeserializeMember(data.AsDictionary, null, KEY_SHADER, out Shader shader);

            if (result.Succeeded)
            {
                return new Material(shader);
            }
            else
            {
                throw new ArgumentException("Failed deserializing shader - " + result.FormattedMessages);
            }
        }
        protected override Result DoSerialize(Material material, Dictionary<string, Data> serialized)
        {
            Result result = Result.Success;

            result += SerializeMembers(serialized, material, _members);
            result += UnityEngineObjectHelper.TrySerialize(serialized, material);

            return result;
        }
        protected override Result DoDeserialize(Dictionary<string, Data> data, ref Material material)
        {
            Result result = Result.Success;

            result += DeserializeMembers(data, material, _members);
            result += UnityEngineObjectHelper.TryDeserialize(data, material);

            return result;
        }
    }
}
