using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UMS.ConverterHelpers;

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

        public override object CreateInstance(Data data, Type storageType)
        {
            return new Mesh();
        }
        protected override Result DoDeserialize(Dictionary<string, Data> data, ref Mesh model)
        {
            Result result = Result.Success;

            result += DeserializeMembers(data, model, _members);

            model.RecalculateBounds();

            result += UnityEngineObjectHelper.TryDeserialize(data, model);

            return result;
        }

        protected override Result DoSerialize(Mesh mesh, Dictionary<string, Data> serialized)
        {
            Result result = Result.Success;

            result += SerializeMembers(serialized, mesh, _members);
            result += UnityEngineObjectHelper.TrySerialize(serialized, mesh);

            return result;
        }
    }
}