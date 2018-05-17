using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            
            Result result = DeserializeMember(data, KEY_SHADER, out Shader shader);

            if (result.Succeeded)
            {
                return new Material(shader);
            }
            else
            {
                throw new ArgumentException("Failed deserializing shader - " + result.FormattedMessage);
            }
        }
        public override Result DoSerialize(Material obj, out Data data)
        {
            Result result = Result.Success;
            data = Data.CreateDictionary();

            result += SerializeMembers(data, obj, _members);
            result += UnityEngineObjectHelper.Serialize(data, obj);

            return result;
        }
        public override Result DoDeserialize(Data data, ref Material obj)
        {
            Result result = Result.Success;

            result += DeserializeMembers(data, obj, _members);
            result += UnityEngineObjectHelper.Deserialize(data, obj);

            return result;
        }
    }
}
