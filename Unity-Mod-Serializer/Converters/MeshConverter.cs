using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UMS.Converters
{
    public class MeshConverter : DirectConverter<Mesh>
    {
        private static readonly List<string> _members = new List<string>()
        {
            "indexFormat",
            "boneWeights",
            "bindposes",
            "subMeshCount",
            "vertices",
            "normals",
            "tangents",
            "triangles",
            "uv",
            "uv2",
            "uv3",
            "uv4",
            "colors",
        };

        public override object CreateInstance(Data data, Type type)
        {
            return new Mesh();
        }
        public override Result DoSerialize(Mesh obj, out Data data)
        {
            Result result = Result.Success;
            data = Data.CreateDictionary();

            result += SerializeMembers(data, obj, _members);
            result += UnityEngineObjectHelper.Serialize(data, obj);

            return result;
        }
        public override Result DoDeserialize(Data data, ref Mesh obj)
        {
            Result result = Result.Success;

            result += DeserializeMembers(data, obj, _members);
            result += UnityEngineObjectHelper.Deserialize(data, obj);

            obj.RecalculateBounds();

            return result;
        }
    }
}
