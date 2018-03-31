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
        private const string KEY_INDEX_FORMAT = "indexFormat";
        private const string KEY_BONE_WEIGHTS = "boneWeights";
        private const string KEY_BIND_POSES = "bindPoses";
        private const string KEY_SUBMESH_COUNT = "submeshCount";
        private const string KEY_VERTICES = "vertices";
        private const string KEY_NORMALS = "normals";
        private const string KEY_TANGENTS = "tangents";
        private const string KEY_TRIANGLES = "triangles";
        private const string KEY_UV = "uv";
        private const string KEY_UV2 = "uv2";
        private const string KEY_UV3 = "uv3";
        private const string KEY_UV4 = "uv4";
        private const string KEY_COLORS = "colors";

        public override object CreateInstance(Data data, Type storageType)
        {
            return new Mesh();
        }
        protected override Result DoDeserialize(Dictionary<string, Data> data, ref Mesh model)
        {
            Result result = Result.Success;

            IndexFormat indexFormat = model.indexFormat;
            result += DeserializeMember(data, null, KEY_INDEX_FORMAT, out indexFormat);
            model.indexFormat = indexFormat;

            BoneWeight[] boneWeights = model.boneWeights;
            result += DeserializeMember(data, null, KEY_BONE_WEIGHTS, out boneWeights);
            model.boneWeights = boneWeights;

            Matrix4x4[] bindPoses = model.bindposes;
            result += DeserializeMember(data, null, KEY_BIND_POSES, out bindPoses);
            model.bindposes = bindPoses;

            int subMeshCount = model.subMeshCount;
            result += DeserializeMember(data, null, KEY_SUBMESH_COUNT, out subMeshCount);
            model.subMeshCount = subMeshCount;

            Vector3[] vertices = model.vertices;
            result += DeserializeMember(data, null, KEY_VERTICES, out vertices);
            model.vertices = vertices;

            Vector3[] normals = model.normals;
            result += DeserializeMember(data, null, KEY_NORMALS, out normals);
            model.normals = normals;

            Vector4[] tangents = model.tangents;
            result += DeserializeMember(data, null, KEY_TANGENTS, out tangents);
            model.tangents = tangents;

            int[] triangles = model.triangles;
            result += DeserializeMember(data, null, KEY_TRIANGLES, out triangles);
            model.triangles = triangles;

            Vector2[] uv = model.uv;
            result += DeserializeMember(data, null, KEY_UV, out uv);
            model.uv = uv;

            Vector2[] uv2 = model.uv2;
            result += DeserializeMember(data, null, KEY_UV2, out uv2);
            model.uv2 = uv2;

            Vector2[] uv3 = model.uv3;
            result += DeserializeMember(data, null, KEY_UV3, out uv3);
            model.uv3 = uv3;

            Vector2[] uv4 = model.uv4;
            result += DeserializeMember(data, null, KEY_UV4, out uv4);
            model.uv4 = uv4;

            Color32[] colors = model.colors32;
            result += DeserializeMember(data, null, KEY_COLORS, out colors);
            model.colors32 = colors;

            model.RecalculateBounds();

            result += UnityEngineObjectHelper.TryDeserialize(data, model);

            return result;
        }

        protected override Result DoSerialize(Mesh mesh, Dictionary<string, Data> serialized)
        {
            Result result = Result.Success;

            result += SerializeMember(serialized, null, KEY_INDEX_FORMAT, mesh.indexFormat);
            result += SerializeMember(serialized, null, KEY_BONE_WEIGHTS, mesh.boneWeights);
            result += SerializeMember(serialized, null, KEY_BIND_POSES, mesh.bindposes);
            result += SerializeMember(serialized, null, KEY_SUBMESH_COUNT, mesh.subMeshCount);
            result += SerializeMember(serialized, null, KEY_VERTICES, mesh.vertices);
            result += SerializeMember(serialized, null, KEY_NORMALS, mesh.normals);
            result += SerializeMember(serialized, null, KEY_TANGENTS, mesh.tangents);
            result += SerializeMember(serialized, null, KEY_TRIANGLES, mesh.triangles);
            result += SerializeMember(serialized, null, KEY_UV, mesh.uv);
            result += SerializeMember(serialized, null, KEY_UV2, mesh.uv2);
            result += SerializeMember(serialized, null, KEY_UV3, mesh.uv3);
            result += SerializeMember(serialized, null, KEY_UV4, mesh.uv4);
            result += SerializeMember(serialized, null, KEY_COLORS, mesh.colors32);

            result += UnityEngineObjectHelper.TrySerialize(serialized, mesh);

            return Result.Success;
        }
    }
}