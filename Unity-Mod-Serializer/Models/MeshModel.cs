using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf.Meta;
using UnityEngine;
using UnityEngine.Rendering;
using ProtoBuf;

namespace UMS.Models
{
    public class MeshModel : ModelBase<Mesh>
    {
        public override void CreateModel(MetaType type)
        {
            type.AsReferenceDefault = true;
            type.SetSurrogate(typeof(MeshSurrogate));
        }

        [ProtoContract(AsReferenceDefault = true)]
        private class MeshSurrogate
        {
            public MeshSurrogate() { }
            public MeshSurrogate(Mesh mesh)
            {
                indexFormat = mesh.indexFormat;
                boneWeights = mesh.boneWeights;
                bindPoses = mesh.bindposes;
                subMeshCount = mesh.subMeshCount;
                vertices = mesh.vertices;
                normals = mesh.normals;
                tangents = mesh.tangents;
                uv = mesh.uv;
                uv2 = mesh.uv2;
                uv3 = mesh.uv3;
                uv4 = mesh.uv4;
                colors = mesh.colors;
                triangles = mesh.triangles;
                name = mesh.name;
            }

            [ProtoMember(1)]
            private IndexFormat indexFormat;
            [ProtoMember(2)]
            private BoneWeight[] boneWeights;
            [ProtoMember(3)]
            private Matrix4x4[] bindPoses;
            [ProtoMember(4)]
            private int subMeshCount;
            [ProtoMember(5)]
            private Vector3[] vertices;
            [ProtoMember(6)]
            private Vector3[] normals;
            [ProtoMember(7)]
            private Vector4[] tangents;
            [ProtoMember(8)]
            private Vector2[] uv;
            [ProtoMember(9)]
            private Vector2[] uv2;
            [ProtoMember(10)]
            private Vector2[] uv3;
            [ProtoMember(11)]
            private Vector2[] uv4;
            [ProtoMember(12)]
            private Color[] colors;
            [ProtoMember(13)]
            private int[] triangles;
            [ProtoMember(14)]
            private string name;

            public Mesh Deserialize()
            {
                Mesh mesh = new Mesh();
                mesh.name = name;
                
                mesh.vertices = vertices;
                mesh.subMeshCount = subMeshCount;
                mesh.triangles = triangles;
                mesh.indexFormat = indexFormat;
                mesh.boneWeights = boneWeights;
                mesh.bindposes = bindPoses;
                mesh.subMeshCount = subMeshCount;
                mesh.normals = normals;
                mesh.tangents = tangents;
                mesh.uv = uv;
                mesh.uv2 = uv2;
                mesh.uv3 = uv3;
                mesh.uv4 = uv4;
                mesh.colors = colors;

                mesh.RecalculateBounds();

                return mesh;
            }

            public static implicit operator MeshSurrogate (Mesh mesh)
            {
                return mesh == null ? null : new MeshSurrogate(mesh);
            }
            public static implicit operator Mesh (MeshSurrogate surrogate)
            {
                return surrogate == null ? null : surrogate.Deserialize();
            }
        }
    }
}
